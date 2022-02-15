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
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;
using VisualPinball.Engine.Math;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Lamp Sequence")]
	[UnitSurtitle("Scene")]
	[UnitCategory("Visual Pinball")]
	public class LampSequenceUnit : GleUnit, IMultiInputUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[SerializeAs(nameof(inputCount))]
		private int _inputCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Lamp IDs")]
		public int inputCount
		{
			get => _inputCount;
			set => _inputCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		public ReadOnlyCollection<ValueInput> multiInputs { get; private set; }

		[DoNotSerialize]
		[PortLabel("Value")]
		public ValueInput Value { get; private set; }

		[DoNotSerialize]
		[PortLabel("Step")]
		public ValueInput Step;

		private int _currentIndex;
		private List<LightComponent> _lightComponentCache = null;

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			var _multiInputs = new List<ValueInput>();

			multiInputs = _multiInputs.AsReadOnly();

			for (var i = 0; i < inputCount; i++) {
				var input = ValueInput(i.ToString(), string.Empty);
				_multiInputs.Add(input);

				Requirement(input, InputTrigger);
			}

			Value = ValueInput(nameof(Value), 0f);
			Step = ValueInput(nameof(Step), 1);

			Succession(InputTrigger, OutputTrigger);

			_lightComponentCache = null;
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow))
			{
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find player.");
				return OutputTrigger;
			}

			var value = flow.GetValue<float>(Value);
			var stepRaw = flow.GetValue<int>(Step);

			if (_lightComponentCache != null) {
				foreach (var component in _lightComponentCache) {
					if (component.IsUnityNull()) {
						_lightComponentCache = null;
						break;
					}
				}
			}

			if (_lightComponentCache == null) {
				_lightComponentCache = new List<LightComponent>();

				foreach (var input in multiInputs) {
					var lampId = flow.GetValue<string>(input);
					_lightComponentCache.AddRange(Flatten(Player.LampDevice(lampId)));
				}
			}

			for (var index = 0; index < _lightComponentCache.Count; index++) {
				Player.Lamp(_lightComponentCache[index]).OnLamp(
					index >= _currentIndex * stepRaw && index < (_currentIndex + 1) * stepRaw ? value : 0, ColorChannel.Alpha); ;
			}

			if (++_currentIndex >= _lightComponentCache.Count / stepRaw) {
				_currentIndex = 0;
			}

			return OutputTrigger;
		}

		private List<LightComponent> Flatten(List<ILampDeviceComponent> lampDeviceList)
		{
			List<LightComponent> lights = new List<LightComponent>();

			foreach (ILampDeviceComponent device in lampDeviceList) {
				if (device is LightComponent) {
					lights.Add(device as LightComponent);
				}
				else if (device is LightGroupComponent) {
					lights.AddRange(Flatten(((LightGroupComponent)device).Lights));
				}
			}

			return lights;
		}
	}
}
