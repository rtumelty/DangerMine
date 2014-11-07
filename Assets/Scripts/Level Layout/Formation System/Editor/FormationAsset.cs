using UnityEngine;
using UnityEditor;

public class FormationAsset
{
	[MenuItem("Formations/Create formation")]
	public static void CreateAsset ()
	{
		Formation newFormation = ScriptableObjectUtility.CreateAsset<Formation> ();
		newFormation.Initialize();

		Selection.activeObject = newFormation;
	}
}