using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildPlayerUnitButton : MonoBehaviour {
	[SerializeField] Text unitCostText;
	[SerializeField] Image unavailableOverlay;
	[SerializeField] Image cooldownOverlay;
	[SerializeField] Image selectedOverlay;

	[SerializeField] PrefabPool playerChar;
	[SerializeField] int goldCost;
	[SerializeField] float spawnCoolDown;

	private Button button;
	private bool ready = false;
	private bool selected = false;
	private bool cooldownActive = false;

	void Awake() {
		button = GetComponent<Button>();
		unitCostText.text = goldCost.ToString();
	}

	void OnGUI() {
		ready = true;

		if (GlobalManagement.PLAYERGOLD < goldCost) {
			ready = false; 
			unitCostText.color = Color.red;
		} else
			unitCostText.color = Color.white;

		if (cooldownActive) ready = false;

		if (!ready) {
			unavailableOverlay.enabled = true;
			button.interactable = false;
		} else {
			unavailableOverlay.enabled = false;
			button.interactable = true;
		}

		if (selected) selectedOverlay.enabled = true;
		else selectedOverlay.enabled = false;
	}

	void OnMouseDown() {
		Clicked();
	}

	public void Clicked()
	{
		//Spawns character at cursor position
		GameObject mySpawnedCharacter = playerChar.Spawn (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 1f));
		mySpawnedCharacter.GetComponent <Character>().enabled = false;

		PlayerCharacterPlacement placement = mySpawnedCharacter.GetComponent<PlayerCharacterPlacement>();
		if (placement == null) placement = mySpawnedCharacter.AddComponent<PlayerCharacterPlacement>();
		placement.PurchaseButton = this;

		selected = true;
	}

	public void Cancelled() {
		selected = false;
	}

	void StartCooldown() {
		selected = false;
		GlobalManagement.PLAYERGOLD -= goldCost;

		cooldownActive = true;
		
		StartCoroutine("CoolDownCount");
	}


	IEnumerator CoolDownCount()
	{
		float timer = 0f;

		while (timer < spawnCoolDown) {
			cooldownOverlay.fillAmount = Mathf.Lerp(1, 0, timer / spawnCoolDown);
			timer += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		cooldownActive = false;
	}
}
