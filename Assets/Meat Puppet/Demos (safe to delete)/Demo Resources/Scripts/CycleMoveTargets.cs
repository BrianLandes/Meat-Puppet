
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.MeatPuppet {
	public class CycleMoveTargets : MonoBehaviour {

		public Transform moveTarget;

		public MeatPuppet meatPuppet;

		public List<Transform> targets = new List<Transform>();

		public float waitTime = 6f;

		private void Start() {
			meatPuppet.Movement.MoveTargetTransform = moveTarget;

			if (moveTarget!=null && targets.Count > 0) {
				StartCoroutine(CycleTargets());

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

	}

}
