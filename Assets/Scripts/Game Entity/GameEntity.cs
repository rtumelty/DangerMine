using UnityEngine;
using System.Collections;

public enum Allegiance {
	Ally,
	Enemy,
	Neutral
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class GameEntity : MonoBehaviour {
	
	[SerializeField] protected Allegiance _allegiance;
	[SerializeField] protected int health = 10;
	protected float currentHealth;
	
	public Allegiance allegiance {
		get {
			return _allegiance;
		}
	}

	protected virtual void Awake() {
		rigidbody2D.gravityScale = 0;
		rigidbody2D.isKinematic = true;
	}

	protected virtual void OnEnable() {
		GridManager.Instance.RegisterEntity(this);
		
		currentHealth = health;
	}
	
	protected virtual void OnDisable() {
		GridManager.Instance.UnregisterEntity(this);
	}

	protected virtual void Hit(Character character) {
		Debug.Log ("Attacked, taking " + character.AttackStrength + " damage");
		currentHealth = Mathf.Clamp (currentHealth - character.AttackStrength, 0, 9999);
		if (currentHealth == 0)
			Die ();
	}
	
	protected virtual void Die() {
		gameObject.SetActive (false);
	}
}
