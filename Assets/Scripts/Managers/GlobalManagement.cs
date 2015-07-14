using UnityEngine;
using System.Collections;

public static class GlobalManagement
{
	public static int PLAYERGOLD = 100;
	public static int PLAYERDIAMONDS = 0;

	public static int MAX_PLAYER_CHARS;
	public static int SCORE {
		get {
			return LAST_DISTANCE_COVERED * 100;
		}
	}
	public static int LAST_DISTANCE_COVERED;
	public static int GOLD_COLLECTED;

	public static void AddGold(int gold)
	{
		PLAYERGOLD += gold;
		GOLD_COLLECTED += gold;
	}

	public static void AddDiamonds(int diamonds) {
		PLAYERDIAMONDS += diamonds;
	}
}
