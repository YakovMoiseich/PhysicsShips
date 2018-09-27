﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour {

	public float deckFrictionForceFactor = 0.5f;
	public float deckMass = 5.0f;
	private Vector3 _deckSize;
	private Vector3 _drawdownSize;

	void Awake() {
		_deckSize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	public void SetDrawdownSize(float additionalMass) {
		_drawdownSize = PhysicsHelper.CalculateObjectDrawdown(deckMass + additionalMass, _deckSize);
	}

	public Vector3 GetDrawdownSize() {
		return _drawdownSize;
	}

	public Vector3 GetDeckWaterFrictionForce(Vector3 shipSpeed) {
		Vector3 resultFrictionForce = Vector3.zero;
		int deckNormals = 4;
		for (int i = 0; i < deckNormals; ++i){
			resultFrictionForce += PhysicsHelper.CalculateObjectFrictionForce(shipSpeed, GetDeckPartNormal(i), GetDrawdownSquare(i), deckFrictionForceFactor);
		}

		return resultFrictionForce;
	}

	Vector3 GetDeckPartNormal(int partIndex) {
		switch (partIndex) {
			case 0: return transform.forward;
				break;
			case 1: return transform.right;
				break;
			case 2: return -transform.forward;
				break;
			case 3: return -transform.right;
				break;
			default: return Vector3.zero;
		}
	}

	float GetDrawdownSquare(int partIndex) {
		int partMultiplicity = partIndex % 2;
		float partWide = partIndex % 2 == 0 ? transform.localScale.x : transform.localScale.z;
		return GetDrawdownSize().y * partWide;
	}

	void Start () {
		
	}
	
	void Update () {
		
	}
}
