using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	float moveTime = .3f;
	
	protected virtual void Awake() {
		
		gameObject.renderer.sortingLayerName = "Pickups";
	}

	protected virtual void OnEnable() {
		StartCoroutine(InitialMove(Random.insideUnitCircle));
	}

	protected virtual void OnMouseOver() {
		
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE || CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE)
		{
			Collect();
		}
	}

	protected virtual void Collect() {
		gameObject.SetActive(false);
	}

	IEnumerator InitialMove(Vector2 destination) {
		destination.Normalize();

		Vector3 startPosition = transform.position;
		Vector3 targetPosition = transform.position + new Vector3(destination.x, destination.y, 0);

		float elapsedTime = 0f;

		while (elapsedTime < moveTime) {
			transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime/moveTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}

	}
}
