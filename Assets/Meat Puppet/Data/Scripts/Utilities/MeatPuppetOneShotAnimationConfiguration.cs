
using System;
using UnityEngine;

namespace PBG.MeatPuppet {
	[Serializable]
	public class MeatPuppetOneShotAnimationConfiguration {
		// TODO: combine looping and one-shot configurations?
		[Header("Settings")]
		public bool applyRootMotion = false;
		public bool makeIntangible = false;
		public bool makeKinematic = false;
		public bool overrideLegs = true;
	}
}

