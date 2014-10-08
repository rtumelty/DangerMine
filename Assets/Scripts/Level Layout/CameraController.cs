using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField] float moveSpeed = 1f;

	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 position = transform.position;
		position += new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
		transform.position = position;
	}
}
