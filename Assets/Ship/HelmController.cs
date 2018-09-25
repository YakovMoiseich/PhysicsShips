using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmController : MonoBehaviour {

	private float _helmAlignSpeedDegrees = 0.5f;
	private float _helmRotateSpeedDegrees = 2.0f;

	private float maxHelmAngle = 55.0f;
	private float minHelmAngle = -55.0f;

	private float _helmFrictionFactor = 0.1f;

	private float _helmSquare;
	private float _helmOffsetDegrees;
	private Transform _helm;

	void Awake() {
		_helm = transform;
		_helmOffsetDegrees = 0.0f;
		_helmSquare = transform.lossyScale.x * transform.lossyScale.y;
	}

	void Start() {

	}

	void Update() {
		AlignHelm();
		float horizontalInput = Input.GetAxis("Horizontal");
		RotateHelmOnInput(horizontalInput);
	}

	public float GetHelmTurnForce(float currentShipSpeed) {
		return currentShipSpeed * _helmSquare * _helmFrictionFactor * -GetCurrentHelmSignedRotation();
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
		float currentHelmRotation = _helm.transform.localEulerAngles.y;
		currentHelmRotation = currentHelmRotation > 180.0f ? currentHelmRotation - 360.0f : currentHelmRotation;
		return currentHelmRotation;
	}
}
