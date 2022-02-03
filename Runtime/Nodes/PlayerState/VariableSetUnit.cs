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
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Set Player Variable")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class PlayerVariableSetUnit : VariableSetUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		protected override State State => VsGle.CurrentPlayerState;
		protected override VariableDefinition VariableDefinition => Variable;
	}

	[UnitTitle("Set Table Variable")]
	[UnitSurtitle("Table State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class TableVariableSetUnit : VariableSetUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public TableVariableDefinition Variable { get; set; }

		protected override State State => VsGle.TableState;
		protected override VariableDefinition VariableDefinition => Variable;
	}

	public abstract class VariableSetUnit : GleUnit
	{
		[DoNotSerialize, PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize, PortLabel("Value"), Inspectable]
		public ValueInput Value { get; private set; }

		protected abstract State State { get; }
		protected abstract VariableDefinition VariableDefinition { get; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Succession(InputTrigger, OutputTrigger);

			if (VariableDefinition == null) {
				return;
			}

			Value = VariableDefinition.Type switch {
				VariableType.String => ValueInput<string>(nameof(Value), string.Empty),
				VariableType.Integer => ValueInput<int>(nameof(Value), 0),
				VariableType.Float => ValueInput<float>(nameof(Value), 0f),
				VariableType.Boolean => ValueInput<bool>(nameof(Value), false),
				_ => throw new ArgumentOutOfRangeException()
			};
			Requirement(Value, InputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			switch (VariableDefinition.Type) {
				case VariableType.String:
					State.Set(VariableDefinition.Id, flow.GetValue<string>(Value));
					break;
				case VariableType.Integer:
					State.Set(VariableDefinition.Id, new Integer(flow.GetValue<int>(Value)));
					break;
				case VariableType.Float:
					State.Set(VariableDefinition.Id, new Float(flow.GetValue<float>(Value)));
					break;
				case VariableType.Boolean:
					State.Set(VariableDefinition.Id, new Bool(flow.GetValue<bool>(Value)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return OutputTrigger;
		}
	}
}
