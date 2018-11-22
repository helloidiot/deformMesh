﻿using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SurfaceFlow : MonoBehaviour {

	public SurfaceCreator surface;

	private ParticleSystem system;
	private ParticleSystem.Particle[] particles;

	public float flowStrength;

	private void LateUpdate () {
		if (system == null) {
			system = GetComponent<ParticleSystem>();
		}
		if (particles == null || particles.Length < system.maxParticles) {
			particles = new ParticleSystem.Particle[system.maxParticles];
		}
		int particleCount = system.GetParticles(particles);
		PositionParticles();
		system.SetParticles(particles, particleCount);
	}
	
	private void PositionParticles () {
		Quaternion q = Quaternion.Euler(surface.rotation);
		Quaternion qInv = Quaternion.Inverse(q);
		NoiseMethod method = Noise.noiseMethods[(int)surface.type][surface.dimensions - 1];
		float amplitude = surface.damping ? surface.strength / surface.frequency : surface.strength;
		
		for (int i = 0; i < particles.Length; i++) {

			Vector3 position = particles[i].position;
			Vector3 point = q * new Vector3(position.x, position.z) + surface.offset;
			NoiseSample sample = Noise.Sum(method, point, surface.frequency, surface.octaves, surface.lacunarity, surface.persistence);
			sample = surface.type == NoiseMethodType.Value ? (sample - 0.5f) : (sample * 0.5f);
			sample *= amplitude;
			sample.derivative = qInv * sample.derivative;
			Vector3 curl = new Vector3(sample.derivative.y, 0f, -sample.derivative.x);
			position += curl * Time.deltaTime * flowStrength;
			position.y = sample.value + system.startSize;
			particles[i].position = position;

			// Kill particle if if goes beyond the shape
			if (position.x < -0.5f || position.x > 0.5f || position.z < -0.5f || position.z > 0.5f) {
				particles[i].lifetime = 0f;
			}
		}
	}

}