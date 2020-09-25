
using System;
using UnityEngine;

namespace PBG.MeatPuppet {
	public class VaultOverBox : MonoBehaviour {

		public AnimationClip vaultAnimation;
		public MeatPuppetOneShotAnimationConfiguration animationConfiguration;

		private bool triggered = false;
		
		private void OnTriggerEnter(Collider other) {
			if (!triggered && other.gameObject.CompareTag("Player")) {
				var meatPuppet = other.gameObject.GetComponent<MeatPuppet>();
				if (!meatPuppet.Animation.PlayingAnimation) {
					meatPuppet.Animation.PlayOneShotAnimation(vaultAnimation, animationConfiguration);
					triggered = true;
				}
			}
		}

		private void OnTriggerExit(Collider other) {
			if ( other.gameObject.CompareTag("Player")) {
				triggered = false;
			}
		}
	}

}
