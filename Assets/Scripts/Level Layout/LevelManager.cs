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

	static Section currentSection;

	public static Section CurrentSection {
		get {
			return currentSection;
		}
	}

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
				currentSection = sections[i];
				return sections[i];
			}
		}

		currentSection = sections[sections.Count -1];

		return sections[sections.Count-1];
	}
#endregion

#region Difficulty
	[SerializeField] AnimationCurve easy;
	[SerializeField] AnimationCurve medium;
	[SerializeField] AnimationCurve hard;
#endregion

#region Formations
	[SerializeField] Transform spawnReference;
	Vector3 spawnReferenceOffset;

	static int nextFormation = 0;
	static public int NextFormation {
		get {
			return nextFormation;
		}
		set {
			nextFormation = value;
		}
	}

	void PlaceFormation() {
		spawnReference.position = mainCamera.transform.position + spawnReferenceOffset;

		float easyWeight = easy.Evaluate(SampleLoopedTime());
		float medWeight = medium.Evaluate(SampleLoopedTime());
		float hardWeight = hard.Evaluate(SampleLoopedTime());
		
		int difficulty;
		float probability = Random.Range(0, easyWeight + medWeight + hardWeight);
		if (probability < easyWeight)
			difficulty = 1;
		else if (probability < easyWeight + medWeight)
			difficulty = 2;
		else 
			difficulty = 3;

		LayerMask mask = 1 << 19;
		Ray ray = new Ray(new Vector3(spawnReference.position.x, spawnReference.position.y, -10), Vector3.forward * 50);
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50, mask);

		if (hit.collider== null) { Debug.Log("No object intersected by ray! " + spawnReference.position); return; }
		GroundSegment targetSegment = hit.collider.gameObject.GetComponent<GroundSegment>();
		if (targetSegment == null) { Debug.Log("Object " + hit.collider.gameObject + " doesn't have a GroundSegment component"); return; }

		FormationEntry entry = targetSegment.GetActiveProfile(difficulty);
		for (int i = 0; i < entry.formation.slots.Length; i++) {
			Vector3 spawnCoordinate = spawnReference.position + new Vector3(entry.formation.interval, 0) + entry.formation.slots[i].ToVector3();
			PrefabPool.GetPool(entry.profile.prefabs[i]).Spawn(spawnCoordinate);
		}

		nextFormation += entry.formation.interval + entry.formation.width;
	}
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

		spawnReferenceOffset = spawnReference.position - mainCamera.transform.position;

		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.SCORE = 0;

		gameStarted = false;
		gameOver = false;

		Ally.ActiveAllies = 0;

		currentSection = sections[0];

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

		if (cameraDistanceCovered > nextFormation)
			PlaceFormation();
	}



#if UNITY_EDITOR
	bool expandDifficulty = false;
	bool expandSections = false;

	public void ShowGUI(Editor editor) {
		cameraTransform = EditorGUILayout.ObjectField("Camera:", cameraTransform, typeof(Transform), true) as Transform;
		spawnReference = EditorGUILayout.ObjectField("Formation origin:", spawnReference, typeof(Transform), true) as Transform;

		EditorGUILayout.Space();
		loopStartTime = EditorGUILayout.FloatField("Loop start time:", loopStartTime);
		loopLength = EditorGUILayout.FloatField("Loop start time:", loopLength);
		EditorGUILayout.Space();

		if (easy == null) easy = new AnimationCurve();
		if (medium == null) medium = new AnimationCurve();
		if (hard == null) hard = new AnimationCurve();

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
