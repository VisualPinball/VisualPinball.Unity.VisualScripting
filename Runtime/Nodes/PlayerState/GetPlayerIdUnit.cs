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
	[UnitTitle("Get Player ID")]
	[UnitSurtitle("Player State")]
	[UnitCategory("Visual Pinball/Variables")]
	public class GetPlayerIdUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public WhichPlayer Which { get; set; }

		[DoNotSerialize, PortLabel("Player ID"), Inspectable]
		public ValueOutput PlayerId { get; private set; }

		protected override void Definition()
		{
			PlayerId = ValueOutput(nameof(PlayerId), GetPlayerId);
		}

		private int GetPlayerId(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			return Which switch {
				WhichPlayer.First => VsGle.PlayerStates.Keys.Min(),
				WhichPlayer.Last => VsGle.PlayerStates.Keys.Max(),
				WhichPlayer.Current => VsGle.CurrentPlayerState.Id,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}

	public enum WhichPlayer
	{
		Current, First, Last
	}
}
