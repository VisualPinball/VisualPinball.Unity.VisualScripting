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
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;

namespace VisualPinball.Unity.VisualScripting
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Visual Pinball/Gamelogic Engine/Visual Scripting Game Logic")]
	public class VisualScriptingGamelogicEngine : MonoBehaviour, IGamelogicEngine
	{
		public string Name => "Visual Scripting Gamelogic Engine";

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
	}
}
