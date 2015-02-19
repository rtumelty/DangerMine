using UnityEngine;
using System.Collections;

public class Enemy : Character {
	public enum EnemyMoveState {
		Default,
		Blocked,
		Chase
	}

	protected bool chasing = false;
	public float followDistance;

	protected EnemyMoveState moveState = EnemyMoveState.Default;

	public EnemyMoveState MoveState {
		get {
			return moveState;
		}
		set {
			if (value == EnemyMoveState.Default) transform.rotation = Quaternion.Euler(0, 0, 0);
			else if (value == EnemyMoveState.Chase) {
				LogMessage("Entering Chase state");
				transform.rotation = Quaternion.Euler(0, -180, 0);
				attackHitbox.Resize();

				if (!chasing) {
					LogMessage("Registering with LaneManager");
					LaneManager.Instance.JoinRow(this, ScreenCoords.y);
					chasing = true;
				}
			}

			moveState = value;
		}
	}

	protected override IEnumerator Dying(GameEntity cause) {
		if (chasing)
			LaneManager.Instance.RemoveFromRow(this, ScreenCoords.y);

		yield return StartCoroutine(base.Dying(cause));

	}

	protected override void OnEnable() {
		base.OnEnable();

		moveState = EnemyMoveState.Default;
		chasing = false;
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		DestructibleEntity entity = collision.gameObject.GetComponent<DestructibleEntity>();

		if (entity == null);
		else if (entity is Enemy) {
			Enemy enemy = entity as Enemy;
			if (enemy.MoveState == EnemyMoveState.Chase)
				MoveState = EnemyMoveState.Chase;
		}

		base.OnCollisionEnter2D(collision);
	}

	protected override void Update() {
		switch (moveState) {
		case EnemyMoveState.Default:
			LogMessage("Transform coords: " + transform.position + ", World coords: " + WorldCoords, DebugLevel.Warning);
			LogMessage("New target: " +(WorldCoords + new GridCoordinate(-1, 0)).ToVector3(), DebugLevel.Error);
			targetPosition = (WorldCoords + new GridCoordinate(-1, 0)).ToVector3();
			break;
		case EnemyMoveState.Chase:
			Debug.Log("Screen coords: " + ScreenCoords + ", target: " + new GridCoordinate(-followDistance, ScreenCoords.y));
			targetPosition = GridManager.ScreenCoordsToWorldPosition(new GridCoordinate(-followDistance, ScreenCoords.y));
			AttackMultiplier = 2f * (LaneManager.MaxFollowDistance - followDistance);
			break;
		case EnemyMoveState.Blocked:
			targetPosition = WorldCoords.ToVector3();
			break;
		}

		//targetPosition.y = Mathf.Clamp(targetPosition.y, GridManager.minY, GridManager.maxY);

		base.Update();
	}

	protected override void Move() {
		/*
		Vector2 velocity = rigidbody2D.velocity;
		velocity.y = 0;
		rigidbody2D.velocity = velocity;
		*/
		LogMessage("Move target: " + targetPosition + ", difference: " + (targetPosition - transform.position));

		Vector2 targetVelocity = Vector2.ClampMagnitude(targetPosition - transform.position, maxMoveSpeed);
		Vector2 velocityChange = targetVelocity - rigidbody2D.velocity;
		
		velocityChange = Vector2.ClampMagnitude(velocityChange, maxVelocityChange);

		if (MoveState == EnemyMoveState.Chase)
		{
			velocityChange *= 3;
		}
		rigidbody2D.AddForce(velocityChange * rigidbody2D.mass, ForceMode2D.Force);
	}
}
