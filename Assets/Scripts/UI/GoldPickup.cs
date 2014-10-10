using UnityEngine;
using System.Collections;

public class GoldPickup : MonoBehaviour 
{
	[SerializeField] int minGoldValue;
	[SerializeField] int maxGoldValue;
	private int myGoldDrop = 0;

	void OnEnable ()
	{
		gameObject.renderer.sortingLayerName = "OverLay";
		myGoldDrop = Random.Range (minGoldValue, maxGoldValue + 1);
	}

	void OnMouseOver()
	{
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE || CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE)
		{
			GlobalManagement.AddGold(myGoldDrop);
			gameObject.SetActive(false);
		}
	}
}
