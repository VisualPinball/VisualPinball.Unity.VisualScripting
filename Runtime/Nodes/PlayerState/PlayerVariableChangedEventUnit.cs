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
	[UnitTitle("On Player Variable Changed")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class PlayerVariableChangedEventUnit : GleEventUnit<PlayerVariableChangedArgs>
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		[DoNotSerialize, PortLabel("Value"), Inspectable]
		public ValueOutput Value { get; private set; }

		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.PlayerVariableChanged);
		protected override bool register => true;

		protected override void Definition()
		{
			base.Definition();

			if (Variable == null) {
				return;
			}

			Value = Variable.Type switch {
				VariableType.String => ValueOutput<string>(nameof(Value)),
				VariableType.Integer => ValueOutput<int>(nameof(Value)),
				VariableType.Float => ValueOutput<float>(nameof(Value)),
				VariableType.Boolean => ValueOutput<bool>(nameof(Value)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		protected override bool ShouldTrigger(Flow flow, PlayerVariableChangedArgs args)
		{
			return Variable.Id == args.VariableId;
		}

		protected override void AssignArguments(Flow flow, PlayerVariableChangedArgs args)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			switch (Variable.Type) {
				case VariableType.String:
					flow.SetValue(Value,  VsGle.CurrentPlayerState.Get<string>(Variable.Id));
					break;
				case VariableType.Integer:
					flow.SetValue(Value,  (int)VsGle.CurrentPlayerState.Get<Integer>(Variable.Id));
					break;
				case VariableType.Float:
					flow.SetValue(Value,  (float)VsGle.CurrentPlayerState.Get<Float>(Variable.Id));
					break;
				case VariableType.Boolean:
					flow.SetValue(Value,  (bool)VsGle.CurrentPlayerState.Get<Bool>(Variable.Id));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
