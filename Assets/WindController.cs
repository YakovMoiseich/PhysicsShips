using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {

	public float windSpeed = 5.0f;

	private ParticleSystem _windParticles;
	private Vector3 _windDirection;

	public Vector3 GetWindVector() {
		return _windDirection * windSpeed;
	}

	void Awake() {
		_windParticles = gameObject.GetComponent<ParticleSystem>();
		_windDirection = new Vector3(0.0f, 0.0f, 1.0f);
	}

	void HideParticles() {
		_windParticles.Stop();
	}

	void UpdateParticlesSpeed(float newSpeed) {
		ParticleSystem.MainModule updatableMain = _windParticles.main;
		if (newSpeed == updatableMain.startSpeed.constant) {
			return;
		}

		ParticleSystem.NoiseModule updatableNoise = _windParticles.noise;
		updatableNoise.frequency = windSpeed / 25.0f;
		updatableMain.startSpeed = windSpeed;
	}

	void Start() {
		
	}
	
	void Update() {
		UpdateParticlesSpeed(windSpeed);
	}

}
