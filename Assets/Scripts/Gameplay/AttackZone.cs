using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class AttackZone : MonoBehaviour {
	BoxCollider2D attackArea;
	Character owner;

	List<DestructibleEntity> ignoredTargets;
	LayerMask targetedLayers;

	List<DestructibleEntity> targets;
	public List<DestructibleEntity> Targets {
		get {
			return targets;
		}
	}

 

	void Awake() {
		targets = new List<DestructibleEntity>();

		owner = transform.GetComponentInParent<Character>();
		ignoredTargets = owner.IgnoredTargets;
		targetedLayers = owner.TargetingMask;

		Resize();
	}

	void OnEnable() {
		Resize();
	}

	void AddTarget(DestructibleEntity entity) {
		
		if (entity != null) {
			if (targets.Contains(entity)) return;
			
			if ((1 << entity.gameObject.layer & targetedLayers) == 0) return;
			
			foreach (DestructibleEntity ignoredTarget in ignoredTargets) {
				if (ignoredTarget.GetType() == entity.GetType())
					return;
			}
			
			targets.Add(entity);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		DestructibleEntity entity = other.GetComponentInChildren<DestructibleEntity>();
		
		AddTarget(entity);
	}
	
	void OnTriggerStay2D(Collider2D other) {
		DestructibleEntity entity = other.GetComponentInChildren<DestructibleEntity>();
		
		AddTarget(entity);
	}
	
	void OnTriggerExit2D(Collider2D other) {
		DestructibleEntity entity = other.GetComponentInChildren<DestructibleEntity>();
		
		if (entity != null) {
			if (targets.Contains(entity)) {
				targets.Remove(entity);
			}
		}
	}

	void FixedUpdate() {
		if (owner.State != DestructibleEntity.EntityState.Active) return;

		List<Collider2D> collidersInRange = new List<Collider2D>( Physics2D.OverlapAreaAll(attackArea.bounds.min, attackArea.bounds.max));
		List<DestructibleEntity> activeTargets = new List<DestructibleEntity>();

		for (int i = 0; i < targets.Count;i++) {
			DestructibleEntity entity = targets[i];

			if (entity.gameObject.activeSelf == false || entity.State != DestructibleEntity.EntityState.Active 
			    	|| !collidersInRange.Contains(entity.collider2D)) {
				targets.Remove(entity);
				break;
			} else if ( entity.State == DestructibleEntity.EntityState.Active )
				activeTargets.Add(entity);
		}

		owner.UpdateTargets(activeTargets);
	}

	public void Resize() {
		if (attackArea == null) {
			attackArea = gameObject.GetComponent<BoxCollider2D>();
			attackArea.isTrigger = true;
		}
		
		attackArea.size = owner.AttackRange;
		
		float xOffset = 0, yOffset = 0;
		if ((owner.AttackDirection & Character.AttackDirections.Forwards) != 0)
			xOffset ++;
		if ((owner.AttackDirection & Character.AttackDirections.Backwards) != 0)
			xOffset --;
		if ((owner.AttackDirection & Character.AttackDirections.Up) != 0)
			yOffset ++;
		if ((owner.AttackDirection & Character.AttackDirections.Down) != 0)
			xOffset --;
		
		attackArea.center = new Vector2(attackArea.size.x / 2 * xOffset, attackArea.size.y / 2 * yOffset);
	}
}
