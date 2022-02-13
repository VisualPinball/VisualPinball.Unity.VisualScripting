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
		[DoNotSerialize, PortLabel("Old Value"), Inspectable]
		public ValueOutput OldValue { get; private set; }

		[DoNotSerialize, PortLabel("New Value"), Inspectable]
		public ValueOutput NewValue { get; private set; }

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

			OldValue = VariableDefinition.Type switch {
				VariableType.String => ValueOutput<string>(nameof(OldValue)),
				VariableType.Integer => ValueOutput<int>(nameof(OldValue)),
				VariableType.Float => ValueOutput<float>(nameof(OldValue)),
				VariableType.Boolean => ValueOutput<bool>(nameof(OldValue)),
				_ => throw new ArgumentOutOfRangeException()
			};

			NewValue = VariableDefinition.Type switch {
				VariableType.String => ValueOutput<string>(nameof(NewValue)),
				VariableType.Integer => ValueOutput<int>(nameof(NewValue)),
				VariableType.Float => ValueOutput<float>(nameof(NewValue)),
				VariableType.Boolean => ValueOutput<bool>(nameof(NewValue)),
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
					flow.SetValue(OldValue,  (string)args.OldValue);
					flow.SetValue(NewValue,  (string)args.NewValue);
					break;
				case VariableType.Integer:
					flow.SetValue(OldValue,  (int)(Integer)args.OldValue);
					flow.SetValue(NewValue, (int)(Integer)args.NewValue);
					break;
				case VariableType.Float:
					flow.SetValue(OldValue,  (float)(Float)args.OldValue);
					flow.SetValue(NewValue,  (float)(Float)args.NewValue);
					break;
				case VariableType.Boolean:
					flow.SetValue(OldValue,  (bool)(Bool)args.OldValue);
					flow.SetValue(NewValue,  (bool)(Bool)args.NewValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
