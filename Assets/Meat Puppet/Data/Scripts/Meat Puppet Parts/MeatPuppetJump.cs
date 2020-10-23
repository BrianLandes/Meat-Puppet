
using System;
using UnityEngine;
namespace PBG.MeatPuppet {


	[Serializable]
	public class JumpSettings {
		public bool enableJump = true;
		[NonSerialized] public float initialJumpForce = 6.0f;
		[NonSerialized] public float jumpDelay = 0.38f;
		public bool useJumpBoost = true;
		[NonSerialized] public float jumpBoostTime = 0.6f;
		[NonSerialized] public float boostForce = 15f;
	}

	public class MeatPuppetJump {

		private MeatPuppet parentPuppet;

		private float boostTimer = 0f;

		private bool invokingForce = false;

		// private bool waitingForReturnToLocomotion = false;
		
		public MeatPuppetJump(MeatPuppet parent) {
			parentPuppet = parent;
			
			// parentPuppet.AnimatorHook.onStateChange += 
		}

		public void Launch() {
			// require that the puppet is grounded in order to jump
			if (!parentPuppet.Legs.IsGrounded) {
				return;
			}

			// prevent Launch from trigger multiple jumps at once
			if (invokingForce) {
				return;
			}

			// // prevent the puppet from jumping until we've fully returned to the locomation state
			// if (waitingForReturnToLocomotion) {
			// 	return;
			// }
			
			// trigger the animation
			parentPuppet.AnimatorHook.Animator.SetTrigger("Jump");

			// allow the animation to play for a short time before applying force
			parentPuppet.Invoke("ApplyJumpForce", parentPuppet.jumpSettings.jumpDelay);
			invokingForce = true;

			if (parentPuppet.jumpSettings.useJumpBoost) {
				// allow the jump to be boosted for a short amount of time
				boostTimer = parentPuppet.jumpSettings.jumpBoostTime;
			}
			
			// parentPuppet.Legs.StartNormalUngroundedMode();

			// waitingForReturnToLocomotion = true;
		}

		public void ApplyJumpForce() {
			invokingForce = false;

			// apply an immediate velocity to the body
			var velocity = parentPuppet.Rigidbody.velocity;
			velocity.y = parentPuppet.jumpSettings.initialJumpForce;
			parentPuppet.Rigidbody.velocity = velocity;

			parentPuppet.Legs.StartJumpMode();
		}

		public void EndBoost() {
			boostTimer = 0f;
		}
		
		/// <summary>
		/// Called by MeatPuppet in FixedUpdate()
		/// </summary>
		public void Update() {
			if ( boostTimer > 0 && !invokingForce) {
				boostTimer -= Time.fixedDeltaTime;
				parentPuppet.Rigidbody.AddForce(Vector3.up * parentPuppet.jumpSettings.boostForce, ForceMode.Force);
			}
		}

		// private void OnStateChange(int stateHash) {
		// 	
		// }
	}

}
