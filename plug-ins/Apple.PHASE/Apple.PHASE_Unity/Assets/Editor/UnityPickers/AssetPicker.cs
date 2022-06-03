using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityPickers.Utility;
using Object = UnityEngine.Object;

namespace UnityPickers
{
	public class AssetPicker : EditorWindow
	{
		public class HierarchyEntry : IComparable<HierarchyEntry>
		{
			public HierarchyEntry Parent;

			private string path;

			public string Path
			{
				get { return path; }
				set
				{
					path = value;
					SearchPath = System.IO.Path.ChangeExtension(path, null);
					if (SearchPath != null)
					{
						SearchPath = SearchPath.Replace("\\", "/");
						if (SearchPath.StartsWith("Assets/"))
							SearchPath = SearchPath.Substring("Assets/".Length);
					}
				}
			}

			public string SearchPath;
			public string Name;
			public string AssetGuid;
			public Object Asset;
			public List<HierarchyEntry> Children;
			public bool Expanded = true;
			public bool Hidden;

			public void FindAsset()
			{
				Asset = Asset ?? AssetDatabase.LoadAssetAtPath(Path, typeof(Object));
			}

			public int CompareTo(HierarchyEntry other)
			{
				if (Children == null && other.Children != null)
				{
					return 1;
				}
				if (Children != null && other.Children == null)
				{
					return -1;
				}
				return string.Compare(Path, other.Path, StringComparison.Ordinal);
			}
		}

		private static GUIStyle selectedAssetStyle;

		private static GUIStyle assetStyle;

		private static GUIStyle folderStyle;

		public const int AssetShowCountStep = 50;

		private int assetShowCount = AssetShowCountStep;

		private int assetsShown;

	    private bool enableSelectionOnClick;

	    private string[] labelsFilter;

        private readonly StringBuilder stringBuilder = new StringBuilder();

		private HierarchyEntry SelectedAssetEntry
		{
			get { return selectedAssetEntry; }
			set
			{
				var e = value;
				while (e != null)
				{
					e.Expanded = true;
					e = e.Parent;
				}
				selectedAssetEntry = value; 				
			}
		}

		[SerializeField]
		private Type assetType;

		[SerializeField]
		private string typeFilter;

		private Type AssetType
		{
			get { return assetType; }
			set
			{
				assetType = value;
				if (typeof(MonoBehaviour).IsAssignableFrom(assetType)
					|| typeof(GameObject).IsAssignableFrom(assetType))
				{
					typeFilter = "Prefab";
				}
				else if (typeof(Object).IsAssignableFrom(assetType))
				{
					typeFilter = assetType.Name;
				}
				else
				{
					typeFilter = "";
				}
			}
		}

		[SerializeField]
		private bool isFlatMode;

		[SerializeField]
		private string nameFilter;

		private HierarchyProperty hierarchyProperty;

		private List<HierarchyEntry> loadedAssets;

		private readonly List<HierarchyEntry> loadedAssetsFlat = new List<HierarchyEntry>();

		private Vector2 scroll;

		private readonly HashSet<string> foldedPaths = new HashSet<string>();

		[CanBeNull]
		private Action<Object> selectionCallback;

		private readonly List<Func<HierarchyEntry, bool>> filters = new List<Func<HierarchyEntry, bool>>();

		public static List<Func<HierarchyEntry, bool>> DependenciesFilters = new List<Func<HierarchyEntry, bool>>();

		private bool focusNameFilter;

		private HierarchyEntry selectedAssetEntry;

		private bool scrollToSelected;

		private HierarchyEntry firstVisibleEntry;

		[MenuItem("Tools/Asset Picker %&#O", false, 5000)]
		public static void OpenWindow()
		{
			var w = GetWindow<AssetPicker>();
			w.focusNameFilter = true;
			w.UpdateAssetList();
		}

