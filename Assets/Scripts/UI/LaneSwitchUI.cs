using UnityEngine;
using System.Collections;

public class LaneSwitchUI : MonoBehaviour {
	static LaneSwitchUI _instance;

	public static LaneSwitchUI Instance {
		get {
			if (_instance == null) {
				GameObject go = new GameObject("_LaneSwitchUI");
				_instance = go.AddComponent<LaneSwitchUI>();
			}
			return _instance;
		}
	}

	static bool canSwitchLanes = true;
	private static float laneSwitchTimeout = 1.5f;

	public static bool CanSwitchLanes {
		get {
			return canSwitchLanes;
		}
	}

	[SerializeField] Color readyColour;
	[SerializeField] Color waitingColour;
	TextMesh textMesh;

	void Awake() {
		if (_instance != null) Destroy(this);
		else _instance = this;

		textMesh = GetComponentInChildren<TextMesh>() as TextMesh;
	}

	void OnGUI() {
		if (CanSwitchLanes) {
			renderer.materials[0].SetColor("_Color", readyColour);
			textMesh.text = "Ready";
		} else {
			renderer.materials[0].SetColor("_Color", waitingColour);
			textMesh.text = "Wait";
		}
	}

	public static void ResetLaneSwitch() {
		if (canSwitchLanes) {
			_instance.StartCoroutine(_ResetLaneSwitch());
		}
	}

	static IEnumerator _ResetLaneSwitch() {
		canSwitchLanes = false;
		float lastSwitchedLanes = 0f;
		
		while (lastSwitchedLanes < laneSwitchTimeout) {
			lastSwitchedLanes += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		canSwitchLanes = true;
	}

}
