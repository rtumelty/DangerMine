using UnityEngine;
using System.Collections;

public class FastForward : MonoBehaviour {
	bool fastForward = false;


	public void ToggleSpeed() {
		fastForward = !fastForward;

		if (fastForward)
			SpeedUp();
		else 
			SlowDown();
	}

	void SpeedUp() {
		Time.timeScale = 2;
	}
	
	void SlowDown() {
		Time.timeScale = 1;
	}
}
