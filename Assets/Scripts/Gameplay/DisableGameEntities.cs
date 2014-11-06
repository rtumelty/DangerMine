using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DisableGameEntities : MonoBehaviour {
	void Awake() {
		collider2D.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameEntity entity = other.GetComponent<GameEntity>();

		if (entity != null) other.gameObject.SetActive(false);
	}
}
