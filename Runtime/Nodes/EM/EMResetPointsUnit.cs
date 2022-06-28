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
	[UnitShortTitle("EM Reset Points")]
	[UnitTitle("EM Reset Points")]
	[UnitSurtitle("EM")]
	[UnitCategory("Visual Pinball/EM")]
	public class EMResetPointsUnit : GleUnit
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

		[DoNotSerialize, PortLabel("Point Value"), Inspectable]
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

				var seconds = flow.GetValue<float>(duration) / 6;
				var realtime = flow.GetValue<bool>(unscaledTime);

				while (points > 0) {
					for (int loop = 0; loop < 6; loop++) {
						points = AdvancePoints(points);

						Debug.Log($"Pulse {loop + 1} of 6 - waiting {seconds} and triggering with {points} points");

						if (realtime) {
							yield return new WaitForSecondsRealtime(seconds);
						}
						else {
							yield return new WaitForSeconds(seconds);
						}

						flow.SetValue(OutputPointValue, points);

						yield return pulse;
					}
				}

				Debug.Log("Stopping score motor");

				running = false;

				yield return stopped;
			}
		}

		private static int NumDigits(int n)
		{
			if (n < 0)
			{
				n = n == int.MinValue ? int.MaxValue : -n;
			}
			return n switch
			{
				< 10 => 1,
				< 100 => 2,
				< 1000 => 3,
				< 10000 => 4,
				< 100000 => 5,
				< 1000000 => 6,
				< 10000000 => 7,
				< 100000000 => 8,
				< 1000000000 => 9,
				_ => 10
			};
		}

		private static int[] DigitArr(int n)
		{
			var result = new int[NumDigits(n)];
			for (var i = result.Length - 1; i >= 0; i--) {
				result[i] = n % 10;
				n /= 10;
			}
			return result;
		}

		private static int AdvancePoints(int points) {
			var value = 0;

			foreach (var i in DigitArr(points)) {
				value = (value * 10) + ((i > 0 && i < 9) ? (i + 1) : 0);
			}

			return value;
		}
	}
}
