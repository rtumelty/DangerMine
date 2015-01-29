using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacement : MonoBehaviour {
	private Vector3 defaultPosition = new Vector3(-1000, 0, 0);

	private Vector3 mySnapPoint;
	private Vector3 myWorldPos;

	private bool dragging = false;

	private BuildPlayerUnitButton purchaseButton;

	public BuildPlayerUnitButton PurchaseButton {
		get {
			return purchaseButton;
		}
		set {
			purchaseButton = value;
		}
	}

	[SerializeField] private bool released = false;

	private float initialClick = 0;
#if UNITY_EDITOR || UNITY_STANDALONE
	private float clickThreshold = 0.1f;
	#elif UNITY_ANDROID || UNITY_IPHONE
	private float clickThreshold = 0.25f;
#endif

	private GameEntity entity;

	void Awake()
	{
		entity = GetComponent<GameEntity>();
	}

	void OnEnable() {
		released = false;
		dragging = false;
		initialClick = Time.time;

		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach ( Renderer rend in renderers) {
			rend.sortingLayerName = "Overlay" ;
		}

		StartCoroutine(UpdatePosition());
	}

	IEnumerator UpdatePosition() {
		while (!released) {
			StickToCursor();
			CheckForRelease();

			yield return new WaitForSeconds(Time.deltaTime);
		}
	}


	void StickToCursor()
	{
		//Snaps character to cursor
		Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		GridCoordinate moveTarget = GridManager.WorldToScreenGridCoords(inputPosition);

		moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);

		Debug.LogError(moveTarget.ToString() + " " +GridManager.minY + " " +GridManager.maxY);

		if (moveTarget.y < GridManager.minY || moveTarget.y > GridManager.maxY) 
			transform.position = defaultPosition;
		else
			transform.position = GridManager.ScreenCoordsToWorldPosition(moveTarget);

		LaneHighlight.Instance.UpdatePosition(transform.position);

		mySnapPoint = transform.position;
	}


	void CheckForRelease()
	{
		if (Time.time - initialClick < clickThreshold) return;

		if (CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE || CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) dragging = true;

		//Checks for release of character. Snaps to lane or returns to pool if no valid lane.
		if(dragging && (CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE || CheckInputType.TOUCH_TYPE == InputType.NO_TYPE)) {
			if ((mySnapPoint != defaultPosition) && 
			   //!GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, transform.position as GridCoordinate) && 
			   !GridManager.Instance.IsOccupied(GridManager.Grid.ScreenGrid, GridManager.WorldToScreenGridCoords(transform.position)))
			{
				entity.enabled = true;
				entity.State = GameEntity.EntityState.Active;

				LaneHighlight.Instance.Hide();
				purchaseButton.SendMessage("StartCooldown");
				released = true;

				if (!LevelManager.Instance.GameStarted) LevelManager.Instance.GameStarted = true;
			}
			else 
			{
				purchaseButton.Cancel();
				gameObject.SetActive(false);
				LaneHighlight.Instance.Hide();
			}
		}
	}
}
