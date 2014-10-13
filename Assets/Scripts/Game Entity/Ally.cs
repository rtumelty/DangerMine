using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ally : Character {
	
	protected static int activeAllies = 0;
	
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
		Debug.Log ("Start drag?");

		float dragLength = 0f;
		
		float totalY = 0f;

		List<Vector2> mouseCoords = new List<Vector2>();
		
		Debug.Log (CheckInputType.TOUCH_TYPE);
		while (CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE || CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
#if UNITY_EDITOR || UNITY_STANDALONE
			mouseCoords.Add(new Vector2(0, Input.GetAxis("Mouse Y")));
			totalY += Input.GetAxis("Mouse Y");
			#elif UNITY_ANDROID
			mouseCoords.Add(Input.touches[0].deltaPosition);
			totalY += Input.touches[0].deltaPosition.y;
#endif
			Debug.Log (CheckInputType.TOUCH_TYPE);
			dragLength += Time.deltaTime;
			yield return new WaitForSeconds (Time.deltaTime);
		}
		Debug.Log (CheckInputType.TOUCH_TYPE);
		
		// Ignore if drag too short
		if (dragLength < .05f) {
			Debug.Log("Too short!");
			return false;
		}

		float up = Mathf.Sign (totalY);
		Debug.Log ("Drag direction: " + up);

		GridCoordinate newCoord = gridCoords + new GridCoordinate (0, up);

		// Do nothing if out of level range or coordinates are occupied
		if (GridManager.Instance.IsOccupied(newCoord)) return false;

		ignoreUpdate = true;

		float swipeTime = .5f;
		float elapsedTime = 0f;

		Vector3 startPosition = transform.position;
		
		while (elapsedTime <= swipeTime) {
			transform.position = startPosition + (up * new Vector3(0, Mathf.Lerp(0, 1, elapsedTime * (1 / swipeTime))));
		}

		ignoreUpdate = false;
		transform.position = newCoord.ToVector3 (transform.position.z);
		gridCoords = newCoord;
	}


}
