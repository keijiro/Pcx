// Pcx - Point cloud importer & renderer for Unity with Draco
// Based off https://github.com/keijiro/Pcx
// https://github.com/millerhooks/DracoAnimatedPointClouds

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using Pcx;

namespace Drc
{
	// Note: Not sure why but EnumPopup doesn't work in ScriptedImporterEditor,
	// so it has been replaced with a normal Popup control.

	[CustomEditor(typeof(DrcImporter))]
	class DrcImporterInspector : ScriptedImporterEditor
	{
		SerializedProperty _containerType;

		string[] _containerTypeNames;

		protected override bool useAssetDrawPreview { get { return false; } }

		public override void OnEnable()
		{
			base.OnEnable();

			_containerType = serializedObject.FindProperty("_containerType");
			_containerTypeNames = System.Enum.GetNames(typeof(DrcImporter.ContainerType));
		}

		public override void OnInspectorGUI()
		{
			_containerType.intValue = EditorGUILayout.Popup(
				"Container Type", _containerType.intValue, _containerTypeNames);

			base.ApplyRevertGUI();
		}
	}
}
