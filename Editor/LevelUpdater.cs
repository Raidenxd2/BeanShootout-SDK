using UnityEditor;
using UnityEngine;
using KillItMyself.Independent;

public class LevelUpdater
{
    [MenuItem("Bean Shootout/Update level to current SDK version")]
    public static void UpdateLevel()
    {
        if (!GameObject.Find("GravityManager"))
        {
            GameObject go = new GameObject("GravityManager");
            go.AddComponent<GravityManager>();
        }
    }
}