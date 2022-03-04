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
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;
using Color = VisualPinball.Engine.Math.Color;

namespace VisualPinball.Unity.VisualScripting
{
	public struct LightComponentMapping
	{
		public LightComponent lightComponent;
		public string id;

		public bool IsValid() => !lightComponent.IsUnityNull();
	}

	[UnitTitle("Lamp Sequence")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class LampSequenceUnit : GleUnit, IMultiInputUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("Value")]
		public LampDataType ValueDataType { get; set; }

		[Serialize, Inspectable, UnitHeaderInspectable("Non Step Value")]
		public LampDataType NonStepValueDataType { get; set; }

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Lamp IDs")]
		public int inputCount
		{
			get => _inputCount;
			set => _inputCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[SerializeAs(nameof(inputCount))]
		private int _inputCount = 1;

		[DoNotSerialize]
		public ReadOnlyCollection<ValueInput> multiInputs { get; private set; }

		[DoNotSerialize]
		[PortLabel("Step")]
		public ValueInput Step;

		[DoNotSerialize]
		public ValueInput Value { get; private set; }

		[DoNotSerialize]
		public ValueInput NonStepValue { get; private set; }

		[DoNotSerialize]
		private readonly Dictionary<string, float> _intensityMultipliers = new();

		private List<LightComponentMapping> _lightComponentCache = new();
		private int _currentIndex;

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			var mi = new List<ValueInput>();
			multiInputs = mi.AsReadOnly();

			for (var i = 0; i < inputCount; i++) {
				var input = ValueInput(i.ToString(), string.Empty);
				mi.Add(input);
				Requirement(input, InputTrigger);
			}

			Step = ValueInput(nameof(Step), 1);

			Value = ValueDataType switch
			{
				LampDataType.OnOff => ValueInput(nameof(Value), false),
				LampDataType.Status => ValueInput(nameof(Value), LampStatus.Off),
				LampDataType.Intensity => ValueInput(nameof(Value), 0f),
				LampDataType.Color => ValueInput(nameof(Value), UnityEngine.Color.white),
				_ => throw new ArgumentOutOfRangeException()
			};

			NonStepValue = NonStepValueDataType switch
			{
				LampDataType.OnOff => ValueInput(nameof(NonStepValue), false),
				LampDataType.Status => ValueInput(nameof(NonStepValue), LampStatus.Off),
				LampDataType.Intensity => ValueInput(nameof(NonStepValue), 0f),
				LampDataType.Color => ValueInput(nameof(NonStepValue), UnityEngine.Color.white),
				_ => throw new ArgumentOutOfRangeException()
			};

			Succession(InputTrigger, OutputTrigger);

			_lightComponentCache.Clear();
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find player.");
				return OutputTrigger;
			}

			foreach (var mapping in _lightComponentCache) {
				if (!mapping.IsValid()) {
					_lightComponentCache.Clear();
					break;
				}
			}

			if (_lightComponentCache.Count == 0) {
				foreach (var input in multiInputs) {
					var lampId = flow.GetValue<string>(input);

					var mappingList = Player.LampMapping.Where(l => l.Id == lampId);
					if (mappingList.Any()) {
						foreach (var mapping in mappingList) {
							UpdateLightComponentCache(mapping.Device, lampId);
						}
					}
					else {
						Debug.LogError($"Unknown lamp ID {lampId}.");
						_lightComponentCache.Clear();

						break;
					}
				}
			}

			var stepRaw = flow.GetValue<int>(Step);

			LampDataType dataType;
			ValueInput value;

			for (var index = 0; index < _lightComponentCache.Count; index++) {
				if (index >= _currentIndex * stepRaw && index < (_currentIndex + 1) * stepRaw) {
					dataType = ValueDataType;
					value = Value;
				}
				else {
					dataType = NonStepValueDataType;
					value = NonStepValue;
				}

				var lampApi = Player.Lamp(_lightComponentCache[index].lightComponent);

				switch (dataType) {
					case LampDataType.OnOff:
						lampApi.OnLamp(flow.GetValue<bool>(value) ? LampStatus.On : LampStatus.Off);
						break;
					case LampDataType.Status:
						lampApi.OnLamp(flow.GetValue<LampStatus>(value));
						break;
					case LampDataType.Intensity:
						lampApi.OnLamp(flow.GetValue<float>(value) * GetIntensityMultiplier(_lightComponentCache[index].id));
						break;
					case LampDataType.Color:
						lampApi.OnLamp(flow.GetValue<UnityEngine.Color>(value));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (++_currentIndex >= _lightComponentCache.Count / stepRaw) {
				_currentIndex = 0;
			}
			
			return OutputTrigger;
		}

		private float GetIntensityMultiplier(string id)
		{
			if (_intensityMultipliers.ContainsKey(id)) {
				return _intensityMultipliers[id];
			}

			var mapping = Player.LampMapping.FirstOrDefault(l => l.Id == id);
			if (mapping == null) {
				Debug.LogError($"Unknown lamp ID {id}.");
				_intensityMultipliers[id] = 1;
				return 1;
			}

			_intensityMultipliers[id] = mapping.Type == LampType.Rgb ? 255 : 1;
			return _intensityMultipliers[id];
		}

		private void UpdateLightComponentCache(ILampDeviceComponent device, string id)
		{
			if (device is LightComponent) {
				_lightComponentCache.Add(new LightComponentMapping { lightComponent = device as LightComponent, id = id });
			}
			else if (device is LightGroupComponent) {
				foreach (var light in (device as LightGroupComponent).Lights) {
					UpdateLightComponentCache(light, id);
				}
			}
		}
	}
}
