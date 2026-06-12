using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Collections;
using UnityEngine.Rendering;

public class LevelThumbnail : EditorWindow
{
    private bool CreatingThumbnail;
    private bool SavingThumbnail;
    public string CurrentSceneName;
    private GameObject ThumbnailCreatorRootGO;

    [MenuItem("Bean Shootout/Create Level Thumbnail")]
    public static void ShowWindow()
    {
        EditorWindow win = GetWindow(typeof(LevelThumbnail));
        win.titleContent = new GUIContent("Level Thumbnail Creator");
    }

    private void OnGUI()
    {
        if (SavingThumbnail)
        {
            GUILayout.Label("Saving...");

            return;
        }
        if (!CreatingThumbnail)
        {
            // Spawns the ThumbnailCreatorRoot object
            if (GUILayout.Button("Start creating level thumbnail..."))
            {
                ThumbnailCreatorRootGO = Instantiate(AssetDatabase.LoadMainAssetAtPath("Packages/com.onewing.beanshootout-customlevels/Core/Prefabs/ThumbnailCreatorRoot.prefab") as GameObject);
                ThumbnailCreatorRootGO.name = "Thumbnail";

                CreatingThumbnail = true;
            }
        }
        else
        {
            GUILayout.Label("Please position the 'Thumbnail' object for the thumbnail and press 'Save Thumbnail' to save it as the thumbnail for the level", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Save Thumbnail"))
            {
                CreatingThumbnail = false;
                SavingThumbnail = true;
                
                CurrentSceneName = EditorSceneManager.GetActiveScene().name;
                SaveScreenshot();
            }
        }
    }

    // Takes a screenshot of the camera and saves it as the level image
    private void SaveScreenshot()
    {
        RenderTexture rt = ThumbnailCreatorRootGO.GetComponentInChildren<Camera>().targetTexture;
        SaveTextureToFile(rt, "Assets/Levels/" + CurrentSceneName + "/image.png", 1512, 926, SaveTextureFileFormat.PNG, 100, true, SaveScreenshotResult);
    }

    private void SaveScreenshotResult(bool success)
    {
        SavingThumbnail = false;
        
        DestroyImmediate(ThumbnailCreatorRootGO);
        AssetDatabase.Refresh();
        
        if (success)
        {
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath("Assets/Levels/" + CurrentSceneName + "/image.png");
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            
            EditorDialog.DisplayAlertDialog(Constants.PackageName, "Created thumbnail under 'Assets/Levels/" + CurrentSceneName + "/image.png'", "OK", DialogIconType.Info);
        }
        else
        {
            EditorDialog.DisplayAlertDialog(Constants.PackageName, "Failed to save level thumbnail.", "OK", DialogIconType.Error);
        }
    }
    
    // https://discussions.unity.com/t/save-rendertexture-or-texture2d-as-image-file-utility/891718/14
    private void SaveTextureToFile(Texture source, string filePath, int width, int height, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 100, bool asynchronous = true, System.Action<bool> done = null)
    {
        // check that the input we're getting is something we can handle:
        if (!(source is Texture2D || source is RenderTexture))
        {
            done?.Invoke(false);
            return;
        }

        // use the original texture size in case the input is negative:
        if (width < 0 || height < 0)
        {
            width = source.width;
            height = source.height;
        }

        // resize the original image:
        var resizeRT = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(source, resizeRT);

        // create a native array to receive data from the GPU:
        var narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        // request the texture data back from the GPU:
        var request = AsyncGPUReadback.RequestIntoNativeArray (ref narray, resizeRT, 0, (AsyncGPUReadbackRequest request) =>
        {
            // if the readback was successful, encode and write the results to disk
            if (!request.hasError)
            {
                NativeArray<byte> encoded;

                switch (fileFormat)
                {
                    case SaveTextureFileFormat.EXR:
                        encoded = ImageConversion.EncodeNativeArrayToEXR(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    case SaveTextureFileFormat.JPG:
                        encoded = ImageConversion.EncodeNativeArrayToJPG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0, jpgQuality);
                        break;
                    case SaveTextureFileFormat.TGA:
                        encoded = ImageConversion.EncodeNativeArrayToTGA(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    default:
                        encoded = ImageConversion.EncodeNativeArrayToPNG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                }

                System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                encoded.Dispose();
            }

            narray.Dispose();

            // notify the user that the operation is done, and its outcome.
            done?.Invoke(!request.hasError);
        });

        if (!asynchronous)
            request.WaitForCompletion();
    }
}

public enum SaveTextureFileFormat
{
    EXR,
    JPG,
    TGA,
    PNG
}