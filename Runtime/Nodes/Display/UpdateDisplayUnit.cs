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
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("Update Display")]
	[UnitSurtitle("Gamelogic Engine")]
	[UnitCategory("Visual Pinball")]
	public class UpdateDisplayUnit : GleUnit
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
		[PortLabel("Numeric")]
		public ValueInput NumericInput { get; private set; }

		[DoNotSerialize]
		[PortLabel("Text")]
		public ValueInput TextInput { get; private set; }

		[DoNotSerialize]
		[PortLabel("Data")]
		public ValueInput FrameInput { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			if (Display != null) {
				if (Display.SupportsNumericInput) {
					NumericInput = ValueInput<float>(nameof(NumericInput));
				}
				if (Display.SupportsTextInput) {
					TextInput = ValueInput<string>(nameof(TextInput));
				}
				if (Display.SupportsImageInput) {
					TextInput = ValueInput<byte[]>(nameof(FrameInput));
				}
			}

			Succession(InputTrigger, OutputTrigger);
		}

		private ControlOutput Process(Flow flow)
		{
			if (!AssertVsGle(flow)) {
				throw new InvalidOperationException("Cannot retrieve GLE from unit.");
			}

			if (Display != null) {
				if (Display.SupportsNumericInput) {
					var numValue = (int)flow.GetValue<float>(NumericInput);
					VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Segment, ScoreConverter.Convert(numValue, Display.Width)));
				}
				if (Display.SupportsTextInput) {
					var strValue = flow.GetValue<string>(TextInput);
					if (!string.IsNullOrEmpty(strValue)) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Segment, ScoreConverter.Text(strValue, Display.Width)));
					}
				}
				if (Display.SupportsImageInput) {
					var byteValue = flow.GetValue<byte[]>(TextInput);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Segment, byteValue));
					}
				}
			}

			return OutputTrigger;
		}
	}
}
