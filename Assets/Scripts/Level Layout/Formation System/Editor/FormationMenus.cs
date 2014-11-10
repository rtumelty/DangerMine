using UnityEngine;
using UnityEditor;

public class FormationMenus
{
	[MenuItem("Assets/Formations/Create formation")]
	public static void CreateFormationAsset ()
	{
		Formation newFormation = ScriptableObjectUtility.CreateAsset<Formation> ();
		newFormation.Initialize();

		Selection.activeObject = newFormation;
	}
	
	[MenuItem("Assets/Formations/Create sequence")]
	public static void CreateSequenceAsset ()
	{
		Sequence newSequence = ScriptableObjectUtility.CreateAsset<Sequence> ();
		
		Selection.activeObject = newSequence;
	}
}