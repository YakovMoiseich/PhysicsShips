using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {

	public float windSpeed = 5.0f;

	private GameObject _windZone;
	private Mesh _windZoneMesh;
	private ParticleSystem _windParticles;

	void Awake() {
		_windZone = gameObject;
		_windParticles = gameObject.GetComponent<ParticleSystem>();
		_windZoneMesh = gameObject.GetComponent<MeshFilter>().mesh;

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
		_windZoneMesh.RecalculateBounds();
		Debug.Log("_windZoneMesh.bounds = " + _windZoneMesh.bounds);
	}

}
