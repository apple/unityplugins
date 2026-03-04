using UnityEngine;
using UnityEditor;

public class Editor_Accessory_Tracking : EditorWindow
{
    // Reference to your prefab - you'll need to assign this in the inspector or load it from Resources
    [SerializeField] private GameObject accessoryTrackingPrefab;

    private bool doesWantToSetupCurrentScene = false;


    /// <summary>
    /// shows which view in the editor is selected
    /// </summary>
    private int ToolBarSelection = 0;

    private UnityEngine.Vector2 SceneSetupScrollingPosition = UnityEngine.Vector2.zero;

    private const string AccessoryTrackingPrefabName = "Apple-AccessoryTracking";

    [MenuItem("Apple/Spatial Accessory Tracking")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one
        Editor_Accessory_Tracking window = GetWindow<Editor_Accessory_Tracking>("Apple - Spatial Accessory Tracking");
        // window.minSize = new Vector2(300, 500);
        window.Show();
    }

    

    void OnGUI()
    {
        ToolBarView();
        SceneSetupView();
        APIReferenceView();
        ScriptingReferenceView();
        DocumentationView();

        EditorGUILayout.Space(20);

        GUILayout.FlexibleSpace();
    }


    /// <summary>
    /// Adds the Accessory Tracking Prefab to the Current Scene
    /// If Accessory Tracking Prefab already exists focus on the gameobject
    /// </summary>
    public void CreateAccessoryTrackingPrefab()
    {
        // Check if an object with the name "Apple-AccessoryTracking" already exists
        GameObject existingObject = GameObject.Find(AccessoryTrackingPrefabName);
        if (existingObject != null)
        {
            Debug.LogWarning("Apple-AccessoryTracking already exists in the scene. Selecting existing object instead of creating a new one.");

            // Select the existing object
            Selection.activeGameObject = existingObject;

            // Focus the scene view on the existing object
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }

            return; // Exit early, don't create a new object
        }

        if (accessoryTrackingPrefab == null)
        {
            Debug.LogError("No prefab assigned!");
            return;
        }

        // Instantiate the prefab in the scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(accessoryTrackingPrefab);

        if (instance is null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            return;
        }


        //make sure the prefab instance isn't null
        if (instance != null)
        {
            // Position it at the scene view camera position or origin
            if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
            {
                // Force position to 0,0,0
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                // Select the created object in the hierarchy
                Selection.activeGameObject = instance;
            }
            else
            {
                // Default to origin
                instance.transform.position = Vector3.zero;
            }

            // Select the created object in the hierarchy
            Selection.activeGameObject = instance;

            // Focus the scene view on the object
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }

