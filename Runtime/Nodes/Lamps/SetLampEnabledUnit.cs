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

namespace VisualPinball.Unity.VisualScripting
{
	[UnitShortTitle("Set Lamp")]
	[UnitTitle("Set Lamp (ID, on/off)")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class SetLampEnabledUnit : GleUnit, IMultiInputUnit
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
		[PortLabel("Is Enabled")]
		public ValueInput IsEnabled { get; private set; }

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

			IsEnabled = ValueInput(nameof(IsEnabled), false);

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			var isEnabled = flow.GetValue<bool>(IsEnabled);

			foreach (var input in multiInputs) {
				var lampId = flow.GetValue<string>(input);
				Gle.SetLamp(lampId, isEnabled ? 255f : 0f);
			}

			return OutputTrigger;
		}
	}
}
