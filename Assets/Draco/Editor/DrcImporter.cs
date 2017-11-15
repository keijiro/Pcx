// Pcx - Point cloud importer & renderer for Unity with Draco
// Based off https://github.com/keijiro/Pcx
// https://github.com/millerhooks/DracoAnimatedPointClouds

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pcx;



namespace Drc
{
	[ScriptedImporter(1, "drc")]
	public unsafe class DrcImporter : ScriptedImporter
	{
		#region ScriptedImporter implementation


		public enum ContainerType { Mesh, ComputeBuffer  }

		[SerializeField] ContainerType _containerType;


		public override void OnImportAsset(AssetImportContext context)
		{
			if (_containerType == ContainerType.Mesh)
			{
				// Mesh container
				// Create a prefab with MeshFilter/MeshRenderer.
				var gameObject = new GameObject();
				var mesh = ImportAsMesh(context.assetPath);

				var meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = mesh;

				var meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = GetDefaultMaterial();

				context.AddObjectToAsset("prefab", gameObject);
				if (mesh != null) context.AddObjectToAsset("mesh", mesh);

				context.SetMainObject(gameObject);
			}
			else
			{
				// ComputeBuffer container
				// Create a prefab with PointCloudRenderer.
				var gameObject = new GameObject();
				var data = ImportAsPointCloudData(context.assetPath);

				var renderer = gameObject.AddComponent<PointCloudRenderer>();
				renderer.sourceData = data;

				context.AddObjectToAsset("prefab", gameObject);
				if (data != null) context.AddObjectToAsset("data", data);

				context.SetMainObject(gameObject);
			}
		}

		#endregion

		#region Internal utilities

		static Material GetDefaultMaterial()
		{
			return AssetDatabase.LoadAssetAtPath<Material>(
				"Assets/Pcx/Editor/Default Point.mat"
			);
		}

		#endregion

		#region Internal data structure

		enum DataProperty {
			Invalid,
			X, Y, Z,
			R, G, B, A,
			Data8, Data16, Data32
		}

		static int GetPropertySize(DataProperty p)
		{
			switch (p)
			{
			case DataProperty.X: return 4;
			case DataProperty.Y: return 4;
			case DataProperty.Z: return 4;
			case DataProperty.R: return 1;
			case DataProperty.G: return 1;
			case DataProperty.B: return 1;
			case DataProperty.A: return 1;
			case DataProperty.Data8: return 1;
			case DataProperty.Data16: return 2;
			case DataProperty.Data32: return 4;
			}
			return 0;
		}

		class DataHeader
		{
			public List<DataProperty> properties = new List<DataProperty>();
			public int vertexCount = -1;
		}

		class DataBody
		{
			public List<Vector3> vertices;
			public List<Color32> colors;

			public DataBody(int vertexCount)
			{
				vertices = new List<Vector3>(vertexCount);
				colors = new List<Color32>(vertexCount);
			}

			public void AddPoint(
				float x, float y, float z,
				byte r, byte g, byte b, byte a
			)
			{
				vertices.Add(new Vector3(x, y, z));
				colors.Add(new Color32(r, g, b, a));
			}
		}

		#endregion

		#region Reader implementation


		Mesh ImportAsMesh(string path)
		{
			try
			{
				
				List<Vector3> points = new List<Vector3>();
				List<Color32> colors = new List<Color32>();
				DracoPointCloudLoader draco_loader = new DracoPointCloudLoader ();

				int num_points = draco_loader.LoadPointsFromPath (path, ref points, ref colors);


				var mesh = new Mesh();
				mesh.name = Path.GetFileNameWithoutExtension(path);

				mesh.indexFormat = points.Count > 65535 ?
					IndexFormat.UInt32 : IndexFormat.UInt16;

				mesh.SetVertices(points);
				mesh.SetColors(colors);

				mesh.SetIndices(
					Enumerable.Range(0, points.Count).ToArray(),
					MeshTopology.Points, 0
				);

				mesh.UploadMeshData(true);

				return mesh;
			}
			catch (Exception e)
			{
				Debug.LogError("Failed importing " + path + ". " + e.Message);
				return null;
			}
		}

		PointCloudData ImportAsPointCloudData(string path)
		{
			try
			{
				List<Vector3> points = new List<Vector3>();
				List<Color32> colors = new List<Color32>();
				DracoPointCloudLoader draco_loader = new DracoPointCloudLoader ();

				int num_points = draco_loader.LoadPointsFromPath (path, ref points, ref colors);

				var data = ScriptableObject.CreateInstance<PointCloudData>();
				data.Initialize(points, colors);
				data.name = Path.GetFileNameWithoutExtension(path);
				return data;
			}
			catch (Exception e)
			{
				Debug.LogError("Failed importing " + path + ". " + e.Message);
				return null;
			}
		}
	}

	#endregion
}
