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
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On Switch Enabled")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class SwitchEnabledEventUnit : GleEventUnit<SwitchEventArgs2>
	{
		[SerializeAs(nameof(idCount))]
		private int _idCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Switch IDs")]
		public int idCount
		{
			get => _idCount;
			set => _idCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		public List<ValueInput> Ids { get; private set; }

		[DoNotSerialize]
		protected override bool register => true;

		// Adding an EventHook with the name of the event to the list of visual scripting events.
		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.SwitchEvent);

		protected override void Definition()
		{
			base.Definition();

			Ids = new List<ValueInput>();

			for (var i = 0; i < idCount; i++) {
				var id = ValueInput<string>("Switch ID " + (i + 1), string.Empty);
				Ids.Add(id);
			}
		}

		protected override bool ShouldTrigger(Flow flow, SwitchEventArgs2 args)
		{
			foreach(var id in Ids) {
				if (flow.GetValue<string>(id) == args.Id && args.IsEnabled) {
					return true;
				}
			}
			
			return false;
		}
	}
}
