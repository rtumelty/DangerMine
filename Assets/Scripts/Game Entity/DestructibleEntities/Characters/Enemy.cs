using UnityEngine;
using System.Collections;

public class Enemy : Character {
	public enum EnemyMoveState {
		Default,
		Blocked,
		Chase
	}

	protected EnemyMoveState moveState = EnemyMoveState.Default;

	public EnemyMoveState MoveState {
		get {
			return moveState;
		}
		set {
			if (value == EnemyMoveState.Default) transform.rotation = Quaternion.Euler(0, 0, 0);
			else if (value == EnemyMoveState.Chase) transform.rotation = Quaternion.Euler(0, -180, 0);

			moveState = value;
		}
	}

	protected override void OnEnable() {
		base.OnEnable();

		moveState = EnemyMoveState.Default;
	}

	protected override void Update() {
		switch (moveState) {
		case EnemyMoveState.Default:
			targetPosition = (WorldCoords + new GridCoordinate(-1, 0)).ToVector3();
			break;
		case EnemyMoveState.Chase:
			targetPosition = (WorldCoords + new GridCoordinate(1, 0)).ToVector3();
			break;
		case EnemyMoveState.Blocked:
			targetPosition = WorldCoords.ToVector3();
			break;
		}

		base.Update();
	}
}
