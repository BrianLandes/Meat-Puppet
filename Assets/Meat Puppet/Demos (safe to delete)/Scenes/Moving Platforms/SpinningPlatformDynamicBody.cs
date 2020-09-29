
using UnityEngine;
namespace PBG.MeatPuppet {
	public class SpinningPlatformDynamicBody : MonoBehaviour {
		
		public float speed = 2f;
		public float force = 4f;

		public Vector3 direction = Vector3.right;

		public void Update() {

			

			direction.Normalize();

			var idealVelocity = direction * speed;
			var myBody = GetComponent<Rigidbody>();
			var currentVelocity = myBody.angularVelocity;
			myBody.AddTorque((idealVelocity - currentVelocity) * force, ForceMode.Force);
			
		}
	}

}
