using UnityEngine;

public class NucleonSpawner : MonoBehaviour {

	public float timeBetweenSpawns;
	public float spawnDistance;
	float timeSinceLastSpawn;
	public bool bMouse;

	Ray ray;
	RaycastHit hit;

	public Nucleon[] nucleonPrefabs;

	void Update(){

	}
	
	void FixedUpdate () {
		timeSinceLastSpawn += Time.deltaTime;

		// Telling ray variable that the ray will go from the center of main camera to mouse
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);	

		// Of we hit something, store info in hit
		if (Physics.Raycast (ray, out hit)) {
			// Spawn on mouse
			if (Input.GetMouseButton(0)) {
				SpawnNucleonOnMouse();
			}
		}

		else if (!bMouse && timeSinceLastSpawn >= timeBetweenSpawns) {
			timeSinceLastSpawn -= timeBetweenSpawns;
			SpawnNucleon();
		}
	}

	public void SpawnNucleon () {
		Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
		Nucleon spawn = Instantiate<Nucleon>(prefab);
		spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
	}

	public void SpawnNucleonOnMouse () {
		Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
		Nucleon spawn = Instantiate<Nucleon>(prefab, hit.point, Quaternion.identity);
		Debug.Log ("Spawned at: " + hit.point);
		spawn.transform.localPosition = hit.point;
	}

}
