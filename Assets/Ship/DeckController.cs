using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour {

	public float deckWaterFrictionFactor = 2.0f;
	public float deckWindAccelerationFactor = 0.01f;
	public float deckMass = 5.0f;
	private Vector3 _deckSize;
	private Vector3 _drawdownSize;

	private List<Transform> _controlPointsDebugs;
	private List<Vector3> _deckControlPoints;
	private Collider _deckCollider;

	void Awake() {
		_deckSize = new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
		_deckCollider = transform.GetComponent<BoxCollider>();
		_controlPointsDebugs = new List<Transform>();
		for (int i = 0; i < 4; ++i) {
			_controlPointsDebugs.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere).transform);
			_controlPointsDebugs[i].GetComponent<SphereCollider>().enabled = false;
		}

		_deckControlPoints = new List<Vector3>();
		UpdateDeckControlPoints();
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
		for (int i = 0; i < deckNormals; ++i) {
			resultFrictionForce += PhysicsHelper.CalculateObjectFrictionForce(shipSpeed, GetDeckPartNormal(i), GetDrawdownSquare(i), deckWaterFrictionFactor);
		}

		return resultFrictionForce;
	}

	public Vector3 GetDeckWindForce(Vector3 windSpeed) {
		Vector3 resultWindAccelerationForce = Vector3.zero;
		int deckNormals = 4;
		for (int i = 0; i < deckNormals; ++i) {
			resultWindAccelerationForce += PhysicsHelper.CalculateObjectAcceleratingForce(windSpeed, GetDeckPartNormal(i), GetUnderWaterSquare(i), deckWindAccelerationFactor);
		}

		return resultWindAccelerationForce;
	}

	Vector3 GetDeckPartNormal(int partIndex) {
		switch (partIndex) {
			case 0: return transform.parent.forward;
				break;
			case 1: return transform.parent.right;
				break;
			case 2: return -transform.parent.forward;
				break;
			case 3: return -transform.parent.right;
				break;
			default: return Vector3.zero;
		}
	}

	float GetDrawdownSquare(int partIndex) {
		int partMultiplicity = partIndex % 2;
		float partWide = partIndex % 2 == 0 ? transform.lossyScale.x : transform.lossyScale.z;
		return GetDrawdownSize().y * partWide;
	}

	float GetUnderWaterSquare(int partIndex) {
		int partMultiplicity = partIndex % 2;
		float partWide = partIndex % 2 == 0 ? transform.lossyScale.x : transform.lossyScale.z;
		return (transform.localScale.y - GetDrawdownSize().y) * partWide;
	}

	void UpdateDeckControlPoints() {
		Vector3 centerToBottom = _deckCollider.bounds.center - transform.up;
		Vector3 toForward = transform.forward * transform.localScale.z / 2.0f;
		Vector3 toRight = transform.right * transform.localScale.x / 2.0f;
		Vector3 forwardLeft = centerToBottom + toForward - toRight;
		Vector3 forwardRight = centerToBottom + toForward + toRight;
		Vector3 backLeft = centerToBottom - toForward - toRight;
		Vector3 backRight = centerToBottom - toForward + toRight;

		if (_deckControlPoints.Count == 0) {
			for (int i = 0; i < 4; ++i) {
				_deckControlPoints.Add(Vector3.zero);
			}
		}

		_deckControlPoints[0] = forwardLeft;
		_deckControlPoints[1] = forwardRight;
		_deckControlPoints[2] = backLeft;
		_deckControlPoints[3] = backRight;

		for (int j = 0; j < _deckControlPoints.Count; ++j) {
			_controlPointsDebugs[j].position = _deckControlPoints[j];
		}
	}
	
	void OnCTriggerStay(Collider water) {
		if (water.tag != "Water") {
			return;
		}

//		water.ClosestPoint()
	}

	

	void Start () {
		
	}
	
	void Update ()
	{
		UpdateDeckControlPoints();
	}
}
