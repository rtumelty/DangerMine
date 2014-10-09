using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class AttackHitbox : MonoBehaviour {
	BoxCollider2D hitbox;

	LayerMask layerToTarget = 0;
	LayerMask characterLayer = 0;

	void Awake() {
		hitbox = GetComponent<BoxCollider2D> () as BoxCollider2D;
		hitbox.isTrigger = true;

		Character character = transform.parent.GetComponent<Character> ();
		if (character.allegiance == Allegiance.Ally) {
			characterLayer = LayerMask.NameToLayer ("Ally");
			layerToTarget = LayerMask.NameToLayer ("Enemy");
		}
		else {
			characterLayer = LayerMask.NameToLayer ("Enemy");
			layerToTarget = LayerMask.NameToLayer ("Ally");
		}
	}

	public void SetSize(Vector2 size, int direction) {
		hitbox.size = size;
		hitbox.center = new Vector2 (size.x / 2 * direction, 0);
	}

	protected virtual void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == layerToTarget)
			transform.SendMessageUpwards ("InAttackRange", collider.gameObject);
	}
	protected virtual void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.layer == layerToTarget)
			transform.SendMessageUpwards ("LeftAttackRange", collider.gameObject);
	}
}
