using UnityEngine;

public class PhysicsHelper {
	
	public const float WATER_DENSITY = 1000.0f;
	public const float GRAVITY_CONSTANT = 9.8f;

	public static Vector3 CalculateObjectDrawdown(float objectMass, Vector3 objectSize) {
		float drawdownVolume = objectMass / WATER_DENSITY;
		float objectVolume = objectSize.x * objectSize.y * objectSize.z;
		Vector3 objectDrawdownVolume = objectSize * drawdownVolume / objectVolume;
		return objectDrawdownVolume;
	}

	public static float CalcuateOverallExtrudingYForce(float objectUnderwaterVolume) {
		return WATER_DENSITY * GRAVITY_CONSTANT * objectUnderwaterVolume;
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
}
