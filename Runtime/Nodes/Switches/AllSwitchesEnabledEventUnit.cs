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
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On All Switches Enabled")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class AllSwitchesEnabledEventUnit : GleEventUnit<SwitchEventArgs2>, IMultiInputUnit
	{
		[SerializeAs(nameof(inputCount))]
		private int _itemCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Switch IDs")]
		public int inputCount
		{
			get => _itemCount;
			set => _itemCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		public ReadOnlyCollection<ValueInput> multiInputs { get; private set; }

		[DoNotSerialize]
		protected override bool register => true;

		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.SwitchEvent);

		protected override void Definition()
		{
			base.Definition();

			var list = new List<ValueInput>();
			for (var i = 0; i < inputCount; i++) {
				var item = ValueInput($"item{i}", string.Empty);
				list.Add(item);
			}

			multiInputs = new ReadOnlyCollection<ValueInput>(list);
		}

		protected override bool ShouldTrigger(Flow flow, SwitchEventArgs2 args)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return false;
			}

			var validSwitch = false;
			foreach(var item in multiInputs) {
				var swId = flow.GetValue<string>(item);
				if (swId == args.Id) {
					validSwitch = true;
				}
				if (!Gle.GetSwitch(swId)) {
					return false;
				}
			}
			return validSwitch;
		}
	}
}
