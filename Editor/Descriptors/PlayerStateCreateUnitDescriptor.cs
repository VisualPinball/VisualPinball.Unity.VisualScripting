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
	[Descriptor(typeof(PlayerStateCreateUnit))]
	public class PlayerStateCreateUnitDescriptor : UnitDescriptor<PlayerStateCreateUnit>
	{
		public PlayerStateCreateUnitDescriptor(PlayerStateCreateUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node creates a new player state.";
		}

		//protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.BallRoller(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(PlayerStateCreateUnit.PlayerId):
					desc.summary = "The player ID of the new state";
					break;
				// case nameof(PlayerStateCreateUnit.AutoIncrement):
				// 	desc.summary = "If set, the new player ID will be the currently largest ID, plus one.";
				// 	break;
				case nameof(PlayerStateCreateUnit.SetAsActive):
					desc.summary = "If set, the new state will be the current state. Otherwise, it will only be the current state if there is no state set.";
					break;
			}
		}
	}
}