		// todo: prettifty Show signatures
		public static void Show(
			[NotNull] Type assetType, 
			[CanBeNull] FieldInfo fieldInfo, 
			[NotNull] Action<Object> callback, 
			[CanBeNull] Object selectedAsset = null, 
			[CanBeNull] Func<HierarchyEntry, bool> filter = null)
		{
			var w = CreateInstance<AssetPicker>();

			w.filters.Clear();
			if (filter != null)
			{
				w.filters.Add(filter);
			}
			if (fieldInfo != null)
			{
				var pathFilter = fieldInfo.GetAttribute<AssetPathFilterAttribute>();
				if (pathFilter != null)
				{
					w.filters.Add(he => pathFilter.Filters.Any(f => he.Path.Contains(f)));
				}
			}

			w.AssetType = assetType;
			w.selectionCallback = callback;
		    w.labelsFilter = null;

			w.UpdateAssetList();
			if (selectedAsset)
			{
				var assetPath = AssetDatabase.GetAssetPath(selectedAsset);
				w.SelectedAssetEntry =
					w.loadedAssetsFlat.FirstOrDefault(e => e.Path == assetPath);
				w.scrollToSelected = true;
			}
			w.ShowAuxWindow();
			w.focusNameFilter = true;
			w.Focus();
		}

	    public static void Show<T>(
			[NotNull] Action<Object> callback, 
			[CanBeNull] string[] labels = null, 
			bool enableSelectionOnClick = false, 
			[CanBeNull] Object selectedAsset = null)
	    {
	        var w = CreateInstance<AssetPicker>();
	        w.enableSelectionOnClick = enableSelectionOnClick;
			w.AssetType = typeof(T);
	        w.selectionCallback = callback;
	        w.labelsFilter = labels;
	        w.UpdateAssetList();
            if (selectedAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(selectedAsset);
                w.SelectedAssetEntry =
                    w.loadedAssetsFlat.FirstOrDefault(e => e.Path == assetPath);
                w.scrollToSelected = true;
            }
            w.ShowAuxWindow();
            w.focusNameFilter = true;
            w.Focus();
        }

		// todo: c# docs

		/// <summary>
		/// Shows built-in ObjectField with custom value source and pick target
		/// </summary>
		public static void PropertyField(
			Rect position, 
			[NotNull] SerializedProperty property, 
			[NotNull] FieldInfo fieldInfo,
			[CanBeNull] Object currentValue, 
			[NotNull] Action<Object> pickCallback,
			[NotNull] GUIContent label, 
			[NotNull] Type assetType,
			[CanBeNull] Func<HierarchyEntry, bool> filter = null)
		{
			using (var scope = new EditorGUI.PropertyScope(position, label, property))
			{
				EditorGUI.BeginChangeCheck();
				var buttonPos = position;
				buttonPos.xMin = buttonPos.xMax - EditorGUIUtility.singleLineHeight;
				var requesterWindow = focusedWindow;

				string controlName = property.serializedObject.targetObject.GetInstanceID() + "_" + property.propertyPath;
				var e = Event.current;
				bool showHotKey =
					GUI.GetNameOfFocusedControl() == controlName &&
					e.type == EventType.KeyDown &&
					e.keyCode == KeyCode.Return;

				bool deleteHotKey =
					GUI.GetNameOfFocusedControl() == controlName &&
					e.type == EventType.KeyDown &&
					e.keyCode == KeyCode.Delete;

				if (showHotKey || deleteHotKey)
				{
					e.Use();
				}

				if (deleteHotKey)
				{
					pickCallback(null);
				}
				else if (showHotKey || GUI.Button(buttonPos, "", GUIStyle.none))
				{
					// invisible button overrides object picker
					Show(
						assetType,
						fieldInfo,
						o =>
						{
							property.serializedObject.Update();
							pickCallback(o);
							property.serializedObject.ApplyModifiedProperties();

							if (requesterWindow != null)
							{
								requesterWindow.Repaint();
								requesterWindow.Focus();
							}
						},
						currentValue,
						filter
					);
				}

				GUI.SetNextControlName(controlName);
				var obj = EditorGUI.ObjectField(
					position,
					scope.content,
					currentValue,
					assetType,
					false
				);

				if (GUI.GetNameOfFocusedControl() == controlName)
				{
					// todo: implement copy paste
					// CopyPasteController.Process(assetType, property);
				}

				if (EditorGUI.EndChangeCheck())
				{
					pickCallback(obj);
				}
			}
		}

