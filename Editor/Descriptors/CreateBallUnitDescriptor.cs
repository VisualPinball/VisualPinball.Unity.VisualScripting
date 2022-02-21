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
using VisualPinball.Unity.Editor;
using IconSize = VisualPinball.Unity.Editor.IconSize;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(CreateBallUnit))]
	public class CreateBallUnitDescriptor : UnitDescriptor<CreateBallUnit>
	{
		public CreateBallUnitDescriptor(CreateBallUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node spawns a new ball at a given position.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.BallRoller(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(CreateBallUnit.Position):
					desc.summary = "The position in playfield space where the ball should be created.";
					break;
				case nameof(CreateBallUnit.KickAngle):
					desc.summary = "The angle in degrees at which the ball is accelerated at the given position. 0 is straight up, then it goes clock-wise, 180 being straight down.";
					break;
				case nameof(CreateBallUnit.KickForce):
					desc.summary = "The force with which the ball is accelerated at the given position.";
					break;
			}
		}
	}
}
