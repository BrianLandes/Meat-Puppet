
using UnityEngine;

namespace PBG.MeatPuppet {
	[RequireComponent(typeof(Collider))]
	public class ColliderVelocity : MonoBehaviour {

		public float minDeltaTime = 0.1f;

		float lastTime;
		private Vector3 lastPosition;

		private Vector3 lastVelocity;

		private Quaternion lastRotation;
		private Vector3 lastAngularSpeed;

		//private Vector3 lastForward;
		//private float lastTurnSpeed;

		private void Start() {
			lastTime = Time.time;
			lastPosition = transform.position;
			lastRotation = transform.rotation;
			//lastForward = transform.forward;
		}

		public Vector3 GetVelocity() {
			return lastVelocity;
		}

		public Vector3 GetAngularVelocity() {
			return lastAngularSpeed * Time.deltaTime;
		}

		//public float GetTurnSpeed() {
		//	return lastTurnSpeed;
		//}

		private void LateUpdate() {
			// Use the difference in positions to determine the velocity
			var timeDelta = Time.time - lastTime;
			if (timeDelta < minDeltaTime) {
				return;
			}

			lastVelocity = (transform.position - lastPosition) / timeDelta;

			lastPosition = transform.position;
			lastTime = Time.time;

			var rotationDifference = transform.rotation * Quaternion.Inverse(lastRotation);
			float angleInDegrees;
			Vector3 rotationAxis;
			rotationDifference.ToAngleAxis(out angleInDegrees, out rotationAxis);

			Vector3 angularDisplacement = rotationAxis * angleInDegrees;
			lastAngularSpeed = angularDisplacement / timeDelta;
			lastRotation = transform.rotation;

			//var currentTheta = Mathf.Atan2(transform.forward.z, transform.forward.x);
			//var lastTheta = Mathf.Atan2(lastForward.z, lastForward.x);
			//lastTurnSpeed = MeatPuppetToolKit.AngleBetweenThetas(currentTheta, lastTheta) / timeDelta;

			//lastForward = transform.forward;
		}
	}

}
