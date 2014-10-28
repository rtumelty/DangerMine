using UnityEngine;
using System.Collections;

public class GoldPickup : Pickup 
{
	[SerializeField] int normalGoldValue;
	[SerializeField] int highGoldValue;
	[SerializeField] float highGoldDropOdds = .2f;
	private bool highDrop = false;

	protected override void OnEnable ()
	{
		highDrop = Random.Range (0, 1f) < highGoldDropOdds;
		base.OnEnable();
	}

	protected override void Collect() {
		
		GlobalManagement.AddGold(highDrop ? normalGoldValue : highGoldValue);
		base.Collect();
	}
}
