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
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Clear Display")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class ClearDisplayUnit : GleUnit
	{
		[Serialize, Inspectable, UnitHeaderInspectable("ID")]
		public DisplayDefinition Display { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[DoNotSerialize]
		public ControlOutput started;

		[DoNotSerialize]
		public ControlOutput stopped;

		[DoNotSerialize]
		public ControlOutput pulse;

		private Queue<int> queue = new Queue<int>();

		protected override void Definition()
		{
			InputTrigger = ControlInputCoroutine(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			started = ControlOutput(nameof(started));
			stopped = ControlOutput(nameof(stopped));
			pulse = ControlOutput(nameof(pulse));

			Succession(InputTrigger, OutputTrigger);
		}

		private IEnumerator Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			yield return OutputTrigger;

			if (Display != null) {
				bool _running = true;

				queue.Clear();

				VsGle.ClearFrame(new ClearDisplayData(Display.Id, HandleStarted, HandleStopped, HandlePulse));

				while (_running)
				{
					yield return new WaitUntil(() => queue.Count != 0);

					int status = queue.Dequeue();

					if (status == 1)
					{
						yield return started;
					}
					else if (status == 2)
					{
						_running = false;

						yield return stopped;
					}
					else if (status == 3)
					{
						yield return pulse;
					}
				}
			}
		}

		private void HandleStarted(object sender, EventArgs args)
		{
			queue.Enqueue(1);

			Debug.Log("HandleStarted!");
		}


		private void HandleStopped(object sender, EventArgs args)
		{
			queue.Enqueue(2);

			Debug.Log("HandleStopped!");
		}

		private void HandlePulse(object sender, EventArgs args)
		{
			queue.Enqueue(3);

			Debug.Log("HandlePulse!");
		}
	}
}
