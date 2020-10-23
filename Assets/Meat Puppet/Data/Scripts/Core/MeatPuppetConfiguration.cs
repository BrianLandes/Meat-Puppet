
using UnityEngine;

namespace PBG.MeatPuppet {
	[CreateAssetMenu]
	public class MeatPuppetConfiguration : ScriptableObject {
		[Tooltip("The animation controller assigned across all puppets.")]
		public RuntimeAnimatorController meatPuppetAnimationController;

		[Tooltip("The layers that will be considered 'ground' to the puppets.")]
		public LayerMask groundLayer;

		[Tooltip("The layers that will be considered static obstacles to the puppets.")]
		public LayerMask staticLayer;

		public PhysicMaterial characterPhysicMaterial;

		[Header("Movement - Basics")]
		[Tooltip("The top speed this puppet can move while walking, in Unity units.")]
		public float walkSpeed = 4f;

		[Tooltip("The top speed this puppet can move while running, in Unity units.")]
		public float runSpeed = 6f;

		[Tooltip("The distance allowed between the puppet and the destination before stopping.")]
		public float stoppingDistance = 0.7f;

		//[NonSerialized] public bool avoidStaticObstacles = false; // experimental feature, not ready

		[Header("Movement - Advanced")]
		[Tooltip("The max force or acceleration applied to get the puppet to top speed.")]
		public float moveAcceleration = 4f;

		[Tooltip("Whether or not to disable movement when the puppet does not have their feet on the ground.")]
		public bool disableMovementWhenUngrounded = false;

		[Tooltip("The amount of force applied AGAINST the puppet in order to bring it to a stop. It is a factor of the puppet's velocity.")]
		public float brakeFactor = 1f;

		[Tooltip("The amount of angular force applied to turn the puppet.")]
		public float turnForce = 1.5f;

		[Tooltip("The amount of dampening to apply when turning (reduces elasticity).")]
		public float turnDampening = 80f;

		[Tooltip("The turn force to use when the puppet is kinematic.")]
		public float preciseTurnForce = 100f;

		[Tooltip("Allowed distance between the actual position and the navAgent's position.")]
		public float navAgentAllowedDistance = 0.4f;

		[Tooltip("The threshold to use when determining if the puppet has reached the target facing position.")]
		public float facingTolerance = 0.1f;

		[Header("Legs")]
		public float strength = 50f;

		public float damping = 160f;

		public float airborneStrength = 250f;

		public float airborneDamping = 1660f;

		public float offset = 0f;

		public float slopeSpeedModifier = 1.8f;

		public bool inheritGroundsVelocity = true;

		[Header("Jump")]
		public bool enableJump = true;
		public float initialJumpForce = 6.0f;
		public float jumpDelay = 0.38f;
		public bool useJumpBoost = true;
		public float jumpBoostTime = 0.6f;
		public float boostForce = 15f;
	}

}
