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

using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using VisualPinball.Unity.VisualScripting;

[Inspector(typeof(VisualScriptingPlayerStatePropertyDefinition))]
public class VisualScriptingPlayerStatePropertyDefinitionInspector : GleInspector
{
	public VisualScriptingPlayerStatePropertyDefinitionInspector(Metadata metadata) : base(metadata) { }

	protected override void OnGUI(Rect position, GUIContent label)
	{
		// can't get this from the flow
		var gle = Gle;
		if (gle != null) {
			var propDefinitions = gle.PlayerStateDefinition;

			if (propDefinitions.Count == 0) {
				ErrorMessage = "No properties defined.";

			} else {
				var propNames = propDefinitions.Select(d => d.Name).ToArray();
				var currentPropDef = metadata.value as VisualScriptingPlayerStatePropertyDefinition;
				var playerPropDef = propDefinitions.FirstOrDefault(p => p.Id == currentPropDef!.Id);
				var currentIndex = playerPropDef != null ? propDefinitions.IndexOf(playerPropDef) : 0;

				var newIndex = EditorGUI.Popup(position, currentIndex, propNames);
				if (EndBlock(metadata))
				{
					metadata.RecordUndo();
					metadata.value = propDefinitions[newIndex];
				}
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
