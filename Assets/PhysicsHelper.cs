using UnityEngine;

public class PhysicsHelper {
	
	public const float WATER_DENSITY = 10.0f;

	public static Vector3 CalculateObjectDrawdown(float objectMass, Vector3 objectSize) {
		float drowdawnVolume = objectMass / WATER_DENSITY;
		float objectVolume = objectSize.x * objectSize.y * objectSize.z;
		Vector3 objectDrawdownVolume = objectSize * drowdawnVolume / objectVolume;
		return objectDrawdownVolume;
	}

	public static Vector3 CalculateObjectFrictionForce(Vector3 objectVelocity, Vector3 objectNormal, float frictionSquare, float frictionFactor) {
		float dotProduct = Vector3.Dot(-objectVelocity, objectNormal);
		if (dotProduct >= 0) {
			return Vector3.zero;
		}

		return objectVelocity * dotProduct * frictionSquare * frictionFactor;
	}

	public static Vector3 CalculateObjectAcceleratingForce(Vector3 accelerationVelocity, Vector3 objectNormal, float frictionSquare, float frictionFactor) {
		float dotProduct = Vector3.Dot(accelerationVelocity, objectNormal);
		if (dotProduct <= 0) {
			return Vector3.zero;
		}

		return accelerationVelocity * dotProduct * frictionSquare * frictionFactor;
	}

	public static void ShowForce(Transform objectTransform, Vector3 pointToApply, Vector3 forceDirection) {
		Vector3 newObjectScale = objectTransform.localScale;
		newObjectScale.z = 1.0f + forceDirection.magnitude;
		objectTransform.localScale = newObjectScale;
		objectTransform.position = pointToApply;
		objectTransform.eulerAngles = forceDirection;
	}

}
