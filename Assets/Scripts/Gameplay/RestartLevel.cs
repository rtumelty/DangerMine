using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	// Use this for initialization
	public void OnClick() {
		Application.LoadLevel("Prototype");
	}
}
