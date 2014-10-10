using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Rock : GameEntity {
	[SerializeField] bool dropsGold = false;

	[SerializeField] string goldPrefabPoolId = "gold";

	Character destroyedBy;

	protected override void Awake() {
		base.Awake ();

		_allegiance = Allegiance.Neutral;
	}

	protected override void Hit(Character character) {
		if (character is Miner || character is Beetle) {
			currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength * Time.deltaTime), 0, 9999);

			if (currentHealth == 0) {
				destroyedBy = character;
				Die ();
			}
		}
	}

	protected override void OnDisable() {
		if (destroyedBy == null)
			return;

		if (dropsGold && destroyedBy is Miner) {
			GameObject gold = PrefabPool.GetPool(goldPrefabPoolId).Spawn(transform.position);
		}

		destroyedBy = null;
	}
}
