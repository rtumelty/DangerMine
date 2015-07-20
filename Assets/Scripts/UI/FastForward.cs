using UnityEngine;
using System.Collections;

public class FastForward : MonoBehaviour {
	[SerializeField] float speedScale = 4;
	bool fastForward = false;


	public void ToggleSpeed() {
		fastForward = !fastForward;

		if (fastForward)
			SpeedUp();
		else 
			SlowDown();
	}

	void SpeedUp() {
		Time.timeScale = speedScale;
	}
	
	void SlowDown() {
		Time.timeScale = 1;
	}
}
