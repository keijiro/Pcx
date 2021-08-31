// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Pcx
{
    // Note: Not sure why but EnumPopup doesn't work in ScriptedImporterEditor,
    // so it has been replaced with a normal Popup control.

    [CustomEditor(typeof(PlyImporter))]
    class PlyImporterInspector : ScriptedImporterEditor
    {
        SerializedProperty _containerType;
        SerializedProperty _readWriteEnabled;

        string[] _containerTypeNames;

        protected override bool useAssetDrawPreview { get { return false; } }

        public override void OnEnable()
        {
            base.OnEnable();

            _containerType = serializedObject.FindProperty("_containerType");
            _readWriteEnabled = serializedObject.FindProperty("_readWriteEnabled");
            _containerTypeNames = System.Enum.GetNames(typeof(PlyImporter.ContainerType));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            _containerType.intValue = EditorGUILayout.Popup(
                "Container Type", _containerType.intValue, _containerTypeNames);

            if(_containerType.intValue == (int)PlyImporter.ContainerType.Mesh)
            {
                _readWriteEnabled.boolValue = EditorGUILayout.Toggle(
                    "Read/Write Enabled", _readWriteEnabled.boolValue);
            }

            serializedObject.ApplyModifiedProperties();
            base.ApplyRevertGUI();
        }
    }
}
