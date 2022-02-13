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

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Increase Player Variable")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class IncreasePlayerVariableUnit : IncreaseVariableUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		protected override State State => VsGle.CurrentPlayerState;
		protected override VariableDefinition VariableDefinition => Variable;
	}

	[UnitTitle("Increase Table Variable")]
	[UnitSurtitle("Table State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class IncreaseTableVariableUnit : IncreaseVariableUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public TableVariableDefinition Variable { get; set; }

		protected override State State => VsGle.TableState;
		protected override VariableDefinition VariableDefinition => Variable;
	}

	public abstract class IncreaseVariableUnit : GleUnit
	{
		[DoNotSerialize, PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize, PortLabel("Increase By"), Inspectable]
		public ValueInput Value { get; private set; }

		[DoNotSerialize, PortLabel("Updated Value"), Inspectable]
		public ValueOutput OutputValue { get; private set; }

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
				VariableType.Integer => ValueInput<int>(nameof(Value), 0),
				VariableType.Float => ValueInput<float>(nameof(Value), 0f),
				VariableType.Boolean => ValueInput<bool>(nameof(Value), false),
				VariableType.String => ValueInput<string>(nameof(Value), string.Empty),
				_ => throw new ArgumentOutOfRangeException()
			};

			OutputValue = VariableDefinition.Type switch {
				VariableType.Integer => ValueOutput<int>(nameof(Value)),
				VariableType.Float => ValueOutput<float>(nameof(Value)),
				VariableType.Boolean => ValueOutput<bool>(nameof(Value)),
				VariableType.String => ValueOutput<string>(nameof(Value)),
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
				case VariableType.Integer: {
					var current = (int)State.Get<Integer>(VariableDefinition.Id);
					State.Set(VariableDefinition.Id, new Integer(current + flow.GetValue<int>(Value)));
					flow.SetValue(OutputValue, current + flow.GetValue<int>(Value));
					break;
				}
				case VariableType.Float: {
					var current = (float)State.Get<Float>(VariableDefinition.Id);
					State.Set(VariableDefinition.Id, new Float(current + flow.GetValue<float>(Value)));
					flow.SetValue(OutputValue, current + flow.GetValue<float>(Value));
					break;
				}
				case VariableType.Boolean: {
					if (flow.GetValue<bool>(Value)) {
						var current = (bool)State.Get<Bool>(VariableDefinition.Id);
						State.Set(VariableDefinition.Id, new Bool(!current));
						flow.SetValue(OutputValue, !current);
					}
					break;
				}
				case VariableType.String: {
					var current = State.Get<string>(VariableDefinition.Id);
					var next = current + flow.GetValue<string>(Value);
					State.Set(VariableDefinition.Id, next);
					flow.SetValue(OutputValue, next);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}

			return OutputTrigger;
		}
	}
}
