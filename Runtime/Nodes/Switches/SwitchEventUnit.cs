﻿// Visual Pinball Engine
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
	[UnitTitle("On Switch Changed")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Pinball")]
	public class SwitchEventUnit : GleEventUnit<SwitchEventArgs2>, IMultiInputUnit
	{
		[SerializeAs(nameof(inputCount))]
		private int _inputCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Switch IDs")]
		public int inputCount
		{
			get => _inputCount;
			set => _inputCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		public ReadOnlyCollection<ValueInput> multiInputs { get; private set; }

		[DoNotSerialize]
		[PortLabel("Is Enabled")]
		public ValueOutput IsEnabled { get; private set; }

		protected override bool register => true;

		// Adding an EventHook with the name of the event to the list of visual scripting events.
		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(VisualScriptingEventNames.SwitchEvent);
		}

		protected override void Definition()
		{
			base.Definition();

			var _multiInputs = new List<ValueInput>();

			multiInputs = _multiInputs.AsReadOnly();

			for (var i = 0; i < inputCount; i++) {
				_multiInputs.Add(ValueInput(i.ToString(), string.Empty));
			}

			IsEnabled = ValueOutput<bool>(nameof(IsEnabled));
		}

		protected override bool ShouldTrigger(Flow flow, SwitchEventArgs2 args)
		{
			foreach (var input in multiInputs) {
				if (flow.GetValue<string>(input) == args.Id) {
					return true;
				}
			}

			return false;
		}

		protected override void AssignArguments(Flow flow, SwitchEventArgs2 args)
		{
			flow.SetValue(IsEnabled, args.IsEnabled);
		}
	}
}
