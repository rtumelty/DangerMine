﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class ActivateTargets : MonoBehaviour {
	void Awake() {
		collider2D.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameEntity entity = other.GetComponent<GameEntity>();

		if (entity is Ally) return;

		if (entity != null) {
			entity.State = GameEntity.EntityState.Active;
		}
		else if (other.GetComponent<Projectile>() != null) other.gameObject.SetActive(false);
	}
}
