// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEditor;

namespace Pcx
{
    class PointMaterialInspector : ShaderGUI
    {
        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            editor.ShaderProperty(FindProperty("_Tint", props), "Tint");
            editor.ShaderProperty(FindProperty("_PointSize", props), "Point Size");
            editor.ShaderProperty(FindProperty("_Distance", props), "Apply Distance");

            EditorGUILayout.HelpBox(
                "Only some platform support these point size properties.",
                MessageType.None
            );
        }
    }

    class DiskMaterialInspector : ShaderGUI
    {
        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            editor.ShaderProperty(FindProperty("_Tint", props), "Tint");
            editor.ShaderProperty(FindProperty("_PointSize", props), "Point Size");
        }
    }
}