		/// <summary>
		/// Shows built-in ObjectField, but overrides thumb button to call our AssetPicker window instead
		/// </summary>
		public static void PropertyField(
			Rect position, 
			[NotNull] SerializedProperty property, 
			[NotNull] FieldInfo fieldInfo, 
			[NotNull] GUIContent label, 
			[NotNull] Type assetType, 
			[CanBeNull] Func<HierarchyEntry, bool> filter = null)
		{
			Action<Object> pickCallback =
				o => property.objectReferenceValue = o;
			PropertyField(
				position, property, fieldInfo,
				property.objectReferenceValue, pickCallback,
				label, assetType, filter
			);
		}

		public static void ObjectField(
			Rect position, 
			[CanBeNull] Object currentValue, 
			[NotNull] Action<Object> pickCallback,
			[NotNull] GUIContent label, 
			[NotNull] Type assetType,
			[CanBeNull] Func<HierarchyEntry, bool> filter = null)
		{
			EditorGUI.BeginChangeCheck();
			var buttonPos = position;
			buttonPos.xMin = buttonPos.xMax - EditorGUIUtility.singleLineHeight;
			var requesterWindow = focusedWindow;

			string controlName = "ObjField" + label.text;
			var e = Event.current;
			bool showHotKey =
				GUI.GetNameOfFocusedControl() == controlName &&
				e.type == EventType.KeyDown &&
				e.keyCode == KeyCode.Return;

			bool deleteHotKey =
				GUI.GetNameOfFocusedControl() == controlName &&
				e.type == EventType.KeyDown &&
				e.keyCode == KeyCode.Delete;

			if (showHotKey || deleteHotKey)
			{
				e.Use();
			}

			if (deleteHotKey)
			{
				pickCallback(null);
			}
			else if (showHotKey || GUI.Button(buttonPos, "", GUIStyle.none))
			{
				// invisible button overrides object picker
				Show(
					assetType,
					null,
					o =>
					{
						pickCallback(o);

						if (requesterWindow != null)
						{
							requesterWindow.Repaint();
							requesterWindow.Focus();
						}
					},
					currentValue,
					filter);
			}

			GUI.SetNextControlName(controlName);
			var obj = EditorGUI.ObjectField(
				position,
				label,
				currentValue,
				assetType,
				false);

			if (EditorGUI.EndChangeCheck())
			{
				pickCallback(obj);
			}
		}

		public IEnumerable<Type> CollectAssetTypes()
		{
			yield return typeof(GameObject);
			yield return typeof(Material);
			yield return typeof(Texture);
			yield return typeof(SceneAsset);
			yield return typeof(ScriptableObject);

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var gameAssembly = assemblies.Single(a => a.FullName.Contains("Assembly-CSharp,"));
			var soTypes = TypeEx.GetSubclasses<ScriptableObject>(true, gameAssembly);

			foreach (var type in soTypes)
			{
				yield return type;
			}
		}

		static AssetPicker()
		{
		}

		public AssetPicker()
		{
			Selection.selectionChanged += Repaint;
			AssetType = typeof(Object);
		}

