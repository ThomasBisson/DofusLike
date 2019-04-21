using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GreyOut : PropertyAttribute
{

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(GreyOut))]
public class GrayOutDrawer : PropertyDrawer
{
public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
{
    return EditorGUI.GetPropertyHeight(property, label, true);
}

public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    GUI.enabled = false;
    EditorGUI.PropertyField(position, property, label, true);
    GUI.enabled = true;
}
}

#endif
