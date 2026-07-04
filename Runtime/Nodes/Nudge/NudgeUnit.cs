// Visual Pinball Engine
// Copyright (C) 2026 freezy and VPE Team
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
	[UnitTitle("Nudge")]
	[UnitSurtitle("Player")]
	[UnitCategory("Pinball")]
	public class NudgeUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Angle")]
		public ValueInput Angle { get; private set; }

		[DoNotSerialize]
		[PortLabel("Force")]
		public ValueInput Force { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));
			Angle = ValueInput(nameof(Angle), 0f);
			Force = ValueInput(nameof(Force), 2f);
			Requirement(Angle, InputTrigger);
			Requirement(Force, InputTrigger);
			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find Player.");
				return OutputTrigger;
			}

			Player.Nudge(flow.GetValue<float>(Angle), flow.GetValue<float>(Force));
			return OutputTrigger;
		}
	}
}
