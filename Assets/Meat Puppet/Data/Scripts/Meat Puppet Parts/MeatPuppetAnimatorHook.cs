
using System;
using UnityEngine;

namespace PBG.MeatPuppet {

	[AddComponentMenu("")]
	[RequireComponent(typeof(Animator))]
	public class MeatPuppetAnimatorHook : MonoBehaviour {

		/// <summary>
		/// Events to call through from OnAnimatorMove().
		/// </summary>
		public Action onAnimatorMove;

		/// <summary>
		/// Events to fire if the animator's state changes.
		/// The int passed through is the new state's hash.
		/// </summary>
		public Action<AnimatorStateInfo, int> onStateChange;

		//private int lastState;

		private Animator animator;

		private bool initializedAnimator = false;

		private AnimatorOverrideController animatorOverrideController;

		private void Start() {
			InitializeAnimatorMaybe();
		}

		public bool HasAnimator {
			get {
				InitializeAnimatorMaybe();
				return animator != null;
			}
		}

		public Animator Animator {
			get {
				InitializeAnimatorMaybe();
				return animator;
			}
		}

		public bool HasAnimatorController {
			get {
				InitializeAnimatorMaybe();
				return animatorOverrideController != null;
			}
		}

		public AnimatorOverrideController AnimatorController {
			get {
				InitializeAnimatorMaybe();
				return animatorOverrideController;
			}
		}

		//private void Update() {

			//if (!HasAnimator) {
			//	return;
			//}

			//int currentState = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

			//if (currentState != lastState) {
			//	onStateChange?.Invoke(currentState);
			//	lastState = currentState;
			//}

		//}

		/// <summary>
		/// Called by Unity.Animator through message.
		/// </summary>
		void OnAnimatorMove() {
			onAnimatorMove?.Invoke();
		}

		private void InitializeAnimatorMaybe() {
			// TODO: initialize this better

			if (initializedAnimator) {
				return;
			}

			animator = GetComponent<Animator>();

			// TODO: check if animator is null -> throw an error

			animator.applyRootMotion = false;

			// TODO: allow auto-assign to be false, and assign animator controller to be called from outside (ie: when using UMA)
			AssignAnimationController();

			initializedAnimator = true;

		}

		public void AssignAnimationController() {
			var animationController = MeatPuppetManager.Instance.meatPuppetAnimationController;
			// use the animation controller from the manager
			// create an override controller over it
			animatorOverrideController = new AnimatorOverrideController(animationController);
			animatorOverrideController.name = "Controlled by Meat Puppet";

			animator.runtimeAnimatorController = animatorOverrideController;
		}

		public void SetFloat(string name, float value) {
			if (HasAnimator && HasAnimatorController) {
				Animator.SetFloat(name, value);
			}
		}

		public float GetFloat(string name) {
			if (HasAnimator && HasAnimatorController) {
				return Animator.GetFloat(name);
			}
			return 0;
		}

		public void SetTrigger(string name) {
			if (HasAnimator && HasAnimatorController) {
				Animator.SetTrigger(name);
			}
		}

		public void ResetTrigger(string name) {
			if (HasAnimator && HasAnimatorController) {
				Animator.ResetTrigger(name);
			}
		}

		public void SetBool(string name, bool value) {
			if (HasAnimator && HasAnimatorController) {
				Animator.SetBool(name, value);
			}
		}

		public bool GetBool(string name) {
			if (HasAnimator && HasAnimatorController) {
				return Animator.GetBool(name);
			}
			return false;
		}

		public void InformOfEmptyState(AnimatorStateInfo stateInfo, int layerIndex) {
			onStateChange?.Invoke(stateInfo, layerIndex);
		}
	}
}