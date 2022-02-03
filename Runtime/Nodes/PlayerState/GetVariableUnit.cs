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
	[UnitTitle("Get Player Variable")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class GetPlayerVariableUnit : GetVariableUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		protected override VariableDefinition VariableDefinition => Variable;
		protected override State State => VsGle.CurrentPlayerState;
	}

	[UnitTitle("Get Table Variable")]
	[UnitSurtitle("Table State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class GetTableVariableUnit : GetVariableUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public TableVariableDefinition Variable { get; set; }
		protected override VariableDefinition VariableDefinition => Variable;

		protected override State State => VsGle.TableState;
	}

	public abstract class GetVariableUnit : GleUnit
	{
		[DoNotSerialize, PortLabel("Value"), Inspectable]
		public ValueOutput Value { get; private set; }

		protected abstract State State { get; }
		protected abstract VariableDefinition VariableDefinition { get; }

		protected override void Definition()
		{
			if (VariableDefinition == null) {
				return;
			}

			Value = VariableDefinition.Type switch {
				VariableType.String => ValueOutput(nameof(Value), GetString),
				VariableType.Integer => ValueOutput(nameof(Value), GetInt),
				VariableType.Float => ValueOutput(nameof(Value), GetFloat),
				VariableType.Boolean => ValueOutput(nameof(Value), GetBool),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private string GetString(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return State.Get<string>(VariableDefinition.Id);
		}

		private bool GetBool(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (bool)State.Get<Bool>(VariableDefinition.Id);
		}

		private float GetFloat(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (float)State.Get<Float>(VariableDefinition.Id);
		}

		private int GetInt(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (int)State.Get<Integer>(VariableDefinition.Id);
		}
	}
}
