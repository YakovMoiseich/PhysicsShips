using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

	public Transform deckFrictionForceArrow;
	public Transform helmFrictionForceArrow;
	public Transform windAcceleratingForceArrow;

	private HelmController _helmController;
	private MastController _mastController;
	private DeckController _deckController;
	private Rigidbody _shipBody;

	private float _shipMass;

	private Vector3 _incomingWind = Vector3.zero;

	void Awake() {
		_shipBody = gameObject.GetComponent<Rigidbody>();
		_helmController = transform.Find("Helm").gameObject.GetComponent<HelmController>();
		_mastController = transform.Find("Mast").gameObject.GetComponent<MastController>();
		_deckController = transform.Find("Deck").gameObject.GetComponent<DeckController>();

//		Instantiate(deckFrictionForceArrow, false);
//		Instantiate(helmFrictionForceArrow, false);
//		Instantiate(windAcceleratingForceArrow, false);
	}

	void Start() {
		
	}

	void Update() {
		WindAcceleratingForce(_incomingWind);
		UpdateShipDrawdown();
		WaterFrictionForce();
	}

	void WaterFrictionForce() {
		Vector3 deckWaterFrictionForce = _deckController.GetDeckWaterFrictionForce(_shipBody.velocity);
		PhysicsHelper.ShowForce(deckFrictionForceArrow, transform.position, deckWaterFrictionForce);
		_shipBody.AddForce(deckWaterFrictionForce);
		Vector3 helmWaterFrictionForce = _helmController.GetHelmWaterFrictionForce(_shipBody.velocity);
		PhysicsHelper.ShowForce(helmFrictionForceArrow, _helmController.GetHelmTurnApplyForcePosition(), helmWaterFrictionForce);
		_shipBody.AddForceAtPosition(helmWaterFrictionForce, _helmController.GetHelmTurnApplyForcePosition());
	}

	void WindAcceleratingForce(Vector3 windVector) {
		Vector3 forceFromSail = _mastController.GetWindAccelerationForce(windVector);
		Vector3 forceFromDeck = _deckController.GetDeckWindForce(windVector);
		Vector3 resultWindForce = forceFromSail + forceFromDeck;
		PhysicsHelper.ShowForce(windAcceleratingForceArrow, transform.position, resultWindForce);
		_shipBody.AddForce(resultWindForce);
	}

	void UpdateShipDrawdown() {
		float additionalMass = _mastController.GetMass();
		_deckController.SetDrawdownSize(additionalMass);
		Vector3 drawdownSize = _deckController.GetDrawdownSize();
		Vector3 normalizedYPosition = transform.position;
		normalizedYPosition.y = -drawdownSize.y;
		transform.position = normalizedYPosition;
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

		_incomingWind = windZone.gameObject.GetComponent<WindController>().GetWindVector();
	}
}
