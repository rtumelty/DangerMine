using UnityEngine;
using System.Collections;

public class GoldCounter : MonoBehaviour {

	[SerializeField] float updateRate;
	private float count = 0;
	private int localGoldReference;
	private TextMesh myTextMesh;

	void Start ()
	{
		myTextMesh = GetComponent<TextMesh> ();
	}

	void Update ()
	{
		//Fix updates per second.
		count += Time.deltaTime;
		if(count > updateRate)
		{
			count = 0;
			return;
		}
		//Update Gold & Ui Values.
		localGoldReference = GlobalManagement.PLAYERGOLD;
		myTextMesh.text = "Gold:     " + localGoldReference.ToString ();
	}
}
