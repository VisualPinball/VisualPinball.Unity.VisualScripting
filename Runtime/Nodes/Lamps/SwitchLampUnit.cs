﻿// Visual Pinball Engine
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
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[Serializable]
	public struct LampIdValue
	{
		public string id;
		public int value;

		public static LampIdValue FromJson(string json)
		{
			return JsonUtility.FromJson<LampIdValue>(json);
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public static readonly LampIdValue Empty = new LampIdValue { id = string.Empty, value = 0 };
	}

	[UnitShortTitle("Switch Lamp")]
	[UnitTitle("Switch Lamp (ID, match value)")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class SwitchLampUnit : GleUnit
	{
		[SerializeAs(nameof(idCount))]
		private int _idCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Lamp IDs")]
		public int idCount
		{
			get => _idCount;
			set => _idCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Source Value")]
		public ValueInput SourceValue { get; private set; }

		[DoNotSerialize]
		public List<ValueInput> Items { get; private set; }

		private Dictionary<int, LampIdValue> _lampIdValueCache = new Dictionary<int, LampIdValue>();

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			SourceValue = ValueInput<int>(nameof(SourceValue));

			Items = new List<ValueInput>();

			for (var i = 0; i < idCount; i++) {
				var item = ValueInput($"item{i + 1}", LampIdValue.Empty.ToJson());
				Items.Add(item);

				Requirement(item, InputTrigger);
			}

			_lampIdValueCache.Clear();

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}
			
			var value = flow.GetValue<int>(SourceValue);

			foreach (var item in Items) {
				var json = flow.GetValue<string>(item);

				if (!_lampIdValueCache.ContainsKey(json.GetHashCode())) {
					_lampIdValueCache[json.GetHashCode()] = LampIdValue.FromJson(json);
				}

				var obj = _lampIdValueCache[json.GetHashCode()];
				Gle.SetLamp(obj.id, obj.value == value ? 255f : 0f);
			}

			return OutputTrigger;
		}
	}
}