		private static void InitStyles()
		{
			if (selectedAssetStyle == null)
			{
				selectedAssetStyle = new GUIStyle(EditorStyles.label);
				selectedAssetStyle.normal = selectedAssetStyle.focused;
				selectedAssetStyle.active = selectedAssetStyle.focused;
				selectedAssetStyle.onNormal = selectedAssetStyle.focused;
				selectedAssetStyle.onActive = selectedAssetStyle.focused;
			}

			if (assetStyle == null)
			{
				assetStyle = new GUIStyle(EditorStyles.label);
				assetStyle.active = assetStyle.normal;
				assetStyle.focused = assetStyle.normal;
				assetStyle.onActive = assetStyle.normal;
				assetStyle.onFocused = assetStyle.normal;
			}

			if (folderStyle == null)
			{
				folderStyle = new GUIStyle(EditorStyles.foldout);
				folderStyle.focused = folderStyle.normal;
				folderStyle.active = folderStyle.normal;
				folderStyle.hover = folderStyle.normal;
				folderStyle.onFocused = folderStyle.normal;
				folderStyle.onActive = folderStyle.normal;
				folderStyle.onHover = folderStyle.normal;
			}
		}

		private void OnEnable()
		{
			titleContent = new GUIContent("Assets");

			if (!string.IsNullOrEmpty(typeFilter))
			{
				UpdateAssetList();
			}

			var folded = EditorPrefs.GetString("AP_Folded_Paths", "");
			foldedPaths.UnionWith(folded.Split('\n'));
		}

	    private void OnDisable()
		{
			EditorPrefs.SetString("AP_Folded_Paths", string.Join("\n", foldedPaths.ToArray()));
		}

		private void OnGUI()
		{
			InitStyles();

			HandleKeyDown();

			// header (filters)
			using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(false)))
			{
				GUILayout.Space(1);
				using (new EditorGUILayout.HorizontalScope())
				{
					// disable changing type if this is a pop-up picker
					GUILayout.Label("Asset type:", GUILayout.Width(70), GUILayout.ExpandWidth(false));
					if (selectionCallback == null)
					{
						// browse mode - type can be changed freely
						TypePicker.Button(
							AssetType.Name,
							CollectAssetTypes,
							type =>
							{
								AssetType = type;
								UpdateAssetList();
								focusNameFilter = true;
							},
							style: EditorStyles.popup,
							options: GUILayout.Width(120)
						);
					}
					else
					{
						// picker mode - type cannot be changed
						GUILayout.Label(typeFilter ?? "");
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(
						isFlatMode ? "Flat mode" : "Folders mode",
						EditorStyles.miniButton,
						GUILayout.Width(90),
						GUILayout.Height(18)))
					{
						isFlatMode = !isFlatMode;
					}
				}
				using (new EditorGUILayout.HorizontalScope())
				{
					GUILayout.Label("Name filter:", GUILayout.Width(70), GUILayout.ExpandWidth(false));

					EditorGUI.BeginChangeCheck();
					GUI.SetNextControlName("NameFilter");
					nameFilter = EditorGUILayout.TextField(nameFilter ?? "");
					if (focusNameFilter)
					{
						EditorGUI.FocusTextInControl("NameFilter");
						focusNameFilter = false;
					}
					if (EditorGUI.EndChangeCheck())
					{
						UpdateFilter();
					}
				}
			}

			// browse mode
			if (selectionCallback == null)
			{
				if (SelectedAssetEntry != null && !Selection.assetGUIDs.Contains(SelectedAssetEntry.AssetGuid))
				{
					SelectedAssetEntry = null;
				}
				if (SelectedAssetEntry == null && Selection.assetGUIDs.Length > 0)
				{
					SelectedAssetEntry = loadedAssetsFlat
						.Where(e => Selection.assetGUIDs.Contains(e.AssetGuid))
						.FirstOrDefault(e => !e.Hidden);
				}
			}

			// asset list
			using (var scope = new EditorGUILayout.ScrollViewScope(scroll, GUI.skin.box, GUILayout.ExpandHeight(true)))
			{
				scroll = scope.scrollPosition;
				assetsShown = 0;
				DisplayList(isFlatMode ? loadedAssetsFlat : loadedAssets);
			}

