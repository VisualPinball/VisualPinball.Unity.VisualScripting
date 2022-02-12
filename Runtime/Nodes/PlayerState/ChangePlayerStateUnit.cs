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
using NLog;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Change Player State")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class ChangePlayerStateUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("Next Player")]
		public bool NextPlayer { get; set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		[PortLabel("Player ID")]
		public ValueInput PlayerId { get; private set; }

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			if (!NextPlayer) {
				PlayerId = ValueInput(nameof(PlayerId), 0);
				Requirement(PlayerId, InputTrigger);
			}

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			int playerId;
			if (NextPlayer) {
				var lastPlayer = VsGle.PlayerStates.Keys.Max();
				if (VsGle.CurrentPlayerState.Id == lastPlayer) {
					playerId = VsGle.PlayerStates.Keys.Min();

				} else if (VsGle.PlayerStates.ContainsKey(VsGle.CurrentPlayerState.Id + 1)) {
					playerId = VsGle.CurrentPlayerState.Id + 1;

				} else {
					Logger.Warn($"Non-existent next player {VsGle.CurrentPlayerState.Id + 1}.");
					playerId = VsGle.CurrentPlayerState.Id;
				}
			} else {
				playerId = flow.GetValue<int>(PlayerId);
			}

			VsGle.SetCurrentPlayer(playerId);

			return OutputTrigger;
		}
	}
}
