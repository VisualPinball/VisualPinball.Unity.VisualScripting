// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
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
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(IGamelogicEngine))]
	[RequireComponent(typeof(Player))]
	[AddComponentMenu("Visual Pinball/Gamelogic Engine/Visual Scripting Bridge")]
	public class VisualScriptingGamelogicBridge : MonoBehaviour
	{
		private Player _player;
		private IGamelogicEngine _gle;

		private bool _init;

		private void Awake()
		{
			_init = false;

			_player = GetComponent<Player>();
			if (_player == null) {
				Debug.LogWarning("Cannot find player.");
			}

			_gle = GetComponent<IGamelogicEngine>();
			if (_gle != null) {
				_gle.OnStarted += OnStarted;
			}
			else { 
				Debug.LogWarning("Cannot find gamelogic engine.");
			}
		}

		private void OnDestroy() { 
			if (_gle != null) {
				_gle.OnStarted -= OnStarted;

				if (_init) {
					_gle.OnSwitchChanged -= OnSwitchChanged;
					_gle.OnCoilChanged -= OnCoilChanged;
					_gle.OnLampChanged -= OnLampChanged;
				}
			}
		}

		private void OnStarted(object sender, EventArgs e) 
		{
			if (_gle != null) {
				_gle.OnSwitchChanged += OnSwitchChanged;
				_gle.OnCoilChanged += OnCoilChanged;
				_gle.OnLampChanged += OnLampChanged;

				_init = true;

				EventBus.Trigger(VisualScriptingEventNames.GleStartedEvent, e);
			}
		}

		private static void OnSwitchChanged(object sender, SwitchEventArgs2 e)
		{
			EventBus.Trigger(VisualScriptingEventNames.SwitchEvent, new SwitchEventArgs2(e.Id, e.IsEnabled));
		}

		private static void OnCoilChanged(object sender, CoilEventArgs e)
		{
			EventBus.Trigger(VisualScriptingEventNames.CoilEvent, e);
		}

		private static void OnLampChanged(object sender, LampEventArgs e)
		{
			EventBus.Trigger(VisualScriptingEventNames.LampEvent, e);
		}
	}
}
