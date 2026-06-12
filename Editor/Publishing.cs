using UnityEngine;
using UnityEditor;
using Unity.SharpZipLib.Utils;
using UnityEditor.SceneManagement;
using System.IO;

public class Publishing : EditorWindow
{
    [MenuItem("Bean Shootout/Publishing")]
    public static void ShowWindow()
    {
        EditorWindow win = GetWindow(typeof(Publishing));
        win.titleContent = new GUIContent("Publishing");
    }

    private void OnGUI()
    {
        GUILayout.Label("The Zip will be created under Assets/Levels/<LEVEL NAME>/Build/<LEVEL NAME>.zip", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Create Zip for Windows x64"))
        {
            MakeZip("WindowsBuild", "win64");
        }

        if (GUILayout.Button("Create Zip for Mac"))
        {
            MakeZip("MacBuild", "mac");
        }

        if (GUILayout.Button("Create Zip for Linux x64"))
        {
            MakeZip("LinuxBuild", "linux");
        }

        if (GUILayout.Button("Create Zip for Android"))
        {
            MakeZip("AndroidBuild", "android");
        }
    }

    private void MakeZip(string BuildPathName, string ZipName)
    {
        EditorUtility.DisplayProgressBar(Constants.PackageName, "Creating Zip file...", 0);

        string SceneName = EditorSceneManager.GetActiveScene().name;

        // Delete the zip and meta files if it exists
        if (File.Exists("Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName + "-" + ZipName + ".zip"))
        {
            File.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName + "-" + ZipName + ".zip");
            File.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName + "-" + ZipName + ".zip.meta");
            AssetDatabase.Refresh();
        }

        // Delete meta files
        File.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName + "/level_data.bundle.meta");
        File.Delete("Assets/Levels/" + SceneName + "/" + BuildPathName + "/level_info.bundle.meta");

        ZipUtility.CompressFolderToZip("Assets/Levels/" + SceneName + "-" + ZipName + ".zip", null, "Assets/Levels/" + SceneName + "/" + BuildPathName);

        File.Move("Assets/Levels/" + SceneName + "-win64.zip", "Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName + "-" + ZipName + ".zip");

        AssetDatabase.Refresh();
        
        EditorDialog.DisplayAlertDialog(Constants.PackageName, string.Format(Strings.Publishing_Created, "'Assets/Levels/" + SceneName + "/" + BuildPathName + "/" + SceneName + "-" + ZipName + ".zip'"), "OK", DialogIconType.Info);

        EditorUtility.ClearProgressBar();
    }
}