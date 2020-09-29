
using UnityEngine;
namespace PBG.MeatPuppet {

	[RequireComponent(typeof(MeatPuppet))]
	public class MeatPuppetPlayerController : MonoBehaviour {

		public bool faceForward = true;

		public string horizontalAxisName = "Horizontal";
		public string verticalAxisName = "Vertical";
		public string sprintInputName = "Fire3";

		private MeatPuppet puppet;

		private void Start() {
			puppet = GetComponent<MeatPuppet>();

			puppet.movementSettings.avoidStaticObstacles = false;
		}

		private void Update() {
			
			puppet.Movement.TargetDirection = GetPlayerInput();

			if (faceForward) {
				Transform cameraTransform = Camera.main.transform;
				puppet.Movement.LookAtDirection = cameraTransform.forward;
			}
			else {
				puppet.Movement.StopLookAt();
			}
			
			if (puppet.jumpSettings.enableJump) {
				if (Input.GetButtonDown("Jump")) {
					puppet.Jump.Launch();
				}
				else if (Input.GetButtonUp("Jump")) {
					puppet.Jump.EndBoost();
				}
			}

			puppet.running = Input.GetButton(sprintInputName);
		}

		private Vector3 GetPlayerInput() {
			float horizontal = Input.GetAxisRaw(horizontalAxisName);
			float vertical = Input.GetAxisRaw(verticalAxisName);

			if (horizontal == 0 && vertical == 0) {
				return Vector3.zero;
			}

			// Use the camera's position to determine our 'moveDirection'
			Transform cameraTransform = Camera.main.transform;
			var forwardVector = cameraTransform.forward.DropY().normalized * vertical;
			var sidewaysVector = cameraTransform.right.DropY().normalized * horizontal;
			var result = forwardVector + sidewaysVector;

			// Manipulate the moveDirection so it only goes along the XZ-plane
			result = result.DropY();

			// Normalize maybe
			if (result.sqrMagnitude > 1f) {
				result.Normalize();
			}

			return result;
		}
	}

}
