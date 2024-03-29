﻿// Visual Pinball Engine
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
	[Descriptor(typeof(SwitchLampUnit))]
	public class SwitchLampUnitDescriptor : UnitDescriptor<SwitchLampUnit>
	{
		public SwitchLampUnitDescriptor(SwitchLampUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node enabled or disables lamps based on matching a source value with a specifed value.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Light(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			if (port.key == nameof(SwitchLampUnit.SourceValue)) {
				desc.summary = "Source value to use for matching";
			}
			else if (int.TryParse(port.key, out int id)) {
				id += 1;

				desc.label = $"Lamp ID {id}";
				desc.summary = $"Lamp ID {id} to enable if specified Value matches source Value, or disable if specified Value does not match source Value";
			}
		}
	}
}
