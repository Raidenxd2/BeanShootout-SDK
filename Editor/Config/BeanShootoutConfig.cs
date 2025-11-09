using System.IO;
using UnityEditor;
using UnityEngine;

public class BeanShootoutConfig
{
    // Creates the config if it doesn't exist and selects the config.
    [MenuItem("Bean Shootout/_Config")]
    public static void ViewConfigAsset()
    {
        if (!File.Exists("Assets/BeanShootoutConfig.asset"))
        {
            Setup.CreateConfig();
        }

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/BeanShootoutConfig.asset");
    }
}