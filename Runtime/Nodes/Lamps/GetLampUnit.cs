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
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Get Lamp Value")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class GetLampUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public LampDataType DataType { get; set; }

		[DoNotSerialize]
		[PortLabel("Lamp ID")]
		public ValueInput Id { get; private set; }

		[DoNotSerialize]
		public ValueOutput Value { get; private set; }

		protected override void Definition()
		{
			Id = ValueInput(nameof(Id), string.Empty);

			switch (DataType) {
				case LampDataType.OnOff:
					Value = ValueOutput(nameof(Value), GetEnabled);
					break;
				case LampDataType.Status:
					Value = ValueOutput(nameof(Value), GetEnabled);
					break;
				case LampDataType.Intensity:
					Value = ValueOutput(nameof(Value), GetIntensity);
					break;
				case LampDataType.Color:
					Value = ValueOutput(nameof(Value), GetColor);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private float GetIntensity(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return 0;
			}
			return Gle.GetLamp(flow.GetValue<string>(Id)).Intensity;
		}

		private bool GetEnabled(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return false;
			}

			return Gle.GetLamp(flow.GetValue<string>(Id)).IsOn;
		}

		private Color GetColor(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return Color.black;
			}

			return Gle.GetLamp(flow.GetValue<string>(Id)).Color.ToUnityColor();
		}
	}
}
