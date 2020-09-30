

using UnityEngine;

namespace PBG.MeatPuppet {
	public class ReturnToEmptyStateMachineBehavior : StateMachineBehaviour {

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			var animatorHook = animator.GetComponent<MeatPuppetAnimatorHook>();
			animatorHook?.InformOfEmptyState(stateInfo, layerIndex);
		}
		
	}
}
