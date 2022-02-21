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

using System;
using Unity.VisualScripting;
using VisualPinball.Unity.Editor;
using IconSize = VisualPinball.Unity.Editor.IconSize;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(SetLampUnit))]
	public class SetLampUnitDescriptor : UnitDescriptor<SetLampUnit>
	{
		public SetLampUnitDescriptor(SetLampUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node assigns a given value to a lamp defined by its mapped ID. This will also trigger the lamp changed event and update the internal status.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Light(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			if (port.key == nameof(SetLampUnit.Value)) {
				var setLampUnit = port.unit as SetLampUnit;
				switch (setLampUnit!.DataType) {
					case LampDataType.OnOff:
						desc.label = "Lit?";
						desc.summary = "On or off.";
						break;
					case LampDataType.Status:
						desc.label = "Status";
						desc.summary = "On, off or blinking.";
						break;
					case LampDataType.Intensity:
						desc.label = "Intensity";
						desc.summary = "The intensity of the lamp (0-1).";
						break;
					case LampDataType.Color:
						desc.label = "Color";
						desc.summary = "The color of the lamp.";
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (int.TryParse(port.key, out int id)) {
				id += 1;

				desc.label = $"Lamp ID {id}";
				desc.summary = $"Lamp ID {id} of the lamp to set the intensity";
			}
		}
	}
}
