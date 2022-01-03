﻿// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
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

using Unity.VisualScripting;
using VisualPinball.Unity.Editor;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(SetCoilUnit))]
	public class SetCoilUnitDescriptor : UnitDescriptor<SetCoilUnit>
	{
		public SetCoilUnitDescriptor(SetCoilUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node assigns a given value to a coil defined by its ID.";
		}

		protected override EditorTexture DefinedIcon()
		{
			var texture = VisualPinball.Unity.Editor.Icons.Coil(VisualPinball.Unity.Editor.IconSize.Large, IconColor.Orange);
			return EditorTexture.Single(texture);
		}

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(SetCoilUnit.Id):
					desc.summary = "The ID of the coil to be set.";
					break;
				case nameof(SetCoilUnit.IsEnabled):
					desc.summary = "The value to assign to the coil.";
					break;
			}
		}
	}
}