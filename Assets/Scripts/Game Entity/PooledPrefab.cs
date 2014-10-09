using UnityEngine;
using System.Collections;

public class PooledPrefab : MonoBehaviour {

	[SerializeField] float initialDelay = .5f;
	[SerializeField] float recycleDelay = .8f;

	[SerializeField] bool recycleWhenOffScreen;
	//[SerializeField] Renderer rendererForScreenCheck;

	[SerializeField] bool recycleOnCollision;
	[SerializeField] bool recycleOnCollision2D;

	[SerializeField] bool recycleAfterTime;
	[SerializeField] float recycleTime = 5f;

	[SerializeField] bool recycleWhenFar;
	[SerializeField] public Transform distanceObject;
	[SerializeField] float distance = 10f;

	[SerializeField] bool recyclable = false;
	bool recycling = false;

	void OnEnable() {
		StartCoroutine (InitialDelay ());

		if (recycleAfterTime)
			StartCoroutine(Recycle(recycleTime));
	}
	
	void OnBecameInvisible() {
		if (recycleWhenOffScreen) {
			StartCoroutine (Recycle (recycleDelay));
		}
	}
	
	void OnCollisionEnter(Collision c) {
		if (recycleOnCollision) Recycle(0);
	}
	
	void OnCollisionEnter2D(Collision2D c) {
		if (recycleOnCollision2D) Recycle(0);
	}

	void Update() {
		if (recycleWhenFar && distanceObject != null) {
			if ((transform.position - distanceObject.position).magnitude > distance)
				Recycle(recycleDelay);
		}
	}

	IEnumerator InitialDelay() {
		yield return new WaitForSeconds (initialDelay);
		recyclable = true;
	}

	IEnumerator Recycle (float delay) {
		if (!recycling) {
			recycling = true;
			yield return new WaitForSeconds (delay);

			while (!recyclable)
					yield return new WaitForSeconds (Time.deltaTime);

			recycling = false;
			gameObject.SetActive (false);
		}
	}
}
 