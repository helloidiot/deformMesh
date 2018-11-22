using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class MeshDeformer : MonoBehaviour {

	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3[] vertexVelocities;

	public float springForce = 20f;
	public float damping = 5f;
	float uniformScale = 1f;

	// Debug
	Color red = new Color(1f, 0f, 0f); // Ray

	void Start () {

		// Acquire the mesh and copy its vertices to the displaced vertices
		deformingMesh = GetComponent<MeshFilter>().mesh;

		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];

		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}

		// Store the velocity of each vertex
		vertexVelocities = new Vector3[originalVertices.Length];
	}

	void Update () {

		uniformScale = transform.localScale.x; // Non-uniform scaled objects?

		for (int i = 0; i < displacedVertices.Length; i++) {
			UpdateVertex(i);
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

	void UpdateVertex (int i) {
		Vector3 velocity = vertexVelocities[i];
		Vector3 displacement = displacedVertices[i] - originalVertices[i];

		// Scale the displacement by the uniform scale for correct distance.
		displacement *= uniformScale;

		velocity -= displacement * springForce * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;

		vertexVelocities[i] = velocity;

		displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
	}

	public void AddDeformingForce (Vector3 point, float force) {
		// Draw ray
		Debug.DrawLine(Camera.main.transform.position, point, red);

		point = transform.InverseTransformPoint(point);

		for (int i = 0; i < displacedVertices.Length; i++) {
			AddForceToVertex(i, point, force);
		}
	}

	void AddForceToVertex (int i, Vector3 point, float force) {
		//  Direction and the distance of the deforming force per vertex
		Vector3 pointToVertex = displacedVertices[i] - point;

		// Scale the vertex by the uniform scale to use the correct distance
		pointToVertex *= uniformScale;

		// Inverse square law. Guarantees the force is at full strength when the distance is zero
		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);

		// Calculate the velocity
		float velocity = attenuatedForce * Time.deltaTime;

		// Normalise initial vector and add to velocity
		vertexVelocities[i] += pointToVertex.normalized * velocity;

	}

}