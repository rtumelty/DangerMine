using UnityEngine;
using System.Collections;

public enum LevelState
{
	GAME_OVER,
	PAUSED,
	SEQUENCE,
	RUNNING
}


public class LevelStatusManager : MonoBehaviour
{
	public static LevelState levelState;
	
	static LevelStatusManager instance;
	public static LevelStatusManager Instance 
	{
		get 
		{
			if (instance == null) 
			{
				GameObject go = new GameObject("_LevelStatusManager");
				instance = go.AddComponent<LevelStatusManager>();
			}
			return instance;
		}
	}
	private float distanceCovered;
	private int playerCharsActive;
	private int goldCollectedThisRound;
	private int playerCharactersKilled;
	private int playerCharactersSurvived;

	void Awake ()
	{
		playerCharsActive = 1;
	}

	void FixedUpdate()
	{
		distanceCovered = SpawnObject.cameraDistanceCovered;
	}
}