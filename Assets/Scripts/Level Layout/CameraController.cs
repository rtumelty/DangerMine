using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	static CameraController instance;

	public static CameraController Instance {
		get {
			return instance;
		}
	}

	[SerializeField] float moveSpeed = 1f;

	public static float MoveSpeed {
		get {
			return instance.moveSpeed;
		}
	}

	static Plane[] planes;
	public static GridCoordinate GridCoords {
		get {
			return instance.transform.position as GridCoordinate;
		}
	}

	void Awake() {
		instance = this;
	}
	
	// Update is called once per frame
	void Update() {
		planes = GeometryUtility.CalculateFrustumPlanes(camera);
	}

	void LateUpdate () {
		Vector3 position = transform.position;
		position += new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
		transform.position = position;
	}
	
	public static bool IsVisibleToCamera(Collider c) {
		return GeometryUtility.TestPlanesAABB(planes, c.bounds);
	}
	
	public static bool IsVisibleToCamera(Collider2D c) {
		return GeometryUtility.TestPlanesAABB(planes, c.bounds);
	}
}
