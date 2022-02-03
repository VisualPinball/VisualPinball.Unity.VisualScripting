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
	[Descriptor(typeof(SetLampComponentUnit))]
	public class SetLampComponentUnitDescriptor : UnitDescriptor<SetLampComponentUnit>
	{
		public SetLampComponentUnitDescriptor(SetLampComponentUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node assigns a given value to a light or light group in the scene. \n\nNote that this doesn't pass through the gamelogic engine, thus no event will be triggered. However, it allows you to drive non-mapped lights as well.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Light(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(SetLampComponentUnit.LampComponent):
					desc.summary = "The light component whose value you want to change. Assigning a light group will change all lights in the group.";
					break;

				case nameof(SetLampComponentUnit.Value):
					desc.summary = "The intensity to apply (0-1).";
					break;

				case nameof(SetLampComponentUnit.ColorChannel):
					desc.summary = "Which color channel to use. For non-RGB lights, use alpha.";
					break;
			}
		}
	}
}
