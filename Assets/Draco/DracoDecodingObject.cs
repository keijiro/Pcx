using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DracoDecodingObject : MonoBehaviour {

	public TextMesh debug_text;

	// Use this for initialization
	void Start () {
		debug_text = GetComponentInChildren<TextMesh> ();

		List<Vector3> points = new List<Vector3>();
		List<Color32> colors = new List<Color32>();
		DracoPointCloudLoader draco_loader = new DracoPointCloudLoader ();
		int num_verts = draco_loader.LoadPointsFromAsset ("bunny", ref points, ref colors);

		// TODO: Meshes.
			/*
		if (num_verts > 0) {
			GetComponent<MeshFilter> ().mesh = mesh[0];
		}*/

		debug_text.text = num_verts.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0, 10 * Time.deltaTime, 0);
	}
}