using UnityEngine;
using UnityEditor;
using System.Net;

namespace Everime.WorldManagement.CustomEditors
{
    [CustomEditor(typeof(WorldMaster))]
    public class WorldMasterEditor : Editor
    {
        private static bool WorldeEditorEnabled = false;
        private static bool worldEditorFoldout = false;
        private static bool AutoUpdate = false;

        private WorldMaster worldMaster;
        private Editor worldSettingsEditor;

        private void OnEnable()
        {
            worldMaster = target as WorldMaster;
            worldSettingsEditor = CreateEditor(worldMaster.worldSettings);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine(1);

            WorldeEditorEnabled = EditorGUILayout.Toggle("Enable World Editor", WorldeEditorEnabled);
            if (WorldeEditorEnabled)
            {
                DrawWorldEditor();
                EditorGUILayout.Space();
            }
        }

        private void DrawWorldEditor() 
        {
            EditorGUILayout.Space();

            worldEditorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(worldEditorFoldout, "World Settings");
    
            if (worldEditorFoldout)
                worldSettingsEditor.DrawDefaultInspector();

            EditorGUILayout.Space();
            AutoUpdate = EditorGUILayout.Toggle("Auto Update", AutoUpdate);

            if (AutoUpdate) 
            { 
                worldMaster.CreateWorld();
            }
            else
            {
                if (GUILayout.Button("Create World"))
                    worldMaster.CreateWorld();
                if (GUILayout.Button("Delete World"))
                    worldMaster.ClearExistingWorld();
            }
        }

        private void DrawLine(int height = 1)
        {
            GUILayout.Space(4);
            Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
            rect.height = height;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            Color lineColor = new Color(0.10196f, 0.10196f, 0.10196f, 1);
            EditorGUI.DrawRect(rect, lineColor);
            GUILayout.Space(4);
        }

    }
}
