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
	[UnitTitle("Get Lamp Value")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class GetLampUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabel("Lamp ID")]
		public ValueInput Id { get; private set; }

		[DoNotSerialize]
		[PortLabel("Value")]
		public ValueOutput Value { get; private set; }

		[DoNotSerialize]
		[PortLabel("Is Enabled")]
		public ValueOutput IsEnabled { get; private set; }

		protected override void Definition()
		{
			Id = ValueInput(nameof(Id), string.Empty);

			Value = ValueOutput(nameof(Value), GetValue);
			IsEnabled = ValueOutput(nameof(IsEnabled), GetEnabled);
		}

		private float GetValue(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return 0;
			}

			return Gle.GetLamp(flow.GetValue<string>(Id));
		}

		private bool GetEnabled(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return false;
			}

			return Gle.GetLamp(flow.GetValue<string>(Id)) > 0;
		}
	}
}
