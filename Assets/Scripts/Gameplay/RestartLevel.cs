﻿using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	// Use this for initialization
	public void OnClick() {
		
		GlobalManagement.PLAYERGOLD = 300;

		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.GOLD_COLLECTED = 0;

		Application.LoadLevel("Prototype");
	}
}
