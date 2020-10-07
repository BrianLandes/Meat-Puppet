
using System.Collections.Generic;
using UnityEngine;
namespace PBG.MeatPuppet {
	/// <summary>
	/// Used in test scenes to focus the cameras on various targets.
	/// 
	/// </summary>
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

		public void OnGUI() {
			GUI.Label(new Rect(10, 60, 300, 200), "Press Left Shift to focus on next character");
		}
	}

}
