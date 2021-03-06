﻿using UnityEngine;
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
	private float clickThreshold = 0.2f;
	#elif UNITY_ANDROID || UNITY_IPHONE
	private float clickThreshold = 0.35f;
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
		StickToCursor();
		while (Time.time - initialClick < clickThreshold) yield return new WaitForSeconds(Time.deltaTime);

		while (!released) {
			
			if (dragging) {
				StickToCursor();
				CheckForRelease();
			}
			else {
				if (InputManager.TOUCH_TYPE == InputType.DRAG || InputManager.TOUCH_TYPE == InputType.TOUCH_BEGAN) dragging = true;

			}

			yield return new WaitForSeconds(Time.deltaTime);
		}
	}


	void StickToCursor()
	{
		//Snaps character to cursor
		Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		GridCoordinate moveTarget = GridManager.WorldToScreenGridCoords(inputPosition);

		moveTarget.x = Mathf.Clamp(moveTarget.x, GridManager.minScreenX, GridManager.maxScreenX);

		if (moveTarget.y < GridManager.minY || moveTarget.y > GridManager.maxY) 
			transform.position = defaultPosition;
		else
			transform.position = GridManager.ScreenCoordsToWorldPosition(moveTarget);

		LaneHighlight.Instance.UpdatePosition(transform.position);

		mySnapPoint = transform.position;
	}


	void CheckForRelease()
	{

		if (InputManager.TOUCH_TYPE == InputType.DRAG || InputManager.TOUCH_TYPE == InputType.TOUCH_BEGAN) dragging = true;

		//Checks for release of character. Snaps to lane or returns to pool if no valid lane.
		if(dragging && (InputManager.TOUCH_TYPE == InputType.TOUCH_RELEASED || InputManager.TOUCH_TYPE == InputType.NONE)) {
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
