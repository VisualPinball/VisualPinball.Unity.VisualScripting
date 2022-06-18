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

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	/* 
	 * Well, I think the node should be an "EM Add Points" node that would accept a "Point Value"  
	 * and a "Score Motor Value".  It would send a signal directly to the point value's score reel 
	 * "coil" if the "Score Motor Value" was set to 0.  If the "Score Motor Value" was 1-5 then 
	 * the "Point Value" along with the "Score Motor Value" would be sent to the Score Motor.  
	 * The score motor would then send series of pulsed signals to the "point value's" score reel 
	 * coil equal to the "Score Motor Value."  The reels would have to be set up with a 9's 
	 * carry over ability (ie 00190 sent a pulse to the 10's coil would have to pulse the 
	 * 100's coil once to get to 200)  If the score motor is in motion then all scores that occur 
	 * during this window need to not be scored (per @bord observations today).
	 */

	[UnitShortTitle("EM Add Points")]
	[UnitTitle("EM Add Points")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class EMAddPointsUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("ID")]
		public DisplayDefinition Display { get; private set; }

		[DoNotSerialize]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		public ControlOutput tick { get; private set; }

		[DoNotSerialize]
		public ValueInput scoreMotor { get; private set; }

		[DoNotSerialize]
		public ValueInput pointValue { get; private set; }

		[DoNotSerialize]
		public ValueInput duration { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInputCoroutine(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			tick = ControlOutput(nameof(tick));

			pointValue = ValueInput(nameof(pointValue), 0);
			scoreMotor = ValueInput(nameof(scoreMotor), 0);

			duration = ValueInput(nameof(duration), 0f);
		}

		private IEnumerator Process(Flow flow)
		{
			if (!AssertGle(flow))
			{
				Debug.LogError("Cannot find GLE.");
				yield return OutputTrigger;
			}

			if (!AssertPlayer(flow))
			{
				Debug.LogError("Cannot find player.");
				yield return OutputTrigger;
			}

			var seconds = flow.GetValue<float>(this.duration);

			for (int loop = 0; loop < 5; loop++)
			{
				yield return new WaitForSeconds(seconds);

				yield return tick;

				Debug.Log($"ITERATION LOOP {loop}");
			}

			yield return OutputTrigger;
		}
	}
}
