﻿// Visual Pinball Engine
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
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Widget(typeof(PulseCoilUnit))]
	public sealed class PulseCoilUnitWidget : GleUnitWidget<PulseCoilUnit>
	{
		private readonly List<Func<Metadata, VariableNameInspector>> _coilIdInspectorConstructorList;

		public PulseCoilUnitWidget(FlowCanvas canvas, PulseCoilUnit unit) : base(canvas, unit)
		{
			_coilIdInspectorConstructorList = new List<Func<Metadata, VariableNameInspector>>();
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (_coilIdInspectorConstructorList.Count() < unit.idCount) {
				for (var index = 0; index < unit.idCount - _coilIdInspectorConstructorList.Count(); index++) {
					_coilIdInspectorConstructorList.Add(meta => new VariableNameInspector(meta, GetNameSuggestions));
				}
			}

			for (var index = 0; index < unit.idCount; index++) {
				if (unit.Ids[index] == port) {
					VariableNameInspector coilIdInspector = new VariableNameInspector(meta, GetNameSuggestions);
					InspectorProvider.instance.Renew(ref coilIdInspector, meta, _coilIdInspectorConstructorList[index]);

					return coilIdInspector;
				}
			}

			return base.GetPortInspector(port, meta);
		}

		private IEnumerable<string> GetNameSuggestions()
		{
			if (!GameObjectAvailable) {
				return new List<string>();
			}
			var gle = Gle;
			return gle == null ? new List<string>() : gle.AvailableCoils.Select(coil => coil.Id).ToList();
		}
	}
}
