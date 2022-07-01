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

// ReSharper disable UnusedType.Global

using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(EMResetPointsUnit))]
	public class EMResetPointsUnitDescriptor : UnitDescriptor<EMResetPointsUnit>
	{
		public EMResetPointsUnitDescriptor(EMResetPointsUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node takes an incoming point value and pulses values to that can be used to simulate the resetting of a score reel. "
			   + "\n\nFor example, an incoming point value of 2041 will provide the following pulses: 3052, 4063, 5074, 6085, 7096, 8007, 9008, 0009, 0000";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Mech(Unity.Editor.IconSize.Large, Unity.Editor.IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(EMResetPointsUnit.pointValue):
					desc.summary = "The starting points value used to seed the reset sequence.";
					break;

				case nameof(EMResetPointsUnit.duration):
					desc.summary = "The amount of time (in ms) the score motor runs.";
					break;

				case nameof(EMAddPointsUnit.positions):
					desc.summary = "Score motor positions.";
					break;

				case nameof(EMResetPointsUnit.started):
					desc.summary = "Triggered when score motor starts.";
					break;

				case nameof(EMResetPointsUnit.stopped):
					desc.summary = "Triggered when score motor finishes.";
					break;

				case nameof(EMResetPointsUnit.pulse):
					desc.summary = "Triggered during each pulse of the score motor.";
					break;

				case nameof(EMResetPointsUnit.OutputPointValue):
					desc.summary = "The current pulses calculated points value that can be used to update a score reel.";
					break;
			}
		}
	}
}
