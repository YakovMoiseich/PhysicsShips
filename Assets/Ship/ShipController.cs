using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private HelmController _helmController;
	private MastController _mastController;

	void Awake() {
		_helmController = transform.Find("Helm").gameObject.GetComponent<HelmController>();
		_mastController = transform.Find("Mast").gameObject.GetComponent<MastController>();
	}

	void Start() {
		
	}

	void Update() {

	}

	void OnTriggerStay(Collider windZone) {
		Vector3 windVector = windZone.gameObject.GetComponent<WindController>().GetWindVector();
		Rigidbody shipBody = gameObject.GetComponent<Rigidbody>();
		float forceFactor = 1.0f;
		shipBody.AddForce(windVector * forceFactor);
	}
}
