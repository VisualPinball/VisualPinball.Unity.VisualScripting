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

// ReSharper disable AssignmentInConditionalExpression

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VisualPinball.Unity.Editor;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[CustomEditor(typeof(VisualScriptingGamelogicEngine))]
	public class VisualScriptingGamelogicEngineInspector : BaseEditor<VisualScriptingGamelogicEngine>
	{
		private VisualScriptingGamelogicEngine _gle;
		private SerializedProperty _displaysProperty;
		private SerializedProperty _switchesProperty;
		private SerializedProperty _soilsProperty;
		private SerializedProperty _lampsProperty;
		private SerializedProperty _tableVariableDefinitionsProperty;
		private SerializedProperty _playerVariableDefinitionsProperty;

		private readonly Dictionary<int, bool> _playerVarFoldout = new();

		private void OnEnable()
		{
			_gle = target as VisualScriptingGamelogicEngine;

			_displaysProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.Displays));
			_switchesProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.Switches));
			_soilsProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.Coils));
			_lampsProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.Lamps));

			_tableVariableDefinitionsProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.TableVariableDefinitions));
			_playerVariableDefinitionsProperty = serializedObject.FindProperty(nameof(VisualScriptingGamelogicEngine.PlayerVariableDefinitions));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_displaysProperty);
			EditorGUILayout.PropertyField(_switchesProperty);
			EditorGUILayout.PropertyField(_soilsProperty);
			EditorGUILayout.PropertyField(_lampsProperty);

			EditorGUILayout.PropertyField(_tableVariableDefinitionsProperty);
			EditorGUILayout.PropertyField(_playerVariableDefinitionsProperty);

			serializedObject.ApplyModifiedProperties();

			// what follows is runtime data
			if (!Application.isPlaying) {
				return;
			}

			EditorGUILayout.TextField("Table Variables", new GUIStyle(EditorStyles.boldLabel));
			PrintState(_gle.TableState, _gle.TableVariableDefinitions);

			if (_gle.PlayerStates.Count == 0) {
				EditorGUILayout.HelpBox("No player states created.", MessageType.Info);
				return;
			}

			EditorGUILayout.TextField("Player States", new GUIStyle(EditorStyles.boldLabel));
			foreach (var playerId in _gle.PlayerStates.Keys) {
				if (!_playerVarFoldout.ContainsKey(playerId)) {
					_playerVarFoldout[playerId] = true;
				}
				if (_playerVarFoldout[playerId] = EditorGUILayout.BeginFoldoutHeaderGroup(_playerVarFoldout[playerId], $"Player {playerId}")) {
					EditorGUI.indentLevel++;

					if (_gle.CurrentPlayerState == _gle.PlayerStates[playerId]) {
						EditorGUILayout.HelpBox("Current Player", MessageType.Info);
					}

					PrintState(_gle.PlayerStates[playerId], _gle.PlayerVariableDefinitions);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFoldoutHeaderGroup();
			}
		}

		private static void PrintState(State state, IEnumerable<VariableDefinition> definitions)
		{
			foreach (var varDef in definitions) {
				EditorGUILayout.LabelField(varDef.Name, state.Get(varDef.Id).ToString());
			}
		}
	}
}
