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

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(DisplayEventUnit))]
	public class DisplayEventUnitDescriptor : UnitDescriptor<DisplayEventUnit>
	{
		public DisplayEventUnitDescriptor(DisplayEventUnit target) : base(target) { }

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.DisplayEvent);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key)
			{
				case nameof(DisplayEventUnit.NumericOutput):
					desc.summary = "The numerical value of the display event.";
					break;
				case nameof(DisplayEventUnit.TextOutput):
					desc.summary = "The text value of the display event.";
					break;
			}
		}
	}
}
