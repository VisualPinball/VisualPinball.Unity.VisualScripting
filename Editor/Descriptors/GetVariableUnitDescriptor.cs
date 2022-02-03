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
	[Descriptor(typeof(GetPlayerVariableUnit))]
	public class GetPlayerVariableUnitDescriptor : UnitDescriptor<GetPlayerVariableUnit>
	{
		public GetPlayerVariableUnitDescriptor(GetPlayerVariableUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node retrieves the value of a given player variable.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.PlayerVariable);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(GetPlayerVariableUnit.Value):
					desc.summary = "The current value of the player variable.";
					break;
			}
		}
	}

	[Descriptor(typeof(GetTableVariableUnit))]
	public class GetTableVariableUnitDescriptor : UnitDescriptor<GetTableVariableUnit>
	{
		public GetTableVariableUnitDescriptor(GetTableVariableUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This node retrieves the value of a given table variable.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.TableVariable);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(GetTableVariableUnit.Value):
					desc.summary = "The current value of the table variable.";
					break;
			}
		}
	}
}
