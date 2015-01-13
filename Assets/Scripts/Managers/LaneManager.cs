using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaneManager : MonoBehaviour {
	static LaneManager instance;

	public static LaneManager Instance {
		get {
			return instance;
		}
	}

	static int maxFollowDistance = 5;
	Enemy[][] lanes;
	int members = 0;

	// Use this for initialization
	void Awake () {
		instance = this;

		lanes = new Enemy[5][];

		for (int i = 0; i < lanes.Length; i++) lanes[i] = new Enemy[10];
	}

	public void JoinRow(Enemy enemy, int row) {
		row += 2;

		for (int i = 0; i < lanes[row].Length; i++) {
			if (lanes[row][i] == null) {
				lanes[row][i] = enemy;

				enemy.followDistance = maxFollowDistance - i;
				members++;
				break;
			}
		}
	}

	public void RemoveFromRow(Enemy enemy, int row) {
		row += 2;
		int position = 0;

		while (enemy != lanes[row][position]) { position++; }

		if (lanes[row][position] != enemy) {
			Debug.LogError(enemy + " not registered in row " + row);
			return;
		}

		members--;

		for (int nextPosition = position + 1; nextPosition < lanes[row].Length; position++, nextPosition++) {
			lanes[row][position] = lanes[row][nextPosition];
			if (lanes[row][position] == null)
				break;
		}
	}

	void Update() {
		if (members == 0) return;

		foreach (Enemy[] lane in lanes) {
			for (int i = 0; i < lane.Length; i++) {
				if (lane[i] != null)
					lane[i].followDistance = maxFollowDistance - i;
				else break;
			}
		}
	}
}
