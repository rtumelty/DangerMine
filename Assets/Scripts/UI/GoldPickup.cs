using UnityEngine;
using System.Collections;

public class GoldPickup : Pickup 
{
	[SerializeField] int normalGoldValue;
	[SerializeField] int highGoldValue;
	[SerializeField] float highGoldDropOdds = .2f;
	private bool highDrop = false;

	protected override void OnEnable ()
	{
		transform.localScale = new Vector3(1, 1, 1);	
		highDrop = Random.value < highGoldDropOdds;
		base.OnEnable();
	}

	protected override void Collect() {
		
		GlobalManagement.AddGold(highDrop ? normalGoldValue : highGoldValue);
		base.Collect();
	}

	protected override IEnumerator InitialMove(Vector2 destination) {
		yield return StartCoroutine(base.InitialMove(destination));

		StartCoroutine(AutoCollect());
	}

	IEnumerator AutoCollect() {
		yield return new WaitForSeconds(.2f + Random.Range(0f, .4f));

		Camera uiCamera = UIPositionReference.FindUIObject("UI Camera").GetComponent<Camera>();
		Vector3 goldIconPosition = UIPositionReference.FindUIObject("GoldIcon").transform.position;
		Vector3 screenTarget = uiCamera.WorldToScreenPoint(goldIconPosition);
		
		Vector3 target;
		while (true) {
			target = Camera.main.ScreenToWorldPoint(screenTarget);
			float magnitude = (target - transform.position).magnitude;
			if (magnitude < .5)
				break;

			if (magnitude < 3) {
				float newScale = magnitude / 3;
				transform.localScale = new Vector3(magnitude / 3, magnitude / 3, magnitude / 3);
			}

			transform.position = Vector3.Slerp(transform.position, target, 8 * Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}

		Collect();
	}
}
