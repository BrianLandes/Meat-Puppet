
using System.Collections.Generic;
using UnityEngine;
namespace PBG.MeatPuppet {
	public class CycleOrbitalTargets : MonoBehaviour {

		public List<Transform> targets = new List<Transform>();

		public Transform cameraController;

		private int currentTargetIndex = -1;

		private void Start() {
			AssignNextTarget();
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				AssignNextTarget();
			}
		}

		private void AssignNextTarget() {
			if (targets.Count == 0) {
				return;
			}
			currentTargetIndex++;
			while (currentTargetIndex >= targets.Count || !targets[currentTargetIndex].gameObject.activeInHierarchy) {
				if (currentTargetIndex >= targets.Count) {
					currentTargetIndex = 0;
				} else
				if (!targets[currentTargetIndex].gameObject.activeInHierarchy) {
					currentTargetIndex++;
				}
			}
			


			var newTarget = targets[currentTargetIndex];

			cameraController.SetParent(newTarget);
			cameraController.localPosition = Vector3.zero;
		}
	}

}
