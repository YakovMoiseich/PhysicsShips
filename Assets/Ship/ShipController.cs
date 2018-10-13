using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private HelmController _helmController;
	private MastController _mastController;
	private DeckController _deckController;
	private Rigidbody _shipBody;

	private float _shipMass;

	private Dictionary<Collider, Vector3> _windZones;

	private Vector3 _deckFrictionForce;
	private Vector3 _helmFrictionForce;
	private Vector3 _windAcceleratingForce;

	void Awake() {
		_windZones = new Dictionary<Collider, Vector3>();
		_shipBody = gameObject.GetComponent<Rigidbody>();
		_helmController = transform.Find("Helm").gameObject.GetComponent<HelmController>();
		_mastController = transform.Find("Mast").gameObject.GetComponent<MastController>();
		_deckController = transform.Find("Deck").gameObject.GetComponent<DeckController>();
	}

	void Start() {
		
	}

	void Update() {
		WindAcceleratingForce(GetWindZonesResultVector());
		UpdateShipDrawdown();
		WaterFrictionForce();
	}

	void WaterFrictionForce() {
		Vector3 deckWaterFrictionForce = _deckController.GetDeckWaterFrictionForce(_shipBody.velocity);
		Vector3 helmWaterFrictionForce = _helmController.GetHelmWaterFrictionForce(_shipBody.velocity);
		_deckFrictionForce = deckWaterFrictionForce;
		_helmFrictionForce = helmWaterFrictionForce;
		_shipBody.AddForce(deckWaterFrictionForce);
		_shipBody.AddForceAtPosition(helmWaterFrictionForce, _helmController.GetHelmTurnApplyForcePosition());
	}

	void WindAcceleratingForce(Vector3 windVector) {
		Vector3 forceFromSail = _mastController.GetWindAccelerationForce(windVector);
		Vector3 forceFromDeck = _deckController.GetDeckWindForce(windVector);
		Vector3 resultWindForce = forceFromSail + forceFromDeck;
		_windAcceleratingForce = resultWindForce;
		_shipBody.AddForce(resultWindForce);
	}

	void UpdateShipDrawdown() {
		float additionalMass = _mastController.GetMass();
		_deckController.SetDrawdownSize(additionalMass);
		Vector3 drawdownSize = _deckController.GetDrawdownSize();
		Vector3 normalizedYPosition = transform.position;
//		normalizedYPosition.y = -drawdownSize.y;
		transform.position = normalizedYPosition;
	}

	Vector3 GetWindZonesResultVector() {
		Vector3 resultWind = Vector3.zero;
		foreach (KeyValuePair<Collider, Vector3> windZone in _windZones) {
			resultWind += windZone.Value;
		}

		return resultWind;
	}

	void OnTriggerStay(Collider windZone) {
		if (windZone.tag != "WindZones") {
			return;
		}

		_windZones[windZone] = windZone.gameObject.GetComponent<WindController>().GetWindVector();
	}

	void OnTriggerExit(Collider windZone) {
		if (windZone.tag != "WindZones") {
			return;
		}

		_windZones[windZone] = Vector3.zero;
	}
}
