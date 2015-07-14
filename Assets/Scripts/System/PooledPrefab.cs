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
	[SerializeField] bool xAxis = true;
	[SerializeField] int direction = -1;

	[SerializeField] bool recyclable = false;
	bool recycling = false;

	void OnEnable() {
		StartCoroutine (InitialDelay ());

		if (recycleWhenFar && distanceObject == null) {
			distanceObject = Camera.main.transform;
		}

		if (recycleAfterTime)
			StartCoroutine(Recycle(recycleTime));
	}

	void OnDisable() {
		StopAllCoroutines();
	}
	
	void OnBecameInvisible() {
		if (recycleWhenOffScreen && gameObject.activeSelf) {
			StartCoroutine (Recycle (recycleDelay));
		}
	}
	
	void OnCollisionEnter(Collision c) {
		if (recycleOnCollision) StartCoroutine(Recycle(0));
	}
	
	void OnCollisionEnter2D(Collision2D c) {
		if (recycleOnCollision2D) StartCoroutine(Recycle(0));
	}
	[SerializeField]bool debug;
	void Update() {
		if (recycleWhenFar && distanceObject != null) {
			if (xAxis) {
				float difference = transform.position.x - distanceObject.position.x;
				if (Mathf.Abs(difference) > distance && direction * difference > 0) {
					StartCoroutine(Recycle(recycleDelay)); 
				}
			}
			else if ((transform.position - distanceObject.position).magnitude > distance) {
				StartCoroutine(Recycle(recycleDelay));
			}
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
 