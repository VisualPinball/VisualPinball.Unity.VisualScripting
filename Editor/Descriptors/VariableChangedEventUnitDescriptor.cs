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
	[Descriptor(typeof(PlayerVariableChangedEventUnit))]
	public class PlayerVariableChangedEventUnitDescriptor : UnitDescriptor<PlayerVariableChangedEventUnit>
	{
		public PlayerVariableChangedEventUnitDescriptor(PlayerVariableChangedEventUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This event is emitted when a given player variable changes.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.PlayerVariableEvent);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(PlayerVariableChangedEventUnit.OldValue):
					desc.summary = "The previous value of the player variable.";
					break;
				case nameof(PlayerVariableChangedEventUnit.NewValue):
					desc.summary = "The new value of the player variable.";
					break;
			}
		}
	}


	[Descriptor(typeof(TableVariableChangedEventUnit))]
	public class TableVariableChangedEventUnitDescriptor : UnitDescriptor<TableVariableChangedEventUnit>
	{
		public TableVariableChangedEventUnitDescriptor(TableVariableChangedEventUnit target) : base(target)
		{
		}

		protected override string DefinedSummary()
		{
			return "This event is emitted when a given table variable changes.";
		}

		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.TableVariableEvent);

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);

			switch (port.key) {
				case nameof(TableVariableChangedEventUnit.OldValue):
					desc.summary = "The previous value of the table variable.";
					break;
				case nameof(TableVariableChangedEventUnit.NewValue):
					desc.summary = "The new value of the table variable.";
					break;
			}
		}
	}
}
