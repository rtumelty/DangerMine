using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class AttackHitbox : MonoBehaviour {
	BoxCollider2D hitbox;

	/* Add Layer mask */

	void Awake() {
		hitbox = GetComponent<BoxCollider2D> () as BoxCollider2D;
		hitbox.isTrigger = true;
	}

	public void SetSize(Vector2 size, int direction) {
		hitbox.size = size;
		hitbox.center = new Vector2 (size.x / 2 * direction, 0);
	}

	void OnTriggerEnter2D(Collider2D collision) {
		transform.SendMessageUpwards ("InAttackRange", collision);
	}
	void OnTriggerExit2D(Collider2D collision) {
		transform.SendMessageUpwards ("LeftAttackRange", collision);
	}
}
