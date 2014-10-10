using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField] float moveSpeed = 1f;

	public float MoveSpeed {
		get {
			return moveSpeed;
		}
	}

	public GridCoordinate GridCoords {
		get {
			return transform.position as GridCoordinate;
		}
	}

	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 position = transform.position;
		position += new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
		transform.position = position;
	}
}
