using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityPickers.Utility;

namespace UnityPickers
{
	// ReSharper disable StaticMemberInGenericType
	public abstract class ValuePicker<T> : EditorWindow
	{
		private static readonly Vector2 Size = new Vector2(200, 300);

		private static GUIStyle commonStyle;

		private static GUIStyle selectedStyle;

		[CanBeNull]
	    private T prevPicked;

		[NotNull]
		private Action<T> callback = t => { };

		[NotNull]
		private List<T> allValues = new List<T>();

		[NotNull]
		private List<T> displayValues = new List<T>();

		[CanBeNull]
		private T selected;

		[NotNull]
		private string searchString = "";

		[CanBeNull]
		private EditorWindow parent;

		private bool firstFrame = true;

		private bool scrollToSelected;

		private Vector2 scrollPosition;

		private static void InitStyles()
		{
			if (commonStyle == null)
			{
				commonStyle = new GUIStyle(EditorStyles.label);
				commonStyle.active = commonStyle.normal;
			}

			if (selectedStyle == null)
			{
				selectedStyle = new GUIStyle(EditorStyles.label);
				selectedStyle.normal = selectedStyle.focused;
				selectedStyle.active = selectedStyle.focused;
			}
		}

		private void Init(
			[NotNull] string buttonText, 
			[CanBeNull] EditorWindow parentWindow, 
			[NotNull] Action<T> selectionCallback, 
			[NotNull] IEnumerable<T> values)
		{
			InitStyles();

			titleContent = new GUIContent(buttonText);
			minSize = Size;
			maxSize = Size;

			parent = parentWindow;
			callback = selectionCallback;
			allValues = values.ToList();
			UpdateFilteredValues();
		}

		protected static void Button(
			[NotNull] Func<ValuePicker<T>> windowCreator,
			[NotNull] string buttonText,
			[NotNull] Func<IEnumerable<T>> valuesCollector,
			[NotNull] Action<T> callback, 
			bool showNow = false,
			[CanBeNull] GUIStyle style = null,
			[NotNull] params GUILayoutOption[] options)
		{
			var rect = GUILayoutUtility.GetRect(new GUIContent(buttonText), GUI.skin.button, options);
			Button(windowCreator, rect, buttonText, valuesCollector, callback, showNow, style);
		}

		protected static void Button(
			[NotNull] Func<ValuePicker<T>> windowCreator, 
			Rect rect,
			[NotNull] string buttonText,
			[NotNull] Func<IEnumerable<T>> valuesCollector,
			[NotNull] Action<T> callback, 
			bool showNow = false,
			[CanBeNull] GUIStyle style = null)
		{
			var parent = focusedWindow;
			var actualStyle = style ?? GUI.skin.button;
			if (showNow || GUI.Button(rect, buttonText, actualStyle))
			{
				var window = windowCreator();
				var values = valuesCollector();
				window.Init(buttonText, parent, callback, values);

				var screenRect = rect;
				screenRect.position = GUIUtility.GUIToScreenPoint(rect.position);
				window.ShowAsDropDown(screenRect, Size);
				window.Focus();
			}
		}

		private void OnGUI()
		{
			HandleHotkeys();
			DrawFilter();
			DrawContent();
		}

	    public void OnLostFocus()
		{
			Close();
		}

		private void DrawFilter()
		{
			string prevSearch = searchString;

			GUI.SetNextControlName("filter");
			searchString = GUILayout.TextField(searchString);
			if (firstFrame)
			{
				firstFrame = false;
				GUI.FocusControl("filter");
			}

			if (prevSearch != searchString)
				UpdateFilteredValues();
	    }

		// todo: restore
	    //private void DrawPrevPicked()
	    //{
	    //    if (m_PrevPicked != null && m_AllValues.Contains(m_PrevPicked))
	    //    {
	    //        string valueName = GetValueName(m_PrevPicked);
	    //        var style = m_PrevPicked.Equals(m_Selected) ? m_SelectedStyle : m_CommonStyle;
     //           var btnRect = GUILayoutUtility.GetRect(new GUIContent(valueName), style);
	    //        if (GUI.Button(btnRect, valueName, style))
	    //        {
	    //            if (m_PrevPicked.Equals(m_Selected))
	    //            {
	    //                Select(m_Selected);
	    //            }

	    //            m_Selected = m_PrevPicked;
	    //        }

     //           GUILayout.Box(GUIContent.none);
     //       }
	    //}

        protected virtual string GetValueName(T value)
		{
			return value.ToString();
		}

		private void DrawContent()
		{
			float selectedY = 0f;
			using (var scroll = new GUILayout.ScrollViewScope(scrollPosition))
			{
				foreach (var v in displayValues)
				{
					var style = v.Equals(selected) ? selectedStyle : commonStyle;

					string valueName = GetValueName(v);
					var btnRect = GUILayoutUtility.GetRect(new GUIContent(valueName), style);
					if (GUI.Button(btnRect, valueName, style))
					{
						if (v.Equals(selected))
						{
							Select(selected);
						}

						selected = v;
					}

					if (v.Equals(selected))
					{
						selectedY = btnRect.yMin;
					}

				    if (prevPicked != null && v.Equals(prevPicked))
				    {
				        GUILayout.Space(2);
                        Handles.DrawLine(new Vector3(btnRect.xMin, btnRect.yMax + 2), new Vector3(btnRect.xMax, btnRect.yMax + 2));
				    }
				}
				scrollPosition = scroll.scrollPosition;
			}

			if (Event.current.type == EventType.Repaint)
			{
				if (scrollToSelected)
				{
					scrollPosition.y = selectedY - 150;
					scrollToSelected = false;
					Repaint();
				}
			}
		}

		private void HandleHotkeys()
		{
			var e = Event.current;
			if (e.type != EventType.KeyDown)
				return;

			switch (e.keyCode)
			{
				case KeyCode.DownArrow:
					int i1 = displayValues.IndexOf(selected);
					if (i1 < displayValues.Count - 1)
						selected = displayValues[i1 + 1];
					scrollToSelected = true;
					e.Use();
					break;
				case KeyCode.UpArrow:
					int i2 = displayValues.IndexOf(selected);
					if (i2 > 0)
						selected = displayValues[i2 - 1];
					scrollToSelected = true;
					e.Use();
					break;
				case KeyCode.Return:
					Select(selected);
					e.Use();
					break;
				case KeyCode.Escape:
					Cancel();
					e.Use();
					break;
			}
		}

		private void Select(T result)
		{
			Close();
			if (result != null)
			{
			    prevPicked = result;
				callback(result);
			}
			if (parent != null)
			{
				parent.Repaint();
				parent.Focus();
			}
		}

		private void Cancel()
		{
			Close();
			if (parent != null)
			{
				parent.Repaint();
				parent.Focus();
			}
		}


		private void UpdateFilteredValues()
		{
			if (searchString == "")
			{
				displayValues = allValues;
			}
			else
			{
				displayValues = allValues.Where(t => StringUtils.MatchesFilter(GetValueName(t), searchString)).ToList();
			}

		    if (prevPicked != null)
		    {
		        int lastPickedIndex = displayValues.IndexOf(prevPicked);
		        if (lastPickedIndex > 0)
		        {
		            displayValues.RemoveAt(lastPickedIndex);
		            displayValues.Insert(0, prevPicked);
		        }
		    }

            if (!displayValues.Contains(selected))
				selected = displayValues.FirstOrDefault();
			scrollToSelected = true;
		}
	}
}