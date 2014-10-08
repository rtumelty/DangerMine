using UnityEngine;
using System.Collections;

public class BuildPlayerUnitButton : MonoBehaviour {

	[SerializeField] PrefabPool playerChar;
	[SerializeField] int myGoldCost;

	public static bool click = false;
	public static bool stillOverButton = true;

	void OnMouseDown ()
	{
		//Spawns character at cursor position

		if((PlayerSpawnManager.PLAYERGOLD - myGoldCost) >= 0)
		{
			stillOverButton = true;
			GameObject mySpawnedCharacter = playerChar.Spawn (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 1f));
			mySpawnedCharacter.GetComponent <Character>().enabled = false;
		}
	}


	void OnMouseExit()
	{
		stillOverButton = false;
	}


	void OnMouseUp()
	{
		click = true;
	}
}
