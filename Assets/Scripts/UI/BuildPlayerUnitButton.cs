using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildPlayerUnitButton : MonoBehaviour {
	[SerializeField] Text unitCostText;
	[SerializeField] Image unavailableOverlay;
	[SerializeField] Image cooldownOverlay;
	[SerializeField] RectTransform cooldownTarget;

	[SerializeField] PrefabPool playerChar;
	[SerializeField] int goldCost;
	[SerializeField] float spawnCoolDown;

	private Button button;
	private Vector3 cooldownStartPosition;
	private bool ready = false;
	private bool cooldownActive = false;

	void Awake() {
		button = GetComponent<Button>();
		unitCostText.text = goldCost.ToString();
		cooldownStartPosition = cooldownOverlay.rectTransform.anchoredPosition;
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
	}

	public void Clicked()
	{
		//Spawns character at cursor position
		GameObject mySpawnedCharacter = playerChar.Spawn (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 1f));
		mySpawnedCharacter.GetComponent <Character>().enabled = false;

		PlayerCharacterPlacement placement = mySpawnedCharacter.GetComponent<PlayerCharacterPlacement>();
		if (placement == null) placement = mySpawnedCharacter.AddComponent<PlayerCharacterPlacement>();
		placement.PurchaseButton = this;
	}

	void StartCooldown() {
		GlobalManagement.PLAYERGOLD -= goldCost;

		cooldownActive = true;
		
		StartCoroutine("CoolDownCount");
	}


	IEnumerator CoolDownCount()
	{
		float timer = 0f;
		Debug.Log(timer + ", " + cooldownStartPosition + ", " + cooldownOverlay.rectTransform.anchoredPosition);

		while (timer < spawnCoolDown) {
			cooldownOverlay.rectTransform.anchoredPosition = Vector3.Lerp(cooldownStartPosition, cooldownTarget.anchoredPosition,
			                                                      timer / spawnCoolDown);
			timer += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		Debug.Log(timer + ", " + cooldownTarget.anchoredPosition + ", " + cooldownOverlay.rectTransform.anchoredPosition);
		cooldownActive = false;
	}
}
