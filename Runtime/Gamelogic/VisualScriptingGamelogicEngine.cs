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

		public List<VisualScriptingPlayerStatePropertyDefinition> PlayerStateDefinition;

		[Tooltip("The switches that are exposed in the Visual Scripting nodes.")]
		public VisualScriptingSwitch[] Switches;
		public VisualScriptingCoil[] Coils;
		public VisualScriptingLamp[] Lamps;
		public GamelogicEngineWire[] Wires;

		public GamelogicEngineSwitch[] AvailableSwitches => Switches.Select(sw => sw as GamelogicEngineSwitch).ToArray();

		public GamelogicEngineLamp[] AvailableLamps => Lamps.Select(lamp => lamp as GamelogicEngineLamp).ToArray();

		public GamelogicEngineCoil[] AvailableCoils => Coils.Select(c => c as GamelogicEngineCoil).ToArray();

		public GamelogicEngineWire[] AvailableWires => Wires;

		public event EventHandler<AvailableDisplays> OnDisplaysAvailable;
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
		[NonSerialized] private readonly Dictionary<int, VisualScriptingPlayerState> _playerStates = new ();

		public VisualScriptingPlayerState CurrentPlayerState {
			get {
				if (!_playerStates.ContainsKey(_currentPlayer)) {
					throw new InvalidOperationException("Must create a player state before accessing it!");
				}
				return _playerStates[_currentPlayer];
			}
		}

		public int CurrentPlayer {
			get => _currentPlayer;
			set {
				if (!_playerStates.ContainsKey(value)) {
					Debug.LogError($"Cannot change to non-existing player {value}.");
					return;
				}
				var previousPlayer = _currentPlayer;
				_currentPlayer = value;
				if (previousPlayer != _currentPlayer) {
					EventBus.Trigger(VisualScriptingEventNames.CurrentPlayerChanged, EventArgs.Empty);
				}
			}
		}

		public void CreatePlayerState(int playerId)
		{
			if (_playerStates.ContainsKey(playerId)) {
				Debug.LogWarning($"Tried to create new player state for existing state {playerId}, skipping.");
				return;
			}
			var playerState = new VisualScriptingPlayerState(playerId);
			foreach (var propertyDefinition in PlayerStateDefinition) {
				playerState.AddProperty(propertyDefinition.Instantiate());
			}
			_playerStates[playerId] = playerState;

			// switch to this state if current state is invalid
			if (!_playerStates.ContainsKey(_currentPlayer)) {
				CurrentPlayer = playerId;
			}
		}

		public void OnInit(Player player, TableApi tableApi, BallManager ballManager)
		{
			_player = player;
			BallManager = ballManager;

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
			foreach (var def in PlayerStateDefinition) {
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
