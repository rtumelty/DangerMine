using UnityEngine;
using System.Collections;

public class ChaseCollider : MonoBehaviour {
	static ChaseCollider _instance;

	public static ChaseCollider Instance {
		get {
			return _instance;
		}
	}

	[SerializeField] float advanceDistance = .5f;
	[SerializeField] float advanceTime = 2f;

	public int LeadingEdge {
		get {
			return Mathf.RoundToInt(transform.position.x);
		}
	}

	void Awake() {
		_instance = this;
	}

	void OnTriggerEnter2D(Collider2D other) {
		Character character = other.gameObject.GetComponent<Character>();

		if (character == null) return;
		else if (character is Enemy) {
			Enemy enemy = character as Enemy;
			if (!enemy.Chasing)
				enemy.Chase();
		}
	}
}
