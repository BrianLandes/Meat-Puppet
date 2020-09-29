
using UnityEngine;

namespace PBG.MeatPuppet {
	public class MeatPuppetAnimation {
		
		public bool PlayingAnimation { get; private set; }
		
		#region Private Members
		
		private readonly MeatPuppet parentPuppet;

		private bool waitingForReturnToLocomotion = false;

		private MeatPuppetLoopingAnimationConfiguration currentLoopingAnimation;

		private MeatPuppetOneShotAnimationConfiguration currentOneShotAnimation;

		private bool playingOneShotAnimation = false;
		private bool usingOneShotAlternative = false;
		
		#endregion
		
		#region Private String Constants for Animation Controller
		
		private const string LoopAnimationKey = "Looping Animation";
		private const string LoopEnterAnimationKey = "Looping Enter";
		private const string LoopExitAnimationKey = "Looping Exit";
		
		private const string LoopStartTrigger = "Start Loop";
		private const string LoopStopTrigger = "Stop Loop";
		private const string LoopEnterBool = "Loop Enter";
		private const string LoopExitBool = "Loop Exit";

		private const int loopLayerIndex = 2;
		private const int loopMaskedLayerIndex = 3;
		private const int oneShotLayerIndex = 4;
		private const int oneShotMaskedLayerIndex = 5;

		//#region Animation Controller State Values

		//private const string LocomotionStateName = "Locomotion Blend Tree";
		//private readonly int locomotionStateHash;

		//private readonly int loopingAnimationStateHash;

		//#endregion
		#endregion


		public MeatPuppetAnimation(MeatPuppet parent) {
			parentPuppet = parent;
			
			parentPuppet.AnimatorHook.onStateChange += OnStateChange;
			parentPuppet.AnimatorHook.onAnimatorMove += OnAnimatorMove;

			//locomotionStateHash = Animator.StringToHash(LocomotionStateName);
			//loopingAnimationStateHash = Animator.StringToHash(LoopAnimationKey);
		}

		#region Public Methods
		
		#region Looping Animation
		
		public void StartLoopingAnimation(AnimationClip animation, AnimationClip enterAnimation = null, AnimationClip exitAnimation = null,
				bool applyRootMotion = false,
				bool makeIntangible = false,
				bool makeKinematic = false,
				bool overrideLegs = true
			) {

			currentLoopingAnimation = new MeatPuppetLoopingAnimationConfiguration() {
				loopingClip = animation,
				entryClip = enterAnimation,
				exitClip =  exitAnimation,
				applyRootMotion = applyRootMotion,
				makeIntangible = makeIntangible,
				makeKinematic = makeKinematic,
				overrideLegs = overrideLegs
			};
			StartLoopingAnimation(currentLoopingAnimation);
		}
		
		public void StartLoopingAnimation(MeatPuppetLoopingAnimationConfiguration animation) {
			// TODO: add callback for when entry animation ends and looping animation starts
			// TODO: wait for the last looping or one-shot animation to finish
			currentLoopingAnimation = animation;
			
			parentPuppet.AnimatorHook.AnimatorController[LoopAnimationKey] = animation.loopingClip;
			parentPuppet.AnimatorHook.AnimatorController[LoopEnterAnimationKey] = animation.entryClip;
			parentPuppet.AnimatorHook.AnimatorController[LoopExitAnimationKey] = animation.exitClip;

			parentPuppet.AnimatorHook.Animator.SetBool("Loop Masked", !animation.overrideLegs);
			parentPuppet.AnimatorHook.Animator.SetTrigger(LoopStartTrigger);
			parentPuppet.AnimatorHook.Animator.ResetTrigger(LoopStopTrigger);
			parentPuppet.AnimatorHook.Animator.SetBool(LoopEnterBool, animation.entryClip!=null);
			parentPuppet.AnimatorHook.Animator.SetBool(LoopExitBool, animation.exitClip!=null);

			PlayingAnimation = true;
			waitingForReturnToLocomotion = true;

			ApplyAnimationConfiguration(animation);
		}

		public void StopLoopingAnimation() {
			// TODO: add callback for when animation finally ends
			parentPuppet.AnimatorHook.Animator.SetTrigger(LoopStopTrigger);
			parentPuppet.AnimatorHook.Animator.ResetTrigger(LoopStartTrigger);
		
			// looping = false;
		}
		
