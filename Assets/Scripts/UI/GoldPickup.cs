using UnityEngine;
using System.Collections;

public class GoldPickup : MonoBehaviour 
{
	[SerializeField] int normalGoldValue;
	[SerializeField] int highGoldValue;
	[SerializeField] float highGoldDropOdds = .2f;
	private bool highDrop = false;

	void Awake() {
		
		gameObject.renderer.sortingLayerName = "OverLay";
	}

	void OnEnable ()
	{
		highDrop = Random.Range (0, 1f) < highGoldDropOdds;
	}

	void OnMouseOver()
	{
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE || CheckInputType.TOUCH_TYPE == InputType.DRAG_TYPE)
		{
			GlobalManagement.AddGold(highDrop ? normalGoldValue : highGoldValue);
			gameObject.SetActive(false);
		}
	}
}
