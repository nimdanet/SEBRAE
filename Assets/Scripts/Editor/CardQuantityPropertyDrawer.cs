using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomPropertyDrawer(typeof(CardQuantity))]
public class CardQuantityPropertyDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = label.text.Replace ("Element", "Card");
		int element = int.Parse(label.text.Substring (label.text.Length - 1));
		label.text = label.text.Substring (0, label.text.Length - 1) + (element + 1);

		EditorGUIUtility.labelWidth = position.width * 0.3f;

		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		#region Card
		Rect cardRect = new Rect (position.x, position.y, position.width * 0.65f, position.height);
		EditorGUIUtility.labelWidth = cardRect.width * 0.2f;
		EditorGUI.PropertyField (cardRect, property.FindPropertyRelative ("card"), new GUIContent("Card"));
		#endregion
		
		#region quantity
		Rect quantityRect = new Rect (cardRect.x + cardRect.width, position.y, position.width * 0.25f, position.height);
		EditorGUIUtility.labelWidth = quantityRect.width * 0.5f;
		EditorGUI.PropertyField (quantityRect, property.FindPropertyRelative ("qnty"), new GUIContent("Qnty"));
		#endregion

		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
}
