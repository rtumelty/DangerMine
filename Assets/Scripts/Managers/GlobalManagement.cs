using UnityEngine;
using System.Collections;

public static class GlobalManagement
{
	public static int PLAYERGOLD = 0;
	public static int PLAYERDIAMONDS = 0;

	public static int MAX_PLAYER_CHARS;
	public static int SCORE;
	public static int LAST_DISTANCE_COVERED;
	public static int GOLD_COLLECTED;

	public static void AddGold(int gold)
	{
		PLAYERGOLD += gold;
		GOLD_COLLECTED += gold;
	}
}
