using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MobileBasicGUI : ShaderGUI
{
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
            
        Material material = materialEditor.target as Material;
            
        if (material.HasProperty(BumpMap))
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._NORMALMAP, material.GetTexture(BumpMap));
    }
}