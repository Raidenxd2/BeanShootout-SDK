using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelCreator : EditorWindow
{
    private string LevelName;

    [MenuItem("Bean Shootout/Create Level")]
    public static void ShowWindow()
    {
        EditorWindow win = GetWindow(typeof(LevelCreator));
        win.titleContent = new GUIContent("Level Creator");
    }

    private void OnGUI()
    {
        LevelName = EditorGUILayout.TextField("Level Name", LevelName);

        // Creates the level folders, copys the default level Scene and image, and creates the name file
        if (GUILayout.Button("Create Level"))
        {
            Debug.Log("(BeanShootout) Creating level");

            if (!Directory.Exists("Assets/Levels"))
            {
                Directory.CreateDirectory("Assets/Levels");
                AssetDatabase.Refresh();
            }

            if (!Directory.Exists("Assets/Levels/" + LevelName))
            {
                Directory.CreateDirectory("Assets/Levels/" + LevelName);
                AssetDatabase.Refresh();
            }

            if (File.Exists("Assets/Levels/" + LevelName + "/" + LevelName + ".unity"))
            {
                EditorDialog.DisplayAlertDialog(Constants.PackageName, Strings.LevelCreator_AlreadyExists, "OK", DialogIconType.Error);
                return;
            }

            File.Copy("Packages/com.onewing.beanshootout-customlevels/Core/Scenes/Level.unity", "Assets/Levels/" + LevelName + "/" + LevelName + ".unity");
            File.Copy("Packages/com.onewing.beanshootout-customlevels/Core/Textures/GenericLevelImage.png", "Assets/Levels/" + LevelName + "/image.png", true);
            File.WriteAllText("Assets/Levels/" + LevelName + "/name.txt", LevelName);
            AssetDatabase.Refresh();

            AssetImporter.GetAtPath("Assets/Levels/" + LevelName + "/" + LevelName + ".unity").assetBundleName = LevelName + "_ab";
            AssetDatabase.Refresh();

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Levels/" + LevelName + "/" + LevelName + ".unity");
            }
        }
    }
}