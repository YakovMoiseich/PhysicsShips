using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastController : MonoBehaviour
{

	public float sailForceMultiplier = 0.05f;

	private Transform _sail;
	private Mesh _sailMesh;
	private float _minSailSize = 0.1f;
	private float _maxSailSize = 4.0f;
	private float _initialSailSizeY;
	private float _initialSailPositionY;

	void ChangeSailState(float sailSizeChange) {
		sailSizeChange *= 0.1f;
		Vector3 updatedSaleSize = _sail.localScale;
		Vector3 updatedSalePosition = _sail.localPosition;

		float sailNewSizeY = Mathf.Min(Mathf.Max(updatedSaleSize.y + sailSizeChange, _minSailSize), _maxSailSize);
		float sailNewPositionY = _initialSailPositionY - (sailNewSizeY - _initialSailSizeY) / 2.0f;

		Debug.Log("sailSizeChange = " + sailSizeChange + "; _initialSailSizeY = " + _initialSailSizeY + "; sailNewPosition = " + _initialSailPositionY);

		updatedSaleSize.y = sailNewSizeY;
		updatedSalePosition.y = sailNewPositionY;
		_sail.localScale = updatedSaleSize;
		_sail.localPosition = updatedSalePosition;
	}

	public Vector3 ApplyWind(Vector3 windVector) {
		float windToForceFactor = Vector3.Dot(GetSailNormal(), windVector);
		Vector3 resultWindForce = GetSailNormal() * GetSailSquare() * sailForceMultiplier * windToForceFactor;
		return resultWindForce;
//		return Vector3.zero;
	}

	void Awake() {
		_sail = transform.Find("Sail");
		_initialSailSizeY = _sail.localScale.y;
		_initialSailPositionY = _sail.localPosition.y;
		_sailMesh = _sail.GetComponent<MeshFilter>().mesh;
	}

	void Start () {
		
	}
	
	void Update () {
		float verticalInput = Input.GetAxis("Vertical");
		ChangeSailState(verticalInput);
	}

	Vector3 GetSailNormal() {
		return transform.forward;
	}

	float GetSailSquare() {
		return _sail.lossyScale.x * _sail.lossyScale.y;
	}
}
