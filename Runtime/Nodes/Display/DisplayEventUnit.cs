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
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On Display Changed")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Events\\Visual Pinball")]
	public class DisplayEventUnit : GleEventUnit<DisplayEventArgs>
	{
		[Serialize]
		[Inspectable]
		[UnitHeaderInspectable("ID")]
		public DisplayDefinition Display { get; private set; }

		[DoNotSerialize]
		[PortLabel("Numeric")]
		public ValueOutput NumericOutput { get; private set; }

		[DoNotSerialize]
		[PortLabel("Text")]
		public ValueOutput TextOutput { get; private set; }

		[DoNotSerialize]
		protected override bool register => true;

		public override EventHook GetHook(GraphReference reference) => new EventHook(VisualScriptingEventNames.DisplayEvent);

		protected override void Definition()
		{
			base.Definition();

			if (Display != null) {
				if (Display.Supports(DisplayFrameFormat.Numeric)) {
					NumericOutput = ValueOutput<float>(nameof(NumericOutput));
				}

				if (Display.Supports(DisplayFrameFormat.AlphaNumeric)) {
					TextOutput = ValueOutput<string>(nameof(TextOutput));
				}
			}
		}

		protected override bool ShouldTrigger(Flow flow, DisplayEventArgs args)
		{
			return Display != null && Display.Id.Equals(args.DisplayFrameData.Id);
		}

		protected override void AssignArguments(Flow flow, DisplayEventArgs args)
		{
			if (Display.Supports(DisplayFrameFormat.Numeric)) {
				if (args.DisplayFrameData.Format == DisplayFrameFormat.Numeric) {
					flow.SetValue(NumericOutput, BitConverter.ToSingle(args.DisplayFrameData.Data));
				}
			}

			if (Display.Supports(DisplayFrameFormat.AlphaNumeric)) {
				if (args.DisplayFrameData.Format == DisplayFrameFormat.AlphaNumeric) {
					flow.SetValue(TextOutput, Encoding.UTF8.GetString(args.DisplayFrameData.Data));
				}
			}
		}
	}
}
