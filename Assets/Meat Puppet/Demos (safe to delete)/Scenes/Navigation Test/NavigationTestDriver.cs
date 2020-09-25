
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.MeatPuppet {
	public class NavigationTestDriver : MonoBehaviour {

		public Transform moveTarget;
		public Transform facingTarget;

		public MeatPuppet meatPuppet;

		public List<Transform> targets = new List<Transform>();

		public List<Transform> facingTargets = new List<Transform>();

		public float waitTime = 6f;

		private void Start() {
			meatPuppet.Movement.MoveTargetTransform = moveTarget;
			meatPuppet.Movement.FacingTargetTransform = facingTarget;

			if (moveTarget!=null && targets.Count > 0) {
				StartCoroutine(CycleTargets());

			}

			if (facingTarget!=null && facingTargets.Count > 0) {
				StartCoroutine(CycleFacingTargets());
			}
		}

		private IEnumerator CycleTargets() {
			while (true) {
				foreach (var target in targets) {
					moveTarget.position = target.position;

					yield return new WaitForSeconds(waitTime);
				}
			}
			
		}

		private IEnumerator CycleFacingTargets() {
			while (true) {
				foreach (var target in facingTargets) {
					facingTarget.position = target.position;

					yield return new WaitForSeconds(waitTime);
				}
			}

		}
	}

}
