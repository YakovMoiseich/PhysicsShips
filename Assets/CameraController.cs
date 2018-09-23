using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;

	private Vector3 constDistanceFromTarget;

	void Awake() {
		constDistanceFromTarget = transform.position - target.transform.position;
	}

	void Start () {
		
	}
	
	void Update () {
		transform.position = target.transform.position + constDistanceFromTarget;
	}
}
