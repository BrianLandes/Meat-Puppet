
using System;
using UnityEngine;

namespace PBG.MeatPuppet {
	[Serializable]
	public class MeatPuppetLoopingAnimationConfiguration : MeatPuppetOneShotAnimationConfiguration {
		[Tooltip("The humanoid animation to play on the puppet, looping.")]
		public AnimationClip loopingClip;

		[Header("Entry and Exit")]
		[Tooltip("(Optional) The humanoid animation to play as a transition into the looping animation.")]
		public AnimationClip entryClip;
		[Tooltip("(Optional) The humanoid animation to play as a transition out of the looping animation.")]
		public AnimationClip exitClip;

		// [Header("Settings")]
		// public bool applyRootMotion = false;
		// public bool makeIntangible = false;
		// public bool makeKinematic = false;
	}
}

