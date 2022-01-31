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
	[Descriptor(typeof(GetSwitchUnit))]
	public class GetSwitchUnitDescriptor : UnitDescriptor<GetSwitchUnit>
	{
		public GetSwitchUnitDescriptor(GetSwitchUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node retrieves the current status of the switch.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Switch(false, IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(GetSwitchUnit.Ids):
					desc.summary = "The IDs of the switches for which to get the current status.";
					break;
				case nameof(GetSwitchUnit.IsEnabled):
					desc.summary = "Whether the switch is enabled.";
					break;
			}
		}
	}
}
