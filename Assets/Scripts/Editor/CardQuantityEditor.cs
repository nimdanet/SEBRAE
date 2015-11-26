using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomEditor(typeof(CardQuantity))]
public class CardQuantityEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		//CardQuantity myScript = target as CardQuantity;
		
		/*myScript.secondsToRestart = EditorGUILayout.FloatField("Seconds To Reset", myScript.secondsToRestart);
		
		myScript.isEditor = GUILayout.Toggle(myScript.isEditor, "Testing Mode " + ((myScript.isEditor) ? "Enabled" : "Disabled"));
		
		if(myScript.isEditor)
			myScript.startLevel = EditorGUILayout.IntField("Start Level", myScript.startLevel);*/
	}
}