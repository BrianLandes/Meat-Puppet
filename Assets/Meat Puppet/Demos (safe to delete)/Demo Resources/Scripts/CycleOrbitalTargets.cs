
using System.Collections.Generic;
using UnityEngine;
namespace PBG.MeatPuppet {
	public class CycleOrbitalTargets : MonoBehaviour {

		public List<Transform> targets = new List<Transform>();

		public Transform cameraController;

		private int currentTargetIndex = 0;

		private void Start() {
			var newTarget = targets[currentTargetIndex];

			cameraController.SetParent(newTarget);
			cameraController.localPosition = Vector3.zero;
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				currentTargetIndex++;
				if (currentTargetIndex >= targets.Count) {
					currentTargetIndex = 0;
				}

				var newTarget = targets[currentTargetIndex];

				cameraController.SetParent(newTarget);
				cameraController.localPosition = Vector3.zero;
			}
		}

	}

}
