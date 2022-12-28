using UnityEngine;
using UnityEditor;

namespace Main.WorldManagement.CustomEditors
{
    [CustomEditor(typeof(WorldMaster))]
    public class WorldMasterEditor : Editor
    {
        private WorldMaster worldMaster;

        private void OnEnable()
        {
            worldMaster = target as WorldMaster;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

    }
}
