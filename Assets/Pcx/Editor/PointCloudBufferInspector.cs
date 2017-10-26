using UnityEngine;
using UnityEditor;

namespace Pcx
{
    [CustomEditor(typeof(PointCloudBuffer))]
    public sealed class PointCloudBufferInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var count = ((PointCloudBuffer)target).pointCount;
            EditorGUILayout.LabelField("Point Count", count.ToString("N0"));
        }
    }
}
