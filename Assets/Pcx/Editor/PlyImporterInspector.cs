using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace Pcx
{
    [CustomEditor(typeof(PlyImporter))]
    class PlyImporterInspector : ScriptedImporterEditor
    {
        SerializedProperty _containerType;

        string[] _containerTypeNames;

        protected override bool useAssetDrawPreview { get { return false; } }

        public override void OnEnable()
        {
            base.OnEnable();

            _containerType = serializedObject.FindProperty("_containerType");
            _containerTypeNames = System.Enum.GetNames(typeof(PlyImporter.ContainerType));
        }

        public override void OnInspectorGUI()
        {
            _containerType.intValue = EditorGUILayout.Popup(
                "Container Type", _containerType.intValue, _containerTypeNames);

            base.ApplyRevertGUI();
        }
    }
}
