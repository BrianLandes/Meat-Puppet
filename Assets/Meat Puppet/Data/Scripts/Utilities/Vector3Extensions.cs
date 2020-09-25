
using UnityEngine;
namespace PBG.MeatPuppet {
	public static class Vector3Extensions {

		public static bool ApproxEquals(this Vector3 self, Vector3 other, float threshold) {
			float diff = (self - other).sqrMagnitude;
			return diff < threshold * threshold;
		}


		public static Vector2 JustXZ(this Vector3 point) {
			return new Vector2(point.x, point.z);
		}

		public static Vector3 JustY(this Vector3 point) {
			return new Vector3(0, point.y, 0);
		}

		public static Vector3 DropY(this Vector3 point) {
			return new Vector3(point.x, 0, point.z);
		}

		public static Vector3 FromXZ(this Vector2 point) {
			return new Vector3(point.x, 0, point.y);
		}

		public static Vector3 ElementMultipy(this Vector3 self, Vector3 other) {
			return new Vector3(self.x * other.x, self.y * other.y, self.z * other.z);
		}

		public static Vector3 DivideBy(this Vector3 self, Vector3 other) {
			return new Vector3(self.x / other.x, self.y / other.y, self.z / other.z);
		}
	}

}
