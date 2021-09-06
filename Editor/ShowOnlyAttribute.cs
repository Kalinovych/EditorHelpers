using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
public class ShowOnlyAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof(ShowOnlyAttribute) )]
public class ShowOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		string valueStr;

		switch (prop.propertyType)
		{
			case SerializedPropertyType.Integer:
				valueStr = prop.intValue.ToString();
				break;
			case SerializedPropertyType.Boolean:
				valueStr = prop.boolValue.ToString();
				break;
			case SerializedPropertyType.Float:
				valueStr = prop.floatValue.ToString( "0.00000" );
				break;
			case SerializedPropertyType.String:
				valueStr = prop.stringValue;
				break;
			case SerializedPropertyType.Enum:
				valueStr = prop.enumDisplayNames[prop.enumValueIndex];
				break;
			case SerializedPropertyType.ObjectReference:
				valueStr = null;

				EditorGUI.BeginDisabledGroup( true );
				EditorGUI.ObjectField( position, prop, new GUIContent( label.text ) );
				EditorGUI.EndDisabledGroup();
				break;
			default:
				valueStr = "(not supported type)";
				break;
		}

		if (valueStr is { }) {
			EditorGUI.LabelField( position, label.text, valueStr );
		}
	}
}
#endif
