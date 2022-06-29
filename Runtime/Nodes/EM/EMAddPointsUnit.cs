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
	[UnitShortTitle("EM Add Points")]
	[UnitTitle("EM Add Points")]
	[UnitSurtitle("EM")]
	[UnitCategory("Visual Pinball/EM")]
	public class EMAddPointsUnit : GleUnit
	{
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

		[DoNotSerialize]
		[PortLabel("Point Value")]
		public ValueOutput OutputPointValue { get; private set; }

		private Bool running = false;

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
			if (running) {
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

				var seconds = flow.GetValue<float>(duration) / 6;
				var realtime = flow.GetValue<bool>(unscaledTime);

				for (int loop = 0; loop < 6; loop++) {
					var outputPoints = loop < pulses ? pointsPerPulse : 0;

					Debug.Log($"Pulse {loop + 1} of 6 - waiting {seconds} and triggering with {outputPoints} points");

					if (realtime) {
						yield return new WaitForSecondsRealtime(seconds);
					}
					else {
						yield return new WaitForSeconds(seconds);
					}

					flow.SetValue(OutputPointValue, outputPoints);

					yield return pulse;
				}

				Debug.Log("Stopping score motor");

				running = false;

				yield return stopped;
			}
		}
	}
}
