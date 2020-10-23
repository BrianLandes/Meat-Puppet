
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PBG.MeatPuppet {


	[Serializable]
	public class LegsSettings {
		[NonSerialized] public float strength = 50f;

		[NonSerialized] public float damping = 160f;

		[NonSerialized] public float airborneStrength = 250f;

		[NonSerialized] public float airborneDamping = 1660f;

		public float offset = 0f;

		public float slopeSpeedModifier = 1.8f;

		public bool inheritGroundsVelocity = true;
	}

	public class MeatPuppetLegs {

		// TODO: apply force to ground if dynamic
		

		private float lastDisplacement = 0;

		private MeatPuppet parentPuppet;

		//private float slopeSpeedModifier = 1f;
		
		enum Mode {
			Normal,
			Jumping,
			// NormalUngrounded,
		}

		private Mode mode = Mode.Normal;

		/// <summary>
		/// The minimum time to spend in Jump mode before trying to switch back.
		/// </summary>
		private float minJumpTime = 0.1f;
		private float minJumpTimer = 0.0f;

		private float ungroundedDelay = 0.1f;
		private float ungroundedDelayTimer = 0.0f;

		private bool useAirborneStrength = false;
		private float airborneStrengthTime = 0.2f;
		private float airborneStrengthTimer = 0.0f;

		//private bool needsGroundPenetrationCorrection = false;
		//private float lastOffset = 0.0f;

		//private float lastGroundPenetration;

		public MeatPuppetLegs(MeatPuppet parent) {
			parentPuppet = parent;
		}

		public bool IsGrounded { get; private set; }
		
		private Vector3 GetDirection() {
			return Vector3.down;
		}
		
		public void Update() {
			switch( mode ) {
				case Mode.Normal:
					UpdateNormal();
					break;
				case Mode.Jumping:
					UpdateJump();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}
		
		/// <summary>
		/// During Normal Mode, the legs will look for ground beneath the puppet and apply force (usually upwards)
		/// to keep the puppet a certain distance above the ground.
		/// If no ground is detected, the puppet is considered 'ungrounded'.
		/// </summary>
		private void UpdateNormal() {
			
			var distance = parentPuppet.bodyDimensions.legLength * 1.66f;

			if (CastForGround(distance, out var results, out var raycastHit)) {
				
				var displacement = results.distanceToGround;
				var direction = GetDirection();

				var strength = parentPuppet.configuration.strength;
				var damping = parentPuppet.configuration.damping;

				if (useAirborneStrength) {
					airborneStrengthTimer -= Time.deltaTime;
					useAirborneStrength = airborneStrengthTimer > 0f;
					strength = parentPuppet.configuration.airborneStrength;
					damping = parentPuppet.configuration.airborneDamping;
				}

				if (displacement > 0) {
					
					parentPuppet.Rigidbody.AddForce(-direction * (displacement * strength), ForceMode.Acceleration);

					//apply a damping force, proportional to the velocity
					float changeInDisplacement = displacement - lastDisplacement;

					parentPuppet.Rigidbody.AddForce(-direction * (changeInDisplacement * damping), ForceMode.Acceleration);
				}
				
				lastDisplacement = displacement;

				parentPuppet.Locomotion.SetIgnoreVelocity(Vector3.zero);
				parentPuppet.Locomotion.SetIgnoreTurnSpeed(Vector3.zero);
				if (parentPuppet.configuration.inheritGroundsVelocity && !raycastHit.collider.gameObject.isStatic) {
					// handle each case:
					// other object is a dynamic rigidbody
					// other object is a kinematic rigidbody
					// other object is just a collider that is moving
					if (raycastHit.collider.attachedRigidbody != null) {
						var theirBody = raycastHit.collider.attachedRigidbody;

						InheritVelocityFromGround(
							theirBody.position,
							theirBody.velocity,
							theirBody.angularVelocity
						);
					}
					else {
						var theirObject = raycastHit.collider.gameObject;
						var colliderVelocity = theirObject.GetComponent<ColliderVelocity>();
						if (colliderVelocity == null) {
							colliderVelocity = theirObject.AddComponent<ColliderVelocity>();
						}

						InheritVelocityFromGround(
							theirObject.transform.position,
							colliderVelocity.GetVelocity(),
							colliderVelocity.GetAngularVelocity()
						);
					}
				}

				//float newModifier = 1f - (1f - raycastHit.normal.y) * parentPuppet.configuration.slopeSpeedModifier;
				//slopeSpeedModifier = slopeSpeedModifier + (newModifier - slopeSpeedModifier) * 0.4f;

				UpdateGrounded( true );
			}

			else {
				lastDisplacement = 0;

				//slopeSpeedModifier = 1f;

				UpdateGrounded(false);
			}
		}

		/// <summary>
		/// While in Jump mode, the legs will only search for ground in order to land and return to normal mode.
		/// </summary>
		private void UpdateJump() {
			// Remain in Jump mode until a variety of conditions are met
			//UpdateGrounded(false);
			//slopeSpeedModifier = 1f;
			// stay in jump mode for a minimum amount of time
			if (minJumpTimer > 0) {
				minJumpTimer -= Time.deltaTime;
				UpdateGrounded(false);
				return;
			}
			
			if (parentPuppet.Rigidbody.velocity.y > 0) {
				// if we're moving upwards -> remain in Jump mode
				UpdateGrounded(false);
				return;
			}

			var landForce = -parentPuppet.Rigidbody.velocity.y / 5f - 0.4f;
			parentPuppet.AnimatorHook.Animator.SetFloat("Land Force", landForce);

			// check for ground within leg distance
			var distance = parentPuppet.bodyDimensions.legLength - parentPuppet.bodyDimensions.bodyRadius * 0.25f;
			if (CastForGround(distance, out var distanceToGround, out var raycastHit)) {
				// if there IS ground -> switch back to normal mode
				mode = Mode.Normal;
				UpdateGrounded( true );
			}
			else {
				UpdateGrounded(false);
			}
		}

		private bool CastForGround(float distanceToCast, out CastResults results, out RaycastHit raycastHit ) {

			results = new CastResults();

			var origin = parentPuppet.GetPelvisPoint();
			var radius = parentPuppet.bodyDimensions.bodyRadius * 0.25f;
			var direction = GetDirection();
			var groundLayer = parentPuppet.configuration.groundLayer;

			var hits = Physics.SphereCastAll(origin, radius, direction, distanceToCast, groundLayer);

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

			results.offset = (Physics.gravity.y) / parentPuppet.configuration.strength;

			if (useAirborneStrength) {
				results.offset = (Physics.gravity.y) / parentPuppet.configuration.airborneStrength;
			}

			float targetPosition = results.offset + parentPuppet.configuration.offset + parentPuppet.transform.position.y;
			float groundPoint = raycastHit.point.y;
			// sometimes the collision result will return Vector3.zero, which does us no good
			if (raycastHit.point == Vector3.zero) {
				groundPoint = origin.y;
			}

			results.distanceToGround = groundPoint - targetPosition;

			//results.groundPenetration = Mathf.Max(0f, groundPoint - parentPuppet.transform.position.y);

			return closestHitDistance >= 0;
		}

		private struct CastResults {
			public float distanceToGround;
			public float offset;
			//public float groundPenetration;
		}
		
		private void InheritVelocityFromGround(Vector3 groundCenterOfMass, Vector3 groundVelocity, Vector3 groundAngularVelocity) {

			parentPuppet.Rigidbody.position += groundVelocity * Time.deltaTime;

			var spin = Quaternion.AngleAxis(groundAngularVelocity.y, Vector3.up);
			//var spin = Quaternion.Euler(colliderVelocity.GetAngularVelocity() * Time.deltaTime);
			parentPuppet.Rigidbody.rotation *= spin;

			// if the ground is rotating -> move the position of the puppet in a circle around the ground's center of mass
			var lastPosition = parentPuppet.Rigidbody.position - groundCenterOfMass;

			spin = Quaternion.Euler(groundAngularVelocity);
			var changeInPosition = spin * lastPosition - lastPosition;
			parentPuppet.Rigidbody.position += changeInPosition;

			parentPuppet.Locomotion.SetIgnoreVelocity(groundVelocity + changeInPosition / Time.deltaTime);

			parentPuppet.Locomotion.SetIgnoreTurnSpeed(groundAngularVelocity / Time.deltaTime);
		}

		private void UpdateGrounded( bool touchingGround ) {
			if (IsGrounded && !touchingGround) {
				ungroundedDelayTimer -= Time.deltaTime;
				if (ungroundedDelayTimer <=0) {
					IsGrounded = false;
				}
			}
			else if (IsGrounded && touchingGround) {
				ungroundedDelayTimer = ungroundedDelay;
			}
			else {
				IsGrounded = touchingGround;

			}

			parentPuppet.AnimatorHook.Animator.SetBool("Grounded", IsGrounded);

			if ( !IsGrounded) {
				airborneStrengthTimer = airborneStrengthTime;
				useAirborneStrength = true;
			}
		}

		//public float GetSlopeSpeedModifier() {
		//	return slopeSpeedModifier;
		//}

		public void StartJumpMode() {
			mode = Mode.Jumping;
			minJumpTimer = minJumpTime;
			ungroundedDelayTimer = 0;
			UpdateGrounded(false);
		}
		
		private float CalculateOffset(float currentOffset, float groundY, float puppetY) {
			if (Mathf.Approximately(groundY, puppetY)) {
				if (Mathf.Approximately(0, parentPuppet.Rigidbody.velocity.y)) {
					return currentOffset * 0.98f;
				}
			}
			else if (groundY > puppetY) {
				float difference = puppetY - groundY;
				return currentOffset + (difference*3f - currentOffset) * 0.1f;
			}

			return currentOffset * 0.98f;
		}
	}

}