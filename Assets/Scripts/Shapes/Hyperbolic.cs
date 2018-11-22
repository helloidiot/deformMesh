using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Hyperbolic : MonoBehaviour {

	public int xSize, ySize;
	private Vector3[] vertices;

	public float wave = 0.5f;
	public float hyperFx = 3f;
	public float hyperFy = 3f;
	public float amplify = 1f;
	public float scl = 1;

	private bool bDrawGizmos = false;

	// public float wave, hyperFx, hyperFy, amplify, scl;

	private void Awake () {
		Generate();
	}

	private Mesh mesh;

	private void Generate () {

			GetComponent<MeshFilter>().mesh = mesh = new Mesh();
			mesh.name = "Hyperbolic Paraboloid";

			CreateVertices();
			CreateTriangles();
			CreateColliders();
	}

	private void CreateVertices(){

		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		float newWave = 2 * Mathf.PI * Mathf.Sqrt(wave);

		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x * (xSize * scl / 2), y * (ySize * scl / 2));

				float fx = map(vertices[i].x, 0f, xSize, 0f, hyperFx);
				float fy = map(vertices[i].y, 0f, ySize, 0f, hyperFy);

				// Calculate hyperbola
				float gaussian = Mathf.Exp(-(Mathf.Sqrt(fx) + Mathf.Sqrt(fy)) / newWave); 

				Vector3 temp = new Vector3(fx, fy);

				vertices[i].z = gaussian * (Vector3.Distance(vertices[i], temp)) * amplify;

				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
	}

	private void CreateTriangles(){
		// Create triangles
		int[] triangles = new int[xSize * ySize * 6];

		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
				mesh.triangles = triangles;
				mesh.RecalculateNormals();
			}
		}
	}

	private void Update(){
		UpdateMesh();

		if (Input.GetKeyDown("d")){
            bDrawGizmos = !bDrawGizmos;
			print("Gizmos: " + bDrawGizmos);
        }

	}

	private void UpdateMesh(){
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	private void CreateColliders () {
		gameObject.AddComponent<MeshCollider>();
	}

	private void OnDrawGizmos () {

		// Check whether there are any vertices (to avoid errors when not in play mode)
		if (vertices == null) {
			return;
		}
		if (bDrawGizmos){
			// Draw gizmos for each vertex position
			Gizmos.color = Color.yellow;
			for (int i = 0; i < vertices.Length; i++) {
				Gizmos.DrawSphere(vertices[i], 0.1f);
			}
		}
		
	}

	public float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax){
 
		float OldRange = (OldMax - OldMin);
		float NewRange = (NewMax - NewMin);
		float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
	
		return(NewValue);
	}
}
