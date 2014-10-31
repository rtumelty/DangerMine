using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
			attackRange = gameObject.AddComponent<BoxCollider2D>();
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
		for (int i = 0; i < targets.Count;i++) {
			GameEntity entity = targets[i];
			if (entity.gameObject.activeSelf == false) {
				targets.Remove(entity);
			}
		}

		owner.UpdateTargets(targets.ToArray());

	}
}
