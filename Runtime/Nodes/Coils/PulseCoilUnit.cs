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

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Pulse Coil")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class PulseCoilUnit : GleUnit
	{
		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		[SerializeAs(nameof(idCount))]
		private int _idCount = 1;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Coil IDs")]
		public int idCount
		{
			get => _idCount;
			set => _idCount = Mathf.Clamp(value, 1, 10);
		}

		[DoNotSerialize]
		public List<ValueInput> Ids { get; private set; }

		[DoNotSerialize]
		[PortLabel("Duration (ms)")]
		public ValueInput PulseDuration { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Ids = new List<ValueInput>();

			for (var i = 0; i < idCount; i++) {
				var id = ValueInput<string>("Coil ID " + (i + 1), string.Empty);
				Ids.Add(id);

				Requirement(id, InputTrigger);
			}

			PulseDuration = ValueInput<int>(nameof(PulseDuration), 80);

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertGle(flow)) {
				Debug.LogError("Cannot find GLE.");
				return OutputTrigger;
			}

			if (!AssertPlayer(flow)) {
				Debug.LogError("Cannot find Player.");
				return OutputTrigger;
			}

			foreach (var id in Ids) {
				var idValue = flow.GetValue<string>(id);
				var pulseDuration = flow.GetValue<int>(PulseDuration);

				Gle.SetCoil(idValue, true);
				Player.ScheduleAction(pulseDuration, () => Gle.SetCoil(idValue, false));
			}

			return OutputTrigger;
		}
	}
}
