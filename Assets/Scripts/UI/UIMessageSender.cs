using UnityEngine;
using System.Collections;

public class UIMessageSender : MonoBehaviour {

	[SerializeField] string triggerName;

	public void Send() {
		UIMessageReceiver.Instance.SendTrigger(triggerName);
	}
}
