using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

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

	protected bool blocked;

	public bool IsBlocked {
		get {
			return blocked;
		}
	}

	protected bool dying = false;
	protected bool canAttack = true;
	protected bool attacking = false;
	protected List<GameEntity> attackTargets = null;
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
		attackTargets = new List<GameEntity>();
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

		Spine.Skeleton skeleton = mySpineMultiSkeleton.skeleton.skeleton;

		if (skeleton != null) {
			skeleton.R = 1;
			skeleton.G = 1;
			skeleton.B = 1;
			skeleton.A = 1;
		}

	}
	
	protected override void OnDisable() {
		base.OnDisable();
	}

	protected virtual void Update () {
		if (GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0))) 
			Blocked(GridManager.Instance.EntitiesAt(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0)));

		if (blocked) {		
			Vector3 coordsInV3 = worldGridCoords.ToVector3(transform.position.z);
			if (transform.position.x != coordsInV3.x) {
				Vector3 position = transform.position;
				position += new Vector3((coordsInV3.x - transform.position.x) * currentMoveSpeed * Time.deltaTime, 0, 0);
				transform.position = position;
			}
			if (!GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0)))
				Unblocked();
		} else {
			
			Vector3 position = transform.position;
			position += new Vector3(currentMoveSpeed * moveDirection * Time.deltaTime, 0, 0);
			transform.position = position;
			worldGridCoords = position as GridCoordinate;

			if (_allegiance == Allegiance.Ally) {
				if (worldGridCoords.x >= (CameraController.GridCoords.x + 1.5)) {
					currentMoveSpeed = CameraController.MoveSpeed;
				}
				else {
					currentMoveSpeed = defaultMoveSpeed;
				}
			}
		}
	}
	
	public virtual void Blocked(List<GameEntity> targets) { 
		if (!blocked) {
			blocked = true;
			/*
			if (target is Character) {
				Character character = target as Character;
				currentMoveSpeed = character.CurrentMoveSpeed;
			}
			*/
		}
	}

	public virtual void Unblocked() {
		blocked = false; 

		currentMoveSpeed = defaultMoveSpeed;
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
	}

	public virtual void UpdateTargets(List<GameEntity> targets) {
		attackTargets = targets;

		if (attacking && attackTargets.Count == 0) {

			attacking = false;
		} else if (!attacking && attackTargets.Count > 0 && canAttack) {
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
			}
		}

		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
	}

	protected override void Hit(Character character) {
		if (character.allegiance != allegiance) {
			/* DPS approach
			currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength / character.AttackSpeed * Time.deltaTime), 0, 9999);
			 */
			currentHealth = Mathf.Clamp (currentHealth - character.AttackStrength, 0, 9999);

			if (currentHealth == 0 && !dying) {
				Die ();
			}
			else 
				StartCoroutine(TintSkeleton());
		}
	}

	IEnumerator TintSkeleton() {
		Spine.Skeleton skeleton = mySpineMultiSkeleton.skeleton.skeleton;
		
		float initialR = skeleton.R;
		float initialG = skeleton.G;
		float initialB = skeleton.B;
		float initialA = skeleton.A;

		skeleton.R = 255;
		skeleton.G = 255;
		skeleton.B = 255;
		skeleton.A = 255;

		yield return new WaitForSeconds(.2f);

		skeleton.R = initialR;
		skeleton.G = initialG;
		skeleton.B = initialB;
		skeleton.A = initialA;
	}
	
	protected override void Die() {
		attacking = false;
		dying = true;
		mySpineMultiSkeleton.SetAnimation (deathAnimation, 0, false);
		mySpineMultiSkeleton.skeleton.state.Complete += DisableAfterAnimation;
	}

	protected void DisableAfterAnimation(Spine.AnimationState state, int track, int loopCount = 0) {
		mySpineMultiSkeleton.skeleton.state.Complete -= DisableAfterAnimation;
		gameObject.SetActive (false);
		}
}
