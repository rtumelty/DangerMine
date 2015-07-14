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
	[SerializeField] Transform screenBottom;
	float currentAspect;

	public static float MoveSpeed {
		get {
			return instance.moveSpeed;
		}
	}

	public static GridCoordinate GridCoords {
		get {
			return new GridCoordinate(instance.transform.position);
		}
	}

	void Awake() {
		instance = this;

		Resize();
	}

	void Resize() {
		currentAspect = camera.aspect;

		Vector3 gridOrigin = GridManager.Instance.ScreenOrigin;

		camera.orthographicSize = (float)GridManager.PlayableAreaWidth / camera.aspect / 2;
		
		camera.transform.position = new Vector3(transform.position.x, screenBottom.position.y + 
		                                        camera.orthographicSize, transform.position.z);

		GridManager.Instance.ScreenOrigin = gridOrigin;
	}
	
	// Update is called once per frame
	void Update() {
		if (currentAspect != camera.aspect) Resize();
	}

	void LateUpdate () {
		if (LevelManager.Instance.GameStarted) {
			Vector3 position = transform.position;
			Vector3 targetPosition = position + (new Vector3(1, 0, 0) * MoveSpeed);
			transform.position = Vector3.Lerp(position, targetPosition, Time.deltaTime);
		}
	}
}
