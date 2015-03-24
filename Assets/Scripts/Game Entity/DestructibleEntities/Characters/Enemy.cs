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
			if (value == EnemyMoveState.Default) {
				moveDirection = new Vector3(-1, 0, 0);
			}
			else if (value == EnemyMoveState.Chase) {
				LogMessage("Entering Chase state");
				moveDirection = new Vector3(1, 0, 0);
				attackHitbox.Resize();

				if (!chasing) {
					LogMessage("Registering with LaneManager");
					LaneManager.Instance.JoinRow(this, ScreenCoords.y);
					chasing = true;
				}
				Debug.Log(transform.eulerAngles);
				transform.Rotate(new Vector3(0, 180));
				
				Debug.Log(transform.eulerAngles);
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

		MoveState = EnemyMoveState.Default;
		transform.rotation = default(Quaternion);
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
			break;
		case EnemyMoveState.Chase:
			AttackMultiplier = 2f * (LaneManager.MaxFollowDistance - followDistance);
			break;
		case EnemyMoveState.Blocked:
			break;
		}

		//targetPosition.y = Mathf.Clamp(targetPosition.y, GridManager.minY, GridManager.maxY);

		base.Update();
	}

	protected override void Move() {

		Vector2 targetVelocity;

		if (moveState == EnemyMoveState.Chase)
			targetVelocity = moveDirection *  CameraController.MoveSpeed;
		else 
			targetVelocity = moveDirection * maxMoveSpeed;
		Vector2 velocityChange = targetVelocity - rigidbody2D.velocity;
		
		velocityChange = Vector2.ClampMagnitude(velocityChange, maxVelocityChange);

		if (MoveState == EnemyMoveState.Chase)
		{
			velocityChange *= 3;
		}
		rigidbody2D.AddForce(velocityChange * rigidbody2D.mass, ForceMode2D.Force);
	}
}