		#endregion // End of Looping Animation
		
		#region One-Shot Animation

		public void PlayOneShotAnimation(AnimationClip animation, MeatPuppetOneShotAnimationConfiguration configuration = null) {
			// TODO: add callback for when animation finally ends
			// TODO: promote strings to constants
			if (!playingOneShotAnimation || usingOneShotAlternative) {
				parentPuppet.AnimatorHook.AnimatorController["One Shot Animation"] = animation;
				usingOneShotAlternative = false;
			}
			else {
				parentPuppet.AnimatorHook.AnimatorController["One Shot Animation Alternative"] = animation;
				usingOneShotAlternative = true;
			}
			parentPuppet.AnimatorHook.Animator.SetBool("One Shot Masked", configuration!=null && !configuration.overrideLegs);
			parentPuppet.AnimatorHook.Animator.SetTrigger("Start One Shot");
			
			playingOneShotAnimation = true;

			currentOneShotAnimation = configuration;
			if (configuration != null) {
				ApplyAnimationConfiguration(configuration);

			}

			PlayingAnimation = true;
		}

		#endregion // End of One-Shot Animation

		#endregion // End of Public Methods

		#region Private Methods

		private void OnStateChange(AnimatorStateInfo stateInfo, int layerIndex) {
			//if ( locomotionStateHash == stateNameHash) {
			if (layerIndex == loopLayerIndex || layerIndex == loopMaskedLayerIndex) { 
				// animator is ENTERING empty on looping layer

				if (waitingForReturnToLocomotion && !playingOneShotAnimation) {
					// typical case: looping animation has ended and we should end/finalize it

					// edge case: a looping animation is about to start and a one-shot animation has ended

					FinalizeLoopEnd();
				}
				
			}
			else
			if (layerIndex == oneShotLayerIndex || layerIndex == oneShotMaskedLayerIndex) {
				// animator is ENTERING empty on one shot layer
				
				if (playingOneShotAnimation) {
					FinalizeOneShotEnd();
				}
			}
			//}
			//else
			//if (loopingAnimationStateHash == stateNameHash) {
			//	// animator is ENTERING the looping animation state
			//
			//	if (PlayingAnimation) {
			//		// InformOfLoopingStart();
			//	}
			//}
		}

		private void OnAnimatorMove() {
			if (ShouldApplyRootMotion()) {
				var myBody = parentPuppet.Rigidbody;
				myBody.MovePosition(myBody.position + parentPuppet.AnimatorHook.Animator.deltaPosition);
				myBody.MoveRotation(myBody.rotation * parentPuppet.AnimatorHook.Animator.deltaRotation);
			}
		
		}

		private bool ShouldApplyRootMotion() {
			if (!PlayingAnimation) {
				return false;
			}

			if (currentLoopingAnimation != null && currentLoopingAnimation.applyRootMotion) {
				return true;
			}
			
			if (currentOneShotAnimation != null && currentOneShotAnimation.applyRootMotion) {
				return true;
			}

			return false;
		}
		
		private void FinalizeLoopEnd() {
			waitingForReturnToLocomotion = false;
			PlayingAnimation = false;
			
			UnapplyAnimationConfiguration(currentLoopingAnimation);

			currentLoopingAnimation = null;
		}

		private void FinalizeOneShotEnd() {

			if (currentOneShotAnimation != null) {
				UnapplyAnimationConfiguration(currentOneShotAnimation);
			}

			currentOneShotAnimation = null;
			
			playingOneShotAnimation = false;
			usingOneShotAlternative = false;

			PlayingAnimation = false;
		}

		private void ApplyAnimationConfiguration(MeatPuppetOneShotAnimationConfiguration configuration) {
			if (configuration.makeIntangible) {
				parentPuppet.Tangibility = Tangibility.Intangible;
			}

			if (configuration.makeKinematic) {
				parentPuppet.MotionType = PhysicalMotionType.Kinematic;
			}
		}

		private void UnapplyAnimationConfiguration(MeatPuppetOneShotAnimationConfiguration configuration) {
			if (configuration.makeIntangible) {
				parentPuppet.Tangibility = Tangibility.Corporeal;
			}

			if (configuration.makeKinematic) {
				parentPuppet.MotionType = PhysicalMotionType.Dynamic;
			}
		}
		
		#endregion
	}

}
