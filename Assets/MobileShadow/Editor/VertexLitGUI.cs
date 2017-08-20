using System;
using UnityEngine;
using UnityEditor;

public class VertexLitGUI : ShaderGUI
{
    enum ObjectType {
        Item, Character, Ground
    }
    
    Material target;
    
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.target = editor.target as Material;
        
        MaterialProperty mainTex = FindProperty("_MainTex", properties);

        float controlSize = 64;
        EditorGUIUtility.fieldWidth = controlSize;
        
        string[] keyWords = target.shaderKeywords;

        editor.ShaderProperty(mainTex, mainTex.displayName);
        
        //Type
        EditorGUILayout.Space();
        ObjectType type = ObjectType.Item;
        if (IsKeywordEnabled("CHARACTER")) {
            type = ObjectType.Character;
        }
        else if (IsKeywordEnabled("GROUND")) {
            type = ObjectType.Ground;
        }
        
        //alphaClip
        bool clip = Array.IndexOf(keyWords, "CLIP") != -1;

        EditorGUI.BeginChangeCheck();
        clip = EditorGUILayout.Toggle("Alpha Clip", clip);

        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("CLIP", clip);
        }

        if (clip)
        {
            EditorGUI.indentLevel++;
            MaterialProperty alphaTex = FindProperty("_AlphaR", properties);
            editor.TextureProperty(alphaTex, alphaTex.displayName, false);
            MaterialProperty dissolve = FindProperty("_Dissolve", properties);
            editor.ShaderProperty(dissolve, dissolve.displayName);
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            if (type == ObjectType.Character)
            {
                target.SetOverrideTag("MobileShadow", "CharacterAlphaClip");
            }
            else if (type == ObjectType.Item)
            {
                target.SetOverrideTag("MobileShadow", "AlphaClip");
            }
            else
            {
                target.SetOverrideTag("MobileShadow", "GroundAlphaClip");
            }
        }
        else
        {
            if (type == ObjectType.Character)
            {
                target.SetOverrideTag("MobileShadow", "Character");
            }
            else if (type == ObjectType.Item)
            {
                target.SetOverrideTag("MobileShadow", "Geometry");
            }
            else
            {
                target.SetOverrideTag("MobileShadow", "Ground");
            }
        }

        bool specular = Array.IndexOf(keyWords, "SPECULAR") != -1 || Array.IndexOf(keyWords, "SPECULARFROMCAMERA") != -1;
        bool specularFromCamera = Array.IndexOf(keyWords, "SPECULARFROMCAMERA") != -1;

        EditorGUI.BeginChangeCheck();
        specular = EditorGUILayout.Toggle("Specular", specular);

        if (EditorGUI.EndChangeCheck())
        {
            if (specular)
                target.EnableKeyword("SPECULAR");
            else
                {
                    target.DisableKeyword("SPECULAR");
                    target.DisableKeyword("SPECULARFROMCAMERA");
                }
        }

        if (specular)
        {
            EditorGUI.indentLevel++;
            MaterialProperty shininess = FindProperty("_Shininess", properties);
            editor.ShaderProperty(shininess, shininess.displayName);
            
            EditorGUI.BeginChangeCheck();
            specularFromCamera = EditorGUILayout.Toggle("From camera", specularFromCamera);
            if (EditorGUI.EndChangeCheck())
            {
                if (specularFromCamera)
                {
                    target.DisableKeyword("SPECULAR");
                    target.EnableKeyword("SPECULARFROMCAMERA");
                }
                else
                {
                    target.DisableKeyword("SPECULARFROMCAMERA");
                    target.EnableKeyword("SPECULAR");
                }
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        bool emission = Array.IndexOf(keyWords, "EMISSION") != -1 || Array.IndexOf(keyWords, "EMISSION_UV2") != -1;
        bool emission_UV2 = Array.IndexOf(keyWords, "EMISSION_UV2") != -1;

        EditorGUI.BeginChangeCheck();
        emission = EditorGUILayout.Toggle("Emission", emission);

        if (EditorGUI.EndChangeCheck())
        {
            if (emission)
            {
                target.EnableKeyword("EMISSION");
            }
            else
            {
                target.DisableKeyword("EMISSION_UV2");
                target.DisableKeyword("EMISSION");
            }

        }

        if (emission)
        {
            EditorGUI.indentLevel++;
            MaterialProperty emitTex = FindProperty("_EmitTex", properties);
            editor.TextureProperty(emitTex, emitTex.displayName, false);
            MaterialProperty emitValue = FindProperty("_EmitValue", properties);
            editor.ShaderProperty(emitValue, emitValue.displayName);

            EditorGUI.BeginChangeCheck();
            emission_UV2 = EditorGUILayout.Toggle("UV2", emission_UV2);
            if (EditorGUI.EndChangeCheck())
            {
                if (emission_UV2)
                {
                    target.DisableKeyword("EMISSION");
                    target.EnableKeyword("EMISSION_UV2");
                }
                else
                {
                    target.DisableKeyword("EMISSION_UV2");
                    target.EnableKeyword("EMISSION");
                }

            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }


        //Type ending
        EditorGUI.BeginChangeCheck();
        type = (ObjectType) EditorGUILayout.EnumPopup("Type", type);
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("CHARACTER", type == ObjectType.Character);
            SetKeyword("GROUND", type == ObjectType.Ground);
        }

        if (type == ObjectType.Character)
        {
            if (clip)
            {
                target.SetOverrideTag("MobileShadow", "CharacterAlphaClip");
            }
            else
            {
                target.SetOverrideTag("MobileShadow", "Character");
            }
        }
        else if (type == ObjectType.Item)
        {
            if (clip)
            {
                target.SetOverrideTag("MobileShadow", "AlphaClip");
            }
            else
            {
                target.SetOverrideTag("MobileShadow", "Geometry");
            }
        }
        else
        {
            if (clip)
            {
                target.SetOverrideTag("MobileShadow", "GroundAlphaClip");
            }
            else
            {
                target.SetOverrideTag("MobileShadow", "Ground");
            }
        }
    }
    
    void SetKeyword (string keyword, bool state) {
        if (state) {
            target.EnableKeyword(keyword);
        }
        else {
            target.DisableKeyword(keyword);
        }
    }
    
    bool IsKeywordEnabled (string keyword) {
        return target.IsKeywordEnabled(keyword);
    }
}
