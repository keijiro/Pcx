// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;

namespace Pcx
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PointCloudRenderer))]
    public class PointCloudRendererInspector : Editor
    {
        SerializedProperty _source;
        SerializedProperty _pointTint;
        SerializedProperty _pointSize;

        void OnEnable()
        {
            _source = serializedObject.FindProperty("_source");
            _pointTint = serializedObject.FindProperty("_pointTint");
            _pointSize = serializedObject.FindProperty("_pointSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_source);
            EditorGUILayout.PropertyField(_pointTint);
            EditorGUILayout.PropertyField(_pointSize);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
