using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ally : Character {
	
	protected static int activeAllies = 0;
	
	private static float laneSwitchTimeout = 1.5f;
	private static float laneSwitchThreshold = 10f;
	//private static float minimumDragTime = 0.005f;
	private static bool canSwitchLanes = true;

	public static bool CanSwitchLanes {
		get {
			return canSwitchLanes;
		}
	}

	public static int ActiveAllies {
		get {
			return activeAllies;
		}
	}

	protected override void Awake() {
		_allegiance = Allegiance.Ally;

		moveDirection = 1;

		base.Awake ();
	}

	protected override void OnEnable() {
		base.OnEnable ();
		activeAllies++;
	}

	protected override void OnDisable() {
		base.OnDisable ();
		activeAllies--;
	}

	protected virtual void OnMouseDown () {
		StartCoroutine (Drag ());
	}

	protected virtual IEnumerator Drag() {
		if (!canSwitchLanes)
			yield break;

		yield return new WaitForSeconds (Time.deltaTime);

		//float dragLength = 0f;
		
		float totalY = 0f;
		bool insufficientDeltaY = true;

		while (CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE || CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
#if UNITY_EDITOR || UNITY_STANDALONE
			totalY += Input.GetAxis("Mouse Y");
			#elif UNITY_ANDROID
			totalY += Input.touches[0].deltaPosition.y;
#endif
			if (Mathf.Abs(totalY) > laneSwitchThreshold) {
				insufficientDeltaY = false;
				break;
			}

			//dragLength += Time.deltaTime;
			yield return new WaitForSeconds (Time.deltaTime);
		}

		if (insufficientDeltaY)
			yield break;

		/*
		// Ignore if drag too short
		if (dragLength < minimumDragTime) {
			Debug.Log("Too short!");
			yield break;
		}*/

		float up = Mathf.Sign (totalY);
		Debug.Log ("Drag direction: " + up);

		GridCoordinate newCoord = gridCoords + new GridCoordinate (0f, up);

		// Do nothing if out of level range or coordinates are occupied
		if (GridManager.Instance.IsOccupied(newCoord)) return false;

		canSwitchLanes = false;
		StartCoroutine (ResetLaneSwitch ());
		ignoreUpdate = true;

		float swipeTime = .2f;
		float elapsedTime = 0f;

		Vector3 startPosition = transform.position;
		
		while (elapsedTime <= swipeTime) {
			transform.position = startPosition + (up * new Vector3(0, Mathf.Lerp(0, 1, elapsedTime * (1 / swipeTime))));
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}

		ignoreUpdate = false;
		gridCoords = new GridCoordinate (transform.position);
	}

	IEnumerator ResetLaneSwitch() {
		float lastSwitchedLanes = 0f;

		while (lastSwitchedLanes < laneSwitchTimeout) {
			lastSwitchedLanes += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}

		canSwitchLanes = true;
	}

}
