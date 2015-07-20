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
	[MenuItem("Assets/Formations/Create profile")]
	public static void CreateProfileAsset ()
	{
		FormationProfile newProfile = ScriptableObjectUtility.CreateAsset<FormationProfile> ();
		newProfile.Initialize();
		
		Selection.activeObject = newProfile;
	}
}