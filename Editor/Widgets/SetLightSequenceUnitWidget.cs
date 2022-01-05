﻿// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
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
	[Widget(typeof(SetLightSequenceUnit))]
	public sealed class SetLightSequenceUnitWidget : UnitWidget<SetLightSequenceUnit>
	{
		private ObjectPickerInspector<LightGroupComponent> _objectInspector;
		private readonly Func<Metadata, ObjectPickerInspector<LightGroupComponent>> _setObjectInspectorConstructor;

		public SetLightSequenceUnitWidget(FlowCanvas canvas, SetLightSequenceUnit unit) : base(canvas, unit)
		{
			var tc = reference.gameObject.GetComponentInParent<TableComponent>();
			_setObjectInspectorConstructor = meta => new ObjectPickerInspector<LightGroupComponent>(meta, "Light Groups", tc);
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (port == unit.LightGroup) {
				InspectorProvider.instance.Renew(ref _objectInspector, meta, _setObjectInspectorConstructor);
				return _objectInspector;
			}
			return base.GetPortInspector(port, meta);
		}
	}
}
