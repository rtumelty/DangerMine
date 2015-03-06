﻿using UnityEngine;
using System.Collections;

public class Hole : EnvironmentalHazard {
	void OnTriggerEnter2D(Collider2D other) {
		Character entity = other.gameObject.GetComponent<Character>();

		if (entity != null)
			entity.SendMessage("Fall", this);
	}
}
