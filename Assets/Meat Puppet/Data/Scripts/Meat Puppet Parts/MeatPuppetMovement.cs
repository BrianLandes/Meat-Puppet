
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace PBG.MeatPuppet {

	[Serializable]
	public class MovementSettings {
		[FormerlySerializedAs("moveSpeed")]
		[Header("Basics")]
		[Tooltip("The top speed this puppet can move while walking, in Unity units.")]
		public float walkSpeed = 4f;
		[Tooltip("The top speed this puppet can move while running, in Unity units.")]
		public float runSpeed = 6f;
		[Tooltip("The distance allowed between the puppet and the destination before stopping.")]
		[NonSerialized] public float stoppingDistance = 0.7f;

		// [Tooltip("Used to scale and modifier the movement speed.")]
		// public float moveSpeedModifier = 1f;

		[NonSerialized] public bool avoidStaticObstacles = false;

		[Header("Advanced")]
		[Tooltip("The max force or acceleration applied to get the puppet to top speed.")]
		public float moveAcceleration = 4f;

		[Tooltip("The amount of force applied AGAINST the puppet in order to bring it to a stop. It is a factor of the puppet's velocity.")]
		[NonSerialized] public float brakeFactor = 8f;

		[NonSerialized] public float turnForce = 1.5f;
		[NonSerialized] public float turnDampening = 80f;

		[NonSerialized] public float preciseTurnForce = 100f;

		[Tooltip("Allowed distance between the actual position and the navAgent's position.")]
		public float navAgentAllowedDistance = 0.2f;

		public float facingTolerance = 0.1f;

		public float avoidanceForce = 4f;
	}

	[AddComponentMenu("")]
	/// <summary>
	/// Handles a puppet's basic movement.
	/// Unity NavMesh Agent is used for pathfinding, but actual translation
	/// is handled manually by applying force to the puppet's rigidbody.
	/// </summary>
	public class MeatPuppetMovement : MonoBehaviour {

		// TODO: better static avoidance
		// TODO: avoid other puppets
		// TODO: rename everything
		// TODO: demote from a MonoBehaviour
		// TODO: Grounded
		// TODO: stairs

		private MeatPuppet parentPuppet;

		#region Private Data Structures

		enum InputType {
			Point,
			Transform,
			Direction,
		}

		#endregion

		#region Private Members

		private NavMeshAgent navAgent;

		private bool hasMoveTarget = false;
		private InputType moveInputType = InputType.Point;
		private Transform moveTargetTransform;
		private Vector3 moveTargetPoint;
		private Vector3 moveTargetDirection;


		private bool hasFacingTarget = false;
		private InputType facingInputType = InputType.Point;
		private Transform facingTargetTransform;
		private Vector3 facingTargetPoint;
		private Vector3 facingTargetDirection;


		private float lastTurnDifference;

		#endregion

		#region Public Methods

		#region Move Target

		public Transform MoveTargetTransform {
			get {
				return moveTargetTransform;
			}
			set {
				hasMoveTarget = value != null;
				moveTargetTransform = value;
				moveInputType = InputType.Transform;
			}
		}

		public Vector3 TargetPoint {
			get {
				return moveTargetPoint;
			}
			set {
				moveTargetPoint = value;
				moveInputType = InputType.Point;
				hasMoveTarget = true;
			}
		}

		public Vector3 TargetDirection {
			get {
				return moveTargetDirection;
			}
			set {
				moveTargetDirection = value;
				moveInputType = InputType.Direction;
				hasMoveTarget = moveTargetDirection != Vector3.zero;
			}
		}

		public bool HasMoveTarget() {
			return hasMoveTarget;
		}

		public void SetNoMoveTarget() {
			hasMoveTarget = false;
			navAgent.enabled = false;
		}

		public float GetSquaredDistanceToTarget() {
			switch (moveInputType) {
				case InputType.Point:
				case InputType.Transform:
					var currentPosition = transform.position;
					var targetPosition = GetTargetPoint();
					var difference = targetPosition - currentPosition;
					return difference.sqrMagnitude;

				default:
				case InputType.Direction:
					return -1;
			}
		}

		public Vector3 GetTargetPoint() {

			switch (moveInputType) {
				default:
				case InputType.Point:
					return TargetPoint;
				case InputType.Transform:
					return MoveTargetTransform.position;
				case InputType.Direction:
					throw new NotImplementedException();
			}
		}

		public bool ReachedTarget {
			get {
				switch (moveInputType) {
					case InputType.Point:
					case InputType.Transform:
						var currentPosition = transform.position;
						var targetPosition = GetTargetPoint();
						bool distanceResult =
							MeatPuppetToolKit.PointAndPointWithinDistanceOfEachOther(
								currentPosition, targetPosition, parentPuppet.movementSettings.stoppingDistance);
						return distanceResult;

					default:
					case InputType.Direction:
						return false;
				}

			}
		}

		#endregion

		#region Facing Target

		public Transform FacingTargetTransform {
			get {
				return facingTargetTransform;
			}
			set {
				hasFacingTarget = value != null;
				facingTargetTransform = value;
				facingInputType = InputType.Transform;
			}
		}

		public Vector3 LookAtPoint {
			get {
				return facingTargetPoint;
			}
			set {
				facingTargetPoint = value;
				facingInputType = InputType.Point;
				hasFacingTarget = true;
			}
		}

		public Vector3 LookAtDirection {
			get {
				return facingTargetDirection;
			}
			set {
				facingTargetDirection = value;
				facingInputType = InputType.Direction;
				hasFacingTarget = true;
			}
		}

		public void StopLookAt() {
			hasFacingTarget = false;
		}

		#endregion

		#endregion

		#region Monobehavior Lifecycle

		private void Start() {
			parentPuppet = GetComponent<MeatPuppet>();

			InitializeNavAgent();
		}

		public void FixedUpdate() {

			//UpdateGrounded();
			UpdateNavAgent();

			//if (IsGrounded()) {
				if (hasMoveTarget && parentPuppet.movementSettings.stoppingDistance <= 0.01f && GetSquaredDistanceToTarget() < 1f) {
					UpdatePositionUsingPreciseMovement();
				}
				else {
					UpdatePosition();
				}


			//}

			UpdateRotation();

		}

		#endregion

		#region Private Methods

		private void UpdatePosition() {

			if (!HasMoveTarget() || ReachedTarget) {
				// If we're within stopping distance of the target -> bring the unit to a stop
				ApplyBrakeForce();

				//if (avoidOtherCharacters) {
				//	var avoidVector = ModifyMoveVectorForAvoidance(Vector3.zero);
				//	if (avoidVector.sqrMagnitude > 0.01f) {
				//		MoveInDirection(avoidVector);

				//	}
				//}

				return;
			}

			// calculate the direction to move towards the target
			var moveVector = GetMoveVector();

			//if (avoidOtherCharacters) {
			//	moveVector = ModifyMoveVectorForAvoidance(moveVector);
			//}

			//if (parentPuppet.movementSettings.avoidStaticObstacles) {
			//	moveVector = GetVectorToAvoidStaticObstacles(moveVector);
			//}

			// As long as the direction isn't zero -> apply some force in that direction
			if (moveVector.sqrMagnitude > 0.01f) {
				MoveInDirection(moveVector);

			}
			// if the move Vector is zero -> bring the unit to a stop
			else {
				ApplyBrakeForce();
			}


		}

		/// <summary>
		/// If the stopping distance is ~0 and the puppet is almost in position, ignore acceleration and velocity and
		/// 'fake' movement in order to get them smoothely and accurately into that position.
		/// </summary>
		private void UpdatePositionUsingPreciseMovement() {
			var moveVector = GetMoveVector();
			var velocity = moveVector * GetTopSpeed() * 0.6f;
			//var newPosition = myBody.position + moveVector * moveSpeed * moveSpeedModifier * Time.deltaTime ;
			//myBody.MovePosition(newPosition);
			parentPuppet.Rigidbody.velocity = velocity;
		}

		private void UpdateRotation() {

			// Try to use the turn vector first
			if (hasFacingTarget) {
				var lookDirection = GetLookAtDirection();

				TurnToDirection(lookDirection.JustXZ());

				//LookAtDirection = Vector3.zero;
			}
			else {

				// calculate the direction to move towards the target
				var moveVector = GetMoveVector();

				// otherwise, turn to face the direction we're moving
				if (moveVector.sqrMagnitude > 0.01f) {

					Vector3 angleVector = moveVector;

					TurnToDirection(angleVector.JustXZ());

				}
				else {
					lastTurnDifference = 0f;
				}

			}

		}

		private float GetTopSpeed() {
			if (parentPuppet.running) {
				return parentPuppet.movementSettings.runSpeed;
			}
			else {
				return parentPuppet.movementSettings.walkSpeed;
			}
		}
		
		#endregion

		#region NavAgent Control

		public void InitializeNavAgent() {
			navAgent = GetComponent<NavMeshAgent>();

			if (navAgent == null) {
				navAgent = gameObject.AddComponent<NavMeshAgent>();
			}

			navAgent.enabled = false;
			navAgent.speed = GetTopSpeed();
			navAgent.acceleration = parentPuppet.movementSettings.moveAcceleration * Time.fixedDeltaTime;
			//navMeshAgent.angularSpeed = 270f;
			//navMeshAgent.acceleration = speed * 1.99f;
			navAgent.stoppingDistance = parentPuppet.movementSettings.stoppingDistance;

			// Don't update position automatically, we'll handle it through the rigid body
			navAgent.updatePosition = false;
			navAgent.updateRotation = false;
			navAgent.autoTraverseOffMeshLink = true;

			navAgent.avoidancePriority = UnityEngine.Random.Range(20, 80);

			navAgent.radius = parentPuppet.bodyDimensions.bodyRadius;
			navAgent.height = parentPuppet.bodyDimensions.bodyHeight;
		}

		private void UpdateNavAgent() {
			if (!HasMoveTarget() || ReachedTarget) {
				navAgent.enabled = false;
			}
			//else if (!IsGrounded()) {
			//	navAgent.enabled = false;
			//}
			else
			if (moveInputType == InputType.Direction) {
				navAgent.enabled = false;
			}
			else {
				if ((navAgent.nextPosition - transform.position).sqrMagnitude > 
						parentPuppet.movementSettings.navAgentAllowedDistance * parentPuppet.movementSettings.navAgentAllowedDistance) {
					navAgent.enabled = false;
					navAgent.nextPosition = transform.position;
				}


				navAgent.enabled = true;
				navAgent.destination = GetTargetPoint();
				navAgent.stoppingDistance = parentPuppet.movementSettings.stoppingDistance;

				// Force the simulated navAgent to maintain the same speed as the actual body
				navAgent.velocity = parentPuppet.Rigidbody.velocity;
				
				navAgent.speed = GetTopSpeed();
				navAgent.acceleration = parentPuppet.movementSettings.moveAcceleration * Time.fixedDeltaTime;
			}
		}
		private bool UseNavAgentPath() {
			if (!navAgent.enabled) {
				return false;
			}
			if (!navAgent.hasPath) {
				return false;
			}
			if (navAgent.desiredVelocity == Vector3.zero) {
				return false;
			}
			return true;
		}

		#endregion

		#region Private Utilities

		private float DifferenceBetweenGivenFacingAndCurrentFacing(Vector2 xzVector) {
			float targetTheta = Mathf.Atan2(xzVector.y, xzVector.x);
			var currentVector = transform.forward.JustXZ().normalized;
			float currentTheta = Mathf.Atan2(currentVector.y, currentVector.x);
			return MeatPuppetToolKit.AngleBetweenThetas(targetTheta, currentTheta);
		}

		/// <summary>
		/// Calculates and returns the unit vector that this puppet should go in order to grow closer to the target
		/// </summary>
		private Vector3 GetMoveVector() {
			if (!hasMoveTarget) {
				return Vector3.zero;
			}

			switch (moveInputType) {
				case InputType.Direction: {
						return moveTargetDirection;
					}
				default:
				case InputType.Point:
				case InputType.Transform: {

						var target = GetTargetPoint();

						Vector3 direction;

						if (UseNavAgentPath()) {
							direction = navAgent.desiredVelocity.DropY();

						}
						else {
							direction = (target - transform.position).DropY();
						}

						if (direction.sqrMagnitude > 1f) {
							direction.Normalize();
						}

						return direction;

					} // end of if inputType is Transform or Point
			}
		}

		private Vector3 GetLookAtDirection() {
			switch (facingInputType) {
				default:
				case InputType.Point: {
						var vector = LookAtPoint - transform.position;
						// TODO: Make sure the vector isn't zero
						// TODO: don't normalize it if it is already less than one
						return vector.normalized;
					}
				case InputType.Transform: {
						var vector = FacingTargetTransform.position - transform.position;
						// TODO: Make sure the vector isn't zero
						// TODO: don't normalize it if it is already less than one
						return vector.normalized;
					}
				case InputType.Direction:
					return facingTargetDirection;
			}
		}

		/// <summary>
		/// Cause the unit to move in the given moveVector, using acceleration.
		/// </summary>
		private void MoveInDirection(Vector3 moveVector) {
			// combine the direction/moveVector with the avoidance vector so that we can move towards 
			// our target while maintaining a certain distance to nearby units.

			var idealVelocity = moveVector * GetTopSpeed();

			// modify the speed
			// idealVelocity *= parentPuppet.movementSettings.moveSpeedModifier;
			idealVelocity *= Mathf.Min( parentPuppet.Legs.GetSlopeSpeedModifier(), parentPuppet.Locomotion.GetSlopeSpeedModifier());

			var currentVelocity = parentPuppet.Rigidbody.velocity.DropY();

			// two vectors; the direction we ARE going and the direction we WANT to go.
			// if they are close to being equal -> the difference between them will be close to zero.
			// if they are perpendicular or opposite -> the difference will be %100 or %200.
			// use the difference so that the acceleration/force has little or no effect
			// if the unit is already going in the right direction, at top speed.
			// And the acceleration will have a large effect on changing the unit's velocity
			// if the unit is going in the wrong direction.
			var idealDifference = idealVelocity - currentVelocity;
			idealDifference.y = 0;
			parentPuppet.Rigidbody.AddForce(idealDifference * parentPuppet.movementSettings.moveAcceleration, ForceMode.Acceleration);

		}

		/// <summary>
		/// Bring the unit to a stop, using acceleration.
		/// </summary>
		private void ApplyBrakeForce() {
			var idealDifference = -parentPuppet.Rigidbody.velocity;
			parentPuppet.Rigidbody.AddForce(idealDifference * parentPuppet.movementSettings.brakeFactor, ForceMode.Acceleration);

		}


		/// <summary>
		/// Cause the unit to rotate toward the given vector, around the y-axis, using acceleration.
		/// </summary>
		/// <param name="xzVector"></param>
		private void TurnToDirection(Vector2 xzVector) {
			float difference = DifferenceBetweenGivenFacingAndCurrentFacing(xzVector);

			if (parentPuppet.Rigidbody.isKinematic) {
				if (difference > parentPuppet.movementSettings.facingTolerance) {
					difference = Mathf.Max(difference, 1);
				}
				else if (difference < -parentPuppet.movementSettings.facingTolerance) {
					difference = Mathf.Min(difference, -1);
				}
				parentPuppet.Rigidbody.MoveRotation(parentPuppet.Rigidbody.rotation * 
					Quaternion.Euler(0, difference * parentPuppet.movementSettings.preciseTurnForce * Time.deltaTime, 0));
			}
			else {

				// add turning force
				parentPuppet.Rigidbody.AddTorque(0, difference * parentPuppet.movementSettings.turnForce, 0, ForceMode.Acceleration);

				// apply turning dampening (to prevent elasticity)
				float changeInDifference = difference - lastTurnDifference;

				var damping = parentPuppet.movementSettings.turnDampening;
				parentPuppet.Rigidbody.AddTorque(0, changeInDifference * damping, 0, ForceMode.Acceleration);

			}

			lastTurnDifference = difference;

			// Prevent the body from rotating on any other axis
			parentPuppet.Rigidbody.angularVelocity = new Vector3(0, parentPuppet.Rigidbody.angularVelocity.y, 0);

			// keep the body pointing straight up and down
			if (parentPuppet.Rigidbody.transform.rotation.eulerAngles.x != 0 || parentPuppet.Rigidbody.transform.rotation.eulerAngles.z != 0) {
				parentPuppet.Rigidbody.transform.rotation = Quaternion.Euler(0, parentPuppet.Rigidbody.transform.rotation.eulerAngles.y, 0);
			}
		}

		private Vector3 GetVectorToAvoidStaticObstacles(Vector3 moveVector) {
			var direction = parentPuppet.Rigidbody.velocity;
			var layer = MeatPuppetManager.Instance.staticLayer;
			if (CastForObstacle(direction, layer, out var raycastHit)) {

				var perpendicularVector = Quaternion.Euler(0, 90, 0) * moveVector;
				if (!CastForObstacle(perpendicularVector, layer, out var raycastHitRight, bodyRadiusMod: 1f)) {

					var avoidVector = perpendicularVector * parentPuppet.movementSettings.avoidanceForce;
					return (moveVector + avoidVector) * 0.5f;
				} else {
					perpendicularVector = Quaternion.Euler(0, -90, 0) * moveVector;
					if (!CastForObstacle(perpendicularVector, layer, out var raycastHitLeft, bodyRadiusMod: 1f)) {

						var avoidVector = perpendicularVector * parentPuppet.movementSettings.avoidanceForce;
						return (moveVector + avoidVector) * 0.5f;
					}
				}
			}

			return moveVector;
		}

		private bool CastForObstacle(Vector3 direction, LayerMask layerMask, out RaycastHit raycastHit, float bodyRadiusMod = 1.5f) {
			var point1 = parentPuppet.GetPelvisPoint();
			var point2 = parentPuppet.GetHeadPoint();
			var radius = parentPuppet.bodyDimensions.bodyRadius * bodyRadiusMod;
			var distance = parentPuppet.bodyDimensions.bodyHeight * 0.5f;
			var obstacles = Physics.CapsuleCastAll(point1, point2, radius, direction, distance, layerMask);

			if (obstacles.Length == 0) {
				raycastHit = new RaycastHit();
				return false;
			}

			float closestCollision = -1f;
			raycastHit = obstacles[0];

			foreach (var obstacle in obstacles) {
				if (obstacle.collider == parentPuppet.Collider) {
					continue;
				}

				if (!obstacle.collider.gameObject.isStatic) {
					continue;
				}

				var differenceVector = parentPuppet.transform.position - obstacle.point;
				if (closestCollision<0 || differenceVector.sqrMagnitude< closestCollision) {
					raycastHit = obstacle;
					closestCollision = differenceVector.sqrMagnitude;
				}
			}

			return closestCollision >= 0;
		}

		#endregion

	}

}