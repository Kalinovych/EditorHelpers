using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorHelpers
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class EditorButtonAttribute : Attribute
	{
		public readonly ButtonMode mode = ButtonMode.AlwaysEnabled;

		public EditorButtonAttribute() { }

		public EditorButtonAttribute(ButtonMode mode)
		{
			this.mode = mode;
		}
	}

	public enum ButtonMode
	{
		AlwaysEnabled,
		EnabledInPlayMode,
		DisabledInPlayMode
	}

	#if UNITY_EDITOR
	[CanEditMultipleObjects]
	[CustomEditor( typeof(UnityEngine.Object), true )]
	public class EditorButtonEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var targetType = target.GetType();
			var methods = targetType
				.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
				.Where( m => m.GetParameters().Length == 0 );

			// Loop through all methods with no parameters
			foreach (var method in methods)
			{
				var attributes = method.GetCustomAttributes( typeof(EditorButtonAttribute), true );
				if (attributes.Length > 0)
				{
					var attr = (EditorButtonAttribute) attributes[0];
					// Determine whether the button should be enabled based on its mode
					GUI.enabled = attr.mode == ButtonMode.AlwaysEnabled
								|| (EditorApplication.isPlaying
									? attr.mode == ButtonMode.EnabledInPlayMode
									: attr.mode == ButtonMode.DisabledInPlayMode);

					// Draw a button which invokes the method
					if (GUILayout.Button( ObjectNames.NicifyVariableName( method.Name ) ))
					{
						foreach (var script in targets)
						{
							method.Invoke( script, null );
							
							if (!Application.isPlaying)
								EditorUtility.SetDirty( script );
						}
					}

					GUI.enabled = true;
				}
			}

			// Draw the rest of the inspector as usual
			DrawDefaultInspector();
		}
	}
	#endif
}