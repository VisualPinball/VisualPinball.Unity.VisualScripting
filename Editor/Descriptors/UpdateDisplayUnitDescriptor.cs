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
	[Descriptor(typeof(UpdateDisplayUnit))]
	public class UpdateDisplayUnitDescriptor : UnitDescriptor<UpdateDisplayUnit>
	{
		public UpdateDisplayUnitDescriptor(UpdateDisplayUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node takes in a value and sends it to the display connected through the given ID.\n\nDisplay might be able display different types of data, so depending on how you configure your display in the Visual Scripting Gamelogic Engine, you might get multiple data inputs.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.UpdateDisplay);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(UpdateDisplayUnit.NumericInput):
					desc.summary = "Sets the display to a new numerical value.";
					break;
				case nameof(UpdateDisplayUnit.TextInput):
					desc.summary = "Sets the display to a new text value.";
					break;
				case nameof(UpdateDisplayUnit.SegmentInput):
					desc.summary = "Updates the display with new frame data.";
					break;
			}
		}
	}
}
