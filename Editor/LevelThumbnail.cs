using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;

public class LevelThumbnail : EditorWindow
{
    private bool CreatingThumbnail;

    private Vector2Int PrevRes;

    [MenuItem("Bean Shootout/Create Level Thumbnail")]
    public static void ShowWindow()
    {
        EditorWindow win = GetWindow(typeof(LevelThumbnail));
        win.titleContent = new GUIContent("Level Thumbnail Creator");
    }

    private void OnGUI()
    {
        if (!CreatingThumbnail)
        {
            // Spawns the ThumbnailCamera object and sets the Game View resoultion to 1512,926
            if (GUILayout.Button("Start creating level thumbnail..."))
            {
                Instantiate(AssetDatabase.LoadMainAssetAtPath("Packages/com.onewing.beanshootout-customlevels/Core/Prefabs/ThumbnailCamera.prefab") as GameObject);
                EditorSceneManager.MarkAllScenesDirty();
                
                PrevRes = GetResoultion();

                GameViewUtils.AddAndSelectCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, 1512, 926, "BeanShootoutThumbnailResolution");

                CreatingThumbnail = true;
            }
        }
        else
        {
            GUILayout.Label("Please position the 'ThumbnailCamera(Clone)' object for the thumbnail and press 'Save Thumbnail' to save it as the thumbnail for the level", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Save Thumbnail"))
            {
                SaveScreenshot();

                CreatingThumbnail = false;
            }
        }
    }

    // Takes a screenshot of the camera and saves it as the level image
    private async void SaveScreenshot()
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;

        ScreenCapture.CaptureScreenshot("Assets/Levels/" + sceneName + "/image.png");

        EditorUtility.DisplayDialog(Constants.PackageName, "Created thumbnail under 'Assets/Levels/" + sceneName + "/image.png'", "OK");
        
        await Task.Delay(100);

        DestroyImmediate(GameObject.Find("TumbnailCamera"));
        AssetDatabase.Refresh();
    }

    private Vector2Int GetResoultion()
    {
        Vector2 size = Handles.GetMainGameViewSize();
        Vector2Int sizeInt = new((int)size.x, (int)size.y);
        return sizeInt;
    }
}