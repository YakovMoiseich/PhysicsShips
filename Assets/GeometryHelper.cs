using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryHelper {

	public static float CalculateTriangleVolume(Vector3 triangleVertex1, Vector3 triangleVertex2, Vector3 triangleVertex3) {
		float v321 = triangleVertex3.x * triangleVertex2.y * triangleVertex1.z;
		float v231 = triangleVertex2.x * triangleVertex3.y * triangleVertex1.z;
		float v312 = triangleVertex3.x * triangleVertex1.y * triangleVertex2.z;
		float v132 = triangleVertex1.x * triangleVertex3.y * triangleVertex2.z;
		float v213 = triangleVertex2.x * triangleVertex1.y * triangleVertex3.z;
		float v123 = triangleVertex1.x * triangleVertex2.y * triangleVertex3.z;
		return (1.0f / 6.0f) * Mathf.Abs(-v321 + v231 + v312 - v132 - v213 + v123);
	}

}
