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
using System.Text;
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
		[PortLabel("Segment Data")]
		public ValueInput SegmentInput { get; private set; }

		[DoNotSerialize]
		[PortLabel("DMD (2-bit)")]
		public ValueInput Dmd2Input { get; private set; }

		[DoNotSerialize]
		[PortLabel("DMD (4-bit)")]
		public ValueInput Dmd4Input { get; private set; }

		[DoNotSerialize]
		[PortLabel("DMD (8-bit)")]
		public ValueInput Dmd8Input { get; private set; }

		[DoNotSerialize]
		[PortLabel("DMD (RGB24)")]
		public ValueInput Dmd24Input { get; private set; }

		protected override void Definition()
		{
			InputTrigger = ControlInput(nameof(InputTrigger), Process);
			OutputTrigger = ControlOutput(nameof(OutputTrigger));

			if (Display != null) {
				if (Display.Supports(DisplayFrameFormat.Numeric)) {
					NumericInput = ValueInput<float>(nameof(NumericInput));
				}
				if (Display.Supports(DisplayFrameFormat.AlphaNumeric)) {
					TextInput = ValueInput<string>(nameof(TextInput));
				}
				if (Display.Supports(DisplayFrameFormat.Segment)) {
					SegmentInput = ValueInput<byte[]>(nameof(SegmentInput));
				}
				if (Display.Supports(DisplayFrameFormat.Dmd2)) {
					Dmd2Input = ValueInput<byte[]>(nameof(Dmd2Input));
				}
				if (Display.Supports(DisplayFrameFormat.Dmd4)) {
					Dmd4Input = ValueInput<byte[]>(nameof(Dmd4Input));
				}
				if (Display.Supports(DisplayFrameFormat.Dmd8)) {
					Dmd8Input = ValueInput<byte[]>(nameof(Dmd8Input));
				}
				if (Display.Supports(DisplayFrameFormat.Dmd24)) {
					Dmd24Input = ValueInput<byte[]>(nameof(Dmd24Input));
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
				if (Display.Supports(DisplayFrameFormat.Numeric) && NumericInput.hasValidConnection) {
					var numValue = flow.GetValue<float>(NumericInput);
					VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Numeric, BitConverter.GetBytes(numValue)));
				}
				if (Display.Supports(DisplayFrameFormat.AlphaNumeric) && flow.IsLocal(TextInput)) {
					var strValue = flow.GetValue<string>(TextInput);
					if (!string.IsNullOrEmpty(strValue)) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.AlphaNumeric, Encoding.UTF8.GetBytes(strValue)));
					}
				}
				if (Display.Supports(DisplayFrameFormat.Segment) && SegmentInput.hasValidConnection) {
					var byteValue = flow.GetValue<byte[]>(SegmentInput);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Segment, byteValue));
					}
				}
				if (Display.Supports(DisplayFrameFormat.Dmd2) && Dmd2Input.hasValidConnection) {
					var byteValue = flow.GetValue<byte[]>(Dmd2Input);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Dmd2, byteValue));
					}
				}
				if (Display.Supports(DisplayFrameFormat.Dmd4) && Dmd4Input.hasValidConnection) {
					var byteValue = flow.GetValue<byte[]>(Dmd4Input);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Dmd4, byteValue));
					}
				}
				if (Display.Supports(DisplayFrameFormat.Dmd8) && Dmd8Input.hasValidConnection) {
					var byteValue = flow.GetValue<byte[]>(Dmd8Input);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Dmd8, byteValue));
					}
				}
				if (Display.Supports(DisplayFrameFormat.Dmd24) && Dmd24Input.hasValidConnection) {
					var byteValue = flow.GetValue<byte[]>(Dmd24Input);
					if (byteValue is { Length: > 0 }) {
						VsGle.DisplayFrame(new DisplayFrameData(Display.Id, DisplayFrameFormat.Dmd24, byteValue));
					}
				}
			}

			return OutputTrigger;
		}
	}
}
