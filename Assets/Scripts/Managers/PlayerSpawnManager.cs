using UnityEngine;
using System.Collections;

public static class PlayerSpawnManager
{
	public static int PLAYERGOLD = 100;

	public static void AddGold(int gold)
	{
		Debug.Log ("Gold " + gold);
		PLAYERGOLD += gold;
		Debug.Log ("Gold was added from pickup");
	}
}
