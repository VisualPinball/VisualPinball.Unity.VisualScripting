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
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Create Player State")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class CreatePlayerStateUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("Auto-increment")]
		public bool AutoIncrement { get; set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Player ID")]
		public ValueInput PlayerId { get; private set; }

		[DoNotSerialize]
		[PortLabel("Set as Active")]
		public ValueInput SetAsActive { get; set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			if (!AutoIncrement) {
				PlayerId = ValueInput(nameof(PlayerId), 0);
				Requirement(PlayerId, InputTrigger);
			}

			SetAsActive = ValueInput(nameof(SetAsActive), false);

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			// determine new player id
			var newPlayerId = AutoIncrement && VsGle.PlayerStates.Count > 0
				? VsGle.PlayerStates.Keys.Max() + 1
				: VsGle.PlayerStates.Count == 0 ? 0 : flow.GetValue<int>(PlayerId);

			// create new state
			VsGle.CreatePlayerState(newPlayerId);

			// set as active
			if (flow.GetValue<bool>(SetAsActive)) {
				VsGle.CurrentPlayer = newPlayerId;
			}

			return OutputTrigger;
		}
	}
}
