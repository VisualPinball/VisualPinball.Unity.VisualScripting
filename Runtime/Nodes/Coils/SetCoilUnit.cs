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

using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Set Coil")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class SetCoilUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Coil ID")]
		public ValueInput Id { get; private set; }

		[DoNotSerialize]
		[PortLabel("Value")]
		public ValueInput IsEnabled { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Id = ValueInput<string>(nameof(Id), string.Empty);
			IsEnabled = ValueInput<bool>(nameof(IsEnabled), false);

			Requirement(Id, InputTrigger);
			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			var id = flow.GetValue<string>(Id);
			var isEnabled = flow.GetValue<bool>(IsEnabled);

			Gle.SetCoil(id, isEnabled);

			return OutputTrigger;
		}
	}
}
