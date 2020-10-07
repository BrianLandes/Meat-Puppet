
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.MeatPuppet {
	public class CycleFacingTargets : MonoBehaviour {

		public Transform facingTarget;

		public MeatPuppet meatPuppet;

		public List<Transform> targets = new List<Transform>();

		public float waitTime = 6f;

		private void Start() {
			meatPuppet.Movement.SetFacingTarget(facingTarget);
			//meatPuppet.Movement.FacingTargetTransform = facingTarget;

			StartCoroutine(CycleTargets());
		}

		private IEnumerator CycleTargets() {
			while (true) {
				foreach (var target in targets) {
					facingTarget.position = target.position;

					yield return new WaitForSeconds(waitTime);
				}
			}

		}
	}

}
