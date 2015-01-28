using UnityEngine;
using System.Collections;

public class DiamondPickup : Pickup 
{
	[SerializeField] int diamondValue;

	protected override void OnEnable ()
	{
		base.OnEnable();
	}

	protected override void Collect() {
		
		GlobalManagement.AddDiamonds(diamondValue);
		base.Collect();
	}
}
