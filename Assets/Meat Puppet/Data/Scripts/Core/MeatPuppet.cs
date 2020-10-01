
using System;
using UnityEngine;
namespace PBG.MeatPuppet {
	[DefaultExecutionOrder(-999)]
	public class MeatPuppet : MonoBehaviour {

		#region Settings

		[Tooltip("Whether to use the 'walking' animation and speed, or the 'running' animation and speed.")]
		public bool running = false;

		[Tooltip("(Optional) The puppet's initial destination on start.")]
		public Transform initialMoveTarget;
		
		[Tooltip("(Optional) The puppet's initial facing target on start.")]
		public Transform initialFacingTarget;
		
		public MovementSettings movementSettings;

		public LegsSettings legsSettings;

		public JumpSettings jumpSettings;

		public BodyDimensions bodyDimensions;

		#endregion

		#region Properties
		
		public bool Initialized { get; private set; }

		public Tangibility Tangibility {
			get {
				if (Collider.enabled && !Collider.isTrigger) {
					return Tangibility.Corporeal;
				}

				return Tangibility.Intangible;
			}
			set {
				if (value == Tangibility.Corporeal) {
					Collider.enabled = true;
					Collider.isTrigger = false;
				}
				else {
					Collider.enabled = false;
				}
			}
		}

		public PhysicalMotionType MotionType {
			get {
				if (Rigidbody.isKinematic) {
					return PhysicalMotionType.Kinematic;
				}
				// TODO: static?
				return PhysicalMotionType.Dynamic;
			}
			set {
				if (value == PhysicalMotionType.Kinematic) {
					Rigidbody.isKinematic = true;
				}
				else {
					Rigidbody.isKinematic = false;
				}
			}
		}
		
		#endregion
		
		#region Public Parts

		public MeatPuppetAnimatorHook AnimatorHook { get; private set; }

		/// <summary>
		/// Capsule collider that approximates the puppet's torso and head.
		/// </summary>
		public Collider Collider { get; private set; }

		public Rigidbody Rigidbody { get; private set; }

		public MeatPuppetMovement Movement { get; private set; }

		public MeatPuppetLocomotion Locomotion { get; private set; }

		public MeatPuppetJump Jump { get; private set; }

		public MeatPuppetLegs Legs { get; private set; }
		
		public MeatPuppetAnimation Animation { get; private set; }
		

		#endregion

		#region MonoBehaviour Lifecycle

		public void Start() {
			GrowParts();

			Movement.MoveTargetTransform = initialMoveTarget;
			Movement.FacingTargetTransform = initialFacingTarget;
		}

		public void OnEnable() {
			Locomotion?.Reset();
		}

		private void LateUpdate() {
			Locomotion.Update();
		}

		private void FixedUpdate() {
			Legs.Update();
			Jump.Update();
		}

#if UNITY_EDITOR

		private void OnValidate() {
			if (Movement != null) {
				Movement.InitializeNavAgent();
			}
		}
#endif
		#endregion

		#region Public Methods


		#endregion


		#region Body Positions


		public Vector3 GetPelvisPoint() {
			// TODO: assumes that the puppet is standing
			return transform.position + Vector3.up * bodyDimensions.legLength;
		}

		public Vector3 GetHeadPoint() {
			// TODO: assumes that the puppet is standing
			return transform.position + Vector3.up * bodyDimensions.bodyHeight;
		}

		#endregion

		#region Private Methods

		private void GrowParts() {
			Locomotion = new MeatPuppetLocomotion(this);

			Legs = new MeatPuppetLegs(this);

			var animator = gameObject.GetComponentInChildren<Animator>();
			AnimatorHook = animator.gameObject.AddComponent<MeatPuppetAnimatorHook>();

			Collider = GetComponent<Collider>();
			if (Collider == null) {
				// create and assign a capsule collider

				var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				Collider = capsuleCollider;
			}
			if (bodyDimensions.autoConfigure) {
				AutoConfigureBodyDimensions();
			}

			ConfigureRigidBody();

			Movement = gameObject.AddComponent<MeatPuppetMovement>();

			Jump = new MeatPuppetJump(this);
			
			Animation = new MeatPuppetAnimation(this);

			Initialized = true;
		}

		private void AutoConfigureBodyDimensions() {
			// give it automatic dimensions based on the model
			var capsuleCollider = Collider as CapsuleCollider;
			var meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

			var bounds = new Bounds(meshRenderers[0].bounds.center, meshRenderers[0].bounds.size);

			foreach( var meshRenderer in meshRenderers) {
				bounds.Encapsulate(meshRenderer.bounds);
			}

			bodyDimensions.bodyRadius = capsuleCollider.radius = bounds.extents.z / transform.localScale.z;
			bodyDimensions.bodyHeight = bounds.size.y / transform.localScale.y;
			capsuleCollider.height = bodyDimensions.bodyHeight * bodyDimensions.torsoBodyRatio;
			capsuleCollider.center = Vector3.up * (bodyDimensions.bodyHeight - capsuleCollider.height * 0.5f);

			capsuleCollider.material = MeatPuppetManager.Instance.characterPhysicMaterial;

			bodyDimensions.legLength = bodyDimensions.bodyHeight - capsuleCollider.height;
		}

		private void ConfigureRigidBody() {
			Rigidbody = GetComponent<Rigidbody>();
			if (Rigidbody == null) {
				Rigidbody = gameObject.AddComponent<Rigidbody>();
			}
			// assign some properties:
			Rigidbody.isKinematic = false;
			Rigidbody.angularDrag = 1;

			var capsuleCollider = Collider as CapsuleCollider;
			float capsuleRadius = capsuleCollider.radius * transform.localScale.z;
			float capsuleHeight = capsuleCollider.height * transform.localScale.y;

			Rigidbody.mass = bodyDimensions.density * MeatPuppetToolKit.VolumeOfCapsule(capsuleHeight, capsuleRadius);
			Rigidbody.drag = 1;
			Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}

		private void ApplyJumpForce() {
			Jump.ApplyJumpForce();
		}

		#endregion

	}

	#region Classes and Enums
	
	[Serializable]
	public class BodyDimensions {
		public bool autoConfigure = true;

		[NonSerialized] public float torsoBodyRatio = 0.7f;

		[NonSerialized] public float density = 10f;

		[NonSerialized] public float legLength;

		[NonSerialized] public float bodyRadius;
		[NonSerialized] public float bodyHeight;

	}

	public enum Tangibility {
		Corporeal,
		Intangible
	}

	public enum PhysicalMotionType {
		Kinematic,
		Dynamic,
		Static
	}
	
	#endregion
}