// Visual Pinball Engine
// Copyright (C) 2022 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using UnityEditor;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[CustomPropertyDrawer(typeof(PlayerVariableDefinition))]
	public class PlayerVariableDefinitionPropertyDrawer : PropertyDrawer
	{
		private const float Padding = 2f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 3 * (EditorGUIUtility.singleLineHeight + Padding);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var nameProperty = property.FindPropertyRelative(nameof(PlayerVariableDefinition.Name));
			var typeProperty = property.FindPropertyRelative(nameof(PlayerVariableDefinition.Type));
			var typeIndex = typeProperty.enumValueIndex;

			SerializedProperty valueProp;
			switch (typeIndex) {
				case (int)VariableType.String:
					valueProp = property.FindPropertyRelative(nameof(PlayerVariableDefinition.StringDefaultValue));
					break;
				case (int)VariableType.Integer:
					valueProp = property.FindPropertyRelative(nameof(PlayerVariableDefinition.IntegerDefaultValue));
					break;
				case (int)VariableType.Float:
					valueProp = property.FindPropertyRelative(nameof(PlayerVariableDefinition.FloatDefaultValue));
					break;
				case (int)VariableType.Boolean:
					valueProp = property.FindPropertyRelative(nameof(PlayerVariableDefinition.BooleanDefaultValue));
					break;
				default:
					throw new ArgumentException($"Undefined type index {typeIndex}.");
			}

			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(position, nameProperty, new GUIContent("Name:"));
			position.y += EditorGUIUtility.singleLineHeight + Padding;
			EditorGUI.PropertyField(position, typeProperty, new GUIContent("Type:"));
			position.y += EditorGUIUtility.singleLineHeight + Padding;
			EditorGUI.PropertyField(position, valueProp, new GUIContent("Initial Value:"));

			EditorGUI.EndProperty();
		}
	}
}

