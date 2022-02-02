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
	[UnitCategory("Visual Pinball/State")]
	public class PlayerVariableGetUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		[DoNotSerialize, PortLabel("Value"), Inspectable]
		public ValueOutput Value { get; private set; }

		protected override void Definition()
		{
			if (Variable == null) {
				return;
			}

			Value = Variable.Type switch {
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
			return VsGle.CurrentPlayerState.Get<string>(Variable.Id);
		}

		private bool GetBool(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (bool)VsGle.CurrentPlayerState.Get<Bool>(Variable.Id);
		}

		private float GetFloat(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (float)VsGle.CurrentPlayerState.Get<Float>(Variable.Id);
		}

		private int GetInt(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}
			return (int)VsGle.CurrentPlayerState.Get<Integer>(Variable.Id);
		}
	}
}
