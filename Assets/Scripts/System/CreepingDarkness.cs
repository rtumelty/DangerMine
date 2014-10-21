using UnityEngine;
using System.Collections;

public class CreepingDarkness : MonoBehaviour {
	static CreepingDarkness _instance;

	public static CreepingDarkness Instance {
		get {
			return _instance;
		}
	}

	[SerializeField] float advanceDistance = .5f;
	[SerializeField] float advanceTime = 2f;

	public int LeadingEdge {
		get {
			return Mathf.RoundToInt(transform.position.x);
		}
	}

	void Awake() {
		_instance = this;
	}

	void OnTriggerEnter2D(Collider2D other) {
		Character character = other.gameObject.GetComponent<Character>();

		if (character == null) return;
		else if (character is Ally) {
			if (character.enabled)
				character.SendMessage("Die");
		}
		else if (character is Enemy) {
			StartCoroutine(AdvanceDarkness());
		}
	}

	IEnumerator AdvanceDarkness() {
		float elapsedTime = 0f;

		while (elapsedTime < advanceTime) {
			transform.position = transform.position + new Vector3((advanceDistance / advanceTime) * Time.deltaTime,0,0);
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
}
