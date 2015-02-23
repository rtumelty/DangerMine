#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;	
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	static LevelManager instance;

	public static LevelManager Instance {
		get {
			return instance;
		}
	}

#region Camera
	Camera mainCamera;
	[SerializeField] Transform cameraTransform;
	int cameraStartingXPosition;
	static int cameraDistanceCovered = 0;
	public static int CameraDistanceCovered {
		get {
			return cameraDistanceCovered;
		}
	}
#endregion

#region Curve controls
	static float loopStartTime = 120f;
	static float loopLength = 120f;

	public static float SampleLoopedTime() {
		float time = Time.time;

		if (time > loopStartTime) {
			float moduloTime = (time - loopStartTime) % loopLength;
			time = moduloTime + loopStartTime;
		}

		return time;
	}
#endregion

#region Sections
	public List<Section> sections;

	static int nextSectionStart = 0;
	static public int NextSectionStart {
		get {
			return nextSectionStart;
		}
		set {
			nextSectionStart = value;
		}
	}

	Section NextSection() {
		float totalCurveProbability = 0f;
		foreach (Section section in sections) {
			totalCurveProbability += section.sectionWeight.Evaluate(Time.time);
		}

		float weight = Random.Range(0f, totalCurveProbability);

		for (int i = 0; i < sections.Count; i++) {
			totalCurveProbability -= sections[i].sectionWeight.Evaluate(Time.time);

			if (totalCurveProbability < 0) {
				return sections[i];
			}
		}

		return sections[sections.Count-1];
	}
#endregion

#region Difficulty
	AnimationCurve easy;
	AnimationCurve medium;
	AnimationCurve hard;
#endregion

#region Game Start/End
	bool gameStarted = false;
	bool gameOver = false;

	public bool GameStarted {
		get {
			return gameStarted;
		}
		set {
			gameStarted = value;
		}
	}

	public void CheckEndCondition() {
		if (!gameStarted) { 
			if (Ally.ActiveAllies > 0) 
				gameStarted = true;
			
			return;
		}	
		
		// End conditions
		if (Ally.ActiveAllies == 0 && !gameOver) {
			gameOver = true;
			StartCoroutine (GameOver ());
		}
	}
	
	IEnumerator GameOver() {
		Debug.Log("Last miner died!");
		GlobalManagement.LAST_DISTANCE_COVERED = FormationManager.CameraDistanceCovered;
		GlobalManagement.SCORE = FormationManager.CameraDistanceCovered * 10;
		UIMessageReceiver.Instance.SendTrigger("PlayerDied");
		
		float slowDelay = .05f;
		float slowedTimeScale = .25f;
		
		for (float i = 0; i < slowDelay; i += Time.unscaledDeltaTime) {
			Time.timeScale = Mathf.Lerp(1, slowedTimeScale, i / slowDelay);
			yield return new WaitForSeconds(Time.unscaledDeltaTime);
		}
		
		yield return new WaitForSeconds(1f);
		
		for (float i = 0; i < slowDelay; i += Time.unscaledDeltaTime) {
			Time.timeScale = Mathf.Lerp(slowedTimeScale, 1, i / slowDelay);
			yield return new WaitForSeconds(Time.unscaledDeltaTime);
		}
		Time.timeScale = 1;
		
		UIMessageReceiver.Instance.SendTrigger("GameOver");
		yield return null;
	}
	#endregion

	// Use this for initialization
	void Awake () {
		if (instance == null) instance = this;
		else Destroy(this);

		if (cameraTransform == null)
			cameraTransform = Camera.main.transform;

		mainCamera = Camera.main;
	
		cameraStartingXPosition = Mathf.RoundToInt(mainCamera.transform.position.x);
		cameraDistanceCovered = 0;

		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.SCORE = 0;

		gameStarted = false;
		gameOver = false;

		Ally.ActiveAllies = 0;

		GroundManager.Instance.PlaceSection(sections[0]);
		GroundManager.Instance.PlaceSection(sections[0]);
	}

	void OnDisable() {
		StopAllCoroutines();
	}
	
	// Update is called once per frame
	void Update () {
		cameraDistanceCovered = ( int ) Camera.main.transform.position.x - cameraStartingXPosition;

		if (cameraDistanceCovered > nextSectionStart - 10)
			GroundManager.Instance.PlaceSection(NextSection());
	}



#if UNITY_EDITOR
	bool expandDifficulty = false;
	bool expandSections = false;

	public void ShowGUI(Editor editor) {
		cameraTransform = EditorGUILayout.ObjectField("Camera:", cameraTransform, typeof(Transform), true) as Transform;

		EditorGUILayout.Space();
		loopStartTime = EditorGUILayout.FloatField("Loop start time:", loopStartTime);
		loopLength = EditorGUILayout.FloatField("Loop start time:", loopLength);
		EditorGUILayout.Space();
		
		expandDifficulty = EditorGUILayout.Foldout(expandDifficulty, "Difficulty curves:");
		if (expandDifficulty) {
			EditorGUILayout.BeginVertical(EditorStyles.textArea);
				easy = EditorGUILayout.CurveField("Easy:", easy);
				medium = EditorGUILayout.CurveField("Medium:", medium);
				hard = EditorGUILayout.CurveField("Hard:", hard);
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.Space();
		
		expandSections = EditorGUILayout.Foldout(expandSections, "Sections:");
		if (expandSections) {
			if (sections == null) {
				sections = new List<Section>();
			}

			if (sections.Count > 0)
			{
				EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.MinHeight(30), GUILayout.MaxHeight(100));
				for (int i = 0; i < sections.Count; i++) {
					Section section = sections[i];
					section.ShowGUI();
				
					if (GUILayout.Button("Remove")) {
						sections.RemoveAt(i);
						DestroyImmediate(section);
					}

				}
				EditorGUILayout.EndVertical();
			}
			if (GUILayout.Button("Add")) {
				Section newSection = ScriptableObject.CreateInstance<Section>();
				newSection.Init();
				sections.Add(newSection);
			}
		}

		EditorUtility.SetDirty(this);
	}
#endif
}
