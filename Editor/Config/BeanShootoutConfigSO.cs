using System.IO;
using UnityEditor;
using UnityEngine;

public class BeanShootoutConfigSO : ScriptableObject
{
    [Tooltip("Used for build and run.")]
    public string GamePath;

    [Tooltip("Enables debugging features.")]
    public bool DebugMode;
    [Tooltip("Skips certain parts of loading, such as mod loading and loading screens.")]
    public bool FastLoad;
    [Tooltip("Enables verbose logging.")]
    public bool VerboseLogging = true;
    
    public bool FullscreenWhenTheresNoOtherPlayers;
    public bool ShowMinimap = true;
    public int MaxAmmo = 50;
    public int MaxPlayers = 4;

    public bool IsValid;
    public string ValidReason;
}

[CustomEditor(typeof(BeanShootoutConfigSO))]
public class BeanShootoutConfigSO_Inspector : Editor
{
    private BeanShootoutConfigSO config;

    // Properties
    private SerializedProperty GamePath;
    private SerializedProperty DebugMode;
    private SerializedProperty FastLoad;
    private SerializedProperty VerboseLogging;
    private SerializedProperty FullscreenWhenTheresNoOtherPlayers;
    private SerializedProperty ShowMinimap;
    private SerializedProperty MaxAmmo;
    private SerializedProperty MaxPlayers;

    private void OnEnable()
    {
        config = (BeanShootoutConfigSO)target;

        // Get properties
        GamePath = serializedObject.FindProperty("GamePath");
        DebugMode = serializedObject.FindProperty("DebugMode");
        FastLoad = serializedObject.FindProperty("FastLoad");
        VerboseLogging = serializedObject.FindProperty("VerboseLogging");
        FullscreenWhenTheresNoOtherPlayers = serializedObject.FindProperty("FullscreenWhenTheresNoOtherPlayers");
        ShowMinimap = serializedObject.FindProperty("ShowMinimap");
        MaxAmmo = serializedObject.FindProperty("MaxAmmo");
        MaxPlayers = serializedObject.FindProperty("MaxPlayers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(GamePath);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            config.GamePath = EditorUtility.OpenFolderPanel("Select folder", "", "");
        }
        GUILayout.EndHorizontal();

        // Button to make sure the build selected is valid
        if (GUILayout.Button("Validate Game Path"))
        {
            serializedObject.ApplyModifiedProperties();

            bool DataPathExists = false;
            bool SupportFileExists = false;

            // If BeanShootout_Data doesn't exist then what are you doing
            if (Directory.Exists(config.GamePath + "/BeanShootout_Data"))
            {
                DataPathExists = true;
            }

            if (!DataPathExists)
            {
                config.IsValid = false;
                config.ValidReason = "Game Path doesn't contain 'BeanShootout_Data', you may not have selected where the game is located.";
                EditorUtility.DisplayDialog("Error", "Game Path doesn't contain 'BeanShootout_Data', you may not have selected where the game is located.", "OK");

                return;
            }

            // SUPPORTS_CLP_BAR is a file in every modern build of Bean Shootout. If it doesn't exist, the build most likely doesn't support Build and Run.
            if (File.Exists(config.GamePath + "/BeanShootout_Data/SUPPORTS_CLP_BAR"))
            {
                SupportFileExists = true;
            }

            if (!SupportFileExists)
            {
                config.IsValid = false;
                config.ValidReason = "This game version doesn't support Build and Run, please make sure you have game version 1.0.0-PublicRelease or higher.";
                EditorUtility.DisplayDialog("Error", "This game version doesn't support Build and Run, please make sure you have game version 1.0.0-PublicRelease or higher.", "OK");
                
                return;
            }

            config.IsValid = true;
            config.ValidReason = "";
        }

        #region Launch Settings

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Launch Settings", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(DebugMode);
        EditorGUILayout.PropertyField(FastLoad);
        EditorGUILayout.PropertyField(VerboseLogging);

        #endregion

        #region Game Settings

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Game Settings", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.PropertyField(FullscreenWhenTheresNoOtherPlayers);
        EditorGUILayout.PropertyField(ShowMinimap);
        EditorGUILayout.PropertyField(MaxAmmo);
        EditorGUILayout.PropertyField(MaxPlayers);

        #endregion
        
        serializedObject.ApplyModifiedProperties();
    }
}