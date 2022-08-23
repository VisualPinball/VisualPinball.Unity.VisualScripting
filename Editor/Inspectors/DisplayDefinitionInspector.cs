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
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Inspector(typeof(DisplayDefinition))]
	public class DisplayDefinitionInspector : GleInspector
	{
		public DisplayDefinitionInspector(Metadata metadata) : base(metadata) { }

		protected override void OnGUI(Rect position, GUIContent label)
		{
			// can't get this from the flow
			var gle = Gle;
			if (gle != null) {
				if (gle.Displays == null || gle.Displays.Count(p => !string.IsNullOrEmpty(p.Id)) == 0) {
					ErrorMessage = "No displays defined.";

				} else {
					var varNames = new List<string> { "None" };

					if (metadata.HasAttribute<DisplayTypeFilterAttribute>()) {
						varNames.AddRange(gle.Displays.Where(i => metadata.GetAttribute<DisplayTypeFilterAttribute>().types.Contains(i.Type)).Select(d => d.Id));
					}
					else {
						varNames.AddRange(gle.Displays.Select(d => d.Id));
					}

					var currentDisplayDef = metadata.value as DisplayDefinition;
					var currentIndex = 0;
					if (currentDisplayDef != null) {
						var displayDef = gle.Displays.FirstOrDefault(p => p.Id == currentDisplayDef!.Id);
						currentIndex = displayDef != null ? Array.IndexOf(gle.Displays, displayDef) + 1 : 0;
					}

					var newIndex = EditorGUI.Popup(position, currentIndex, varNames.ToArray());
					metadata.RecordUndo();
					metadata.value = newIndex == 0 ? null : gle.Displays[newIndex - 1];
					ErrorMessage = null;
				}
			}

			if (ErrorMessage != null) {
				position.height -= EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.HelpBox(position, ErrorMessage, MessageType.Error);
			}
		}

		public override float GetAdaptiveWidth() => LudiqGUIUtility.currentInspectorWidth;

		protected override float GetHeight(float width, GUIContent label)
		{
			if (ErrorMessage != null) {
				var height = LudiqGUIUtility.GetHelpBoxHeight(ErrorMessage, MessageType.Error, width);
				height += EditorGUIUtility.standardVerticalSpacing;
				return height;
			}

			return EditorGUIUtility.singleLineHeight;
		}
	}
}
