using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Rock : GameEntity {
	[SerializeField] bool dropsGold = false;
	[SerializeField] int minGoldDropped = 10;
	[SerializeField] int maxGoldDropped = 15;

	[SerializeField] string goldPrefabPoolId = "gold";

	protected override void Awake() {
		base.Awake ();

		_allegiance = Allegiance.Neutral;
	}

	protected override void Hit(Character character) {
		if (character is Miner) {
			currentHealth = Mathf.Clamp (currentHealth - character.AttackStrength, 0, 9999);

			if (dropsGold) {
				GameObject gold = PrefabPool.GetPool(goldPrefabPoolId).Spawn(transform.position);
			}

			if (currentHealth == 0)
				Die ();
		}
	}
}
