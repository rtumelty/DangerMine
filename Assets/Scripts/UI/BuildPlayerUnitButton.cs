using UnityEngine;
using System.Collections;

public class BuildPlayerUnitButton : MonoBehaviour {

	[SerializeField] PrefabPool playerChar;
	[SerializeField] int myGoldCost;
	[SerializeField] string myCharacterType;
	[SerializeField] float mySpawnCoolDown;
	[SerializeField] float updateRate;

	private float count = 0;
	public bool onCoolDown = false;
	public static bool click = false;
	public static bool stillOverButton = true;
	private TextMesh myTextmesh;
	private string ready = "Ready!";
	private string building = "Building!";

	void Start()
	{
		myTextmesh = GetComponent<TextMesh> ();
		myTextmesh.text = ready;
	}


	void OnMouseDown()
	{
		//Spawns character at cursor position

		if(!onCoolDown &&
		  (PlayerSpawnManager.PLAYERGOLD - myGoldCost) >= 0)
		{
			PlayerSpawnManager.PLAYERGOLD -= myGoldCost;
			stillOverButton = true;
			GameObject mySpawnedCharacter = playerChar.Spawn (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 1f));
			mySpawnedCharacter.GetComponent <Character>().enabled = false;
			StartCoroutine("CoolDownCount");
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


	IEnumerator CoolDownCount()
	{
		onCoolDown = true;
		myTextmesh.text = building;
		yield return new WaitForSeconds (mySpawnCoolDown);
		onCoolDown = false;
	}
}
