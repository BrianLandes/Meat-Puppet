
using System;
using UnityEditor;
using UnityEngine;
namespace PBG.MeatPuppet {
	
	public class MeatPuppetFeet : MonoBehaviour {

		private MeatPuppet parentPuppet;

		private Animator animator;

		//public float stepHeightThreshold = 0.07f;

		private float weight = 1f;

		private Vector3 leftFootTarget;
		private Vector3 rightFootTarget;

		//private bool lastLeftFootPlace = false;
		//private bool lastRightFootPlace = false;

		//public MeatPuppetFeet(MeatPuppet parent) {
		//	parentPuppet = parent;

		//}

		private void Start() {
			animator = GetComponent<Animator>();
			parentPuppet = GetComponent<MeatPuppet>();
		}
		
		void OnAnimatorIK() {

			{
				//var footPlaced = GetIsFootPlaced(HumanBodyBones.LeftFoot);

				Color color = Color.red;

				//if (footPlaced) {
				//	color = Color.green;
				//}

				var footTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

				var relativePosition = animator.transform.InverseTransformPoint(footTransform.position);

				var legLength = parentPuppet ? parentPuppet.bodyDimensions.legLength: 1f;

				if ( Physics.Raycast(footTransform.position + Vector3.up * legLength, Vector3.down, out RaycastHit raycastHit,
						legLength + relativePosition.y)) {
					animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
					leftFootTarget = raycastHit.point + Vector3.up * relativePosition.y;

					animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weight);

					var flatAgainstSurfaceRotation = Quaternion.FromToRotation(animator.transform.up, raycastHit.normal) * animator.transform.rotation;

					//var originalRotation = Quaternion.AngleAxis footTransform.up;
					var originalRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
					//var rotAxis = Vector3.Cross(animator.transform.up, footTransform.up);
					//var angle = Vector3.Angle(animator.transform.up, footTransform.up);
					//var originalRotation = Quaternion.AngleAxis(angle, rotAxis) * footTransform.rotation;

					animator.SetIKRotation(AvatarIKGoal.LeftFoot, flatAgainstSurfaceRotation*originalRotation);

					color = Color.green;
				}
				else {
					animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
				}

				Debug.DrawLine(footTransform.position, footTransform.position + Vector3.left * 0.1f, color, 0.2f);

				//if (footPlaced) {
				//	animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
				//}
				//else {
				//	animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
				//}

				//if (footPlaced && !lastLeftFootPlace) {
				//	leftFootTarget = footTransform.position;
				//}

				animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget);

				//lastLeftFootPlace = footPlaced;
			}

			//{
			//	var footPlaced = GetIsFootPlaced(HumanBodyBones.RightFoot);

			//	Color color = Color.red;

			//	if (footPlaced) {
			//		color = Color.green;
			//	}

			//	var footTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			//	Debug.DrawLine(footTransform.position, footTransform.position + Vector3.right * 0.1f, color, 0.2f);

			//	if (footPlaced) {
			//		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
			//	}
			//	else {
			//		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
			//	}

			//	if (footPlaced && !lastRightFootPlace) {
			//		rightFootTarget = footTransform.position;
			//	}

			//	animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget);

			//	lastRightFootPlace = footPlaced;
			//}
		}

		//private bool GetIsFootPlaced(HumanBodyBones bone) {
		//	var footTransform = animator.GetBoneTransform(bone);

		//	var relativePosition = animator.transform.InverseTransformPoint(footTransform.position);
			
		//	return relativePosition.y < stepHeightThreshold;
		//}
	}

}
