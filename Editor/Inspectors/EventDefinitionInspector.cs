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

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Inspector(typeof(EventDefinition))]
	public class EventDefinitionInspector : GleInspector
	{
		public EventDefinitionInspector(Metadata metadata) : base(metadata) { }

		protected override void OnGUI(Rect position, GUIContent label)
		{
			// can't get this from the flow
			var gle = Gle;
			if (gle != null) {
				var eventDefinitions = gle.EventDefinitions;
				if (eventDefinitions == null || eventDefinitions.Count(p => !string.IsNullOrEmpty(p.Name)) == 0) {
					ErrorMessage = "No events defined.";

				} else {
					var eventNames = new List<string> { "None" }
						.Concat(eventDefinitions.Select(d => d.Name))
						.ToArray();
					var currentEventDef = metadata.value as EventDefinition;
					var currentIndex = 0;
					if (currentEventDef != null) {
						var eventDef = eventDefinitions.FirstOrDefault(p => p.Id == currentEventDef!.Id);
						currentIndex = eventDef != null ? eventDefinitions.IndexOf(eventDef) + 1 : 0;
					}

					var newIndex = EditorGUI.Popup(position, currentIndex, eventNames);
					metadata.RecordUndo();
					metadata.value = newIndex == 0 ? null : eventDefinitions[newIndex - 1];
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
