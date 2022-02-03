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
	[UnitSurtitle("Player State")]
	[UnitCategory("Events\\Visual Pinball")]
	public class PlayerVariableChangedEventUnit : VariableChangedEventUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public PlayerVariableDefinition Variable { get; set; }

		protected override State State => VsGle.CurrentPlayerState;
		protected override VariableDefinition VariableDefinition => Variable;
		protected override string EventHookName => VisualScriptingEventNames.PlayerVariableChanged;
	}

	[UnitTitle("On Table Variable Changed")]
	[UnitSurtitle("Table State")]
	[UnitCategory("Events\\Visual Pinball")]
	public class TableVariableChangedEventUnit : VariableChangedEventUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public TableVariableDefinition Variable { get; set; }

		protected override State State => VsGle.TableState;
		protected override VariableDefinition VariableDefinition => Variable;
		protected override string EventHookName => VisualScriptingEventNames.TableVariableChanged;
	}

	public abstract class VariableChangedEventUnit : GleEventUnit<VariableChangedArgs>
	{
		[DoNotSerialize, PortLabel("Value"), Inspectable]
		public ValueOutput Value { get; private set; }

		public override EventHook GetHook(GraphReference reference) => new(EventHookName);
		protected override bool register => true;

		protected abstract State State { get; }
		protected abstract VariableDefinition VariableDefinition { get; }
		protected abstract string EventHookName { get; }

		protected override void Definition()
		{
			base.Definition();

			if (VariableDefinition == null) {
				return;
			}

			Value = VariableDefinition.Type switch {
				VariableType.String => ValueOutput<string>(nameof(Value)),
				VariableType.Integer => ValueOutput<int>(nameof(Value)),
				VariableType.Float => ValueOutput<float>(nameof(Value)),
				VariableType.Boolean => ValueOutput<bool>(nameof(Value)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		protected override bool ShouldTrigger(Flow flow, VariableChangedArgs args)
		{
			return VariableDefinition.Id == args.VariableId;
		}

		protected override void AssignArguments(Flow flow, VariableChangedArgs args)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			switch (VariableDefinition.Type) {
				case VariableType.String:
					flow.SetValue(Value,  State.Get<string>(VariableDefinition.Id));
					break;
				case VariableType.Integer:
					flow.SetValue(Value,  (int)State.Get<Integer>(VariableDefinition.Id));
					break;
				case VariableType.Float:
					flow.SetValue(Value,  (float)State.Get<Float>(VariableDefinition.Id));
					break;
				case VariableType.Boolean:
					flow.SetValue(Value,  (bool)State.Get<Bool>(VariableDefinition.Id));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
