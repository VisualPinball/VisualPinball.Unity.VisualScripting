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
	[Descriptor(typeof(PlayerStateChangeUnit))]
	public class PlayerStateChangeUnitDescriptor : UnitDescriptor<PlayerStateChangeUnit>
	{
		public PlayerStateChangeUnitDescriptor(PlayerStateChangeUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node changes the current player state with another one.";
		}

		//protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.BallRoller(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(PlayerStateChangeUnit.PlayerId):
					desc.summary = "The player ID of the desired state";
					break;
			}
		}
	}
}
