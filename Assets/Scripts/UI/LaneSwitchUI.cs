using UnityEngine;
using System.Collections;

public class LaneSwitchUI : MonoBehaviour {
	[SerializeField] Color readyColour;
	[SerializeField] Color waitingColour;
	TextMesh textMesh;

	void Awake() {
		textMesh = GetComponentInChildren<TextMesh>() as TextMesh;
	}

	void OnGUI() {
		if (Ally.CanSwitchLanes) {
			renderer.materials[0].SetColor("_Color", readyColour);
			textMesh.text = "Ready";
		} else {
			renderer.materials[0].SetColor("_Color", waitingColour);
			textMesh.text = "Wait";
		}
	}
}
