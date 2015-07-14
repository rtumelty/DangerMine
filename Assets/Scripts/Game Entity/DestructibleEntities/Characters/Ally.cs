#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    static Ally selectedCharacter = null;
    public static Ally SelectedCharacter {
        get {
            return selectedCharacter;
        }
        set {
            if (selectedCharacter != null)
            {
               Debug.Log(selectedCharacter + " deselected");
               selectedCharacter.DeactivateHighlight();
            }

            if (value != null) {
                Debug.Log(value + " selected");
                value.ActivateHighlight();
            }

            selectedCharacter = value;
        }
    }

	public enum AllyMoveState {
		Idle,
		Moving,
		Blocked
	}

	protected AllyMoveState moveState = AllyMoveState.Idle;
	public AllyMoveState MoveState {
		get {
			return moveState;
		}
		set {
			switch (value) {
			case AllyMoveState.Idle:
				LogMessage("State change: idle");

				MassMultiplier = 1;
				break;
			case AllyMoveState.Moving:					 
				LogMessage("State change: moving");

				MassMultiplier = chargeMassMultiplier;
				break;
			case AllyMoveState.Blocked:
				LogMessage("State change: blocked");
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
    GameObject characterHighlight;
	List<Collider2D> ignoredColliders;
	bool reactingToInput = false;
	
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

        characterHighlight = transform.FindChild("Character-Highlight").gameObject;
	}

	protected override void OnEnable() {
		base.OnEnable();

		ActiveAllies++;
		ignoredColliders = new List<Collider2D>();
		screenTargetPosition = ScreenCoords;
		collidedObject = null;
	}

	protected override void OnDisable() {
		base.OnDisable();

		ActiveAllies--;
	}

	protected override void Update() {
		if (MoveState == AllyMoveState.Blocked) {
			if (!CheckIfBlocked()) MoveState = AllyMoveState.Idle;
			else
				screenTargetPosition = ScreenCoords;
		}

		if (collidedObject != null) {
			DestructibleEntity entity = collidedObject.GetComponent<DestructibleEntity>();

			if (entity != null) {
				if (!entity.gameObject.activeSelf || entity.Dead)
					collidedObject = null;
			}
		}

		moveDirection = Vector3.forward * CameraController.MoveSpeed;

		base.Update();
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider == collidedObject) {
			if (!collide) return;
		}

		if (CheckIfBlocked()) MoveState = AllyMoveState.Blocked;

		DestructibleEntity entity = collision.gameObject.GetComponent<DestructibleEntity>();

		if (entity != null) {
			collidedObject = collision.collider;
		}
		else return;

		base.OnCollisionEnter2D(collision);

	}

	protected virtual void OnCollisionExit2D(Collision2D collision) {
		if (collision.collider == collidedObject) collidedObject = null;
	}

	bool CheckIfBlocked() {

		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, new Vector2(1,0), .5f);

		if (hits.Length > 0) {

			bool blocked = false;

			foreach (RaycastHit2D hit in hits) {
				if (hit.collider != collider2D) {

					GameEntity entity = hit.collider.GetComponent<GameEntity>();

					if (entity != null) {
						if (entity is Ally) {
							Ally ally = entity as Ally;

							if (ally.MoveState == AllyMoveState.Blocked) {
								blocked = true;
								break;
							}
						}
						else blocked = true;
					}
				}
			}
			return blocked;
		}

		return false;
	}

	public void FallBack() {

		StartCoroutine(MoveAlongPath(AStar.GetPath(WorldCoords, WorldCoords + new GridCoordinate(-1, 0))));
	}

    void ActivateHighlight()
    {
        characterHighlight.SetActive(true);
    }

    void DeactivateHighlight()
    {
        characterHighlight.SetActive(false);
    }
    
    void HandleInput() {
        StartCoroutine(_HandleInput());
    }

    IEnumerator _HandleInput() {
        StartCoroutine(Drag());

        yield return new WaitForSeconds(Time.deltaTime);
        while (SelectedCharacter == this) {
            if (InputManager.TOUCH_TYPE == InputType.TOUCH_BEGAN) {
                GridCoordinate moveTarget = GridManager.WorldToScreenGridCoords(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (moveTarget == ScreenCoords) {
                    SelectedCharacter = null;
                    break;
                }

                if (moveTarget.y >= GridManager.minY && moveTarget.y <= GridManager.maxY)
                {
                    moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);

                    List<GridCoordinate> path = AStar.GetPath(WorldCoords, GridManager.ScreenCoordsToWorldGrid(moveTarget));

                    if (path[0] != path[path.Count - 1])
                        StartCoroutine(MoveAlongPath(path));
                }
                else
                    SelectedCharacter = null;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
	
	/// <summary>
	/// Updates character velocity to move towards targetPosition. targetPosition's value determines in subclasses.
	/// </summary>
	protected override void Move() {
		if (moveState == AllyMoveState.Blocked)
			rigidbody2D.velocity = Vector2.zero;
		else if (moveState == AllyMoveState.Idle) {
			float targetY = GridManager.ScreenCoordsToWorldPosition(ScreenCoords).y - transform.position.y;

			Vector2 targetVelocity = new Vector2(CameraController.MoveSpeed, targetY); //Vector2.ClampMagnitude((targetPosition - transform.position), CameraRelativeMaxSpeed);
			Vector2 newVelocity = Vector2.Lerp(rigidbody2D.velocity, targetVelocity, .9f);

			rigidbody2D.velocity = targetVelocity;
		}
	}

	protected virtual IEnumerator Drag() {
		yield return new WaitForSeconds (Time.deltaTime * 3);
        if (InputManager.TOUCH_TYPE != InputType.DRAG) yield break;
		
		GridCoordinate moveTarget = ScreenCoords;
		GridCoordinate lastMoveTarget = ScreenCoords;
		
		while (InputManager.TOUCH_TYPE == InputType.DRAG || InputManager.TOUCH_TYPE == InputType.TOUCH_BEGAN) {
			lastMoveTarget = moveTarget;
			
			moveTarget = GridManager.WorldToScreenGridCoords(Camera.main.ScreenToWorldPoint(Input.mousePosition));

			if (collidedObject != null) {
				DestructibleEntity entity = collidedObject.GetComponent<DestructibleEntity>();

				if (!entity is Ally)
					moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, ScreenCoords.x);
				else
					moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
			}
			//else

			moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
			moveTarget.y = Mathf.Clamp(moveTarget.y, GridManager.minY, GridManager.maxY);
			
			LaneHighlight.Instance.UpdatePosition(GridManager.ScreenCoordsToWorldPosition(moveTarget));

			yield return new WaitForSeconds (Time.deltaTime);
		}
		
		LaneHighlight.Instance.Hide();
		
		if (State != EntityState.Active) yield break;
		
		LogMessage("Drag ended");

        if (moveTarget == ScreenCoords) 
            yield break; 

		List<GridCoordinate> path = AStar.GetPath(WorldCoords, GridManager.ScreenCoordsToWorldGrid(moveTarget));

		if (path[0] == path[path.Count-1]) yield break;

		StartCoroutine(MoveAlongPath(path));
	}

	IEnumerator MoveAlongPath(List<GridCoordinate> path) {
        SelectedCharacter = null;

		MoveState = AllyMoveState.Moving;
		collider2D.enabled = false;
		rigidbody2D.velocity = Vector2.zero;

		for (int i = 0; i < path.Count; i++) {
			Vector3 nextPosition = path[i].ToVector3();
			Vector3 startPosition = transform.position;

			if (i == path.Count - 1) {
				if (GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, path[i]))
				{
					GridManager.Instance.EntityAt(GridManager.Grid.WorldGrid, path[i]).SendMessage("FallBack");
				}
			}

			float currentLerp = 0;
			while ((nextPosition - transform.position).sqrMagnitude > .1f * .1f && currentLerp < 1) {
				currentLerp += 10f*Time.deltaTime;

				transform.position = Vector3.Lerp(startPosition, nextPosition, currentLerp);
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}

		MoveState = AllyMoveState.Idle;
		collider2D.enabled = true;
		rigidbody2D.velocity = new Vector3(CameraController.MoveSpeed, 0, 0);
		screenTargetPosition = ScreenCoords;
		UpdateSortingLayer();
	}

#if UNITY_EDITOR
	public override void DrawInspectorGUI(Editor editor) {
		base.DrawInspectorGUI(editor);

		chargeMassMultiplier = EditorGUILayout.FloatField("Charge mass multiplier", chargeMassMultiplier);
		EditorGUILayout.EnumPopup("Move state", moveState);
	}
#endif
}
