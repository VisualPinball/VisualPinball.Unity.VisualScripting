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
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On Display Score")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class DisplayScoreEventUnit : GleEventUnit<DisplayScoreEventArgs>
	{
		[Serialize]
		[Inspectable]
		[UnitHeaderInspectable("ID")]
		[DisplayTypeFilterAttribute(DisplayType.ScoreReel)]
		public DisplayDefinition Display { get; private set; }

		[DoNotSerialize]
		[PortLabel("Points")]
		public ValueOutput Points { get; private set; }

		[DoNotSerialize]
		[PortLabel("Score")]
		public ValueOutput Score { get; private set; }

		[DoNotSerialize]
		protected override bool register => true;

		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.DisplayScoreEvent);

		protected override void Definition()
		{
			base.Definition();

			Points = ValueOutput<float>(nameof(Points));
			Score = ValueOutput<float>(nameof(Score));
		}

		protected override bool ShouldTrigger(Flow flow, DisplayScoreEventArgs args)
		{
			return args.Id == Display.Id;
		}

		protected override void AssignArguments(Flow flow, DisplayScoreEventArgs args)
		{
			flow.SetValue(Points, args.Points);
			flow.SetValue(Score, args.Score);
		}
	}
}
