#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class Ally : Character {
	protected static int activeAllies = 0;

	public static int ActiveAllies {
		get {
			return activeAllies;
		}
		set {
			activeAllies = value;
			LevelManager.Instance.CheckEndCondition();
		}
	}

	public enum AllyMoveState {
		Idle,
		Moving
	}

	protected AllyMoveState moveState = AllyMoveState.Idle;
	public AllyMoveState MoveState {
		get {
			return moveState;
		}
		set {
			switch (value) {
			case AllyMoveState.Idle:
				LogMessage("Entering idle state.");
				
				MassMultiplier = 1;
				break;
			case AllyMoveState.Moving:					 
				LogMessage("Responding to player input");

				StartCoroutine(CollisionTimeout());
				MassMultiplier = chargeMassMultiplier;
				break;
			}

			LogMessage("Mass change: " + rigidbody2D.mass);
			moveState = value;
		}
	}

	GridCoordinate screenTargetPosition = default(GridCoordinate);

	float collisionTimeout = .5f;
	bool collide = true;
	Collider2D collidedObject = null;
	bool swapping = false;
	
	float baseMass;
	[SerializeField] float chargeMassMultiplier = .0000001f;
	float massMultiplier = 1;

	public float MassMultiplier {
		get {
			return massMultiplier;
		}
		set {
			massMultiplier = value;
			
			rigidbody2D.mass = baseMass * massMultiplier;
		}
	}

	protected override void Awake() {
		baseMass = rigidbody2D.mass;
	}

	protected override void OnEnable() {
		base.OnEnable();

		ActiveAllies++;
		screenTargetPosition = ScreenCoords;
		collidedObject = null;
	}

	protected override void OnDisable() {
		base.OnDisable();

		ActiveAllies--;
	}

	protected override void Update() {
		if (CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
			Vector2 touchPosition;
#if UNITY_EDITOR || UNITY_STANDALONE
			touchPosition = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
			touchPosition = Input.touches[0].position;
#endif


			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

			if (hit.collider == collider2D) {
				StartCoroutine(Drag());
			}
		}


		if (collidedObject != null) {
			DestructibleEntity entity = collidedObject.GetComponent<DestructibleEntity>();

			if (entity != null) {
				if (!entity.gameObject.activeSelf || entity.Dead)
					collidedObject = null;
			}
		}

		targetPosition = GridManager.ScreenCoordsToWorldPosition(screenTargetPosition);

		if (MoveState == AllyMoveState.Moving) {
			if (ScreenCoords == screenTargetPosition)
				MoveState = AllyMoveState.Idle;
		}

		base.Update();

		LogMessage(screenTargetPosition.ToString(), DebugLevel.Error);
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider == collidedObject) {
			if (!collide) return;
		}

		DestructibleEntity entity = collision.gameObject.GetComponent<DestructibleEntity>();

		if (entity != null) {
			collidedObject = collision.collider;
		}
		else return;

		switch (moveState) {
		case AllyMoveState.Idle:
			if (entity is Ally) {
				Ally ally = entity as Ally;

				if (ally.MoveState == AllyMoveState.Idle) {
					if (ally.ScreenCoords.x > ScreenCoords.x) {
						screenTargetPosition = ally.ScreenCoords - new GridCoordinate(1, 0); 
						MoveState = AllyMoveState.Moving;
						PauseCollision();
					}
					else {
						ally.screenTargetPosition = ScreenCoords - new GridCoordinate(1, 0); 
						ally.MoveState = AllyMoveState.Moving;
						ally.SendMessage("PauseCollision");
					}
				}
			}
			break;
		case AllyMoveState.Moving:
			if (entity is Ally) {
				Ally ally = entity as Ally;

				collider2D.enabled = false;

				screenTargetPosition = ally.ScreenCoords;

				ally.screenTargetPosition = ally.ScreenCoords - new GridCoordinate(1, 0);
				ally.MoveState = AllyMoveState.Moving;

				
				PauseCollision();
			}
			else {
				screenTargetPosition = ScreenCoords;
				MoveState = AllyMoveState.Idle;
			}
			break;
		}

		base.OnCollisionEnter2D(collision);

	}
	
	protected virtual void OnCollisionExit2D(Collision2D collision) {
		if (collision.collider == collidedObject) collidedObject = null;
	}
	
	/// <summary>
	/// Updates character velocity to move towards targetPosition. targetPosition's value determines in subclasses.
	/// </summary>
	protected override void Move() {
		Vector2 targetVelocity = Vector2.ClampMagnitude(targetPosition - transform.position, maxMoveSpeed);
		Vector2 newVelocity = Vector2.Lerp(rigidbody2D.velocity, targetVelocity, .9f);

		if ((targetPosition - transform.position).sqrMagnitude > (.75f * .75f)) {
			newVelocity = newVelocity.normalized * maxMoveSpeed;
		}
		else newVelocity *= 5;

		rigidbody2D.velocity = newVelocity;
	}


	private void PauseCollision() {
		StartCoroutine(_PauseCollision());
	}

	protected virtual IEnumerator _PauseCollision() {
		LogMessage("Pausing collisions");

		yield return new WaitForSeconds(.2f);

		collider2D.enabled = true;
	}

	protected virtual IEnumerator Drag() {
		LogMessage("Starting Drag");
		yield return new WaitForSeconds (Time.deltaTime);
		
		GridCoordinate moveTarget = ScreenCoords;
		GridCoordinate lastMoveTarget = ScreenCoords;
		
		while (CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE || CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
			lastMoveTarget = moveTarget;
			
			moveTarget = GridManager.WorldToScreenGridCoords(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if (collidedObject != null) {
				DestructibleEntity entity = collidedObject.GetComponent<DestructibleEntity>();

				if (!entity is Ally)
					moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, ScreenCoords.x);
				else
					moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
			}
			else
				moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
			moveTarget.y = Mathf.Clamp(moveTarget.y, GridManager.minY, GridManager.maxY);
			
			LaneHighlight.Instance.UpdatePosition(GridManager.ScreenCoordsToWorldPosition(moveTarget));

			yield return new WaitForSeconds (Time.deltaTime);
		}
		
		LaneHighlight.Instance.Hide();
		
		if (State != EntityState.Active) yield break;
		
		LogMessage("Drag ended");

		screenTargetPosition = moveTarget;
		MoveState = AllyMoveState.Moving;
	}

	IEnumerator CollisionTimeout() {
		collide = false;
		yield return new WaitForSeconds(collisionTimeout);
		collide = true;
		yield break;
	}

#if UNITY_EDITOR
	public override void DrawInspectorGUI(Editor editor) {
		base.DrawInspectorGUI(editor);

		chargeMassMultiplier = EditorGUILayout.FloatField("Charge mass multiplier", chargeMassMultiplier);
		EditorGUILayout.EnumPopup("Move state", moveState);
	}
#endif
}
