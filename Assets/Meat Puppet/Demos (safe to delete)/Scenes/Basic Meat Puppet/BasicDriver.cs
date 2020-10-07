
using UnityEngine;
namespace PBG.MeatPuppet {
	public class BasicDriver : MonoBehaviour {
		public MeatPuppet puppet;

		public Transform moveTarget;
		public Transform facingTarget;

		public void Start() {
			AssignTargets();
		}

		public void AssignTargets() {
			puppet.Movement.SetMoveTarget(moveTarget);
			puppet.Movement.SetFacingTarget(facingTarget);
			//puppet.Movement.MoveTargetTransform = moveTarget;
			//puppet.Movement.FacingTargetTransform = facingTarget;
		}

#if UNITY_EDITOR

		private void OnValidate() {
			if (puppet.Initialized) {
				AssignTargets();
			}
			
		}
#endif
	}

}
