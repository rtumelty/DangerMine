using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ParallaxLayer))]
public class ParallaxLayerEditor : Editor {

	public override void OnInspectorGUI() {
		ParallaxLayer parallaxLayer = target as ParallaxLayer;
		parallaxLayer.ShowGUI(this);
	}
}
