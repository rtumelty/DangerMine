using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AttackZone : MonoBehaviour {
	BoxCollider2D attackRange;
	Character owner;

	List<GameEntity> targets;
	public List<GameEntity> Targets {
		get {
			return targets;
		}
	}

 

	void Awake() {
		targets = new List<GameEntity>();

		owner = transform.GetComponentInParent<Character>();
	}

	public void SetSize(Vector2 size, int direction) {
		if (attackRange == null) {
			attackRange = gameObject.GetComponent<BoxCollider2D>();
			attackRange.isTrigger = true;
		}

		attackRange.size = size;
		attackRange.center = new Vector2(size.x / 2 * direction, 0);
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameEntity entity = other.GetComponentInChildren<GameEntity>();
		
		if (entity != null) {
			if (entity.allegiance != owner.allegiance) {
				targets.Add(entity);
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		GameEntity entity = other.GetComponentInChildren<GameEntity>();
		
		if (entity != null) {
			if (targets.Contains(entity)) {
				targets.Remove(entity);
			}
		}
	}

	void Update() {
		if (!owner.enabled) return;

		List<Collider2D> collidersInRange = new List<Collider2D>( Physics2D.OverlapAreaAll(attackRange.bounds.min, attackRange.bounds.max));
		List<GameEntity> activeTargets = new List<GameEntity>();

		for (int i = 0; i < targets.Count;i++) {
			GameEntity entity = targets[i];

			if (entity.gameObject.activeSelf == false || !collidersInRange.Contains(entity.collider2D)) {
				targets.Remove(entity);
				break;
			} else if ( entity.Targetable )
				activeTargets.Add(entity);
		}

		owner.UpdateTargets(activeTargets.ToArray());
	}
}
