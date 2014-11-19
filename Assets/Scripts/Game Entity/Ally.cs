using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ally : Character {
#if UNITY_EDITOR || UNITY_STANDALONE
	private static float laneSwitchThreshold = 1f;
	#elif UNITY_ANDROID
	private static float laneSwitchThreshold = 10f;
#endif

	[SerializeField] bool targetableOnStart = false;
	protected bool cantMove = false;
	protected bool moving = false;

	private static List<Ally> activeAllies;

	public static int ActiveAllies {
		get {
			return activeAllies.Count;
		}
	}

	protected GridCoordinate targetGridPosition;

	protected override void Awake() {
		if (activeAllies == null) {
			activeAllies = new List<Ally>();
			foreach (Ally ally in FindObjectsOfType<Ally>()) {
				if (!activeAllies.Contains(ally))
					activeAllies.Add(ally);
			}
		}

		_allegiance = Allegiance.Ally;

		moveDirection = 1;

		base.Awake ();
	}

	protected override void OnEnable() {
		base.OnEnable ();

		if (targetableOnStart) {
			targetable = true;
			GridManager.Instance.RegisterEntity(GridManager.Grid.ScreenGrid, this);
		}
		
		if (!activeAllies.Contains(this))
			activeAllies.Add(this);
		
		targetGridPosition = GridManager.WorldToScreenGridCoords(transform.position);
	}

	protected override void OnDisable() {
		if (activeAllies.Contains(this))
			activeAllies.Remove(this);

		LevelManager.Instance.CheckEndCondition();
		base.OnDisable ();
	}

	protected override void Update() {
		if (GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0)) && ! cantMove) 
			Blocked(GridManager.Instance.EntityAt(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0)));

		Vector3 targetPosition;

		if (moving) {
			targetPosition = GridManager.ScreenCoordsToWorldPosition(targetGridPosition);
			if ((transform.position - targetPosition).magnitude < .1) moving = false;
		}
		else if (cantMove) {
			Debug.Log("Can't move, staying at " + worldGridCoords);
			targetPosition = worldGridCoords.ToVector3(transform.position.z);
			targetGridPosition = GridManager.WorldToScreenGridCoords(worldGridCoords);

			if (!GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, worldGridCoords + new GridCoordinate(moveDirection, 0)))
				Unblocked();
		} else {
			targetPosition = GridManager.ScreenCoordsToWorldPosition(targetGridPosition);
		}

		Vector3 deltaPosition = targetPosition - transform.position;
		
		float maxSpeed = currentMoveSpeed * Time.deltaTime;
		deltaPosition = new Vector3(Mathf.Clamp(deltaPosition.x, -maxSpeed, maxSpeed), Mathf.Clamp(deltaPosition.y, -maxSpeed, maxSpeed), 0);
		transform.position += deltaPosition;
		worldGridCoords = transform.position as GridCoordinate;

	}

	public override void Blocked(GameEntity entity) {
		cantMove = true;
		targetGridPosition = GridManager.WorldToScreenGridCoords(transform.position);

		base.Blocked(entity);
	}

	public override void Unblocked() {
		cantMove = false;

		base.Unblocked();
	}

	public override void UpdateTargets(List<GameEntity> targets) {
		attackTargets.Clear();

		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] is Enemy) {
				attackTargets.Add(targets[i]);
			}
		}
		
		if (attacking && attackTargets.Count == 0) {
			attacking = false;
		} else if (!attacking && attackTargets.Count > 0 && canAttack) {
			cantMove = true;
			StartCoroutine("Attack");
		}
	}

	protected override void Die() {
		if (activeAllies.Contains(this))
			activeAllies.Remove(this);
		base.Die();
	}

	protected virtual void OnMouseDown () {
		if (dying) return;

		StartCoroutine (Drag ());
	}
	
	protected virtual IEnumerator Drag() {
		Debug.Log("Starting Drag");
		yield return new WaitForSeconds (Time.deltaTime);
		
		GridCoordinate moveTarget = screenGridCoords;
		GridCoordinate lastMoveTarget = screenGridCoords;

		while (CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE || CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
			lastMoveTarget = moveTarget;

			moveTarget = GridManager.WorldToScreenGridCoords(Camera.main.ScreenToWorldPoint(Input.mousePosition));
/*
			if (Mathf.Abs(moveTarget.x - screenGridCoords.x) > Mathf.Abs(moveTarget.y - screenGridCoords.y))
				moveTarget.y = screenGridCoords.y;
			else
				moveTarget.x = screenGridCoords.x;
*/			
			moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
			moveTarget.y = Mathf.Clamp(moveTarget.y, GridManager.minY, GridManager.maxY);

			LaneHighlight.Instance.UpdatePosition(GridManager.ScreenCoordsToWorldPosition(moveTarget));

			if (GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, moveTarget)) {
				GameEntity entityAtCoords = GridManager.Instance.EntityAt(GridManager.Grid.ScreenGrid, moveTarget);
				if (!entityAtCoords is Ally) {
					Debug.Log("Drag hit entity " + entityAtCoords + ", ending input.");
					UpdateTargetCoordinates(lastMoveTarget);
					yield break;
				} else if (entityAtCoords != this) {
					Debug.Log("Drag hit entity " + entityAtCoords + ", ending input.");
					UpdateTargetCoordinates(moveTarget);
					yield break;
				}
			}

			yield return new WaitForSeconds (Time.deltaTime);
		}

		LaneHighlight.Instance.Hide();

		if (dying) yield break;
		
		Debug.Log("Drag ended");
		UpdateTargetCoordinates(moveTarget);
	}

	public void UpdateTargetCoordinates(GridCoordinate coord) {
		GridCoordinate relativeDistance = coord - screenGridCoords;

		GridCoordinate newTarget = screenGridCoords + relativeDistance;
		moving = true;

		targetGridPosition = newTarget;
	}
}
