using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {
	
	private static List<Enemy> activeEnemies;
	private bool chasing = false;
	public float followDistance;

	public bool Chasing {
		get {
			return chasing;
		}
	}
	
	public static int ActiveEnemies {
		get {
			return activeEnemies.Count;
		}
	}
	

	protected override void Awake() {
		if (activeEnemies == null) {
			activeEnemies = new List<Enemy>();

			foreach (Enemy enemy in FindObjectsOfType<Enemy>()) 
			{
				if (!activeEnemies.Contains(enemy))
					activeEnemies.Add(enemy);
			}
		}

		_allegiance = Allegiance.Enemy;
		
		moveDirection = -1;
		
		base.Awake ();

		attackZone = GetComponentInChildren<AttackZone>();
	}
	
	protected override void OnEnable() {
		base.OnEnable ();

		transform.rotation = Quaternion.Euler(Vector3.zero);
		moveDirection = -1;
		chasing = false;
		followDistance = 0;
		
		if (!activeEnemies.Contains(this))
			activeEnemies.Add(this);
	}
	
	protected override void OnDisable() {
		base.OnDisable ();

		if (chasing) LaneManager.Instance.RemoveFromRow(this, gridCoords.y + 2);

		if (activeEnemies.Contains(this))
			activeEnemies.Remove(this);
	}

	protected override void Update () {
		if (!chasing) {
			base.Update();
			return;
		}
		else {
			GameEntity blockingObject = null;
			if (GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0))) {
				blockingObject = GridManager.Instance.EntityAt(gridCoords + new GridCoordinate(moveDirection, 0));
				Blocked(blockingObject);
			} else 
				Unblocked();

			if (blocked) {
				if (blockingObject is Enemy) {
					Enemy enemy = blockingObject as Enemy;

					if (enemy.IsBlocked) {
						Vector3 coordsInV3 = gridCoords.ToVector3(transform.position.z);
						if (transform.position.x != coordsInV3.x) {
							Vector3 position = transform.position;
							position += new Vector3((coordsInV3.x - transform.position.x) * currentMoveSpeed * Time.deltaTime, 0, 0);
							transform.position = position;
						}
					}
					else {
						Vector3 targetPosition = new Vector3(CameraController.Instance.transform.position.x - followDistance, 
						                                     transform.position.y, transform.position.z);
						targetPosition.x = Mathf.Clamp(targetPosition.x, transform.position.x - (currentMoveSpeed * 4 * Time.deltaTime), 
						                               transform.position.x + (currentMoveSpeed * 4 * Time.deltaTime));
						
						transform.position = Vector3.Lerp(transform.position, targetPosition, .9f);
					}
				}
				else {
					Vector3 coordsInV3 = gridCoords.ToVector3(transform.position.z);
					if (transform.position.x != coordsInV3.x) {
						Vector3 position = transform.position;
						position += new Vector3((coordsInV3.x - transform.position.x) * currentMoveSpeed * Time.deltaTime, 0, 0);
						transform.position = position;
					}
				}
			} else {
				Vector3 targetPosition = new Vector3(CameraController.Instance.transform.position.x - followDistance, 
				                                     transform.position.y, transform.position.z);
				targetPosition.x = Mathf.Clamp(targetPosition.x, transform.position.x - (currentMoveSpeed * 4 * Time.deltaTime), 
				                               transform.position.x + (currentMoveSpeed * 4 * Time.deltaTime));

				transform.position = Vector3.Lerp(transform.position, targetPosition, .9f);
			}
			gridCoords = transform.position as GridCoordinate;
		}
	}

	public override void Blocked(GameEntity target) { 
		if (target is Enemy && !chasing) {
			Enemy enemy = target as Enemy;
			if (enemy.Chasing) {
				Chase ();
				return;
			}
		}
	
		base.Blocked(target);
	}

	public override void UpdateTargets(List<GameEntity> targets) {
		attackTargets.Clear();
		
		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] is Ally || targets[i] is Rock) {
				attackTargets.Add(targets[i]);
			}
		}
		
		if (attacking && attackTargets.Count == 0) {
			attacking = false;
		} else if (!attacking && attackTargets.Count > 0 && canAttack) {
			StartCoroutine("Attack");
		}
	}
	
	public void Chase() {
		chasing = true;

		transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
		moveDirection = 1;
		currentMoveSpeed = CameraController.MoveSpeed;


		attackZone.SetSize(attackRange, 1);
		LaneManager.Instance.JoinRow(this, gridCoords.y + 2);
	}
}
