using UnityEngine;
using System.Collections;

public class GroundManager : MonoBehaviour {
	static GroundManager instance;
	public static GroundManager Instance {
		get {
			return instance;
		}
	}

	[SerializeField] GameObject defaultSegment;
	[SerializeField] GameObject[] segments;

	[SerializeField] GroundSegment lastSegment;
	int sortingOrder = -200;

	void Awake() {
		if (instance != null) Destroy(this);
		instance = this;
	}

	void OnDisable() {
		if (instance == this) instance = null;
	}

	/// <summary>
	/// Places the next section.
	/// </summary>
	/// <param name="section">The section to be placed.</param>
	public void PlaceSection(Section section) {
		int sectionLength = Mathf.RoundToInt(Random.Range(section.minimumLength.Evaluate(LevelManager.SampleLoopedTime()), section.maximumLength.Evaluate(LevelManager.SampleLoopedTime())));
		
		Debug.Log("GroundManager: Placing " + section.sectionType + " section, length " + sectionLength);

		if (section.sectionType == Section.SectionType.Avoid) {
			PlaceAvoidSegments(sectionLength);
		}
		else {
			PlaceSegments(sectionLength);
		}

		lastSegment = PrefabPool.GetPool(defaultSegment).Spawn(lastSegment.connectPoint.position).GetComponent<GroundSegment>();
		lastSegment.GetComponent<PooledPrefab>().distanceObject = Camera.main.transform;
		lastSegment.renderer.sortingOrder = sortingOrder++;
		lastSegment = PrefabPool.GetPool(defaultSegment).Spawn(lastSegment.connectPoint.position).GetComponent<GroundSegment>();
		lastSegment.GetComponent<PooledPrefab>().distanceObject = Camera.main.transform;
		lastSegment.renderer.sortingOrder = sortingOrder++;

		LevelManager.NextSectionStart = Mathf.RoundToInt(lastSegment.connectPoint.position.x);
	}

	void PlaceAvoidSegments(int length) {

		for (int i = 0; i < length; i++) {
			lastSegment = PrefabPool.GetPool(segments[Random.Range(1, segments.Length)]).Spawn(lastSegment.connectPoint.position).GetComponent<GroundSegment>();
			lastSegment.GetComponent<PooledPrefab>().distanceObject = Camera.main.transform;
			lastSegment.renderer.sortingOrder = sortingOrder++;
		}	
	}

	void PlaceSegments(int length) {
		GameObject poolKey = segments[Random.Range(0, segments.Length)];

		for (int i = 0; i < length; i++) {
			lastSegment = PrefabPool.GetPool(poolKey).Spawn(lastSegment.connectPoint.position).GetComponent<GroundSegment>();
			lastSegment.GetComponent<PooledPrefab>().distanceObject = Camera.main.transform;
			lastSegment.renderer.sortingOrder = sortingOrder++;
		}
	}
}
