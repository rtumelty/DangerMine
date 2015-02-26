using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {
	[SerializeField] GameObject parallaxReference;
	[SerializeField] float parallaxScale;

	float width;
	float lastX;

	// Use this for initialization
	void Start () {
		SpriteRenderer hills = GetComponentInChildren<SpriteRenderer>() as SpriteRenderer;
		width = hills.bounds.size.x;

		lastX = parallaxReference.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		float deltaX = parallaxReference.transform.position.x - lastX;

		Vector3 position = transform.position;
		position.x -= deltaX * parallaxScale;
		
		if (position.x >= width) position.x -= width;
		else if (position.x <= -width) position.x += width;
		
		transform.position = position;
		lastX = parallaxReference.transform.position.x;
	}
}
