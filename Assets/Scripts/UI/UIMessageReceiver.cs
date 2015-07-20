using UnityEngine;
using System.Collections;

public class UIMessageReceiver : MonoBehaviour {
	static UIMessageReceiver instance;
	
	public static UIMessageReceiver Instance {
		get {
			return instance;
		}
	}

	void Awake() {
		instance = this;
	}

	public void SendTrigger(string triggerName) {
		foreach (Animator animator in GetComponentsInChildren<Animator>()) {
			animator.SetTrigger(triggerName);
		}
	}
}
