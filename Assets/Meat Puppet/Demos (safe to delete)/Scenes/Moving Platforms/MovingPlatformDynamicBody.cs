
using UnityEngine;
namespace PBG.MeatPuppet {
	public class MovingPlatformDynamicBody : MonoBehaviour {

		public Transform positionA;
		public Transform positionB;

		public float speed = 2f;
		public float force = 4f;

		public float pauseTime = 2f;

		private bool forward = true;

		private float pauseDelay = 0f;

		private void Start() {
			//pauseDelay = pauseTime;
		}

		public void Update() {

			if (pauseDelay > 0) {
				pauseDelay -= Time.deltaTime;
			}
			else {
				var direction = positionB.position - transform.position;

				if (!forward) {
					direction = positionA.position - transform.position;
				}

				direction.Normalize();

				var myBody = GetComponent<Rigidbody>();
				var idealVelocity = direction * speed;
				var currentVelocity = myBody.velocity;
				myBody.AddForce((idealVelocity - currentVelocity) * force, ForceMode.Force);

				var target = positionA.position;

				if (forward) {
					target = positionB.position;
				}

				if (target.ApproxEquals(transform.position, 0.05f)) {
					transform.position = target;
					pauseDelay = pauseTime;
					forward = !forward;
				}
			}


		}
	}

}
