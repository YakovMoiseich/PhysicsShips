using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private HelmController _helmController;
	private MastController _mastController;
	private Rigidbody _shipBody;

	void Awake() {
		_shipBody = gameObject.GetComponent<Rigidbody>();
		_helmController = transform.Find("Helm").gameObject.GetComponent<HelmController>();
		_mastController = transform.Find("Mast").gameObject.GetComponent<MastController>();
	}

	void Start() {
		
	}

	void Update() {
		Vector3 normalizedYPosition = transform.position;
		normalizedYPosition.y = 0.0f;
		transform.position = normalizedYPosition;
		RotateShipWithHelm();
	}

	void RotateShipWithHelm() {
		Vector3 shipVelocity = _shipBody.velocity;
		float currentShipForwardSpeed = transform.InverseTransformDirection(shipVelocity).x;
		float forceToRotateFromHelm = _helmController.GetHelmTurnForce(currentShipForwardSpeed);
		_shipBody.AddTorque(0.0f, forceToRotateFromHelm, 0.0f, ForceMode.Force);
	}

	void OnTriggerStay(Collider windZone) {
		if (windZone.tag != "WindZones") {
			return;
		}

		Vector3 windVector = windZone.gameObject.GetComponent<WindController>().GetWindVector();
		Vector3 resultForce = Vector3.zero;
		Vector3 forceFromSail = _mastController.ApplyWind(windVector);

		resultForce += forceFromSail;

		Rigidbody shipBody = gameObject.GetComponent<Rigidbody>();
		float forceFactor = 0.5f;
		_shipBody.AddForce(resultForce * forceFactor);
	}
}
