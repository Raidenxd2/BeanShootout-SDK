using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

[InitializeOnLoad]
public class Setup : EditorWindow
{
    static Setup()
    {
        if (!File.Exists("Assets/DO_NOT_DELETE_THIS_BeanShootout"))
        {
            InitialSetup();
        }

        if (!File.Exists("Assets/BeanShootoutConfig.asset"))
        {
            CreateConfig();
        }
    }

    [MenuItem("Bean Shootout/Dev/Force initial setup")]
    public static void InitialSetup()
    {
        CreateConfig();

        if (!newSystemBackendsEnabled)
        {
            if (EditorUtility.DisplayDialog(Constants.PackageName, "This project is currently configured to use the legacy Input Manager, which is not supported by Bean Shootout. Do you want to change the active input handling to the Input System? This will restart the Unity Editor.", "Yes", "No"))
            {
                EnableNewBackends();
            }
        }

        if (newSystemBackendsEnabled && oldSystemBackendsEnabled)
        {
            if (EditorUtility.DisplayDialog(Constants.PackageName, "This project is currently configured to use both the legacy Input Manager and the new Input System. The legacy Input Manager is not supported by Bean Shootout. Do you want to change the active input handling to be only the new Input System? this will restart the Unity Editor.", "Yes", "No"))
            {
                EnableNewBackends();
            }
        }

        ShowWindow();
    }

    public static void EnableNewBackends()
    {
        newSystemBackendsEnabled = true;
        oldSystemBackendsEnabled = false;

        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }

    [MenuItem("Bean Shootout/Setup...")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(Setup));
        window.minSize = new(525, 90);
        window.maxSize = new(525, 90);
    }

    private void OnGUI()
    {
        GUILayout.Label("This seems to be your first time using this package. In order to properly use this package, you must setup your project for it. Press the 'Setup/Update' button to setup your project.\n\nIt's highly recommended to not use this on an existing project.", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Setup/Update"))
        {
            // Copys certain Project Settings from the package to the project's ProjectSettings folder
            if (EditorUtility.DisplayDialog(Constants.PackageName, "Are you sure you want to setup your project for this package? WARNING: This will overwrite most Project Settings and the Editor will restart.", "Yes", "No"))
            {
                EditorUtility.DisplayProgressBar(Constants.PackageName, "Setting up...", 0);
                
                Directory.CreateDirectory("Assets/Levels");

                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/TagManager.asset", Application.dataPath + "/../ProjectSettings/TagManager.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/DynamicsManager.asset", Application.dataPath + "/../ProjectSettings/DynamicsManager.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/GraphicsSettings.asset", Application.dataPath + "/../ProjectSettings/GraphicsSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/Physics2DSettings.asset", Application.dataPath + "/../ProjectSettings/Physics2DSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/URPProjectSettings.asset", Application.dataPath + "/../ProjectSettings/URPProjectSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/QualitySettings.asset", Application.dataPath + "/../ProjectSettings/QualitySettings.asset", true);

                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/UniversalRenderPipelineGlobalSettings.asset~", Application.dataPath + "/UniversalRenderPipelineGlobalSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/UniversalRenderPipelineGlobalSettings.asset.meta~", Application.dataPath + "/UniversalRenderPipelineGlobalSettings.asset.meta", true);

                EditorUtility.DisplayDialog(Constants.PackageName, "If a dialog says something about the Input System, press Yes on it.", "OK");

                File.WriteAllText(Application.dataPath + "/DO_NOT_DELETE_THIS_BeanShootout", "");

                EditorApplication.OpenProject(Directory.GetCurrentDirectory());
            }
        }
    }

    // Creates an config if it doesn't exist
    public static void CreateConfig()
    {
        Debug.Log("(BeanShootout) Creating config");
        BeanShootoutConfigSO config = ScriptableObject.CreateInstance<BeanShootoutConfigSO>();

        AssetDatabase.CreateAsset(config, "Assets/BeanShootoutConfig.asset");
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Whether the backends for the new input system are enabled in the
    /// player settings for the Unity runtime.
    /// </summary>
    public static bool newSystemBackendsEnabled
    {
        get
        {
#if UNITY_2020_2_OR_NEWER
            var property = GetPropertyOrNull(kActiveInputHandler);
            return property == null || ActiveInputHandlerToTuple(property.intValue).newSystemEnabled;
#else
                var property = GetPropertyOrNull(kEnableNewSystemProperty);
                return property == null || property.boolValue;
#endif
        }
        set
        {
#if UNITY_2020_2_OR_NEWER
            var property = GetPropertyOrNull(kActiveInputHandler);
            if (property != null)
            {
                var tuple = ActiveInputHandlerToTuple(property.intValue);
                tuple.newSystemEnabled = value;
                property.intValue = TupleToActiveInputHandler(tuple);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogError($"Cannot find '{kActiveInputHandler}' in player settings");
            }
#else
                var property = GetPropertyOrNull(kEnableNewSystemProperty);
                if (property != null)
                {
                    property.boolValue = value;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"Cannot find '{kEnableNewSystemProperty}' in player settings");
                }
#endif
        }
    }



    /// <summary>
    /// Whether the backends for the old input system are enabled in the
    /// player settings for the Unity runtime.
    /// </summary>
    public static bool oldSystemBackendsEnabled
    {
        get
        {
#if UNITY_2020_2_OR_NEWER
            var property = GetPropertyOrNull(kActiveInputHandler);
            return property == null || ActiveInputHandlerToTuple(property.intValue).oldSystemEnabled;
#else
                var property = GetPropertyOrNull(kDisableOldSystemProperty);
                return property == null || !property.boolValue;
#endif
        }
        set
        {
#if UNITY_2020_2_OR_NEWER
            var property = GetPropertyOrNull(kActiveInputHandler);
            if (property != null)
            {
                var tuple = ActiveInputHandlerToTuple(property.intValue);
                tuple.oldSystemEnabled = value;
                property.intValue = TupleToActiveInputHandler(tuple);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogError($"Cannot find '{kActiveInputHandler}' in player settings");
            }
#else
                var property = GetPropertyOrNull(kDisableOldSystemProperty);
                if (property != null)
                {
                    property.boolValue = !value;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"Cannot find '{kDisableOldSystemProperty}' in player settings");
                }
#endif
        }
    }

