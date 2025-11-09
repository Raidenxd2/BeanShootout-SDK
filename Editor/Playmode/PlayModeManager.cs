using KillItMyself.Runtime;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayModeManager : Editor
{
    static PlayModeManager()
    {
        EditorApplication.playModeStateChanged += PlayModeState;
    }

    [MenuItem("Bean Shootout/_Enter Playmode on level")]
    public static void EnterPlaymode()
    {
        EnterPlaymode2();
    }

    public async static void EnterPlaymode2()
    {
        // Reload Domain must be disabled or else this wont work
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;

        // Wait 25 ms then enter playmode then wait another 25 ms and load the prefab
        await Task.Delay(25);

        EditorApplication.EnterPlaymode();

        await Task.Delay(25);

        // Load save
        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow") + "/OneWing/The Great Bean Shootout/Saves/Default.bean";
        if (!File.Exists(savePath))
        {
            EditorUtility.DisplayDialog(Constants.PackageName, "Warning: The default save file doesn't exist at " + savePath + ". A blank save file will be used instead.", "OK");
        }

        BetterPrefs.Load(savePath);

        // Set GameSettings
        GameSettings.FullscreenNoOtherPlayers = true;
        GameSettings.ShowMinimap = false;
        GameSettings.MovementSettings = AssetDatabase.LoadMainAssetAtPath("Packages/com.onewing.beanshootout-customlevels/Playmode/SO/MovementSettings/DefaultMovementSettings.asset") as PlayerMovementSettingsSO;

        Instantiate(AssetDatabase.LoadMainAssetAtPath("Packages/com.onewing.beanshootout-customlevels/Playmode/Prefabs/Core/DDOLMinimal.prefab") as GameObject);

        EditorSceneManager.LoadSceneInPlayMode("Packages/com.onewing.beanshootout-customlevels/Playmode/Scenes/Multiplayer/LocalLevelScene.unity", new(UnityEngine.SceneManagement.LoadSceneMode.Additive)); ;
    }

    private static void PlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Enable Reload Domain
            EditorSettings.enterPlayModeOptionsEnabled = false;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;
        }
    }
}