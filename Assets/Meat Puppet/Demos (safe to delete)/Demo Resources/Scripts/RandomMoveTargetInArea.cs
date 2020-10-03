
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PBG.MeatPuppet {
	public class RandomMoveTargetInArea : MonoBehaviour {

		public Transform moveTarget;

		public MeatPuppet meatPuppet;

		public Transform center;

		public float radius;

		//public float waitTime = 6f;

		//public float waitTimeVariance = 1f;

		public Sprite indicator;

		private float time;

		private void Start() {
			meatPuppet.Movement.MoveTargetTransform = moveTarget;

			moveTarget.position = GetRandomPathablePointInCircle(center.position, radius);

			//StartCoroutine(CycleTargets());

		}

		private void Update() {
			if (meatPuppet.Movement.ReachedTarget) {
				moveTarget.position = GetRandomPathablePointInCircle(center.position, radius);
			}
		}

		

		//private IEnumerator CycleTargets() {
		//	while (true) {

		//		moveTarget.position = GetRandomPathablePointInCircle(center.position, radius);

		//		yield return new WaitForSeconds(waitTime + (UnityEngine.Random.value*2f - 1f) * waitTimeVariance);
		//	}

		//}

		public static Vector3 GetRandomPathablePointInCircle(Vector3 worldCenter, float radius) {

			for (int i = 0; i < 300; i++) {
				Vector3 randomPoint = worldCenter + UnityEngine.Random.insideUnitCircle.FromXZ() * radius;
				NavMeshHit hit;
				if (NavMesh.SamplePosition(randomPoint, out hit, 3.0f, NavMesh.AllAreas)) {
					return hit.position;
				}
			}
			return Vector3.zero;
		}
	}

}
