﻿using System.Collections;
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
	

	void Awake() {
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
		float oldTurnForce = currentShipSpeed * _helmSquare * _helmFrictionFactor * -GetCurrentHelmSignedRotation();
		return oldTurnForce;
	}

	public Vector3 GetHelmWaterFrictionForce(Vector3 shipVelocity) {
		Vector3 resultForce = PhysicsHelper.CalculateObjectFrictionForce(shipVelocity, GetHelmNormal(), GetHelmUnderWaterSquare(), _helmFrictionFactor);
		return resultForce;
	}

	public Vector3 GetHelmTurnApplyForcePosition() {
		return transform.position;
	}

	Vector3 GetHelmNormal() {
		return GetCurrentHelmSignedRotation() > 0 ? transform.forward 
			: -transform.forward;
	}

	float GetHelmUnderWaterSquare() {
		return transform.lossyScale.x * transform.lossyScale.y;
	}

	void RotateHelmOnInput(float horizontalInput) {
		float angleToRotateInDegrees = -horizontalInput * _helmRotateSpeedDegrees;
		angleToRotateInDegrees = LimitHelmOffset(angleToRotateInDegrees);
		transform.Rotate(Vector3.up, angleToRotateInDegrees);
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
		transform.Rotate(Vector3.up, alignHelmRotation);
	}

	float GetCurrentHelmSignedRotation() {
		float currentHelmRotation = transform.localEulerAngles.y;
		currentHelmRotation = currentHelmRotation > 180.0f ? currentHelmRotation - 360.0f : currentHelmRotation;
		return currentHelmRotation;
	}
}
