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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitShortTitle("Add Points Display")]
	[UnitTitle("Add Points Display")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class AddPointsDisplayUnit : GleUnit
	{
		[Serialize]
		[Inspectable]
		[UnitHeaderInspectable("ID")]
		[DisplayTypeFilterAttribute(DisplayType.ScoreReel)]
		public DisplayDefinition Display { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlInput InputTrigger;

		[DoNotSerialize]
		[PortLabel("Points")]
		public ValueInput Points { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ControlOutput OutputTrigger;

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			Points = ValueInput<float>(nameof(Points));

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			if (Display != null) {
				var points = flow.GetValue<float>(Points);
				VsGle.DisplayAddPoints(new DisplayAddPointsData(Display.Id, points));
			}

			return OutputTrigger;
		}
	}
}
