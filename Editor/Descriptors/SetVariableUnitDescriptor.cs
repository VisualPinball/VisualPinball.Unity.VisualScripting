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
	[Descriptor(typeof(SetPlayerVariableUnit))]
	public class SetPlayerVariableUnitDescriptor : UnitDescriptor<SetPlayerVariableUnit>
	{
		public SetPlayerVariableUnitDescriptor(SetPlayerVariableUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node sets the value of a given player variable.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.PlayerVariable);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(SetPlayerVariableUnit.Value):
					desc.summary = "The new value of the player variable.";
					break;
				case nameof(SetPlayerVariableUnit.OutputValue):
					desc.summary = "The new value of the player variable.";
					break;
			}
		}
	}

	[Descriptor(typeof(SetTableVariableUnit))]
	public class SetTableVariableUnitDescriptor : UnitDescriptor<SetTableVariableUnit>
	{
		public SetTableVariableUnitDescriptor(SetTableVariableUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node sets the value of a given table variable.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.TableVariable);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(SetTableVariableUnit.Value):
					desc.summary = "The new value of the table variable.";
					break;

				case nameof(SetTableVariableUnit.OutputValue):
					desc.summary = "The new value of the table variable.";
					break;
			}
		}
	}
}