            // Mark the scene as dirty so Unity knows it has been modified
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log($"Created Apple Spatial Tracking prefab: {instance.name}");
        }
        else
        {
            Debug.LogError("Failed to instantiate prefab!");
        }
    }

    /// <summary>
    /// Opens the URL for apple documentation to the browser
    /// </summary>
    [MenuItem("Apple/Spatial Accessory Tracking Documentation")]
    public static void OpenAppleDocs()
    {
        Application.OpenURL("https://developer.apple.com/documentation/arkit/accessorytrackingprovider?changes=__3");
    }

    public void ToolBarView()
    {

        // GUILayout.Label("ToolBarSelection: " + ToolBarSelection);

        EditorGUILayout.BeginHorizontal(EditorStyles.textField);

        if (GUILayout.Button("Scene Setup", GUILayout.Height(25)))
        {
            ToolBarSelection = 0;
        }

        if (GUILayout.Button("Tutorial", GUILayout.Height(25)))
        {
            ToolBarSelection = 1;
        }

        if (GUILayout.Button("Documentation", GUILayout.Height(25)))
        {

            ToolBarSelection = 3;
        }

        EditorGUILayout.EndHorizontal();
    }

    public void SceneSetupView()
    {

        if (ToolBarSelection != 0)
        {
            return;
        }

        EditorGUILayout.BeginVertical();


        //only show this button if the developer hasn't selected to setup the scene
        if (!doesWantToSetupCurrentScene)
        {

            GUILayout.Space(10);

            GUILayout.Label("Add the Spatial Accessory Tracking Prefab to your current scene.", EditorStyles.helpBox);

            if (GUILayout.Button("Create Spatial Accessory Tracking Prefab in Current Scene"))
            {
                Debug.Log("Enabling Setup for Accessory Tracking in Current Scene");
                doesWantToSetupCurrentScene = !doesWantToSetupCurrentScene;
            }
        }
        else
        {
            if (GUILayout.Button("Cancel Accessory Tracking Prefab Setup", GUILayout.Height(30)))
            {
                Debug.Log("Cancelling Setup for Accessory Tracking in Current Scene");
                doesWantToSetupCurrentScene = !doesWantToSetupCurrentScene;
            }
        }

        //only show this button if the user wants to setup the scene
        if (doesWantToSetupCurrentScene)
        {
            GUILayout.Space(20);

            // Prefab reference field
            GUILayout.Label("Accessory Tracking Prefab:", EditorStyles.boldLabel);
            accessoryTrackingPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Prefab",
                accessoryTrackingPrefab,
                typeof(GameObject),
                false
            );

            GUILayout.Space(10);

            // Create prefab button
            GUI.enabled = accessoryTrackingPrefab != null;
            if (GUILayout.Button("Create Accessory Tracking Prefab in Current Scene"))
            {
                CreateAccessoryTrackingPrefab();
            }
            GUI.enabled = true;

            if (accessoryTrackingPrefab == null)
            {
                EditorGUILayout.HelpBox("Please assign an Accessory Tracking prefab above.", MessageType.Warning);
            }

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

    }

    private void CreatePluginsFolder()
    {
        string pluginsFolderPath = "Assets/Plugins";
        
        // Check if the folder already exists
        if (AssetDatabase.IsValidFolder(pluginsFolderPath))
        {
            Debug.Log("Plugins folder already exists!");
            EditorUtility.DisplayDialog("Folder Exists", "The Plugins folder already exists in the Assets directory.", "OK");
            return;
        }
        
        // Create the folder
        string guid = AssetDatabase.CreateFolder("Assets", "Plugins");
        
        if (!string.IsNullOrEmpty(guid))
        {
            Debug.Log("Plugins folder created successfully!");
            EditorUtility.DisplayDialog("Success", "Plugins folder has been created in the Assets directory.", "OK");
            
            // Refresh the Asset Database to show the new folder immediately
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Failed to create Plugins folder!");
            EditorUtility.DisplayDialog("Error", "Failed to create the Plugins folder.", "OK");
        }
    }

    private UnityEngine.Vector2 APIReferenceScrollPosition = UnityEngine.Vector2.zero;

    public void APIReferenceView()
    {

        if (ToolBarSelection != 2)
        {
            return;
        }

        GUILayout.Label("API Reference goes here");

        GUILayout.Space(10);

        // start of the scroll position
        APIReferenceScrollPosition = EditorGUILayout.BeginScrollView(APIReferenceScrollPosition, GUILayout.ExpandHeight(true));

        EditorGUILayout.LabelField("-----------", EditorStyles.boldLabel);

        GUILayout.Space(10);

        // Enums Section
        EditorGUILayout.LabelField("Enums", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ControllerInputName", EditorStyles.label);
        EditorGUILayout.LabelField("Enumeration of controller input names for buttons and directional pads");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("AccessoryChirality", EditorStyles.label);
        EditorGUILayout.LabelField("Enumeration representing the handedness or chirality of an accessory");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("AccessoryTrackingState", EditorStyles.label);
        EditorGUILayout.LabelField("Enumeration representing the tracking state of an accessory");
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("-----------", EditorStyles.boldLabel);

        // Classes Section
        EditorGUILayout.LabelField("Classes", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Controller", EditorStyles.label);
        EditorGUILayout.LabelField("Represents a spatial controller device with its properties and capabilities");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Accessory", EditorStyles.label);
        EditorGUILayout.LabelField("Represents an accessory that can be attached to a controller");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("AccessoryAnchor", EditorStyles.label);
        EditorGUILayout.LabelField("Represents a tracked anchor for an accessory with position, velocity, and tracking information");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ControllerInputState", EditorStyles.label);
        EditorGUILayout.LabelField("Represents the complete input state of a controller, including all buttons and directional pads");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ControllerState", EditorStyles.label);
        EditorGUILayout.LabelField("Represents the complete state of a controller, including input, battery, and accessory information");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Symbol", EditorStyles.label);
        EditorGUILayout.LabelField("Represents a symbol or icon with image data and dimensions");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Plugin", EditorStyles.label);
        EditorGUILayout.LabelField("Main plugin class providing the public API for spatial controller functionality");
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("-----------", EditorStyles.boldLabel);

        // Structs Section
        EditorGUILayout.LabelField("Structs", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("LocationName", EditorStyles.label);
        EditorGUILayout.LabelField("Represents a location name where an accessory can be attached (nested in Accessory)");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ButtonState", EditorStyles.label);
        EditorGUILayout.LabelField("Represents the state of a button input on a controller");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("DPadState", EditorStyles.label);
        EditorGUILayout.LabelField("Represents the state of a directional pad input on a controller");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ControllerBatteryState", EditorStyles.label);
        EditorGUILayout.LabelField("Represents the battery state of a controller");
        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("TimeValue", EditorStyles.label);
        EditorGUILayout.LabelField("Represents a time value for spatial controller operations with arithmetic operators");
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("-----------", EditorStyles.boldLabel);

        // Delegates Section
        EditorGUILayout.LabelField("Delegates", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("ControllerCallback", EditorStyles.label);
        EditorGUILayout.LabelField("Callback delegate for controller connection and disconnection events");
        EditorGUILayout.EndVertical();

        // End scroll view
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
    }

    public void ScriptingReferenceView()
    {
        if (ToolBarSelection != 1)
        {
            return;
        }

        ///
        /// Show Basic Setup Documentation
        ///     

        EditorGUILayout.LabelField("A tutorial on how to script Apple - Spatial Accessory Tracking. Copy and Paste the code into your own script.", EditorStyles.helpBox);
        GUILayout.Space(20);

        SceneSetupScrollingPosition = EditorGUILayout.BeginScrollView(SceneSetupScrollingPosition, GUILayout.ExpandHeight(true));

        ///
        /// add the prefab to your scene
        /// 
        EditorGUILayout.LabelField("Add the Apple - Spatial Accessory Tracking Prefab", EditorStyles.boldLabel);

        string addprefabCodeSnippet = "Add the accessory tracking prefab to your scene. Select Scene Setup and follow the steps. Return here when finished";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(addprefabCodeSnippet, EditorStyles.textArea, GUILayout.Height(35));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        ///
        /// Namespace
        /// 

        EditorGUILayout.LabelField("Add the following namespace to your script:", EditorStyles.boldLabel);

        string namespaceCodeSnippet = "using Apple.visionOS.AccessoryTracking;";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(namespaceCodeSnippet, EditorStyles.textArea, GUILayout.Height(25));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        ///
        /// Initialize the plugin
        /// 

        EditorGUILayout.LabelField("Initialize Apple - Spatial Accessory Tracking Plugin", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Do this as early as possible in your monobehavior. Awake() or OnEnable()", EditorStyles.helpBox);

        string initializePluginCodeSnippet =
@"        public void Awake() {
            Plugin.Init();
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(initializePluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(60));
        EditorGUILayout.EndVertical();

        ///
        /// Deinitialize the plugin
        /// 

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Deinitialize Apple - Spatial Accessory Tracking Plugin", EditorStyles.boldLabel);
        // EditorGUILayout.LabelField("Do this as early as possible in your monobehavior. Awake() or OnEnable()", EditorStyles.helpBox);

        string deinitializePluginCodeSnippet =
@"        public void OnDisable() {
            Plugin.Destroy();
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(deinitializePluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(60));
        EditorGUILayout.EndVertical();

        ///
        /// Grab a reference to the AccessoryTrackingHelper
        /// 

        GUILayout.Space(10);

        EditorGUILayout.LabelField("GetComponent<AccessoryTrackingHelper>()", EditorStyles.boldLabel);

        string grabGetComponentPluginCodeSnippet =
@"        public AccessoryTrackingHelper ACTrackingHelper;

        public void Awake() {
            ACTrackingHelper = GetComponent<AccessoryTrackingHelper>();
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(grabGetComponentPluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(100));
        EditorGUILayout.EndVertical();

        ///
        /// Subscribe to the unity events
        /// 
        /// public static UnityEvent<string> ControllerConnectedEvent;
        /// public static UnityEvent<string> ControllerDisconnectedEvent;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Subscribe To Each Event", EditorStyles.boldLabel);

        string subscribeToUnityEventsPluginCodeSnippet =
@"        ///Create New Unity Events or the events won't invoke
        ACTrackingHelper.ControllerConnectedEvent = new UnityEvent<string>();
        ACTrackingHelper.ControllerDisconnectedEvent = new UnityEvent<string>();

        ///subscribe to each event
        ACTrackingHelper.ControllerConnectedEvent.AddListener(ControllerAdded);
        ACTrackingHelper.ControllerConnectedEvent.AddListener(ControllerRemoved);

        public void ControllerAdded(string id) {
            //controller added
        }

        public void ControllerRemoved(string id) {
            //controller removed
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(subscribeToUnityEventsPluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(320));
        EditorGUILayout.EndVertical();

        ///
        /// Get Controller Accessories
        /// 

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Get Controller Accessories", EditorStyles.boldLabel);

        string getControllerAccessoriesPluginCodeSnippet =
@"        public void Update()
        {
            //get all the accessories
            Accessory[] accessories = Plugin.GetConnectedAccessories();
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(getControllerAccessoriesPluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(80));
        EditorGUILayout.EndVertical();

        ///
        /// Get the state from the accessories
        /// 

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Get Input, Anchors, and State", EditorStyles.boldLabel);

        string getStateAccessoriesPluginCodeSnippet =
@"        //loop through the accessories to get input, state, and transforms
        foreach (var accessory in ACTrackingHelper.accessories) {
            //get the uid from the accessory
            var uid = accessory.source.uniqueId;

            //what is the handedness of the user
            var hand = accessory.inherentChirality;

            //poll the state of the accessories
            //this is the mort important step as it will give us updated transforms, input, and state of the accessory
            var state = AccessoryTracking.PollController(uid);
        }";


        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(getStateAccessoriesPluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(280));
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space(10);


        EditorGUILayout.LabelField("Get Anchors From State", EditorStyles.boldLabel);

        string getStateAnchorsPluginCodeSnippet =
@"        var state = AccessoryTracking.PollController(uid);
        //confirm the accessory tracking has any anchors
        if (state.accessoryAnchors.Length > 0)
        {
            //we need to predict a time in the future so our anchors are where the controllers will be
            //this number can be altered to fit the needs but it will impact tracking
            const double DefaultFrameLatencySeconds = 0.022;
            var timeNow = AccessoryTracking.GetCurrentTime();
            var timeNextFrame = AccessoryTracking.GetPredictedNextFrameTime();
            var timeDisplayed = timeNextFrame.isValid() && timeNextFrame >= timeNow ? timeNextFrame : timeNow + DefaultFrameLatencySeconds;
            
            //ask for a predicted frame
            var accessoryAnchor = AccessoryTracking.PredictAnchor(uid, timeDisplayed);
            
            //we need to make sure the anchor is not null
            if (accessoryAnchor != null)
            {
                if (hand == AccessoryChirality.Left)
                {
                    //Apply the anchor transform to our GameObject LeftController's transform
                    SpatialControllerUtils.ApplyAccessoryAnchorTransform(LeftController, accessoryAnchor);
                }
                else if (hand == AccessoryChirality.Right)
                {
                    //Apply the anchor transform to our GameObject RightController's transform
                    SpatialControllerUtils.ApplyAccessoryAnchorTransform(RightController, accessoryAnchor);
                }
            }
        }";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(getStateAnchorsPluginCodeSnippet, EditorStyles.textArea, GUILayout.Height(550));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);



        ///
        /// Get State from Accessory
        /// 

        EditorGUILayout.LabelField("Get the Input object from State", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("The logic for input is much different than the transforms from anchors", EditorStyles.helpBox);

        var getStateCodeSnippet =
@"        /// get the input from the PollController(uid)
        /// this input object will give us button presses
        var input = state.input;";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(getStateCodeSnippet, EditorStyles.textArea, GUILayout.Height(60));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        ///
        /// Getting Input
        /// 
        
        EditorGUILayout.LabelField("Checking Input", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("This can be implemented with Unity's Input System", EditorStyles.helpBox);

        var showInputCodeSnippet =
@"        //float from 0 or 1
        var buttonAValue = input.buttons[ControllerInputName.ButtonA].value;

        //check if we are detecting the left controller pressing a button
        if (SpatialControllerUtils.IsLeftController(state.accessory))
        {
            //does the button have a value?
            if (buttonAValue != 0)
            {
                //button pressed
            }
            else
            {
                //button not pressed
            }
        }

        ///
        /// repeat the steps above for each button
        ///
        var buttonBValue = input.buttons[ControllerInputName.ButtonB].value;
        var menuButton = input.buttons[ControllerInputName.ButtonMenu].value;
        var gripButton = input.buttons[ControllerInputName.ButtonGrip].value;
        var thumbStickButton = input.buttons[ControllerInputName.ButtonThumbstick].value;

        //trigger - float from 0 to 1
        var triggerButton = input.buttons[ControllerInputName.ButtonTrigger].value;

        // float from -1 to 1
        var thumbStickX = input.dpads[ControllerInputName.DPadThumbstick].xAxis;
        var thumbStickY = input.dpads[ControllerInputName.DPadThumbstick].yAxis;";

        // Create a text area with the code snippet
        EditorGUILayout.BeginVertical();
        EditorGUILayout.SelectableLabel(showInputCodeSnippet, EditorStyles.textArea, GUILayout.Height(500));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);


        // End scroll view
        EditorGUILayout.EndScrollView();

    }

    public void DocumentationView()
    {

        if (ToolBarSelection != 3)
        {
            return;
        }

        GUILayout.Label("Accessory Tracking Documentation", EditorStyles.boldLabel);

        if (GUILayout.Button("Apple - Accessory Tracking Documentation"))
        {
            Application.OpenURL("https://developer.apple.com/documentation/arkit/accessory?changes=__3");
            Debug.Log("Opening Accessory Tracking Documentation");
        }

    }
}
