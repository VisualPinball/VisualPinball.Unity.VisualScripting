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
	[Descriptor(typeof(GetLampUnit))]
	public class GetLampUnitDescriptor : UnitDescriptor<GetLampUnit>
	{
		public GetLampUnitDescriptor(GetLampUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node retrieves the current intensity of the lamp, as well as if the lamp is enabled or not.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.Light(IconSize.Large, IconColor.Orange));

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(GetLampUnit.Id):
					desc.summary = "The ID of the lamp for which the intensity is returned.";
					break;
				case nameof(GetLampUnit.Value):
					var getLampUnit = port.unit as GetLampUnit;
					switch (getLampUnit!.DataType) {
						case LampDataType.OnOff:
							desc.label = "Lit";
							desc.summary = "On or off.";
							break;
						case LampDataType.Status:
							desc.label = "Status";
							desc.summary = "On, off or blinking.";
							break;
						case LampDataType.Intensity:
							desc.label = "Intensity";
							desc.summary = "The intensity of the lamp (value depends on the maximal intensity of the mapping).";
							break;
						case LampDataType.Color:
							desc.label = "Color";
							desc.summary = "The color of the lamp.";
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
		}
	}
}
