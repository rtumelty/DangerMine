using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacement : MonoBehaviour {
	
	private Vector3 mySnapPoint;
	private Vector3 myWorldPos;

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
	private float clickThreshold = 0.2f;

	private GameEntity entity;

	void Awake()
	{
		entity = GetComponent<GameEntity>();
	}

	void OnEnable() {
		released = false;
		initialClick = Time.time;
		entity.enabled = false;

		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach ( Renderer rend in renderers) {
			rend.sortingLayerName = "Overlay" ;
		}
	}


	void Update()
	{
		if(released)
		{
			return;
		}

		StickToCursor();
		CheckForRelease();
	}


	void StickToCursor()
	{
		//Snaps character to cursor
		Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		GridCoordinate moveTarget = GridManager.WorldToScreenGridCoords(inputPosition);

		moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);
		moveTarget.y = Mathf.Clamp(moveTarget.y, GridManager.minY, GridManager.maxY);

		transform.position = GridManager.ScreenCoordsToWorldPosition(moveTarget);

		LaneHighlight.Instance.UpdatePosition(transform.position);
		mySnapPoint = transform.position;
	}


	void CheckForRelease()
	{
		if (Time.time - initialClick < clickThreshold) return;

		//Checks for release of character. Snaps to lane or returns to pool if no valid lane.
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE && mySnapPoint != Vector3.zero && 
		   !GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, transform.position as GridCoordinate) && 
		   !GridManager.Instance.IsOccupied(GridManager.Grid.ScreenGrid, GridManager.WorldToScreenGridCoords(transform.position)))
		{
			entity.enabled = true;
			entity.Targetable = true;
			GridManager.Instance.RegisterEntity(GridManager.Grid.ScreenGrid, entity);

			LaneHighlight.Instance.Hide();
			purchaseButton.SendMessage("StartCooldown");
			released = true;

			if (!LevelManager.Instance.GameStarted) LevelManager.Instance.GameStarted = true;
		}
		else if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE)
		{
			gameObject.SetActive(false);
			purchaseButton.Cancelled();
			LaneHighlight.Instance.Hide();
		}
	}
}
