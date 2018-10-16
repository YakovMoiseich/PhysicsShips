using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class DeckController : MonoBehaviour {

	public float deckWaterFrictionFactor = 2.0f;
	public float deckWindAccelerationFactor = 0.01f;
	public float deckMass = 5.0f;
	private Vector3 _deckSize;
	private Vector3 _drawdownSize;
	private float _deckFullVolume = 0.0f;

	private int _controlPointsLevels = 1;
	private int _controlPointsQuantity = 13;
	private int _controlPointsForceStepsMultiplier = 10;
	private float _controlPointDegreesOffset;
	private float _controlPointsLevelHeight;
	private List<Vector3> _deckControlPoints = new List<Vector3>();
	private List<Vector3> _deckControlPointsClosestToWater = new List<Vector3>();
	private List<Transform> _controlPointsDebugs = new List<Transform>();

	private MeshCollider _deckMeshCollider;
	private Mesh _deckDrawdownMesh;

	void Awake() {
		InitDeck();
		UpdateDeckControlPoints();
	}

	public Dictionary<Vector3, Vector3> GetExtrudingControlForces() {
		float underwaterMeshVolume = CalculateDeckDrawdownVolume();
		float overallExtrudingYForce = PhysicsHelper.CalcuateOverallExtrudingYForce(underwaterMeshVolume);
		List<float> offsets = new List<float>();

		float overallOffset = 0.0f;
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			offsets.Add((_deckControlPoints[i] - _deckControlPointsClosestToWater[i]).y);
		}
		float maxFromWaterDistance = offsets.Max();

		List<float> pointsWeigths = new List<float>();
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			float pointWeight = Mathf.Abs(maxFromWaterDistance - _deckControlPoints[i].y);
			pointsWeigths.Add(pointWeight);
		}

		float forceUnit = overallExtrudingYForce / pointsWeigths.Sum();
		Dictionary<Vector3, Vector3> resultPointsForces = new Dictionary<Vector3, Vector3>();
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			resultPointsForces[_deckControlPoints[i]] = new Vector3(0.0f, pointsWeigths[i] * -forceUnit, 0.0f);
		}

		return resultPointsForces;
	}

	void InitDeck() {
		_deckMeshCollider = transform.GetComponent<MeshCollider>();
		_deckDrawdownMesh = new Mesh();

		_controlPointDegreesOffset = 360.0f / _controlPointsQuantity;
		_controlPointsLevelHeight = transform.localScale.y / _controlPointsLevels;
		for (int i = 0; i < _controlPointsLevels; ++i) {
			for (int j = 0; j < _controlPointsQuantity; ++j) {
				_deckControlPoints.Add(Vector3.zero);
				_deckControlPointsClosestToWater.Add(Vector3.zero);
				_controlPointsDebugs.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere).transform);
				_controlPointsDebugs[i * _controlPointsQuantity + j].GetComponent<SphereCollider>().enabled = false;
				_controlPointsDebugs[i * _controlPointsQuantity + j].localScale *= 0.7f;
			}
		}

		for (int i = 0; i < _deckMeshCollider.sharedMesh.vertices.Length - 3; i += 3) {
			_deckFullVolume += GeometryHelper.CalculateTriangleVolume(_deckMeshCollider.sharedMesh.vertices[i], _deckMeshCollider.sharedMesh.vertices[i + 1], _deckMeshCollider.sharedMesh.vertices[i + 2]);
		}
	}

	void UpdateDeckControlPoints() {
		Vector3 pointToRayscast;
		RaycastHit hit;
		for (int i = 0; i < _controlPointsLevels; ++i) {
			pointToRayscast = _deckMeshCollider.bounds.center - transform.up * transform.localScale.y / 2.0f;
			pointToRayscast.y += _controlPointsLevelHeight * i;
			for (int j = 0; j < _controlPointsQuantity; ++j) {
				Vector3 pointFromRaycast = pointToRayscast + Quaternion.Euler(0.0f, _controlPointDegreesOffset * j, 0.0f) * transform.forward * _deckMeshCollider.bounds.size.magnitude / 2.0f;
				pointFromRaycast.y = pointToRayscast.y;
				if (!Physics.Raycast(pointFromRaycast, pointToRayscast - pointFromRaycast, out hit, Mathf.Infinity)) {
					continue;
				}

				_deckControlPoints[i * _controlPointsQuantity + j] = hit.point;
			}
		}
	}

	void UpdateDebugSpheres(List<Vector3> positionsToUpdate) {
		for (int i = 0; i < positionsToUpdate.Count; ++i) {
			Vector3 controlPointWorldPosition = positionsToUpdate[i];
			_controlPointsDebugs[i].position = controlPointWorldPosition;
		}
	}

	Mesh UpdateDrawdownMesh(MeshCollider waterMeshCollider) {
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			_deckControlPointsClosestToWater[i] = waterMeshCollider.ClosestPoint(_deckControlPoints[i]);
		}

		List<Vector3> undewaterVertices = new List<Vector3>();
		int underwaterVertexCounter = 0;
		for (int j = 0; j < _deckMeshCollider.sharedMesh.vertices.Length - 3; j += 3) {
			Vector3 vertex = _deckMeshCollider.sharedMesh.vertices[j];
			float waterHeigth = _deckControlPoints[GetClosestControlPointIndex(vertex)].y;
			if (vertex.y < waterHeigth) {
				undewaterVertices.Add(vertex);
				undewaterVertices.Add(_deckMeshCollider.sharedMesh.vertices[j + 1]);
				undewaterVertices.Add(_deckMeshCollider.sharedMesh.vertices[j + 2]);
			}
		}

		_deckDrawdownMesh.vertices = undewaterVertices.ToArray();
		return _deckDrawdownMesh;
	}

	float CalculateDeckDrawdownVolume() {
		float drawdownResultVolume = 0.0f;
		for (int i = 0; i < _deckDrawdownMesh.vertices.Length - 3; i += 3) {
			drawdownResultVolume += GeometryHelper.CalculateTriangleVolume(_deckDrawdownMesh.vertices[i], _deckDrawdownMesh.vertices[i + 1], _deckDrawdownMesh.vertices[i + 2]);
		}

		drawdownResultVolume = _deckFullVolume - Mathf.Abs(drawdownResultVolume);
		return drawdownResultVolume;
	}

	int GetClosestControlPointIndex(Vector3 vertex) {
		float shortestDistance = Mathf.Infinity;
		int closestIndex = 0;
		for (int i = 0; i < _deckControlPoints.Count; ++i) {
			float distance = Vector3.Distance(vertex, _deckControlPoints[i]);
			if (shortestDistance > distance) {
				shortestDistance = distance; 
				closestIndex = i;
			}
		}

		return closestIndex;
	}

	void OnTriggerStay(Collider water) {
		if (water.tag != "Water") {
			return;
		}

		_deckDrawdownMesh = UpdateDrawdownMesh((MeshCollider)water);
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
			case 0:
				return transform.parent.forward;
				break;
			case 1:
				return transform.parent.right;
				break;
			case 2:
				return -transform.parent.forward;
				break;
			case 3:
				return -transform.parent.right;
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

	void Start () {
		
	}
	
	void Update () {
		UpdateDeckControlPoints();
		UpdateDebugSpheres(_deckControlPointsClosestToWater);
	}
}
