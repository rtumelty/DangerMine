#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// Destructible entity. Provides health and damage functionality to sub-classes.
/// </summary>
public class DestructibleEntity : GameEntity {
	[SerializeField] protected float baseHealth;

	private float currentHealth;
	protected GameEntity destroyedBy;

	/// <summary>
	/// The health multiplier. Allows for buffs/debuffs to affect max health.
	/// </summary>
	private float healthMultiplier = 1f;
	public float HealthMultiplier {
		get {
			return healthMultiplier;
		}
		set {
			LogMessage("Changing health multiplier to " + value);

			float newMultiplier = value;

			if (newMultiplier > 1)
				currentHealth = currentHealth / healthMultiplier * newMultiplier;

			currentHealth = Mathf.Clamp(currentHealth, 0, baseHealth * newMultiplier);

			healthMultiplier = newMultiplier;
		}
	}

	protected override void OnEnable() {
		currentHealth = baseHealth;
		healthMultiplier = 1;
		destroyedBy = null;
	}

	/// <summary>
	/// DestructibleEntity takes damage.
	/// </summary>
	/// <param name="damage">Amount of damage taken.</param>
	/// <param name="cause">Cause of the damage (Character / EnvironmentEffect).</param>
	public void TakeDamage(float damage, GameEntity cause) {
		LogMessage("Taking " + damage + " from " + cause.name);
		currentHealth = Mathf.Clamp(currentHealth - damage, 0, baseHealth * healthMultiplier);

		if (currentHealth == 0) {
			StopAllCoroutines();
			StartCoroutine (Dying (cause));
		}
	}

	/// <summary>
	/// Dummy function - starts the death process of object. Overridable in subclasses for animation handling etc.
	/// </summary>
	/// <param name="cause">Cause.</param>
	protected virtual IEnumerator Dying(GameEntity cause) {
		LogMessage("Dying, cause " + cause.name);

		collider2D.enabled = false;
		yield return new WaitForSeconds(.0f);

		Die (cause);
	}

	/// <summary>
	/// Disables GameObject. Overridable in subclasses.
	/// </summary>
	/// <param name="cause">Cause of object death.</param>
	protected virtual void Die(GameEntity cause = null) {
		LogMessage("Destroyed, cause " + cause.name);
		gameObject.SetActive(false);
	}

#if UNITY_EDITOR
	private bool expandHealth = false;
	
	public override void DrawInspectorGUI(Editor editor) {
		base.DrawInspectorGUI(editor);

		if (showDefaultInspector == true) return;

		expandHealth = EditorGUILayout.Foldout(expandHealth, "Health");
		if (expandHealth) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.BeginVertical(EditorStyles.textArea);

			baseHealth = EditorGUILayout.FloatField("Base health: ", baseHealth);
			EditorGUILayout.LabelField("Current health:", currentHealth.ToString());
			EditorGUILayout.LabelField("Current multiplier:", healthMultiplier.ToString());

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
#endif
}
