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
			   + "For example, an incoming point value of 2041 will provide the following pulses: 3052, 4063, 5074, 6085, 7096, 8007, 9008, 0009, 0000";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Mech(Unity.Editor.IconSize.Large, Unity.Editor.IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
			}
		}
	}
}
