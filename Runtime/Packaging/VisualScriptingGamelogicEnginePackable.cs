// Visual Pinball Engine
// Copyright (C) 2026 freezy and VPE Team
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

// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using VisualPinball.Engine.Game.Engines;

namespace VisualPinball.Unity.VisualScripting
{
	/// <summary>
	/// The serializable configuration of the <see cref="VisualScriptingGamelogicEngine"/>: the
	/// variable, event and display declarations as well as the switch/coil/lamp/wire definitions
	/// the visual scripts address by id.
	///
	/// This is the <see cref="IPackable.Pack"/> payload. The actual visual scripting graphs (the
	/// <c>ScriptMachine</c>/<c>StateMachine</c> components and their graph assets) are packaged
	/// separately as references, see <see cref="VisualScriptingGraphPackager"/>.
	/// </summary>
	public struct VisualScriptingGamelogicEnginePackable
	{
		public List<TableVariableDefinition> TableVariableDefinitions;
		public List<PlayerVariableDefinition> PlayerVariableDefinitions;
		public List<EventDefinition> EventDefinitions;
		public DisplayDefinition[] Displays;
		public VisualScriptingSwitch[] Switches;
		public VisualScriptingCoil[] Coils;
		public VisualScriptingLamp[] Lamps;
		public GamelogicEngineWire[] Wires;

		public static byte[] Pack(VisualScriptingGamelogicEngine gle)
		{
			return PackageApi.Packer.Pack(new VisualScriptingGamelogicEnginePackable {
				TableVariableDefinitions = gle.TableVariableDefinitions,
				PlayerVariableDefinitions = gle.PlayerVariableDefinitions,
				EventDefinitions = gle.EventDefinitions,
				Displays = gle.Displays,
				Switches = gle.Switches,
				Coils = gle.Coils,
				Lamps = gle.Lamps,
				Wires = gle.Wires,
			});
		}

		public static void Unpack(byte[] bytes, VisualScriptingGamelogicEngine gle)
		{
			var data = PackageApi.Packer.Unpack<VisualScriptingGamelogicEnginePackable>(bytes);
			gle.TableVariableDefinitions = data.TableVariableDefinitions ?? new List<TableVariableDefinition>();
			gle.PlayerVariableDefinitions = data.PlayerVariableDefinitions ?? new List<PlayerVariableDefinition>();
			gle.EventDefinitions = data.EventDefinitions ?? new List<EventDefinition>();
			gle.Displays = data.Displays ?? Array.Empty<DisplayDefinition>();
			gle.Switches = data.Switches ?? Array.Empty<VisualScriptingSwitch>();
			gle.Coils = data.Coils ?? Array.Empty<VisualScriptingCoil>();
			gle.Lamps = data.Lamps ?? Array.Empty<VisualScriptingLamp>();
			gle.Wires = data.Wires ?? Array.Empty<GamelogicEngineWire>();
		}
	}
}
