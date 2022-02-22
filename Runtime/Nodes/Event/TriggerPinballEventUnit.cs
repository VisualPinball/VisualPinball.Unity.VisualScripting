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

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	/// <summary>
	/// Triggers a pinball event.
	/// </summary>
	[UnitTitle("Trigger Pinball Event")]
	[UnitSurtitle("Pinball Event")]
	[UnitShortTitle("Trigger")]
	[TypeIcon(typeof(CustomEvent))]
	[UnitCategory("Events/Visual Pinball")]
	public sealed class TriggerPinballEventUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public EventDefinition Event { get; set; }

		[SerializeAs(nameof(argumentCount))]
		private int _argumentCount;

		[DoNotSerialize]
		public List<ValueInput> Arguments { get; private set; }

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Arguments")]
		public int argumentCount {
			get => _argumentCount;
			set => _argumentCount = Mathf.Clamp(value, 0, 10);
		}

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Trigger);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Arguments = new List<ValueInput>();
			for (var i = 0; i < argumentCount; i++) {
				var argument = ValueInput<object>("argument_" + i);
				Arguments.Add(argument);
				Requirement(argument, InputTrigger);
			}

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Trigger(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			var args = Arguments.Select(flow.GetConvertedValue).ToArray();
			PinballEventUnit.Trigger(VsGle.gameObject, Event.Id, args);

			return OutputTrigger;
		}
	}
}
