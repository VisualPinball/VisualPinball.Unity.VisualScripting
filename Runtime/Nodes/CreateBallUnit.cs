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

namespace VisualPinball.Unity.VisualScripting
{

	[UnitTitle("Create Ball")]
	[UnitCategory("Visual Pinball")]
	public class CreateBallUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Position")]
		public ValueInput Position { get; private set; }

		[DoNotSerialize]
		[PortLabel("Kick Angle")]
		public ValueInput KickAngle { get; private set; }

		[DoNotSerialize]
		[PortLabel("Kick Force")]
		public ValueInput KickForce { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Position = ValueInput<Vector3>(nameof(Position), Vector3.zero);
			KickAngle = ValueInput<float>(nameof(KickAngle), 0);
			KickForce = ValueInput<float>(nameof(KickForce), 0);

			Requirement(Position, InputTrigger);
			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			if (Gle is VisualScriptingGamelogicEngine vsGle) {
				var pos = flow.GetValue<Vector3>(Position);
				var kickAngle = flow.GetValue<float>(KickAngle);
				var kickForce = flow.GetValue<float>(KickForce);
				vsGle.BallManager.CreateBall(new DebugBallCreator(pos.x, pos.y, pos.z, kickAngle, kickForce));
			}

			return OutputTrigger;
		}
	}
}
