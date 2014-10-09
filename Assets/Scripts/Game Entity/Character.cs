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
	
	public SpineMultiSkeleton mySpineMultiSkeleton;

	public int AttackStrength {
		get {
			return attackStrength;
		}
	}

	bool blocked;
	GameEntity attackTarget = null;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();

		if (_allegiance == Allegiance.Ally)
			moveDirection = 1;
		else
			moveDirection = -1;

		/*mySpineMultiSkeleton = transform.GetComponentInChildren <SpineMultiSkeleton>() as SpineMultiSkeleton;
		if(mySpineMultiSkeleton == null)
		{
			Debug.LogError("Missing <SpineMultiSkeleton> from characters child 'Skeleton'");
		}*/

	}

	protected override void OnEnable() {
		base.OnEnable();
		mySpineMultiSkeleton.SetAnimation ("miner_01_walk_side", 1);
	}

	void FixedUpdate () {
		if (blocked) {		
			Vector3 coordsInV3 = gridCoords.ToVector3(transform.position.z);
			if (transform.position.x != coordsInV3.x) {
				Vector3 position = transform.position;
				position += new Vector3((coordsInV3.x - transform.position.x) * moveSpeed * Time.fixedDeltaTime, 0, 0);
				transform.position = position;
			}
			if (!GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0)))
				Unblocked();
		} else {
			Vector3 position = transform.position;
			position += new Vector3(moveSpeed * moveDirection * Time.fixedDeltaTime, 0, 0);
			transform.position = position;
			gridCoords = position as GridCoordinate;
			if (GridManager.Instance.IsOccupied(gridCoords + new GridCoordinate(moveDirection, 0)))
			    Blocked(GridManager.Instance.EntityAt(gridCoords + new GridCoordinate(moveDirection, 0)));
		}
	}
	
	public void Blocked(GameEntity target) { 
		if (!blocked) {
			blocked = true;
			attackTarget = target;
			StartCoroutine ("Attack");
		}
	}
	public void Unblocked() {
		blocked = false; 
		attackTarget = null;
		StopCoroutine ("Attack");
	}

	protected virtual IEnumerator Attack() {

		mySpineMultiSkeleton.SetAnimation ("miner_01_drilling_jump_front", 1);
		while (blocked) {
			attackTarget.SendMessage("Hit", this);
			Debug.Log("Attacking " + attackTarget);
			yield return new WaitForSeconds(attackSpeed);
		}
		mySpineMultiSkeleton.SetAnimation ("miner_01_walk_side", 1);
	}
}
