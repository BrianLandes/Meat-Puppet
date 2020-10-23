
using System;
using UnityEngine;
namespace PBG.MeatPuppet {
	/// <summary>
	/// A movement and animation controller for humanoid characters, particularly NPCs.
	/// Anticipates and handles common features or requirements of interesting game characters, such as:
	/// 1. Smooth walking and turning animations.
	/// 2. Simple movement controls (just give them a destination).
	/// 3. Navigate over uneven terrain and up/down slopes and stairs.
	/// 4. Play arbitrary animations such as a melee attack or a conversation loop.
	/// </summary>
	[DefaultExecutionOrder(-999)]
	public class MeatPuppet : MonoBehaviour {

		#region Settings

		[Tooltip("Whether to use the 'walking' animation and speed, or the 'running' animation and speed.")]
		public bool running = false;

		[Tooltip("(Optional) The puppet's initial destination on start.")]
		public Transform initialMoveTarget;
		
		[Tooltip("(Optional) The puppet's initial facing target on start.")]
		public Transform initialFacingTarget;

		public MeatPuppetConfiguration configuration;

		//[Tooltip("Affects the puppet's acceleration, velocity, turning, and navigation distances.")]
		//public MovementSettings movementSettings;

		//[Tooltip("Affects the 'spring' that acts as the puppet's legs, allowing them to 'hover' over uneven terrain.")]
		//public LegsSettings legsSettings;

		//[Tooltip("Affects the how and if the puppet can jump, launching themselves into the air.")]
		//public JumpSettings jumpSettings;

		[Tooltip("Automatically calculated, read-only values that represent the puppet's physical properties.")]
		public BodyDimensions bodyDimensions;

		#endregion

		#region Properties
		
		public bool Initialized { get; private set; }

		/// <summary>
		/// Whether the puppet collides with other objects or passes through them.
		/// </summary>
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

		/// <summary>
		/// Whether the puppet's movement is affected by collisions with other objects.
		/// </summary>
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

		/// <summary>
		/// Hooks into the Unity Animator component, listens for animation events and propogates them to other parts.
		/// </summary>
		public MeatPuppetAnimatorHook AnimatorHook { get; private set; }

		/// <summary>
		/// Capsule collider that approximates the puppet's torso and head.
		/// </summary>
		public Collider Collider { get; private set; }

		/// <summary>
		/// The main rigidbody for the puppet.
		/// </summary>
		public Rigidbody Rigidbody { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public ColliderVelocity ColliderVelocity { get; private set; }

		/// <summary>
		/// Takes in a move target and/or a facing target and controls the puppet's acceleration and velocity,
		/// pathfinding and navigating them towards those positions using dynamic physics.
		/// </summary>
		public MeatPuppetMovement Movement { get; private set; }

		/// <summary>
		/// Measures the puppet's movement and sets important animator values on the puppet's Animator Controller.
		/// </summary>
		public MeatPuppetLocomotion Locomotion { get; private set; }

		/// <summary>
		/// Allows the puppet to 'jump'. (Currently only works with the Player Controller.)
		/// </summary>
		public MeatPuppetJump Jump { get; private set; }

		/// <summary>
		/// An invisible 'spring' that keeps the puppet's capsule body a certain distance above the ground.
		/// Also manages whether the puppet is 'grounded' (whether or not the puppet is currently standing on solid ground).
		/// </summary>
		public MeatPuppetLegs Legs { get; private set; }
		
		/// <summary>
		/// Allows other scripts and systems to cause the puppet to play arbitrary animations; either looping animations or 'one-shot' animations.
		/// </summary>
		public MeatPuppetAnimation Animation { get; private set; }
		

		#endregion

		#region MonoBehaviour Lifecycle

		public void Start() {
			if (configuration == null) {
				configuration = Resources.Load<MeatPuppetConfiguration>("Default Meat Puppet Configuration");
			}

			GrowParts();
			Movement.SetMoveTarget(initialMoveTarget);
			Movement.SetFacingTarget(initialFacingTarget);
			
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

			ColliderVelocity = gameObject.AddComponent<ColliderVelocity>();

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

			bodyDimensions.bodyRadius = capsuleCollider.radius = (bounds.extents.z / transform.localScale.z) * bodyDimensions.bodyWidthModifier;
			bodyDimensions.bodyHeight = (bounds.size.y / transform.localScale.y) * bodyDimensions.bodyHeightModifier;

			// use the position of the hips bone to determine where the lower body and upper body meet each other

			var hips = AnimatorHook.Animator.GetBoneTransform(HumanBodyBones.Hips); // TODO: do this after the animator has been setup (ie: when UMA is done)
			var hipsPosition = transform.InverseTransformPoint(hips.position);
			
			capsuleCollider.height = bodyDimensions.bodyHeight - hipsPosition.y * bodyDimensions.legLengthModifier;
			capsuleCollider.center = Vector3.up * (bodyDimensions.bodyHeight - capsuleCollider.height * 0.5f);
			
			capsuleCollider.material = configuration.characterPhysicMaterial;

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
	
	/// <summary>
	/// Read-only values representing the puppet's physical properties, such
	/// as height, width, and density.
	/// </summary>
	[Serializable]
	public class BodyDimensions {
		[Tooltip("Whether or not to calculate and set the puppet's body dimensions on start.")]
		public bool autoConfigure = true;

		public float bodyHeightModifier = 0.9f;
		public float bodyWidthModifier = 0.9f;
		public float legLengthModifier = 0.9f;

		[Header("Auto-configure settings")]
		//[Tooltip("The ratio of the puppet's height to use for the body. (The remainder of the height will be the length of the legs.)")]
		//public float torsoBodyRatio = 0.7f;

		[Tooltip("The puppet's density. Used to set the mass on the rigidbody after calculating volume of body.")]
		[NonSerialized] public float density = 20f;

		[Header("Advanced (Set on Auto-configure)")]
		[Tooltip("The length of the puppet's 'legs'.")]
		public float legLength;
		[Tooltip("The radius of the puppet's 'body'.")]
		public float bodyRadius;
		[Tooltip("The height of the puppet's 'body'.")]
		public float bodyHeight;

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