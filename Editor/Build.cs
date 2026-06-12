using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

using Debug = UnityEngine.Debug;
public class Build : EditorWindow
{
    CompressionType ct = CompressionType.LZMA;

    [MenuItem("Bean Shootout/Build Level")]
    public static void ShowWindow()
    {
        EditorWindow win = GetWindow(typeof(Build));
        win.titleContent = new GUIContent("Build Level");
    }

    private void OnGUI()
    {
        ct = (CompressionType)EditorGUI.EnumPopup(new Rect(3, 3, position.width - 6, 15), new GUIContent("Compression Type", "None: Fastest to load, biggest file size\nLZ4: Fast to load, medium file size\nLZMA (Default): Slowest to load, smallest file size\n\nInternally, the game will save an uncompressed version of the mod to the cache to improve load times."), ct);

        GUILayout.Space(50);

        GUILayout.Label("The built files will be under Assets/Levels/<LEVEL NAME>/Build/level", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Build for Windows"))
        {
            Debug.Log("(BeanShootout) Building level");

            BuildLevel(ct, BuildTarget.StandaloneWindows64, "WindowsBuild");
        }

        if (GUILayout.Button("Build and Run for Windows"))
        {
            BeanShootoutConfigSO config = AssetDatabase.LoadAssetAtPath<BeanShootoutConfigSO>(Strings.ConfigPath);

            // If the game path specified isn't valid or if the build isn't valid then fail.
            if (string.IsNullOrEmpty(config.GamePath))
            {
                EditorDialog.DisplayAlertDialog(Constants.PackageName, Strings.Build_NoGamePath, "OK", DialogIconType.Error);
                return;
            }

            if (!config.IsValid)
            {
                EditorDialog.DisplayAlertDialog(Constants.PackageName, string.Format(Strings.Build_GamePathInvalid, config.ValidReason), "OK", DialogIconType.Error);
                return;
            }

            BuildLevel(ct, BuildTarget.StandaloneWindows64, "WindowsBuild");

            string SceneName = EditorSceneManager.GetActiveScene().name;

            Debug.Log("(BeanShootout) Running level");

            // Gets the PersistentDataPath of Bean Shootout to copy the level bundles into. (AppData\LocalLow\OneWing\The Great Bean Shootout\Mods\LevelLocalBuild)
            string LevelLocalBuildFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "OneWing", "The Great Bean Shootout", "Mods", "LevelLocalBuild") + "\\";
            Debug.Log("(BeanShootout) LevelLocalBuildFolder: "+ LevelLocalBuildFolder);

            File.Copy("Assets/Levels/" + SceneName + "/WindowsBuild/level_data.bundle", LevelLocalBuildFolder + "level_data.bundle", true);
            File.Copy("Assets/Levels/" + SceneName + "/WindowsBuild/level_info.bundle", LevelLocalBuildFolder + "level_info.bundle", true);

            // Starts BeanShootout.exe with arguments depending on the settings
            Process GameProcess = new();
            GameProcess.StartInfo.FileName = config.GamePath + "/BeanShootout.exe";
            string args = "";
            if (config.FastLoad)
            {
                args += " -FastLoad true";
            }
            if (config.VerboseLogging)
            {
                args += " -verbose";
            }
            args += " -loadlevellocalbuild -gs_fnop " + config.FullscreenWhenTheresNoOtherPlayers + " -gs_sm " + config.ShowMinimap + " -gs_ma " + config.MaxAmmo + " -ga_mp " + config.MaxPlayers + " -gs_sa " + config.SharedAmmo;
            GameProcess.StartInfo.Arguments = args;
            GameProcess.Start();
        }

        if (GUILayout.Button("Build for Mac"))
        {
            Debug.Log("(BeanShootout) Building level");

            BuildLevel(ct, BuildTarget.StandaloneOSX, "MacBuild");
        }

        if (GUILayout.Button("Build for Linux"))
        {
            Debug.Log("(BeanShootout) Building level");

            BuildLevel(ct, BuildTarget.StandaloneLinux64, "LinuxBuild");
        }

        if (GUILayout.Button("Build for Android"))
        {
            Debug.Log("(BeanShootout) Building level");

            BuildLevel(ct, BuildTarget.Android, "AndroidBuild");
        }

        GUILayout.Space(25);

        if (GUILayout.Button("Cleanup build folders for all levels"))
        {
            if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, Strings.Build_CleanupAllBuildFolders, "Yes", "No", DialogIconType.Info))
            {
                Debug.Log("(BeanShootout) Cleaning build folders");

                string[] levelDirs = Directory.GetDirectories("Assets/Levels");
                foreach (var item in levelDirs)
                {
                    if (Directory.Exists(item + "/WindowsBuild"))
                    {
                        File.Delete(item + "/WindowsBuild.meta");
                        Directory.Delete(item + "/WindowsBuild", true);
                    }
                    if (Directory.Exists(item + "/MacBuild"))
                    {
                        File.Delete(item + "/MacBuild.meta");
                        Directory.Delete(item + "/MacBuild", true);
                    }
                    if (Directory.Exists(item + "/LinuxBuild"))
                    {
                        File.Delete(item + "/LinuxBuild.meta");
                        Directory.Delete(item + "/LinuxBuild", true);
                    }
                    if (Directory.Exists(item + "/AndroidBuild"))
                    {
                        File.Delete(item + "/AndroidBuild.meta");
                        Directory.Delete(item + "/AndroidBuild", true);
                    }

                    AssetDatabase.Refresh();
                }
            }
        }

        if (GUILayout.Button("Cleanup build folders for the current level"))
        {
            if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, Strings.Build_CleanupCurrentLevelBuildFolders, "Yes", "No", DialogIconType.Info))
            {
                Debug.Log("(BeanShootout) Cleaning build folders");

                string item = "Assets/Levels/" + EditorSceneManager.GetActiveScene().name;

                if (Directory.Exists(item + "/WindowsBuild"))
                {
                    File.Delete(item + "/WindowsBuild.meta");
                    Directory.Delete(item + "/WindowsBuild", true);
                }
                if (Directory.Exists(item + "/MacBuild"))
                {
                    File.Delete(item + "/MacBuild.meta");
                    Directory.Delete(item + "/MacBuild", true);
                }
                if (Directory.Exists(item + "/LinuxBuild"))
                {
                    File.Delete(item + "/LinuxBuild.meta");
                    Directory.Delete(item + "/LinuxBuild", true);
                }
                if (Directory.Exists(item + "/AndroidBuild"))
                {
                    File.Delete(item + "/AndroidBuild.meta");
                    Directory.Delete(item + "/AndroidBuild", true);
                }

                AssetDatabase.Refresh();
            }
        }
    }

    private void BuildLevel(CompressionType ct, BuildTarget target, string BuildPathName)
    {
        // Makes sure that the correct GraphicsAPIs are set
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            GraphicsDeviceType[] requiredGDT = new GraphicsDeviceType[] {GraphicsDeviceType.Direct3D11, GraphicsDeviceType.Direct3D12, GraphicsDeviceType.Vulkan};
            if (!PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64).Contains(GraphicsDeviceType.Direct3D11) || !PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64).Contains(GraphicsDeviceType.Direct3D12) || !PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64).Contains(GraphicsDeviceType.Vulkan))
            {
                // The build requires the GraphicsAPIs to be set to the same as Bean Shootout, otherwise levels may be incompatible with certain GraphicsAPIs or systems
                if (EditorDialog.DisplayDecisionDialog(Constants.PackageName, Strings.Build_InvalidWindowsGraphicsAPIs, "Yes", "No", DialogIconType.Warning))
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows, false);
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, requiredGDT);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, requiredGDT);
                }
                else
                {
                    Debug.LogError("Build Failed: The GraphicsAPI setting for Windows are not set to the required values.");

                    return;
                }
            }
        }

        string SceneName = EditorSceneManager.GetActiveScene().name;

        if (Directory.Exists("Assets/Levels/" + SceneName + "/" + BuildPathName))
        {
            Directory.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName, true);
        }

        // Sets the AssetBundle names of the level image and name
        if (AssetImporter.GetAtPath("Assets/Levels/" + SceneName + "/image.png").assetBundleName != SceneName.ToLower() + "_info")
        {
            AssetImporter.GetAtPath("Assets/Levels/" + SceneName + "/image.png").assetBundleName = SceneName.ToLower() + "_info";
            AssetImporter.GetAtPath("Assets/Levels/" + SceneName + "/name.txt").assetBundleName = SceneName.ToLower() + "_info";
        }

        Directory.CreateDirectory("Assets/Levels/" + SceneName + "/" + BuildPathName);
        AssetDatabase.Refresh();

        AssetBundleUtils.BuildAssetBundlesByName(new[] { SceneName.ToLower() + "_ab", SceneName.ToLower() + "_info" }, "Assets/Levels/" + SceneName + "/" + BuildPathName, target, ct);

        // Copys the built AssetBundles with names the game uses
        File.Move("Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName.ToLower() + "_ab", "Assets/Levels/" + SceneName + "/level_data.bundle");
        File.Move("Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName.ToLower() + "_info", "Assets/Levels/" + SceneName + "/level_info.bundle");

        Directory.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName, true);

        Directory.CreateDirectory("Assets/Levels/" + SceneName + "/" + BuildPathName);

        File.Move("Assets/Levels/" + SceneName + "/level_data.bundle", "Assets/Levels/" + SceneName + "/" + BuildPathName + "/level_data.bundle");
        File.Move("Assets/Levels/" + SceneName + "/level_info.bundle", "Assets/Levels/" + SceneName + "/" + BuildPathName + "/level_info.bundle");

        AssetDatabase.Refresh();

        EditorDialog.DisplayAlertDialog(Constants.PackageName, string.Format(Strings.Build_Finished, "Assets/Levels/" + SceneName + "/" + BuildPathName + "/"), "OK", DialogIconType.Info);
    }
}

public enum CompressionType
{
    None,
    LZ4,
    LZMA
}