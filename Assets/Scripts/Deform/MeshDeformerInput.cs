using UnityEngine;

public class MeshDeformerInput : MonoBehaviour {

	public float force = 10f;
	public float forceOffset = 0.1f;

	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleAddInput();
		}
		if (Input.GetMouseButton(1)) {
			HandleSubtractInput();
		}
	}

	void HandleAddInput(){
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit)) {
			MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();

			if (deformer) {
				Vector3 point = hit.point;
				// Deform in direction of the normal * the offset
				point += hit.normal * forceOffset;
				deformer.AddDeformingForce(point, force);
			}
		}
	}
	void HandleSubtractInput(){
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit)) {
			MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();

			if (deformer) {
				Vector3 point = hit.point;
				// Deform in direction of the normal * the offset
				point -= hit.normal * forceOffset;
				deformer.AddDeformingForce(point, force);
			}
		}
	}
}