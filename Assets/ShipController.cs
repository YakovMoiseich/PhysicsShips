using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

	private float _helmAlignSpeedDegrees = 0.5f;
	private float _helmRotateSpeedDegrees = 2.0f;

	private float maxHelmAngle = 55.0f;
	private float minHelmAngle = -55.0f;

	private float _helmOffsetDegrees;
	private Transform _helm;

	void Awake() {
		_helm = transform.Find("Helm");
		_helmOffsetDegrees = 0.0f;
	}

	void Start() {
		
	}

	void Update() {
		AlignHelm();

		float verticalInput = Input.GetAxis("Vertical");
		float horizontalInput = Input.GetAxis("Horizontal");
		RotateHelmOnInput(horizontalInput);
	}

	void RotateHelmOnInput(float horizontalInput) {
		float angleToRotateInDegrees = -horizontalInput * _helmRotateSpeedDegrees;
		angleToRotateInDegrees = LimitHelmOffset(angleToRotateInDegrees);
		_helm.transform.Rotate(Vector3.up, angleToRotateInDegrees);
	}

	float LimitHelmOffset(float helmRotationToApply) {
		float currentHelmRotation = GetCurrentHelmSignedRotation();
		helmRotationToApply = currentHelmRotation + helmRotationToApply > maxHelmAngle ? maxHelmAngle - currentHelmRotation
			: currentHelmRotation + helmRotationToApply < minHelmAngle ? minHelmAngle - currentHelmRotation 
			: helmRotationToApply;
		return helmRotationToApply;
	}

	void AlignHelm() {
		float currentHelmRotation = GetCurrentHelmSignedRotation();
		float currentHelmRotationSign = Mathf.Sign(currentHelmRotation);
		float alignHelmRotation = -currentHelmRotationSign * _helmAlignSpeedDegrees;
		bool rotationChangeSign = Mathf.Sign(currentHelmRotation + alignHelmRotation) != currentHelmRotationSign;
		alignHelmRotation = rotationChangeSign ? -currentHelmRotation : alignHelmRotation;
		_helm.transform.Rotate(Vector3.up, alignHelmRotation);
	}

	float GetCurrentHelmSignedRotation() {
		float currentHelmRotation = _helm.transform.eulerAngles.y;
		currentHelmRotation = currentHelmRotation > 180.0f ? currentHelmRotation - 360.0f : currentHelmRotation;
		return currentHelmRotation;
	}
}
