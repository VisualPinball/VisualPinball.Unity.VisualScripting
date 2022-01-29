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

using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On Switch Enabled")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class SwitchEnabledEventUnit : GleEventUnit<SwitchEventArgs2>
	{
		[DoNotSerialize]
		[PortLabel("Switch ID")]
		public ValueInput Id { get; private set; }

		[DoNotSerialize]
		protected override bool register => true;

		// Adding an EventHook with the name of the event to the list of visual scripting events.
		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.SwitchEvent);

		protected override void Definition()
		{
			base.Definition();
			Id = ValueInput(nameof(Id), string.Empty);
		}

		protected override bool ShouldTrigger(Flow flow, SwitchEventArgs2 args)
		{
			return flow.GetValue<string>(Id) == args.Id && args.IsEnabled;
		}
	}
}
