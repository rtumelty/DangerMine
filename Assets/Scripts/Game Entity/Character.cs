using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class Character : GameEntity {

	private int moveDirection = 1;

	[SerializeField] protected float defaultMoveSpeed = 1.5f;
	[SerializeField] protected int attackStrength = 10;
	[SerializeField] protected float attackSpeed = 0.5f;
	[SerializeField] protected float attackRange = 1f;
	
	public SpineMultiSkeleton mySpineMultiSkeleton;
	[SerializeField] string attackAnimation;
	[SerializeField] string walkAnimation;
	[SerializeField] string deathAnimation;

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	bool blocked;
	GameEntity attackTarget = null;
	float currentMoveSpeed;
	CameraController cameraController;

	public float CurrentMoveSpeed {
		get {
			return currentMoveSpeed;
		}
	}

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();

		if (_allegiance == Allegiance.Ally)
			moveDirection = 1;
		else
			moveDirection = -1;

		cameraController = Camera.main.gameObject.GetComponent<CameraController> ();

	}

	protected override void OnEnable() {
		base.OnEnable();
		currentMoveSpeed = defaultMoveSpeed;
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
	}

	void Update () {
		if (blocked) {		
			Vector3 coordsInV3 = gridCoords.ToVector3(transform.position.z);
			if (transform.position.x != coordsInV3.x) {
				Vector3 position = transform.position;
				position += new Vector3((coordsInV3.x - transform.position.x) * currentMoveSpeed * Time.deltaTime, 0, 0);
				transform.position = position;
			}
			if (!GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0)))
				Unblocked();
		} else {
			Vector3 position = transform.position;
			position += new Vector3(defaultMoveSpeed * moveDirection * Time.deltaTime, 0, 0);
			transform.position = position;
			gridCoords = position as GridCoordinate;
			if (GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0)))
			    Blocked(GridManager.Instance.EntityAt(gridCoords + new GridCoordinate(moveDirection, 0)));
		}
	}
	
	public void Blocked(GameEntity target) { 
		if (!blocked) {
			blocked = true;

			if (target.allegiance != _allegiance) {
				attackTarget = target;
				StartCoroutine ("Attack");
			} else if (target is Character) {
				Character character = target as Character;
				currentMoveSpeed = character.CurrentMoveSpeed;
			}
		}
	}
	public void Unblocked() {
		blocked = false; 
		attackTarget = null;
		currentMoveSpeed = defaultMoveSpeed;
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
		StopCoroutine ("Attack");
	}

	protected virtual IEnumerator Attack() {
		mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);

		while (blocked) {
			if (mySpineMultiSkeleton.skeleton.state.GetCurrent(0) == null) mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);
			yield return new WaitForSeconds(Time.deltaTime);
			attackTarget.SendMessage("Hit", this,SendMessageOptions.DontRequireReceiver);
			Debug.Log("Attacking " + attackTarget);
		}
	}

	protected virtual void Hit(Character character) {
		if (character.allegiance != allegiance) {
			currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength * Time.deltaTime), 0, 9999);
			if (currentHealth == 0)
				Die ();
		}
	}
	
	protected override void Die() {
		StopAllCoroutines ();
		Debug.Log (gameObject.name + "Dying! play " + deathAnimation);
		mySpineMultiSkeleton.SetAnimation (deathAnimation, 0, false);
		StartCoroutine(DisableAfterAnimation(0));
	}

	protected IEnumerator DisableAfterAnimation(int layer) {
		while (mySpineMultiSkeleton.skeleton.state.GetCurrent (layer) != null) {
			if (this is Beetle) {
				Debug.Log("waiting for animation to end");
			}
			yield return new WaitForSeconds (Time.deltaTime);
		}
		gameObject.SetActive (false);
	}
}
