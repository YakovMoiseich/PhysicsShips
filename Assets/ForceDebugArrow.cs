using UnityEngine;

public class ForceDebugArrow : MonoBehaviour {

	private Transform arrowPointer;
	private Transform arrowStart;
	private Transform forceValue;

	void Awake() {
		arrowPointer = transform.Find("ArrowPointer");
		arrowStart = transform.Find("ForceApplyPoint");
		forceValue = transform.Find("ForceValue");
	}

	public void UpdateWithForce(Vector3 pointToApply, Vector3 forceVector, Color arrowColor) {
		UpdateForceValue(forceVector.magnitude);
		UpdateRotation(forceVector);
		UpdatePosition(pointToApply);
//		UpdateColor(arrowColor);
	}

	void UpdatePosition(Vector3 position) {
		transform.position = new Vector3(position.x, position.y, position.z + forceValue.localScale.y);
	}

	void UpdateRotation(Vector3 forceDirection) {
		transform.eulerAngles = forceDirection;
	}

	void UpdateColor(Color arrowColor) {
		
	}

	void UpdateForceValue(float forceVectorMagnitude) {
		Vector3 updatedForceValueScale = forceValue.localScale;
		Vector3 updatedForceValuePosition = forceValue.localPosition;
		Vector3 updatedArrowPoiterPosition = arrowPointer.localPosition;
		updatedForceValueScale.y = 1.0f + forceVectorMagnitude;
		updatedForceValuePosition.z = updatedArrowPoiterPosition.z = updatedForceValueScale.y;
		forceValue.localScale = updatedForceValueScale;
		forceValue.localPosition = updatedForceValuePosition;
		arrowPointer.localPosition = updatedArrowPoiterPosition;
	}

	void Start () {
		
	}
	void Update () {
		
	}
}
