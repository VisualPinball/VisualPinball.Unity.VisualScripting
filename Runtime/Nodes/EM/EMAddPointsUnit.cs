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
		public ValueInput blockPoints { get; private set; }

		[DoNotSerialize]
		public ValueInput positions { get; private set; }

		[DoNotSerialize]
		public ValueInput duration { get; private set; }

		[DoNotSerialize]
		[PortLabel("Unscaled")]
		public ValueInput unscaledTime { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

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

		private static string VARIABLE_EM_SCORE_MOTOR = "EM_SCORE_MOTOR";

		protected override void Definition()
		{
			InputTrigger = ControlInputCoroutine(nameof(InputTrigger), Process);

			pointValue = ValueInput(nameof(pointValue), 0);

			blockPoints = ValueInput(nameof(blockPoints), true);
			positions = ValueInput(nameof(positions), 6);
			duration = ValueInput(nameof(duration), 750);
			unscaledTime = ValueInput(nameof(unscaledTime), false);

			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			started = ControlOutput(nameof(started));
			stopped = ControlOutput(nameof(stopped));

			pulse = ControlOutput(nameof(pulse));

			OutputPointValue = ValueOutput<int>(nameof(OutputPointValue));
		}

		private IEnumerator Process(Flow flow)
		{
			var running = false;

			if (Variables.Application.IsDefined(VARIABLE_EM_SCORE_MOTOR)) {
				running = Variables.Application.Get<bool>(VARIABLE_EM_SCORE_MOTOR);
			}

			yield return OutputTrigger;

			var points = flow.GetValue<int>(pointValue);

			if (points > 0) {
				var pulses =
					(points % 100000 == 0) ? points / 100000 :
					(points % 10000 == 0) ? points / 10000 :
					(points % 1000 == 0) ? points / 1000 :
					(points % 100 == 0) ? points / 100 :
					(points % 10 == 0) ? points / 10 :
					points;

				if (pulses == 1) {
					if (!running || (running && !flow.GetValue<bool>(blockPoints))) {
						Debug.Log($"Single pulse triggering with {points} points");

						flow.SetValue(OutputPointValue, points);

						yield return pulse;
					}
				}
				else if (running)
				{
					Debug.Log($"Score motor is already running.");
				}
				else {
					Debug.Log("Starting score motor");

					Variables.Application.Set(VARIABLE_EM_SCORE_MOTOR, true);
		
					yield return started;

					var motorPositions = flow.GetValue<int>(positions);

					var delay = (flow.GetValue<float>(duration) / 1000f) / motorPositions;
					var realtime = flow.GetValue<bool>(unscaledTime);

					var pointsPerPulse = points / pulses;

					for (int loop = 0; loop < motorPositions; loop++) {
						var outputPoints = loop < pulses ? pointsPerPulse : 0;

						Debug.Log($"Pulse {loop + 1} of {motorPositions} - waiting {delay}ms and triggering with {outputPoints} points");

						if (realtime) {
							yield return new WaitForSecondsRealtime(delay);
						}
						else {
							yield return new WaitForSeconds(delay);
						}

						flow.SetValue(OutputPointValue, outputPoints);

						yield return pulse;
					}

					Debug.Log("Stopping score motor");

					Variables.Application.Set(VARIABLE_EM_SCORE_MOTOR, false);

					yield return stopped;
				}
			}
		}
	}
}
