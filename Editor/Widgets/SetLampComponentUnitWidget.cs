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

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Widget(typeof(SetLampComponentUnit))]
	public sealed class SetLampComponentUnitWidget : GleUnitWidget<SetLampComponentUnit>
	{
		private ObjectPickerInspector<ILampDeviceComponent> _objectInspector;
		private readonly Func<Metadata, ObjectPickerInspector<ILampDeviceComponent>> _setObjectInspectorConstructor;

		public SetLampComponentUnitWidget(FlowCanvas canvas, SetLampComponentUnit unit) : base(canvas, unit)
		{
			var tc = reference.gameObject.GetComponentInParent<TableComponent>();
			_setObjectInspectorConstructor = meta => new ObjectPickerInspector<ILampDeviceComponent>(meta, "Lamps", tc);
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (port == unit.LampComponent) {
				InspectorProvider.instance.Renew(ref _objectInspector, meta, _setObjectInspectorConstructor);
				return _objectInspector;
			}
			return base.GetPortInspector(port, meta);
		}

	}
}
