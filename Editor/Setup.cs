using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public class Setup
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
        
        if (PlayerSettings.colorSpace != ColorSpace.Linear)
        {
            if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, "This project is currently using the Gamma color space. This will cause the color in-game to look off, and is also unsupported. Would you like to change the color space to Linear?", "Yes", "No", DialogIconType.Warning))
            {
                PlayerSettings.colorSpace = ColorSpace.Linear;
            }
        }
    }

    [MenuItem("Bean Shootout/Dev/Force initial setup")]
    public static void InitialSetup()
    {
        CreateConfig();

        if (!newSystemBackendsEnabled)
        {
            if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, "This project is currently configured to use the legacy Input Manager, which is not supported by Bean Shootout. Do you want to change the active input handling to the Input System? This will restart the Unity Editor.", "Yes", "No", DialogIconType.Warning))
            {
                EnableNewBackends();

                return;
            }
        }

        if (newSystemBackendsEnabled && oldSystemBackendsEnabled)
        {
            if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, "This project is currently configured to use both the legacy Input Manager and the new Input System. The legacy Input Manager is not supported by Bean Shootout. Do you want to change the active input handling to be only the new Input System? this will restart the Unity Editor.", "Yes", "No", DialogIconType.Warning))
            {
                EnableNewBackends();

                return;
            }
        }

        if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, "You have just installed the Bean Shootout SDK, it is highly recommended that you install this package in an empty Unity project. Do you want to setup this project for the SDK?", "Yes", "No", DialogIconType.Info))
        {
            try
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

                SerializedProperty layers = tagManager.FindProperty("layers");
                if (layers == null)
                {
                    SetupErrorDialog("layers was null");
                    return;
                }

                List<LayerValue> layerValues = new();
                layerValues.Add(new(6, "Ground"));
                layerValues.Add(new(7, "PlayerCollision"));
                layerValues.Add(new(8, "MinimapOnly"));
                layerValues.Add(new(9, "MinimapNoRenderGround"));
                layerValues.Add(new(10, "MinimapNoRenderDefault"));
                layerValues.Add(new(11, "Mover"));
                layerValues.Add(new(12, "BombCollision"));
                layerValues.Add(new(13, "Player"));
                layerValues.Add(new(14, "MinimapNoRenderGround2"));
                layerValues.Add(new(15, "Bossfight/JumbotronScreen"));
                layerValues.Add(new(16, "Canon"));
                layerValues.Add(new(17, "Cutscene"));
                layerValues.Add(new(18, "WinnersGenericLevel"));

                foreach (var layerValue in layerValues)
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(layerValue.LayerIndex);
                    layerSP.stringValue = layerValue.LayerName;
                }

                tagManager.ApplyModifiedProperties();
                
                // Windows GraphicsAPI
                GraphicsDeviceType[] requiredGDTWindows = {GraphicsDeviceType.Direct3D11, GraphicsDeviceType.Direct3D12, GraphicsDeviceType.Vulkan};
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows, false);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, requiredGDTWindows);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, requiredGDTWindows);
                
                // Linux GraphicsAPI
                GraphicsDeviceType[] requiredGDTLinux = { GraphicsDeviceType.Vulkan };
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneLinux64, requiredGDTLinux);

                // macOS GraphicsAPI
                GraphicsDeviceType[] requiredGDTMac = { GraphicsDeviceType.Metal };
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneOSX, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneOSX, requiredGDTMac);

                Physics.gravity = new(0, -15.82f, 0);

                EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;

                PlayerSettings.colorSpace = ColorSpace.Linear;
                
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/GraphicsSettings.asset", Application.dataPath + "/../ProjectSettings/GraphicsSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/URPProjectSettings.asset", Application.dataPath + "/../ProjectSettings/URPProjectSettings.asset", true);
                File.Copy("Packages/com.onewing.beanshootout-customlevels/Editor/Setup/ProjectSettings~/QualitySettings.asset", Application.dataPath + "/../ProjectSettings/QualitySettings.asset", true);
                
                File.WriteAllText("Assets/DO_NOT_DELETE_THIS_BeanShootout", "This file is used to check if setup has been done. Do not delete this file.");
                
                EditorApplication.OpenProject(Directory.GetCurrentDirectory());
            }
            catch (Exception ex)
            {
                EditorDialog.DisplayAlertDialog(Constants.PackageName, "Failed to setup project.\n" + ex, "OK", DialogIconType.Error);
                
                Debug.LogException(ex);
            }
        }
    }

    private static void SetupErrorDialog(string error)
    {
        Debug.LogError(error);
        EditorUtility.DisplayDialog(Constants.PackageName, "An error occurred during the setup process. Please check the console for details.", "OK");
    }

    public static void EnableNewBackends()
    {
        newSystemBackendsEnabled = true;
        oldSystemBackendsEnabled = false;
        
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }

    // Creates an config if it doesn't exist
    public static void CreateConfig()
    {
        if (!File.Exists("Assets/BeanShootoutConfig.asset"))
        {
            Debug.Log("(BeanShootout) Creating config");
            BeanShootoutConfigSO config = ScriptableObject.CreateInstance<BeanShootoutConfigSO>();

            AssetDatabase.CreateAsset(config, "Assets/BeanShootoutConfig.asset");
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Whether the backends for the new input system are enabled in the
    /// player settings for the Unity runtime.
    /// </summary>
    public static bool newSystemBackendsEnabled
    {
        get
        {
            var property = GetPropertyOrNull(kActiveInputHandler);
            return property == null || ActiveInputHandlerToTuple(property.intValue).newSystemEnabled;
        }
        set
        {
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
            var property = GetPropertyOrNull(kActiveInputHandler);
            return property == null || ActiveInputHandlerToTuple(property.intValue).oldSystemEnabled;
        }
        set
        {
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
        }
    }

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

    private static SerializedProperty GetPropertyOrNull(string name)
    {
        var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
        
        if (playerSettings == null)
            return null;
        var playerSettingsObject = new SerializedObject(playerSettings);
        return playerSettingsObject.FindProperty(name);
    }
}