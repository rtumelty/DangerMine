using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {
	[SerializeField] float projectileSpeed;
	Character source;

	void Awake() {
		collider2D.isTrigger = true;
		rigidbody2D.gravityScale = 0;
	}

	public void Init(Character owner, Vector2 direction) {
		source = owner;

		rigidbody2D.velocity = direction * projectileSpeed;
	}

	void OnDisable() {
		rigidbody2D.velocity = Vector2.zero;
	}

	void OnTriggerEnter2D(Collider2D other) {
		DestructibleEntity entity = other.GetComponent<DestructibleEntity>();

		if (entity != null) {
			if ((1 << entity.gameObject.layer & source.TargetingMask) != 0)
				entity.TakeDamage(source.AttackStrength, source);
			gameObject.SetActive(false);
		}
	}
}
