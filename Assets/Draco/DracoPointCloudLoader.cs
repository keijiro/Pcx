using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;


public unsafe class DracoPointCloudLoader
{
	// Must stay the order to be consistent with C++ interface.
	[StructLayout (LayoutKind.Sequential)]
	private struct DracoToUnityPointCloud
	{
		public int num_vertices;
		public IntPtr position;
		public IntPtr color;
		public int num_points;
		public IntPtr vertex_indices;
	}

	private struct DecodedPoints
	{
		public Vector3[] vertices;
		public Color32[] colors;
	}

	[DllImport ("dracodec_unity")]
	private static extern int DecodePointCloudForUnity (byte[] buffer, int length, DracoToUnityPointCloud**tmp_point_cloud);

	static private int max_num_vertices_per_mesh = 60000;

	private float ReadFloatFromIntPtr (IntPtr data, int offset)
	{
		byte[] byte_array = new byte[4];
		for (int j = 0; j < 4; ++j) {
			byte_array [j] = Marshal.ReadByte (data, offset + j);
		}
		return BitConverter.ToSingle (byte_array, 0);
	}
		
	public int LoadPointsFromPath (string path, ref List<Vector3> points, ref List<Color32> colors)
	{
		
		var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

		var bin_reader = new BinaryReader(stream);
		var data = bin_reader.ReadBytes(int.MaxValue);

		if (data.Length == 0) {
			Debug.Log ("Didn't load data!");
			return -1;
		}

		return DecodePoints (data, ref points, ref colors);
	}

	public int LoadPointsFromAsset (string asset_name, ref List<Vector3> points, ref List<Color32> colors)
	{
		TextAsset asset = Resources.Load (asset_name, typeof(TextAsset)) as TextAsset;
		if (asset == null) {
			Debug.Log ("Didn't load file!");
			return -1;
		}

		byte[] data = asset.bytes;
		Debug.Log (data.Length.ToString ());
		if (data.Length == 0) {
			Debug.Log ("Didn't load data!");
			return -1;
		}
			
		return DecodePoints (data, ref points, ref colors);
	}

	/*
	public IEnumerator LoadPointsFromURL(string url, ref Vector3 points) {
		WWW www = new WWW (url);
		yield return www;
		if (www.bytes.Length == 0)
			return -1;
		return DecodePoints (www.bytes, ref points);
	}
*/
	public unsafe int DecodePoints (byte[] data, ref List<Vector3> points, ref List<Color32> colors)
	{

		DracoToUnityPointCloud* tmp_point_cloud;
		if (DecodePointCloudForUnity (data, data.Length, &tmp_point_cloud) <= 0) {
			Debug.Log ("Failed: Decoding error.");
			return -1;
		}

		//Set up verts from draco
		Vector3[] new_points = new Vector3[tmp_point_cloud->num_vertices];

		int byte_stride_per_value = 4;
		int num_value_per_vertex = 3;
		int byte_stride_per_vertex = byte_stride_per_value * num_value_per_vertex;
		for (int i = 0; i < tmp_point_cloud->num_vertices; ++i) {
			for (int j = 0; j < 3; ++j) {
					new_points [i] [j] = 
						ReadFloatFromIntPtr (tmp_point_cloud->position, i * byte_stride_per_vertex + byte_stride_per_value * j);
			}
		}
			
		points.AddRange (new_points);

		// Build Colors for draco
		Color[] nc = new Color[tmp_point_cloud->num_vertices];

		int color_byte_stride_per_value = 4;
		int color_num_value_per_vertex = 4;
		int color_byte_stride_per_vertex = color_byte_stride_per_value * color_num_value_per_vertex;

		Byte r = 255, g = 255, b = 255, a = 255;
		for (int i = 0; i < tmp_point_cloud->num_vertices; ++i) {
			for (int j = 0; j < 4; ++j) {
				nc [i] [j] = 
					ReadFloatFromIntPtr (tmp_point_cloud->color, i * color_byte_stride_per_vertex + color_byte_stride_per_value * j);
			}
			colors.Add((Color32)nc [i]);
		}

		Marshal.FreeCoTaskMem (tmp_point_cloud->position);
		Marshal.FreeCoTaskMem ((IntPtr)tmp_point_cloud);

		return points.Count;


	}

}
