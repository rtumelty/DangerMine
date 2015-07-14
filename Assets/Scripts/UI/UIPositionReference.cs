using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPositionReference : MonoBehaviour {

	static UIPositionReference instance;

	public UIPositionReference Instance {
		get {
			return instance;
		}
	}

	void Awake() {
		if (instance != null) Destroy(this);
		else instance = this;
	}

	[SerializeField] List<GameObject> uiObjects;

	public static GameObject FindUIObject(string objectName) {
		foreach (GameObject uiObject in instance.uiObjects) {
			if (uiObject.name == objectName) {
				return uiObject;
			}
		}

		Debug.Log("UI object named \"" + objectName + "\" not found.");
		return null;	
	}
}
