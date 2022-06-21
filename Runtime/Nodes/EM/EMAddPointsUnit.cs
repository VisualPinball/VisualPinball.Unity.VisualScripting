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
		private Bool running = false;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		public ValueInput duration { get; private set; }

		[DoNotSerialize]
		[PortLabel("Unscaled")]
		public ValueInput unscaledTime { get; private set; }

		[DoNotSerialize]
		public ControlOutput started;

		[DoNotSerialize]
		public ControlOutput stopped;

		[DoNotSerialize]
		public ControlOutput pulse { get; private set; }

		[DoNotSerialize]
		public ValueInput pointValue { get; private set; }

		[DoNotSerialize, PortLabel("Point Value"), Inspectable]
		public ValueOutput OutputPointValue { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInputCoroutine(nameof(InputTrigger), Process);

			pointValue = ValueInput(nameof(pointValue), 0);

			duration = ValueInput(nameof(duration), .750f);
			unscaledTime = ValueInput(nameof(unscaledTime), false);

			started = ControlOutput(nameof(started));
			stopped = ControlOutput(nameof(stopped));

			pulse = ControlOutput(nameof(pulse));

			OutputPointValue = ValueOutput<int>(nameof(OutputPointValue));
		}

		private IEnumerator Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");

				yield return null;
			}
			else if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find player.");

				yield return null;
			}
			else if (running) {
				var points = flow.GetValue<int>(pointValue);

				Debug.Log($"Score motor is already running. Ignoring {points} point(s).");

				yield return null;
			}
			else {
				Debug.Log("Starting score motor");

				yield return started;

				running = true;

				var points = flow.GetValue<int>(pointValue);

				var pulses =
					(points % 100000 == 0) ? points / 100000 :
					(points % 10000 == 0) ? points / 10000 :
					(points % 1000 == 0) ? points / 1000 :
					(points % 100 == 0) ? points / 100 :
					(points % 10 == 0) ? points / 10 :
					points;

				var pointsPerPulse = points / pulses;

				flow.SetValue(OutputPointValue, pointsPerPulse);

				var seconds = flow.GetValue<float>(duration) / 6;
				var realtime = flow.GetValue<bool>(unscaledTime);

				for (int loop = 0; loop < 6; loop++) {
					Debug.Log($"Waiting {seconds}s. ({loop + 1} of 6)");

					if (realtime) {
						yield return new WaitForSecondsRealtime(seconds);
					}
					else {
						yield return new WaitForSeconds(seconds);
					}

					if (loop < pulses) {
						Debug.Log($"Performing {pointsPerPulse} point(s) pulse. ({loop + 1} of {pulses})");

						yield return pulse;
					}
				}

				Debug.Log("Stopping score motor");

				running = false;

				yield return stopped;
			}
		}
	}
}
