using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ally : Character {
#if UNITY_EDITOR || UNITY_STANDALONE
	private static float laneSwitchThreshold = 1f;
	#elif UNITY_ANDROID
	private static float laneSwitchThreshold = 10f;
#endif
	//private static float minimumDragTime = 0.005f;

	private static List<Ally> activeAllies;

	public static int ActiveAllies {
		get {
			return activeAllies.Count;
		}
	}

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
		if (!activeAllies.Contains(this))
			activeAllies.Add(this);
	}

	protected override void OnDisable() {
		if (activeAllies.Contains(this))
			activeAllies.Remove(this);
		base.OnDisable ();
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

		yield return new WaitForSeconds (Time.deltaTime);

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

		GridCoordinate newCoord = gridCoords + new GridCoordinate (0f, up);

		ignoreUpdate = true;
		
		float swipeTime = .06f;
		float elapsedTime = 0f;
		
		Vector3 startPosition = transform.position;
		Vector3 targetPosition = new Vector3(transform.position.x, newCoord.ToVector3().y, transform.position.z);

		// "Wobble" if out of level range or coordinates are occupied
		if (GridManager.Instance.IsOccupied(newCoord)) {
			swipeTime *= 2;
			while (elapsedTime <= swipeTime / 2) {
				transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * (1 / swipeTime));
				elapsedTime += Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
			
			
			while (elapsedTime <= swipeTime) {
				transform.position = Vector3.Lerp(startPosition, targetPosition, 1 - (elapsedTime * (1 / swipeTime)));
				elapsedTime += Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
			transform.position = startPosition;
		}
		else {
			while (elapsedTime <= swipeTime) {
				transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * (1 / swipeTime));
				elapsedTime += Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
			transform.position = targetPosition;
			gridCoords = new GridCoordinate (transform.position);
			UpdateSortingLayer();
		}
		
		ignoreUpdate = false;
	}
}
