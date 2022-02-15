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
	[Descriptor(typeof(LampSequenceUnit))]
	public class LampSequenceUnitDescriptor : UnitDescriptor<LampSequenceUnit>
	{
		public LampSequenceUnitDescriptor(LampSequenceUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node turns on/off lamps in a sequence defined by lamp ids.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.LampSequence);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			if (port.key == nameof(LampSequenceUnit.Step)) {
				desc.summary = "How position is increased and to how many lights the intensity is applied to.";
			}
			else if (port.key == nameof(LampSequenceUnit.Value)) {
				desc.summary = "The intensity to apply to the current step (0-1).";
			}
			else if (int.TryParse(port.key, out int id)) {
				id += 1;

				desc.label = $"Lamp ID {id}";
				desc.summary = $"Lamp ID {id} to cycle through.";
			}
		}
	}
}
