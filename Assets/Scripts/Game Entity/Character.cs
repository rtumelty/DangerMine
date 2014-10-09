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

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	bool attacking;
	GameObject attackTarget = null;
	AttackHitbox hitbox = null;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();

		if (_allegiance == Allegiance.Ally)
			moveDirection = 1;
		else
			moveDirection = -1;

		hitbox = GetComponentInChildren<AttackHitbox> () as AttackHitbox;
		if (hitbox != null)
			hitbox.SetSize(new Vector2(attackRange, 1), moveDirection);
	}

	protected override void OnEnable() {
		base.OnEnable();
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

	protected virtual IEnumerator Attack() {
		while (attacking) {
			attackTarget.SendMessage("Hit", this);
			Debug.Log("Attacking " + attackTarget);
			yield return new WaitForSeconds(attackSpeed);
		}
	}
}
