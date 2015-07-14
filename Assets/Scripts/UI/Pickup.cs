using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	float moveTime = .3f;

	protected virtual void Awake() {
		
		gameObject.renderer.sortingLayerName = "Pickups";
	}

	protected virtual void OnEnable() {
		StartCoroutine(InitialMove(Random.insideUnitCircle * Random.Range(.7f, 1.3f)));
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

	protected virtual IEnumerator InitialMove(Vector2 destination) {
		destination.Normalize();

		Vector3 startPosition = transform.position;
		Vector3 targetPosition = transform.position + new Vector3(destination.x, destination.y, 0);

		targetPosition = new Vector2(Mathf.Clamp(targetPosition.x, GridManager.minWorldX, GridManager.maxWorldX),
		                             Mathf.Clamp(targetPosition.y, GridManager.minWorldY, GridManager.maxWorldY));

		float elapsedTime = 0f;
		float currentBounce = 0f;

		while (elapsedTime < moveTime) {
			currentBounce = elapsedTime / moveTime * 2;
			if (currentBounce > 1f)
				currentBounce = 2 - currentBounce;

			transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime/moveTime)) + new Vector3(0, currentBounce, 0);
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}

	}
}
