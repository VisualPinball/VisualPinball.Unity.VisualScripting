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
	[UnitShortTitle("Switch Lamp")]
	[UnitTitle("Switch Lamp (ID, match value)")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class SwitchLampUnit : GleUnit, IMultiInputUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("Match")]
		public LampDataType MatchDataType { get; set; }

		[Serialize, Inspectable, UnitHeaderInspectable("Non Match")]
		public LampDataType NonMatchDataType { get; set; }

		[Serialize, Inspectable, UnitHeaderInspectable("Value Compare")]
		public CompareType ValueCompareType { get; set; }

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
		[PortLabel("Source Value")]
		public ValueInput SourceValue { get; private set; }

		[DoNotSerialize]
		public ValueInput Match { get; private set; }

		[DoNotSerialize]
		public ValueInput NonMatch { get; private set; }

		[DoNotSerialize]
		private readonly Dictionary<string, float> _intensityMultipliers = new();

		private Dictionary<int, LampIdValue> _lampIdValueCache = new Dictionary<int, LampIdValue>();

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			SourceValue = ValueInput<int>(nameof(SourceValue));

			var mi = new List<ValueInput>();
			multiInputs = mi.AsReadOnly();

			for (var i = 0; i < inputCount; i++) {
				var input = ValueInput(i.ToString(), LampIdValue.Empty.ToJson());
				mi.Add(input);
				Requirement(input, InputTrigger);
			}

			_lampIdValueCache.Clear();

			Match = MatchDataType switch {
				LampDataType.OnOff => ValueInput(nameof(Match), false),
				LampDataType.Status => ValueInput(nameof(Match), LampStatus.Off),
				LampDataType.Intensity => ValueInput(nameof(Match), 0f),
				LampDataType.Color => ValueInput(nameof(Match), UnityEngine.Color.white),
				_ => throw new ArgumentOutOfRangeException()
			};

			NonMatch = NonMatchDataType switch {
				LampDataType.OnOff => ValueInput(nameof(NonMatch), false),
				LampDataType.Status => ValueInput(nameof(NonMatch), LampStatus.Off),
				LampDataType.Intensity => ValueInput(nameof(NonMatch), 0f),
				LampDataType.Color => ValueInput(nameof(NonMatch), UnityEngine.Color.white),
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

			var sourceValue = flow.GetValue<int>(SourceValue);

			foreach (var input in multiInputs) {
				var json = flow.GetValue<string>(input);

				if (!_lampIdValueCache.ContainsKey(json.GetHashCode())) {
					_lampIdValueCache[json.GetHashCode()] = LampIdValue.FromJson(json);
				}

				var lampIdValue = _lampIdValueCache[json.GetHashCode()];

				var match = false;

				switch(ValueCompareType)
				{
					case CompareType.NotEqual:
						match = lampIdValue.value != sourceValue;
						break;

					case CompareType.GreaterThan:
						match = lampIdValue.value > sourceValue;
						break;

					case CompareType.GreaterThanEqual:
						match = lampIdValue.value >= sourceValue;
						break;

					case CompareType.LessThan:
						match = lampIdValue.value < sourceValue;
						break;

					case CompareType.LessThanEqual:
						match = lampIdValue.value < sourceValue;
						break;

					default:
						match = lampIdValue.value == sourceValue;
						break;
				}

				var dataType = match ? MatchDataType : NonMatchDataType;
				var value = match ? Match : NonMatch;

				switch (dataType) {
					case LampDataType.OnOff:
						Player.SetLamp(lampIdValue.id, flow.GetValue<bool>(value) ? LampStatus.On : LampStatus.Off);
						break;
					case LampDataType.Status:
						Player.SetLamp(lampIdValue.id, flow.GetValue<LampStatus>(value));
						break;
					case LampDataType.Intensity:
						Player.SetLamp(lampIdValue.id, flow.GetValue<float>(value) * GetIntensityMultiplier(lampIdValue.id));
						break;
					case LampDataType.Color:
						Player.SetLamp(lampIdValue.id, flow.GetValue<UnityEngine.Color>(value).ToEngineColor());
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
