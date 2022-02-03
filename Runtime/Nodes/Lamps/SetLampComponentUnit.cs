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
using UnityEngine;
using VisualPinball.Engine.Math;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Set Lamp (Component, Value, Channel)")]
	[UnitShortTitle("Set Lamp")]
	[UnitSurtitle("Scene")]
	[UnitCategory("Visual Pinball")]
	public class SetLampComponentUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Component")]
		public ValueInput LampComponent;

		[DoNotSerialize]
		[PortLabel("Intensity")]
		public ValueInput Value;

		[DoNotSerialize]
		[PortLabel("Color Channel")]
		public ValueInput ColorChannel;

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			LampComponent = ValueInput<MonoBehaviour>(nameof(LampComponent), null);

			Value = ValueInput(nameof(Value), 0f);
			ColorChannel = ValueInput(nameof(ColorChannel), Engine.Math.ColorChannel.Alpha);

			Requirement(LampComponent, InputTrigger);
			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find player.");
				return OutputTrigger;
			}

			if (flow.GetValue<MonoBehaviour>(LampComponent) is ILampDeviceComponent lamp) {
				var valueRaw = flow.GetValue<float>(Value);
				var colorChannelRaw = flow.GetValue<ColorChannel>(ColorChannel);

				Player.Lamp(lamp)?.OnLamp(valueRaw, colorChannelRaw);
			}

			return OutputTrigger;
		}


	}
}
