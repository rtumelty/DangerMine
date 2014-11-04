using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {
	AllyRanged source;

	void Awake() {
		collider2D.isTrigger = true;
		rigidbody2D.gravityScale = 0;
	}

	public void Init(AllyRanged owner) {
		source = owner;

		rigidbody2D.velocity = new Vector2(owner.ShotDirection * owner.ProjectileSpeed, 0);
	}

	void OnDisable() {
		rigidbody2D.velocity = Vector2.zero;
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameEntity entity = other.GetComponent<GameEntity>();

		if (entity != null) {
			if (entity.allegiance == source.allegiance)
				return;
			else {
				entity.SendMessage("Hit", source);
				gameObject.SetActive(false);
			}
		}
	}
}
