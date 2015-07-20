#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Section : ScriptableObject {

	public enum SectionType {
		Gold, 
		Enemy,
		Obstacle,
		Avoid,
		Mixed
	}

	public SectionType sectionType;
	public AnimationCurve sectionWeight;
	public AnimationCurve minimumLength;
	public AnimationCurve maximumLength;

	[SerializeField] bool active = true;

	public bool Active {
		get {
			return active;
		}
	}

	public void Init() {
		sectionWeight = new AnimationCurve();
		minimumLength = new AnimationCurve();
		maximumLength = new AnimationCurve();
	}

#if UNITY_EDITOR
	bool expand = false;

	public void ShowGUI() {
		expand = EditorGUILayout.Foldout(expand, name);

		if (expand) {
			name = EditorGUILayout.TextField("Name", name);
			sectionType = (SectionType)EditorGUILayout.EnumPopup("Section type:", sectionType);
			active = EditorGUILayout.Toggle("Active?", active);
			sectionWeight = EditorGUILayout.CurveField("Probability / Time:", sectionWeight);
			
			minimumLength = EditorGUILayout.CurveField("Min Length / Time:", minimumLength);
			maximumLength = EditorGUILayout.CurveField("Max Length / Time:", maximumLength);
		}
	}
#endif
}
