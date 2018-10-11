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

	private int _controlPointsLevels = 2;
	private int _controlPointsQuantity = 6;
	private float _controlPointDegreesOffset;
	private float _controlPointsLevelHeight;
	private List<Vector3> _deckControlPoints = new List<Vector3>();
	private List<Transform> _controlPointsDebugs = new List<Transform>();

	private Collider _deckCollider;

	void Awake() {
		_deckSize = new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
		_deckCollider = transform.GetComponent<Collider>();

		InitDeckControlPoints();
		UpdateDeckControlPoints();
	}

	void InitDeckControlPoints() {
		_controlPointDegreesOffset = 360.0f / _controlPointsQuantity;
		_controlPointsLevelHeight = transform.localScale.y / _controlPointsLevels;
		for (int i = 0; i < _controlPointsLevels; ++i) {
			for (int j = 0; j < _controlPointsQuantity; ++j) {
				_deckControlPoints.Add(Vector3.zero);
				_controlPointsDebugs.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere).transform);
				_controlPointsDebugs[i * _controlPointsQuantity + j].GetComponent<SphereCollider>().enabled = false;
				_controlPointsDebugs[i * _controlPointsQuantity + j].localScale *= 0.7f;
			}
		}
	}

	void UpdateDeckControlPoints() {
		Vector3 pointToRayscast;
		RaycastHit hit;
		for (int i = 0; i < _controlPointsLevels; ++i) {
			pointToRayscast = _deckCollider.bounds.center - transform.up * transform.localScale.y / 2.0f;
			pointToRayscast.y += _controlPointsLevelHeight * i;
			for (int j = 0; j < _controlPointsQuantity; ++j) {
				Vector3 pointFromRaycast = pointToRayscast + Quaternion.Euler(0.0f, _controlPointDegreesOffset * j, 0.0f) * transform.forward * _deckCollider.bounds.size.magnitude / 2.0f;
				pointFromRaycast.y = pointToRayscast.y;
				if (!Physics.Raycast(pointFromRaycast, pointToRayscast - pointFromRaycast, out hit, Mathf.Infinity)) {
					continue;
				}

				_deckControlPoints[i * _controlPointsQuantity + j] = hit.point;
			}
		}
	}

	void UpdateDebugSpheres() {
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			Vector3 controlPointWorldPosition = _deckControlPoints[i];
			_controlPointsDebugs[i].position = controlPointWorldPosition;
		}
	}

	void CalculateDeckControlPoints() {

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

	

	
	
	void OnCTriggerStay(Collider water) {
		if (water.tag != "Water") {
			return;
		}

//		water.ClosestPoint()
	}

	

	void Start () {
		
	}
	
	void Update () {
		UpdateDeckControlPoints();
		UpdateDebugSpheres();
	}
}
