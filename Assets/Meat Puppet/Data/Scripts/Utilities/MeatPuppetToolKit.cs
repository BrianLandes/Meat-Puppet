﻿
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

			float distanceSqrd = distance * distance;
			return distanceSqrd > x * x + y * y;
		}

		public static bool PointAndPointWithinDistanceOfEachOther(Vector3 A, Vector3 B, float distance, float yAxisTolerance = 3) {
			bool xzPlaneResult = PointAndPointWithinDistanceOfEachOther(A.JustXZ(), B.JustXZ(), distance);

			if (!xzPlaneResult) {
				return false;
			}

			// For our purposes, we care mostly about the distance across the xz plane, and usually ignore
			// the y-axis
			// Here in the algorithm, the two points are within the distance we want on the xz plane
			// Let's check the y axis, as well, but allow a lot of leeway, or 'tolerance'

			float y = A.y - B.y;
			if (Mathf.Abs(y) > distance + yAxisTolerance) {
				return false;
			}
			// this causes this distance check to result in a cylinder shape, instead of a sphere

			return true;
		}
	}
}
