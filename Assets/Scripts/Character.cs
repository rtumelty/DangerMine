using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Character : GameEntity {

	private int moveDirection = 1;

	[SerializeField] protected float moveSpeed = 1.5f;
	[SerializeField] protected int attackStrength = 10;
	[SerializeField] protected float attackSpeed = 0.5f;
	[SerializeField] protected float attackRange = 1f;

	bool attacking;
	GameObject attackTarget = null;
	AttackHitbox hitbox = null;
	float currentHealth;

	// Use this for initialization
	void Start () {
		if (_allegiance == Allegiance.Ally)
			moveDirection = 1;
		else
			moveDirection = -1;

		rigidbody2D.isKinematic = true;

		hitbox = GetComponentInChildren<AttackHitbox> () as AttackHitbox;
		if (hitbox != null)
			hitbox.SetSize(new Vector2(attackRange, 1), moveDirection);
	}

	void OnEnable() {
		currentHealth = health;
	}

	void FixedUpdate () {
		if (attacking) {		// Fix layer checking
		} else {
			Vector3 position = transform.position;
			position += new Vector3(moveSpeed * moveDirection * Time.fixedDeltaTime, 0, 0);
			transform.position = position;
		}
	}
	
	public void InAttackRange(Object target) { 
		if (!attacking) {
			attacking = true;
			attackTarget = target as GameObject;
			StartCoroutine ("Attack");
		}
	}
	public void LeftAttackRange(Object target) {
		attacking = false; 
		attackTarget = null;
		StopCoroutine ("Attack");
	}

	IEnumerator Attack() {
		while (attacking) {
			attackTarget.SendMessage("Hit", attackStrength);
			yield return new WaitForSeconds(attackSpeed);
		}
	}

	void Hit(int attackStr) {
		currentHealth = Mathf.Clamp (currentHealth - attackStr, 0, 9999);
		if (currentHealth == 0)
						Die ();
	}

	void Die() {
		gameObject.SetActive (false);
	}
}
