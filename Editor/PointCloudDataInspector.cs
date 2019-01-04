// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;

namespace Pcx
{
    [CustomEditor(typeof(PointCloudData))]
    public sealed class PointCloudDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var count = ((PointCloudData)target).pointCount;
            EditorGUILayout.LabelField("Point Count", count.ToString("N0"));
        }
    }
}
