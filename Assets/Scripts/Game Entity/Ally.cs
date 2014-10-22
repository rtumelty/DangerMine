using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ally : Character {
	
	protected static int activeAllies = 0;
#if UNITY_EDITOR || UNITY_STANDALONE
	private static float laneSwitchThreshold = 1f;
	#elif UNITY_ANDROID
	private static float laneSwitchThreshold = 10f;
#endif
	//private static float minimumDragTime = 0.005f;

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
		if (dying) return;

		StartCoroutine (Drag ());
	}

	protected virtual IEnumerator Drag() {

		yield return new WaitForSeconds (Time.deltaTime);

		Debug.Log("Starting drag");

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
				insufficientDeltaY = 	false;
				
				Debug.Log("Drag recognised");
				break;
			}

			//dragLength += Time.deltaTime;
			yield return new WaitForSeconds (Time.deltaTime);
		}

		if (dying) yield break;

		if (insufficientDeltaY){
			
			Debug.Log("Drag too short");
			yield break;
		}

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
		ignoreUpdate = true;

		float swipeTime = .06f;
		float elapsedTime = 0f;

		Vector3 startPosition = transform.position;
		
		while (elapsedTime <= swipeTime) {
			transform.position = Vector3.Lerp(startPosition, newCoord.ToVector3(), elapsedTime * (1 / swipeTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		transform.position = newCoord.ToVector3(transform.position.z);

		ignoreUpdate = false;
		gridCoords = new GridCoordinate (transform.position);
		UpdateSortingLayer();
	}
}
