using System.IO;
using UnityEditor;
using UnityEngine;

public class BeanShootoutConfig
{
    // Creates the config if it doesn't exist and selects the config.
    [MenuItem("Bean Shootout/_Config")]
    public static void ViewConfigAsset()
    {
        if (!File.Exists(Strings.ConfigPath))
        {
            Setup.CreateConfig();
        }

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(Strings.ConfigPath);
    }
}