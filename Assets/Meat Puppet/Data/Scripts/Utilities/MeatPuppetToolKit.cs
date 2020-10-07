
using UnityEngine;
namespace PBG.MeatPuppet {
	public static class MeatPuppetToolKit {
		public static float AngleBetweenThetas(float thetaA, float thetaB) {

			float dif = thetaB - thetaA;

			// we have to consider that if one vector is in Q2 and the other is in Q3 -> wrap around
			if (thetaB > thetaA) {
				// if B is greater than A then moving CCW will never cross the -PI/PI boundary
				if (dif < Mathf.PI) {
					return dif;
				}
				// but moving CW will always cross the -PI/PI boundary
				return dif - Mathf.PI * 2f;

			}
			else {
				// if A is greater than B then moving CW will never cross the -PI/PI boundary
				if (Mathf.Abs(dif) < Mathf.PI) {
					return dif;
				}
				// but moving CCW will always cross the -PI/PI boundary
				return Mathf.PI * 2f + dif;
			}
		}

		public static bool IsOnLayer(GameObject gameObject, LayerMask layer) {
			return layer == (layer | (1 << gameObject.layer));
		}

		public static bool PointAndPointWithinDistanceOfEachOther(Vector2 A, Vector2 B, float distance) {
			float x = A.x - B.x;
			if (Mathf.Abs(x) > distance) {
				return false;
			}
			float y = A.y - B.y;
			if (Mathf.Abs(y) > distance) {
				return false;
			}

			// one 'cheat' we can do is check for zeroes
			if (distance>0 && x==0 && y == 0) {
				return true;
			}

			float distanceSqrd = distance * distance;
			return distanceSqrd > x * x + y * y;
		}

		public static bool PointAndPointWithinDistanceOfEachOther(Vector3 A, Vector3 B, float distance ) {
			float x = A.x - B.x;
			if (Mathf.Abs(x) > distance) {
				return false;
			}
			float y = A.y - B.y;
			if (Mathf.Abs(y) > distance) {
				return false;
			}
			float z = A.z - B.z;
			if (Mathf.Abs(z) > distance) {
				return false;
			}

			// one 'cheat' we can do is check for zeroes
			if (distance > 0 && x == 0 && y == 0 && z == 0) {
				// who knows how much this hurts or helps?
				return true;
			}

			float distanceSqrd = distance * distance;
			return distanceSqrd > x * x + y * y + z * z;
		}

		public static float VolumeOfCapsule(float sideLength, float radius) {
			return Mathf.PI * radius * radius * ( (4f/3f)* radius + sideLength );
		}
	}
}

