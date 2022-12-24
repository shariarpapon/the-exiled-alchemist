using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPaletteGenerator))]
public class ColorPaletteGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.contentColor = Color.yellow;
        EditorGUILayout.LabelField("Generate color palette from materials.");
        GUI.contentColor = Color.white;

        base.OnInspectorGUI();

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Palette")) 
        {
            ((ColorPaletteGenerator)target).Run();
        }
    }
}
