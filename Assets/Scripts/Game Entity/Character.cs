using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class Character : GameEntity {

	protected int moveDirection = 1;

	[SerializeField] protected float defaultMoveSpeed = 1.5f;
	[SerializeField] protected int attackStrength = 10;
	[SerializeField] protected float attackSpeed = 0.5f;
	[SerializeField] protected Vector2 attackRange = new Vector2(1.5f, .8f);
	
	public SpineMultiSkeleton mySpineMultiSkeleton;
	[SerializeField] protected string attackAnimation;
	[SerializeField] protected string walkAnimation;
	[SerializeField] protected string deathAnimation;

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	protected bool ignoreUpdate = false;

	public bool IgnoreUpdate {
		get {
			return ignoreUpdate;
		}
		set {
			ignoreUpdate = value;
		}
	}

	protected bool blocked;

	public bool IsBlocked {
		get {
			return blocked;
		}
	}

	protected bool dying = false;
	bool attacking = false;
	GameEntity[] attackTargets = null;
	protected float currentMoveSpeed;
	CameraController cameraController;
	protected AttackZone attackZone;

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
		AttackZone attackZone = GetComponentInChildren<AttackZone>();
		attackZone.SetSize(attackRange, moveDirection);
	}
	
	protected override void OnEnable() {
		base.OnEnable();

		if (_allegiance == Allegiance.Ally)
			moveDirection = 1;
		else
			moveDirection = -1;

		transform.rotation = Quaternion.Euler(Vector3.zero);
		currentMoveSpeed = defaultMoveSpeed;
		dying = false;
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
	}
	
	protected override void OnDisable() {
		base.OnDisable();
	}

	protected virtual void Update () {
		if (ignoreUpdate)
			return;
		
		if (GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0))) 
			Blocked(GridManager.Instance.EntityAt(gridCoords + new GridCoordinate(moveDirection, 0)));

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
			position += new Vector3(currentMoveSpeed * moveDirection * Time.deltaTime, 0, 0);
			transform.position = position;
			gridCoords = position as GridCoordinate;

			if (_allegiance == Allegiance.Ally) {
				if (gridCoords.x >= (CameraController.GridCoords.x + 1.5)) {
					currentMoveSpeed = CameraController.MoveSpeed;
				}
				else {
					currentMoveSpeed = defaultMoveSpeed;
				}
			}
		}
	}
	
	public virtual void Blocked(GameEntity target) { 
		if (!blocked) {
			blocked = true;

			if (target.allegiance != _allegiance) {
				//attackTarget = target;
				//StartCoroutine ("Attack");
			} else if (target is Character) {
				Character character = target as Character;
				currentMoveSpeed = character.CurrentMoveSpeed;
			}
		}
	}

	public void Unblocked() {
		blocked = false; 
		//attackTarget = null;
		currentMoveSpeed = defaultMoveSpeed;
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
		//StopCoroutine ("Attack");
	}

	public void UpdateTargets(GameEntity[] targets) {
		attackTargets = targets;

		Debug.LogWarning(attackTargets.Length);
		if (attacking && targets.Length == 0) {
			attacking = false;
		} else if (!attacking && targets.Length > 0) {
			StartCoroutine("Attack");
		}
	}

	protected virtual IEnumerator Attack() {
		attacking = true;
		mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);

		while (attacking) {
			if (mySpineMultiSkeleton.skeleton.state.GetCurrent(0) == null) mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);
			/* DPS approach
			yield return new WaitForSeconds(Time.deltaTime);
			*/
			yield return new WaitForSeconds(attackSpeed);

			foreach (GameEntity attackTarget in attackTargets) {
				attackTarget.SendMessage("Hit", this,SendMessageOptions.DontRequireReceiver);
				Debug.Log(gameObject + " attacking " + attackTarget);
			}
		}
	}

	protected override void Hit(Character character) {
		if (character.allegiance != allegiance) {
			/* DPS approach
			currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength / character.AttackSpeed * Time.deltaTime), 0, 9999);
			 */
			currentHealth = Mathf.Clamp (currentHealth - character.AttackStrength, 0, 9999);
			Debug.Log(gameObject + " attacked by " + character);

			if (currentHealth == 0 && !dying) {
				Die ();
			}
		}
	}
	
	protected override void Die() {
		attacking = false;
		dying = true;
		mySpineMultiSkeleton.SetAnimation (deathAnimation, 0, false);
		StartCoroutine(DisableAfterAnimation(0));
	}

	protected IEnumerator DisableAfterAnimation(int layer) {
		mySpineMultiSkeleton.skeleton.state.GetCurrent (0).Loop = false;
		while (mySpineMultiSkeleton.skeleton.state.GetCurrent (layer) != null) {
			yield return new WaitForSeconds (Time.deltaTime);
		}
		gameObject.SetActive (false);
	}
}
