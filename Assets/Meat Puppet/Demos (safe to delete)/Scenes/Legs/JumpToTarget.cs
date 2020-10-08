
using System.Collections;
using UnityEngine;

namespace PBG.MeatPuppet {

	public class JumpToTarget : MonoBehaviour {
		public Transform startPosition;
		public Transform target;

		public MeatPuppet puppet;

		private float resetDelay = 1.6f;

		public float jumpWhenWithinDistance = 6f;

		private bool resetting = false;
		private bool jumped = false;

		private void Start() {
			puppet.Movement.SetMoveTarget(target);
		}

		private void Update() {
			if ( puppet.Movement.ReachedTarget() && !resetting) {
				resetting = true;
				StartCoroutine(ResetTest());
				
			}
			else if (!jumped && puppet.Movement.IsDistanceToTargetLessThan(jumpWhenWithinDistance)) {
				puppet.Jump.Launch();
				jumped = true;
			}
		}

		IEnumerator ResetTest() {
			yield return new WaitForSeconds(resetDelay);
			puppet.Movement.RemoveMoveTarget();
			puppet.Movement.Teleport(startPosition.position);
			yield return new WaitForSeconds(resetDelay);
			puppet.Movement.SetMoveTarget(target);
			resetting = false;
			jumped = false;
		}
	}

}
