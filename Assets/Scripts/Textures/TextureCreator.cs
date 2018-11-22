using UnityEngine;

public class TextureCreator : MonoBehaviour {

	[Range(2, 512)]
	private Texture2D texture;

	public int resolution = 256;
	public float frequency = 1f;

	[Range(1, 8)]
	public int octaves = 1;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(1, 3)]
	public int dimensions = 3;

	public Gradient coloring;

	private void OnEnable () {

		if (texture == null) {
			texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";

			// Clamp the edges
			texture.wrapMode = TextureWrapMode.Clamp;

			// // Point
			// texture.filterMode = FilterMode.Point;
			// // Billinear
			// texture.filterMode = FilterMode.Bilinear;
			// Trillinear
			texture.filterMode = FilterMode.Trilinear;
			// Anistropic filtering
			texture.anisoLevel = 9;

			GetComponent<MeshRenderer>().material.mainTexture = texture;
			FillTexture();
		}
	}

	private void Update () {

		// If there's a change, update the textures
		if (transform.hasChanged) {
			transform.hasChanged = false;
			FillTexture();
		}
		
	}

	public NoiseMethodType type;

	public void FillTexture () {

		// Define world space coordinates
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		// Check whether resolution has changed
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}

		NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];

		// Normalise to 0-1
		float stepSize = 1f / resolution;

		for (int y = 0; y < resolution; y++) {
			
			// Interpolate between bottom left and top left
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			// Interpolate between bottom right and top right
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);

			for (int x = 0; x < resolution; x++) {
				// Interpolate between point0 and point1 intermediate points based on x
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);

				// // World space colours
				// texture.SetPixel(x, y, new Color(point.x, point.y, point.z));

				// Scale if using Perlin noise - mapping -1–1 to 0–1.
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence).value;
				if (type != NoiseMethodType.Value) {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}

		texture.Apply();
	}

	
}