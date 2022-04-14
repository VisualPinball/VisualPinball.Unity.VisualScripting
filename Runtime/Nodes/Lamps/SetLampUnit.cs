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
	[UnitShortTitle("Set Lamp")]
	[UnitTitle("Set Lamp")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class SetLampUnit : GleUnit, IMultiInputUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public LampDataType DataType { get; set; }

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
		public ValueInput Value { get; private set; }

		[DoNotSerialize]
		private readonly Dictionary<string, float> _intensityMultipliers = new();

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

			Value = DataType switch {
				LampDataType.OnOff => ValueInput(nameof(Value), false),
				LampDataType.Status => ValueInput(nameof(Value), LampStatus.Off),
				LampDataType.Intensity => ValueInput(nameof(Value), 0f),
				LampDataType.Color => ValueInput(nameof(Value), UnityEngine.Color.white),
				_ => throw new ArgumentOutOfRangeException()
			};

			Succession(InputTrigger, OutputTrigger);
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

			foreach (var input in multiInputs) {
				var lampId = flow.GetValue<string>(input);
				switch (DataType) {
					case LampDataType.OnOff:
						Player.SetLamp(lampId, flow.GetValue<bool>(Value) ? LampStatus.On : LampStatus.Off);
						break;
					case LampDataType.Status:
						Player.SetLamp(lampId, flow.GetValue<LampStatus>(Value));
						break;
					case LampDataType.Intensity:
						Player.SetLamp(lampId, flow.GetValue<float>(Value) * GetIntensityMultiplier(lampId));
						break;
					case LampDataType.Color:
						Player.SetLamp(lampId, flow.GetValue<UnityEngine.Color>(Value).ToEngineColor());
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
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
	}
}
