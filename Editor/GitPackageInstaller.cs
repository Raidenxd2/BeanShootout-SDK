using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class GitPackageInstaller : Editor
{
    public const string SharedShadersPrefName = "BeanShootoutCLP_DontShowPackageNotInstalledForSharedShaders";

    [InitializeOnLoadMethod]
    public static void InstallPackages()
    {
        if (!CheckPackageInstalled("com.raiden.sharedshaders") && !EditorPrefs.GetBool(SharedShadersPrefName, false))
        {
            if (EditorUtility.DisplayDialog("The Great Bean Shootout Custom Level Package", "The package SharedShaders (com.raiden.sharedshaders) is not installed. This package is required for playmode functionality. Would you like to install it?\n\nThis message will only appear once for this machine if you choose no.", "Yes", "No"))
            {
                AddPackage("com.raiden.sharedshaders", "https://github.com/Raidenxd2/SharedShaders.git#1.1.0");
            }
            else
            {
                EditorPrefs.SetBool(SharedShadersPrefName, true);
            }
        }
    }

    public static void AddPackage(string packageName, string gitURL)
    {
        var path = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        var jsonString = File.ReadAllText(path);
        int indexOfLastBracket = jsonString.IndexOf("}");
        string dependenciesSubstring = jsonString.Substring(0, indexOfLastBracket);
        var endOfLastPackage = dependenciesSubstring.LastIndexOf("\"");
        string oldValue = jsonString.Substring(endOfLastPackage, indexOfLastBracket - endOfLastPackage);
        jsonString = jsonString.Insert(endOfLastPackage + 1,
             $", \n \"{packageName}\": \"{gitURL}\"");
        File.WriteAllText(path, jsonString);
        Client.Resolve();
    }

    public static bool CheckPackageInstalled(string packageName)
    {
        var path = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        var jsonString = File.ReadAllText(path);
        return jsonString.Contains(packageName);
    }

    [MenuItem("Bean Shootout/Dev/Reset BeanShootoutCLP values")]
    public static void ResetBeanShootoutCLPValues()
    {
        EditorPrefs.SetBool(SharedShadersPrefName, false);
    }
}