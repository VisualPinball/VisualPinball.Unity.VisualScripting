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

// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;

namespace VisualPinball.Unity.VisualScripting
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Visual Pinball/Gamelogic Engine/Visual Scripting Game Logic")]
	public class VisualScriptingGamelogicEngine : MonoBehaviour, IGamelogicEngine, ISerializationCallbackReceiver
	{
		public string Name => "Visual Scripting Gamelogic Engine";

		[Tooltip("Define here the global variables of the Visual Scripting engine.")]
		public List<TableVariableDefinition> TableVariableDefinitions;

		[Tooltip("Define here the player-specific variables of the Visual Scripting engine.")]
		public List<PlayerVariableDefinition> PlayerVariableDefinitions;

		[Tooltip("Define the displays this game is going to use.")]
		public DisplayDefinition[] Displays;

		[Tooltip("The switches that are exposed in the Visual Scripting nodes.")]
		public VisualScriptingSwitch[] Switches;

		[Tooltip("The coils that are exposed in the Visual Scripting nodes.")]
		public VisualScriptingCoil[] Coils;

		[Tooltip("The lamps that are exposed in the Visual Scripting nodes.")]
		public VisualScriptingLamp[] Lamps;
		public GamelogicEngineWire[] Wires;

		public DisplayConfig[] RequiredDisplays => Displays.Select(d => d.DisplayConfig).ToArray();

		public GamelogicEngineSwitch[] RequestedSwitches => Switches.Select(sw => sw as GamelogicEngineSwitch).ToArray();

		public GamelogicEngineLamp[] RequestedLamps => Lamps.Select(lamp => lamp as GamelogicEngineLamp).ToArray();

		public GamelogicEngineCoil[] RequestedCoils => Coils.Select(c => c as GamelogicEngineCoil).ToArray();

		public GamelogicEngineWire[] AvailableWires => Wires;

		public event EventHandler<RequestedDisplays> OnDisplaysRequested;
		public event EventHandler<DisplayFrameData> OnDisplayFrame;
		public event EventHandler<LampEventArgs> OnLampChanged;
		public event EventHandler<LampsEventArgs> OnLampsChanged;
		public event EventHandler<LampColorEventArgs> OnLampColorChanged;
		public event EventHandler<CoilEventArgs> OnCoilChanged;
		public event EventHandler<SwitchEventArgs2> OnSwitchChanged;
		public event EventHandler<EventArgs> OnStarted;

		[NonSerialized] public BallManager BallManager;
		[NonSerialized] private Player _player;

		[NonSerialized] private int _currentPlayer;
		[NonSerialized] public readonly TableState TableState = new ();
		[NonSerialized] public readonly Dictionary<int, PlayerState> PlayerStates = new ();

		public PlayerState CurrentPlayerState {
			get {
				if (!PlayerStates.ContainsKey(_currentPlayer)) {
					throw new InvalidOperationException("Must create a player state before accessing it!");
				}
				return PlayerStates[_currentPlayer];
			}
		}

		public void DisplayFrame(DisplayFrameData data)
		{
			OnDisplayFrame?.Invoke(this, data);
		}

		public void SetCurrentPlayer(int value, bool forceNotify = false)
		{
			if (!PlayerStates.ContainsKey(value)) {
				Debug.LogError($"Cannot change to non-existing player {value}.");
				return;
			}
			var previousPlayer = _currentPlayer;
			_currentPlayer = value;
			if (forceNotify || previousPlayer != _currentPlayer) {
				EventBus.Trigger(VisualScriptingEventNames.CurrentPlayerChanged, EventArgs.Empty);
			}

			// also trigger updates for each variable
			foreach (var varDef in PlayerVariableDefinitions) {
				if (PlayerStates.ContainsKey(previousPlayer)) {
					var before = PlayerStates[previousPlayer].GetVariable(varDef.Id);
					var now = PlayerStates[_currentPlayer].GetVariable(varDef.Id);
					if (forceNotify || before != now) {
						EventBus.Trigger(VisualScriptingEventNames.PlayerVariableChanged, new VariableChangedArgs(varDef.Id));
					}

				} else {
					EventBus.Trigger(VisualScriptingEventNames.PlayerVariableChanged, new VariableChangedArgs(varDef.Id));
				}
			}
		}

		public void CreatePlayerState(int playerId)
		{
			if (PlayerStates.ContainsKey(playerId)) {
				Debug.LogWarning($"Tried to create new player state for existing state {playerId}, skipping.");
				return;
			}
			var playerState = new PlayerState(playerId);
			foreach (var propertyDefinition in PlayerVariableDefinitions) {
				playerState.AddProperty(propertyDefinition.Instantiate());
			}
			PlayerStates[playerId] = playerState;

			// switch to this state if current state is invalid
			if (!PlayerStates.ContainsKey(_currentPlayer)) {
				SetCurrentPlayer(playerId, true);
			}
		}

		public void DestroyPlayerStates()
		{
			PlayerStates.Clear();
			_currentPlayer = 0;
		}

		public void OnInit(Player player, TableApi tableApi, BallManager ballManager)
		{
			_player = player;
			BallManager = ballManager;

			// request displays
			OnDisplaysRequested?.Invoke(this, new RequestedDisplays(Displays.Select(d => d.DisplayConfig).ToArray()));

			// create table variables
			foreach (var propertyDefinition in TableVariableDefinitions) {
				TableState.AddProperty(propertyDefinition.Instantiate());
			}
			OnStarted?.Invoke(this, EventArgs.Empty);
			EventBus.Trigger(VisualScriptingEventNames.GleStartedEvent, EventArgs.Empty);
		}

		public void Switch(string id, bool isClosed)
		{
			var args = new SwitchEventArgs2(id, isClosed);

			OnSwitchChanged?.Invoke(this, args);

			EventBus.Trigger(VisualScriptingEventNames.SwitchEvent, args);
		}

		public void SetCoil(string id, bool isEnabled)
		{
			OnCoilChanged?.Invoke(this, new CoilEventArgs(id, isEnabled));
		}

		public void SetLamp(string id, float value, bool isCoil = false, LampSource source = LampSource.Lamp)
		{
			OnLampChanged?.Invoke(this, new LampEventArgs(id, value, isCoil, source));
		}

		public void SetLamp(string id, Color color)
		{
			OnLampColorChanged?.Invoke(this, new LampColorEventArgs(id, color));
		}

		public float GetLamp(string id)
		{
			return _player.LampStatuses.ContainsKey(id) ? _player.LampStatuses[id] : 0;
		}

		public bool GetSwitch(string id)
		{
			return _player.SwitchStatuses.ContainsKey(id) && _player.SwitchStatuses[id].IsSwitchEnabled;
		}

		public bool GetCoil(string id)
		{
			return _player.CoilStatuses.ContainsKey(id) && _player.CoilStatuses[id];
		}


		public void OnBeforeSerialize()
		{
			#if UNITY_EDITOR

			var ids = new HashSet<string>();
			foreach (var def in PlayerVariableDefinitions) {
				if (!def.HasId || ids.Contains(def.Id)) {
					def.GenerateId();
				}
				ids.Add(def.Id);
			}
			ids.Clear();
			foreach (var def in TableVariableDefinitions) {
				if (!def.HasId || ids.Contains(def.Id)) {
					def.GenerateId();
				}
				ids.Add(def.Id);
			}
			#endif
		}
		public void OnAfterDeserialize()
		{
		}
	}
}

