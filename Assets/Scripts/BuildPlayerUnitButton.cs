using UnityEngine;
using System.Collections;

public class BuildPlayerUnitButton : MonoBehaviour {

	[SerializeField] PrefabPool playerChar;
	[SerializeField] int myGoldCost;

	void OnMouseDown ()
	{
		//Spawns character at cursor position

		if((PlayerSpawnManager.PLAYERGOLD - myGoldCost) >= 0)
		{
			GameObject mySpawnedCharacter = playerChar.Spawn (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 1f));
			mySpawnedCharacter.GetComponent <Character>().enabled = false;
		}
	}
}