			// footer
			if (assetsShown == assetShowCount)
			{
				if (GUILayout.Button(assetsShown + " assets shown. Show more..."))
				{
					assetShowCount += AssetShowCountStep;
				}
			}
			else
			{
				GUILayout.Box(assetsShown + " assets shown.", GUILayout.ExpandWidth(true));
			}

			if (scrollToSelected)
			{
				Repaint();
			}
		}

		private void HandleKeyDown()
		{
			// keyboard control when windown is in picker mode
			var evt = Event.current;
			if (evt.type != EventType.KeyDown)
			{
				return;
			}

			// todo: left arrow to collapse
			// todo: right arrow to expand? (would need to make folders selectable)
			// todo: page up / page down
			switch (evt.keyCode)
			{
				case KeyCode.Return:
					OnAssetSelected();
					evt.Use();
					break;
				case KeyCode.Escape:
					Close();
					evt.Use();
					return;
				case KeyCode.DownArrow:
					SelectedAssetEntry =
						loadedAssetsFlat
							.SkipWhile(e => SelectedAssetEntry != null && e != SelectedAssetEntry)
							.Skip(1)
							.FirstOrDefault(e => !e.Hidden) ?? SelectedAssetEntry;

					OnAssetHover();
					evt.Use();
					Repaint();
					break;
				case KeyCode.UpArrow:
					SelectedAssetEntry =
						loadedAssetsFlat
							.Reverse<HierarchyEntry>()
							.SkipWhile(e => SelectedAssetEntry != null && e != SelectedAssetEntry)
							.Skip(1)
							.FirstOrDefault(e => !e.Hidden) ?? SelectedAssetEntry;

					OnAssetHover();
					evt.Use();
					Repaint();
					break;
}
		}

		private void OnAssetSelected()
		{
			if (SelectedAssetEntry == null)
				return;

			SelectedAssetEntry.FindAsset();
			if (selectionCallback != null)
				selectionCallback(SelectedAssetEntry.Asset);
			else
				Selection.activeObject = SelectedAssetEntry.Asset;

			Close();
		}

		private void OnAssetHover(bool scrollToAsset = true)
		{
			if (SelectedAssetEntry == null)
				return;

			if (scrollToAsset)
				scrollToSelected = true;

			SelectedAssetEntry.FindAsset();
			if (selectionCallback == null)
				Selection.activeObject =  SelectedAssetEntry.Asset;

			if (enableSelectionOnClick)
			{
				if (selectionCallback != null)
					selectionCallback(SelectedAssetEntry.Asset);
			}
		}

		private void DisplayList(List<HierarchyEntry> list)
		{
			if (list == null)
			{
				return;
			}
			foreach (var entry in list)
			{
				if (assetsShown >= assetShowCount)
				{
					return;
				}
				if (entry.Hidden)
				{
					continue;
				}

				if (entry.Children == null)
				{
					// this is a leaf node
					DisplayAsset(entry);
					assetsShown++;
				}
				else
				{
					// this is a folder
					var ex = EditorGUILayout.Foldout(entry.Expanded, entry.Name, true, folderStyle);
					// store folded status
					if (ex != entry.Expanded)
					{
						if (ex)
						{
							foldedPaths.Remove(entry.Path);
						}
						else
						{
							foldedPaths.Add(entry.Path);
						}
						entry.Expanded = ex;
					}
					// show contents
					// ReSharper disable once AssignmentInConditionalExpression
					if (entry.Expanded)
					{
						using (new EditorGUILayout.HorizontalScope())
						{
							GUILayout.Space(10);
							using (new EditorGUILayout.VerticalScope())
							{
								DisplayList(entry.Children);
							}
						}
					}
				}
			}
		}

		private void DisplayAsset(HierarchyEntry entry)
		{
			Rect rect;
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(16);
				rect = GUILayoutUtility.GetRect(new GUIContent(entry.Name), GUI.skin.label, GUILayout.ExpandWidth(true));
			}

			var selected = Selection.assetGUIDs.Contains(entry.AssetGuid) || entry == SelectedAssetEntry;

			// browse mode
			if (selectionCallback == null)
			{
				selected = Selection.assetGUIDs.Contains(entry.AssetGuid);
			}


			if (Event.current.type == EventType.Repaint)
			{
				if (scrollToSelected && selected)
				{
					scroll.y = rect.yMin - (position.height - 50) / 2;
					scrollToSelected = false;
				}
			}

			if (rect.Contains(Event.current.mousePosition))
			{
				// click & doubleclick
				if (selectionCallback != null)
				{
					if (Event.current.type == EventType.MouseDown && (enableSelectionOnClick || Event.current.clickCount > 1))
					{
						entry.FindAsset();
						selectionCallback(entry.Asset);
                        if (enableSelectionOnClick)
                        {
                            SelectedAssetEntry = entry;
                        }
                        if (Event.current.clickCount > 1)
                        {
                            Close();
                        }
                        Event.current.Use();
					}
				}
				// drag
				if (selectionCallback == null && Event.current.type == EventType.MouseDrag)
				{
					DragAndDrop.PrepareStartDrag();
					if (!selected)
					{
						entry.FindAsset();
						DragAndDrop.objectReferences = new[] {entry.Asset};
					}
					else
					{
						DragAndDrop.objectReferences = Selection.objects;
					}
					DragAndDrop.StartDrag("AssetPicker");
					Event.current.Use();
				}
				// prevent button from eating RMB
				if (Event.current.isMouse && Event.current.button != 0)
				{
					return;
				}
			}

			var style = selected ? selectedAssetStyle : assetStyle;
			if (GUI.RepeatButton(rect, entry.Name, style))
			{
				entry.FindAsset();

				// browse mode
				if (selectionCallback == null)
				{
					if (Event.current.control)
					{
						// todo: fix blinking
						// ctrl-click: add/remove to selection
						Selection.objects = selected
							? Selection.objects.Where(o => o != entry.Asset).ToArray()
							: Selection.objects.Concat(new[] {entry.Asset}).ToArray();
					}
					// todo: shift-click
					else if (!Event.current.shift)
					{
						// simple click - select asset
						SelectedAssetEntry = entry;
						OnAssetHover(false);
						Repaint();
					}
				}
				else
				{
					SelectedAssetEntry = entry;
				}
			}
		}

		private void UpdateFilter()
		{
			firstVisibleEntry = null;
			UpdateFilterRecursive(loadedAssets);
			if (selectionCallback != null)
			{
				if (SelectedAssetEntry == null || SelectedAssetEntry.Hidden)
				{
					SelectedAssetEntry = firstVisibleEntry;
				}
				scrollToSelected = true;
			}
		}

		private bool UpdateFilterRecursive(List<HierarchyEntry> list)
		{
			if (list == null)
			{
				return false;
			}

			var res = false;
			foreach (var entry in list)
			{
				if (entry.Children != null)
				{
					entry.Hidden = !UpdateFilterRecursive(entry.Children);
				}
				else
				{
					entry.Hidden = !MatchesName(entry);
                    if (!entry.Hidden)
                    {
                        entry.Hidden = !MatchesLabels(entry);
                    }
				    
					if (!entry.Hidden && firstVisibleEntry == null)
					{
						firstVisibleEntry = entry;
					}
				}

				res |= !entry.Hidden;
			}
			return res;
		}

	    private bool MatchesLabels(HierarchyEntry entry)
	    {
            if (labelsFilter == null || labelsFilter.Length == 0)
            {
                return true;
            }

	        entry.FindAsset();
            if (entry.Asset)
            {
                var assetLabels = AssetDatabase.GetLabels(entry.Asset);
                if (assetLabels.Length < labelsFilter.Length)
                {
                    return false;
                }

                foreach (string filterLabel in labelsFilter)
                {
                    var lowerFilterLabel = filterLabel.ToLowerInvariant();
                    if (assetLabels.All(l => l.ToLowerInvariant() != lowerFilterLabel))
                    {
                        return false;
                    }
                }
            }

	        return true;
	    }

	    private bool MatchesName(HierarchyEntry entry)
		{
			if (string.IsNullOrEmpty(nameFilter))
			{
				return true;
			}

			var prettyFilter = nameFilter.Replace("\\", "/");
			return StringUtils.MatchesFilter(entry.SearchPath, prettyFilter);
		}

		private void UpdateAssetList()
		{
			loadedAssets = new List<HierarchyEntry>();
			loadedAssetsFlat.Clear();

			hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.SetSearchFilter(GetFilter(), (int)SearchableEditorWindow.SearchMode.All);

			while (hierarchyProperty.Next(null))
			{
				AddAssetInfo(hierarchyProperty);
			}

			FilterAssets(ref loadedAssets);
			loadedAssets.ForEach(SortChildren);

			loadedAssetsFlat.Clear();
			BuildFlatAssets(loadedAssets);

			UpdateFilter();
			if (selectionCallback != null)
			{
				SelectedAssetEntry = loadedAssetsFlat.FirstOrDefault();
			}
		}

	    private string GetFilter()
	    {
	        stringBuilder.Remove(0, stringBuilder.Length);

	        stringBuilder.AppendFormat("t:{0} ", typeFilter);

            if (labelsFilter != null)
            {
                foreach (string label in labelsFilter)
                {
                    stringBuilder.AppendFormat("l:{0} ", label);
                }
            }

	        return stringBuilder.ToString();
	    }

		private void AddAssetInfo(HierarchyProperty hp)
		{
			var path = AssetDatabase.GUIDToAssetPath(hp.guid);
			var folders = path.Split('/');
			var list = loadedAssets;
			HierarchyEntry parent = null;
			for (int ii = 1; ii < folders.Length - 1; ii++) // 1 to skip Assets folder, -1 to skip asset name
			{
				var folder = folders[ii];
				var p = parent;
				parent = list.SingleOrDefault(e => e.Name == folder);
				if (parent == null)
				{
					parent = new HierarchyEntry
					{
						Parent = p,
						Name = folder,
						Path = string.Join("/", folders, 0, ii + 1),
					};
					list.Add(parent);
					parent.Expanded = !foldedPaths.Contains(parent.Path);
				}
				list = parent.Children = parent.Children ?? new List<HierarchyEntry>();
			}
			var hierarchyEntry = new HierarchyEntry
			{
				Parent = parent,
				Name = hp.name,
				Path = path,
				AssetGuid = hp.guid,
			};
			list.Add(hierarchyEntry);
		}

		private void SortChildren(HierarchyEntry entry)
		{
			if (entry.Children == null)
			{
				return;
			}

			entry.Children.Sort();
			foreach (var child in entry.Children)
			{
				SortChildren(child);
			}
		}

		private void FilterAssets(ref List<HierarchyEntry> entries)
		{
			bool prefabType = typeof(MonoBehaviour).IsAssignableFrom(AssetType);
			if (!prefabType
				&& filters.Count <= 0)
			{
				return;
			}

			if (entries == null)
			{
				return;
			}

			entries.ForEach(e => FilterAssets(ref e.Children));

			entries = entries
				.Where(FitsFilters)
				.ToList();
		}

		private bool FitsFilters(HierarchyEntry he)
		{
			if (he.Children != null && he.Children.Count > 0)
				return true;
			foreach (var filter in filters)
			{
				if (!filter(he))
					return false;
			}

			return true;
		}

		private void BuildFlatAssets(List<HierarchyEntry> entries)
		{
			foreach (var e in entries)
			{
				if (e.Children != null)
				{
					BuildFlatAssets(e.Children);
				}
				else
				{
					loadedAssetsFlat.Add(e);
				}
			}
		}
	}
}