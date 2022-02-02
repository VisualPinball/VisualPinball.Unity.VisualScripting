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
	[Widget(typeof(PlayerVariableSetUnit))]
	public sealed class PlayerStateSetUnitWidget : GleUnitWidget<PlayerVariableSetUnit>
	{
		public PlayerStateSetUnitWidget(FlowCanvas canvas, PlayerVariableSetUnit unit) : base(canvas, unit)
		{
			//_intInspectorConstructor = meta => new IntInspector(meta);
		}

		// private IntInspector _intInspector;
		//
		// private readonly Func<Metadata, IntInspector> _intInspectorConstructor;

		// public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		// {
		// 	if (port != unit.Value) {
		// 		return base.GetPortInspector(port, meta);
		// 	}
		//
		// 	switch (unit.Property.Type) {
		// 		case VisualScriptingPropertyType.String:
		// 			break;
		// 		case VisualScriptingPropertyType.Integer:
		// 			InspectorProvider.instance.Renew(ref _intInspector, meta, _intInspectorConstructor);
		// 			return _intInspector;
		// 		case VisualScriptingPropertyType.Float:
		// 			break;
		// 		case VisualScriptingPropertyType.Boolean:
		// 			break;
		// 		default:
		// 			throw new ArgumentOutOfRangeException();
		// 	}
		//
		// 	return base.GetPortInspector(port, meta);
		// }

	}
}
