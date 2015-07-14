using UnityEngine;
using System.Collections;

public class ParallaxObject : MonoBehaviour {
	public GameObject parallaxReference;
	public bool tiled;
	public float parallaxScale;

	float width;
	float lastX;
	Vector3 startPosition;

	// Use this for initialization
	public void Init () {
		SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>() as SpriteRenderer;
		width = sprite.bounds.size.x;

		startPosition = transform.position;

		lastX = parallaxReference.transform.position.x;
	}

	// Update is called once per frame
	void Update () {/*
		float deltaX = parallaxReference.transform.position.x - lastX;

		Vector3 position = transform.position;
		position.x -= deltaX * parallaxScale;

		if (tiled) {
			if (startPosition.x - position.x >= width) position.x += width;
			else if (startPosition.x - position.x <= -width) position.x -= width;
		}

		transform.position = position;
		lastX = parallaxReference.transform.position.x;*/
		
		if (LevelManager.Instance.GameStarted)
			transform.position = Vector3.Lerp(transform.position, (transform.position + new Vector3(-CameraController.MoveSpeed * parallaxScale, 0, 0)), Time.deltaTime);

		
		if (tiled) {
			if (startPosition.x - transform.position.x >= width) transform.position += new Vector3(width, 0, 0);
			else if (startPosition.x - transform.position.x <= -width) transform.position -= new Vector3(width, 0, 0);
		}
	}
}
