// // Visual Pinball Engine
// // Copyright (C) 2022 freezy and VPE Team
// //
// // This program is free software: you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation, either version 3 of the License, or
// // (at your option) any later version.
// //
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License
// // along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// using UnityEditor;
// using UnityEngine;
//
// namespace VisualPinball.Unity.VisualScripting.Editor
// {
// 	[CustomPropertyDrawer(typeof(DisplayDefinition))]
// 	public class DisplayDefinitionPropertyDrawer : PropertyDrawer
// 	{
// 		private const float Padding = 2f;
//
// 		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
// 		{
// 			return base.GetPropertyHeight(property, label);
// 			// var f = property.FindPropertyRelative(nameof(DisplayDefinition.SupportedFormats));
// 			//
// 			// return 5 * (EditorGUIUtility.singleLineHeight + Padding);
// 		}
//
// 		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
// 		{
// 			var idProperty = property.FindPropertyRelative(nameof(DisplayDefinition.Id));
// 			var widthProperty = property.FindPropertyRelative(nameof(DisplayDefinition.Width));
// 			var heightProperty = property.FindPropertyRelative(nameof(DisplayDefinition.Height));
//
// 			var contentPosition = position;
// 			contentPosition.height = EditorGUIUtility.singleLineHeight;
//
// 			//EditorGUI.BeginProperty(position, label, property);
// 			EditorGUI.PropertyField(contentPosition, idProperty, new GUIContent("ID:"));
// 			//EditorGUI.EndProperty();
// 			position.y += EditorGUIUtility.singleLineHeight + Padding;
//
// 			contentPosition = EditorGUI.PrefixLabel(position, new GUIContent("Size:"));
// 			contentPosition.height = EditorGUIUtility.singleLineHeight;
//
// 			var half = contentPosition.width / 2;
// 			GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);
//
// 			//show the X and Y from the point
// 			var oldLabelWidth = EditorGUIUtility.labelWidth;
// 			EditorGUIUtility.labelWidth = 14f;
// 			contentPosition.width *= 0.5f;
// 			EditorGUI.indentLevel = 0;
//
// 			// Begin/end property & change check make each field
// 			// behave correctly when multi-object editing.
// 			EditorGUI.BeginProperty(contentPosition, label, widthProperty);
// 			{
// 				EditorGUI.BeginChangeCheck();
// 				var newVal = EditorGUI.IntField(contentPosition, new GUIContent("W"), widthProperty.intValue);
// 				if (EditorGUI.EndChangeCheck())
// 					widthProperty.intValue = newVal;
// 			}
// 			EditorGUI.EndProperty();
//
// 			contentPosition.x += half;
// 			EditorGUI.BeginProperty(contentPosition, label, heightProperty);
// 			{
// 				EditorGUI.BeginChangeCheck();
// 				var newVal = EditorGUI.IntField(contentPosition, new GUIContent("H"), heightProperty.intValue);
// 				if (EditorGUI.EndChangeCheck())
// 					heightProperty.intValue = newVal;
// 			}
// 			EditorGUI.EndProperty();
//
// 			EditorGUIUtility.labelWidth = oldLabelWidth;
//
// 			var supportedFormatsProperty = property.FindPropertyRelative(nameof(DisplayDefinition.SupportedFormats));
//
// 			position.y += EditorGUIUtility.singleLineHeight + Padding;
// 			contentPosition = position;
// 			contentPosition.height = EditorGUIUtility.singleLineHeight;
// 			EditorGUI.PropertyField(contentPosition, supportedFormatsProperty, new GUIContent("Supported Formats:"));
// 		}
// 	}
// }
//
