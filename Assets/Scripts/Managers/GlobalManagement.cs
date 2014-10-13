using UnityEngine;
using System.Collections;

public static class GlobalManagement
{
	public static int PLAYERGOLD = 100;

	public static int MAX_PLAYER_CHARS;
	public static int SCORE;
	public static int LAST_DISTANCE_COVERED;
	public static int GOLD_COLLECTED;

	public static void AddGold(int gold)
	{
		Debug.Log ("Gold " + gold);
		PLAYERGOLD += gold;
		GOLD_COLLECTED += gold;
		Debug.Log ("Gold was added from pickup");
	}
}