#if UNITY_2020_2_OR_NEWER
    private const string kActiveInputHandler = "activeInputHandler";

    private enum InputHandler
    {
        OldInputManager = 0,
        NewInputSystem = 1,
        InputBoth = 2
    };

    private static (bool newSystemEnabled, bool oldSystemEnabled) ActiveInputHandlerToTuple(int value)
    {
        switch ((InputHandler)value)
        {
            case InputHandler.OldInputManager:
                return (false, true);
            case InputHandler.NewInputSystem:
                return (true, false);
            case InputHandler.InputBoth:
                return (true, true);
            default:
                throw new ArgumentException($"Invalid value of 'activeInputHandler' setting: {value}");
        }
    }

    private static int TupleToActiveInputHandler((bool newSystemEnabled, bool oldSystemEnabled) tuple)
    {
        switch (tuple)
        {
            case (false, true):
                return (int)InputHandler.OldInputManager;
            case (true, false):
                return (int)InputHandler.NewInputSystem;
            case (true, true):
                return (int)InputHandler.InputBoth;
            // Special case, when using two separate bool's of the public API here,
            // it's possible to end up with both settings in false, for example:
            // - EditorPlayerSettingHelpers.newSystemBackendsEnabled = true;
            // - EditorPlayerSettingHelpers.oldSystemBackendsEnabled = false;
            // - EditorPlayerSettingHelpers.newSystemBackendsEnabled = false;
            // - EditorPlayerSettingHelpers.oldSystemBackendsEnabled = true;
            // On line 3 both settings will be false, even if we set old system to true on line 4.
            case (false, false):
                return (int)InputHandler.OldInputManager;
        }
    }

#else
        private const string kEnableNewSystemProperty = "enableNativePlatformBackendsForNewInputSystem";
        private const string kDisableOldSystemProperty = "disableOldInputManagerSupport";
#endif

    private static SerializedProperty GetPropertyOrNull(string name)
    {
#if UNITY_6000_0_OR_NEWER
        // HOTFIX: the code below works around an issue causing an infinite reimport loop
        // this will be replaced by a call to an API in the editor instead of using reflection once it is available
        var buildProfileType = typeof(BuildProfile);
        var globalPlayerSettingsField = buildProfileType.GetField("s_GlobalPlayerSettings", BindingFlags.Static | BindingFlags.NonPublic);
        if (globalPlayerSettingsField == null)
        {
            Debug.LogError($"Could not find global player settings field in build profile when trying to get property {name}. Please try to update the Input System package.");
            return null;
        }
        var playerSettings = (PlayerSettings)globalPlayerSettingsField.GetValue(null);
        var activeBuildProfile = BuildProfile.GetActiveBuildProfile();
        if (activeBuildProfile != null)
        {
            var playerSettingsOverrideField = buildProfileType.GetField("m_PlayerSettings", BindingFlags.Instance | BindingFlags.NonPublic);
            if (playerSettingsOverrideField == null)
            {
                Debug.LogError($"Could not find player settings override field in build profile when trying to get property {name}. Please try to update the Input System package.");
                return null;
            }
            var playerSettingsOverride = (PlayerSettings)playerSettingsOverrideField.GetValue(activeBuildProfile);
            if (playerSettingsOverride != null)
                playerSettings = playerSettingsOverride;
        }
#else
            var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
#endif
        if (playerSettings == null)
            return null;
        var playerSettingsObject = new SerializedObject(playerSettings);
        return playerSettingsObject.FindProperty(name);
    }
}