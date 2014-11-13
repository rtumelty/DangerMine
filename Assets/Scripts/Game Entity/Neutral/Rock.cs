using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Rock : GameEntity {
	[SerializeField] bool dropsGold = false;

	[SerializeField] string goldPrefabPoolId = "gold";

	Character destroyedBy;

	protected override void Awake() {
		base.Awake ();

		_allegiance = Allegiance.Neutral;
	}

	protected override void Hit(Character character) {
		if (!((character is Enemy) || (character is Miner) || (character is Boz))) return;

		//currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength * Time.deltaTime), 0, 9999);

		currentHealth = Mathf.Clamp (currentHealth - character.AttackStrength, 0, 9999);

		if (currentHealth == 0) {
			destroyedBy = character;
			Die ();
		}
		else {
			foreach (Renderer r in renderers) {
				if (r is SpriteRenderer)
					StartCoroutine(TintSpriteRenderer(r as SpriteRenderer));
			}
		}
	}

	protected override void OnDisable() {
		if (destroyedBy == null || destroyedBy is Enemy);
		else if (dropsGold) {
			PrefabPool.GetPool(goldPrefabPoolId).Spawn(transform.position);
		}

		destroyedBy = null;
		base.OnDisable ();
	}
}
