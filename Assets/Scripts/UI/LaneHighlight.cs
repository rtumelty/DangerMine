using UnityEngine;
using System.Collections;

public class LaneHighlight : MonoBehaviour {
	static LaneHighlight instance;

	public static LaneHighlight Instance {
		get {
			return instance;
		}
	}

	[SerializeField] GameObject horizontal;
	[SerializeField] GameObject vertical;

	bool hidden = true;

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy(this);

		Hide();
	}
	
	public void Hide() {
		horizontal.SetActive(false);
		vertical.SetActive(false);

		hidden = true;
	}
	
	public void Unhide() {
		horizontal.SetActive(true);
		vertical.SetActive(true);
		
		hidden = false;
	}

	public void UpdatePosition (Vector3 position) {
		if (hidden) {
			Unhide();
		}
		
		horizontal.transform.position = position;
		vertical.transform.position = position;	
	}
}
