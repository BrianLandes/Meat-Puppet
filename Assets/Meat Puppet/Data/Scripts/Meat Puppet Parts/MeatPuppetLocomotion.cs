
using UnityEngine;
namespace PBG.MeatPuppet {
	public class MeatPuppetLocomotion {

		private float forwardSpeedFactor = 1.9f;
		private float forwardSpeedSmoothing = 16f;
		private float lastForwardSpeed;
		
		//private float horizontalSpeedFactor = 1.9f;
		private float horizontalSpeedSmoothing = 16f;
		private float lastHorizontalSpeed;
		
		private float turnSpeedFactor = 0.01f;

		private float minDeltaTime = 0.1f;

		// private float velocitySmoothing = 30.0f;
		
		private float turnSpeedSmoothing = 0.2f;

		float lastTime;
		Vector3 lastPosition;
		Vector3 lastVelocity;

		Vector3 ignoreVelocity;
		Vector3 ignoreTurnSpeed;

		private Quaternion lastRotation;
		private Vector3 lastAngularSpeed;

		private MeatPuppet parentPuppet;

		private float slopeSpeedModifier = 1f;

		float acceleration = 0f;

		private readonly int forwardSpeedKey;
		private readonly int horizontalSpeedKey;
		private readonly int turnSpeedKey;
		private readonly int groundSteepKey;
		
		public MeatPuppetLocomotion(MeatPuppet parent) {
			parentPuppet = parent;
			Reset();

			forwardSpeedKey = Animator.StringToHash("Forward Speed");
			horizontalSpeedKey = Animator.StringToHash("Horizontal Speed");
			turnSpeedKey = Animator.StringToHash("Turn Speed");
			groundSteepKey = Animator.StringToHash("Ground Steep");
		}

		public void Reset() {
			lastPosition = parentPuppet.transform.position;
			lastTime = Time.time;
			lastVelocity = Vector3.zero;

			lastRotation = parentPuppet.transform.rotation;

			lastForwardSpeed = 0f;
			lastHorizontalSpeed = 0f;
		}

		public void Update() {
			UpdateRelativeSpeeds();

			var velocity = GetVelocity();
			velocity.y = 0;
			velocity = parentPuppet.transform.InverseTransformDirection(velocity);

			float speedFactor = 1f;
			if (parentPuppet.running) {
				speedFactor = (forwardSpeedFactor / parentPuppet.movementSettings.runSpeed) * 2f;
			}
			else {
				speedFactor = forwardSpeedFactor / parentPuppet.movementSettings.walkSpeed;
			}
			
			float forwardSpeed = velocity.z*speedFactor;
			//forwardSpeed = MeatPuppetManager.Instance.forwardSpeedCurve.Evaluate(forwardSpeed);
			lastForwardSpeed = Mathf.Lerp(lastForwardSpeed, forwardSpeed, forwardSpeedSmoothing* Time.deltaTime);
			parentPuppet.AnimatorHook.Animator.SetFloat(forwardSpeedKey, lastForwardSpeed);

			float horizontalSpeed = velocity.x*speedFactor;
			//horizontalSpeed = MeatPuppetManager.Instance.forwardSpeedCurve.Evaluate(horizontalSpeed);
			lastHorizontalSpeed = Mathf.Lerp(lastHorizontalSpeed, horizontalSpeed, horizontalSpeedSmoothing* Time.deltaTime);
			parentPuppet.AnimatorHook.Animator.SetFloat(horizontalSpeedKey, lastHorizontalSpeed);

			parentPuppet.AnimatorHook.Animator.SetFloat(turnSpeedKey, lastAngularSpeed.y * turnSpeedFactor, turnSpeedSmoothing, Time.deltaTime);

			float groundSteep = CalculateGroundSteep();
			// if groundSteep > 0 -> reduce speedModifer from 1f -> 0f
			//slopeSpeedModifier = 1f - Mathf.Max(0f, groundSteep*0.5f);

			parentPuppet.AnimatorHook.Animator.SetFloat(groundSteepKey, groundSteep, turnSpeedSmoothing, Time.deltaTime);

			//parentPuppet.AnimatorHook.Animator.SetFloat("Acceleration", acceleration);
		}

		private void UpdateRelativeSpeeds() {
			// Use the difference in positions to determine the velocity
			var timeDelta = Time.time - lastTime;
			if (timeDelta < minDeltaTime) {
				return;
			}

			var velocity = (parentPuppet.transform.position - lastPosition) / timeDelta;
			velocity -= ignoreVelocity;

			acceleration = velocity.magnitude - lastVelocity.magnitude;

			// lastVelocity = Vector3.Lerp(lastVelocity, velocity, velocitySmoothing * timeDelta);
			lastVelocity = (lastVelocity + velocity) * 0.5f;
			// lastVelocity = velocity;

			

			lastPosition = parentPuppet.transform.position;

			var rotationDifference = parentPuppet.transform.rotation * Quaternion.Inverse(lastRotation);
			float angleInDegrees;
			Vector3 rotationAxis;
			rotationDifference.ToAngleAxis(out angleInDegrees, out rotationAxis);

			Vector3 angularDisplacement = rotationAxis * angleInDegrees ;
			lastAngularSpeed = angularDisplacement / timeDelta;
			lastAngularSpeed -= ignoreTurnSpeed;
			lastRotation = parentPuppet.transform.rotation;

			lastTime = Time.time;
		}

		private Vector3 GetVelocity() {
			return lastVelocity;
		}



		public void SetIgnoreVelocity(Vector3 ignoreVelocity) {
			this.ignoreVelocity = ignoreVelocity;
		}

		public void SetIgnoreTurnSpeed(Vector3 ignoreTurnSpeed) {
			this.ignoreTurnSpeed = ignoreTurnSpeed;
		}

		private float CalculateGroundSteep() {
			var distance = parentPuppet.bodyDimensions.legLength + parentPuppet.bodyDimensions.bodyHeight;
			if (CastForGround(distance, out var raycastHit)) {
				float verticalDifference = raycastHit.point.y - parentPuppet.transform.position.y;
				return verticalDifference / (parentPuppet.bodyDimensions.legLength * 1.0f);
			}
			else {
				return -1f;
			}
		}
		
		private bool CastForGround(float distance, out RaycastHit raycastHit) {

			// check forward in front of us for ground
			// if the ground ahead of the puppet is higher
			
			var origin = parentPuppet.GetHeadPoint() + parentPuppet.transform.forward * (parentPuppet.bodyDimensions.bodyRadius * 2f);
			var radius = parentPuppet.bodyDimensions.bodyRadius * 0.25f;
			var direction = Vector3.down;
			var groundLayer = MeatPuppetManager.Instance.groundLayer;

			var hits = Physics.SphereCastAll(origin, radius, direction, distance, groundLayer);

			float closestHitDistance = -1;
			raycastHit = new RaycastHit();

			
			foreach( var hit in hits ) {
				// ignore collisions with the puppet's own collider
				if ( hit.collider == parentPuppet.Collider ) {
					continue;
				}

				// ignore collisions with triggers
				if (hit.collider.isTrigger) {
					continue;
				}

				float thisDistance = Vector3.Distance(origin, hit.point);
				if (hit.point == Vector3.zero) {
					thisDistance = 0;
				}
				if (closestHitDistance < 0 || thisDistance < closestHitDistance ) {
					closestHitDistance = thisDistance;
					raycastHit = hit;
				}

			}

			return closestHitDistance >= 0;
		}

		public float GetSlopeSpeedModifier() {
			return slopeSpeedModifier;
		}
	}
}