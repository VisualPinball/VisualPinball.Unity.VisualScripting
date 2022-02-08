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
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Widget(typeof(SwitchEnabledEventUnit))]
	public sealed class SwitchEnabledEventUnitWidget : GleUnitWidget<SwitchEnabledEventUnit>
	{
		private readonly List<Func<Metadata, VariableNameInspector>> _switchIdInspectorConstructorList;

		public SwitchEnabledEventUnitWidget(FlowCanvas canvas, SwitchEnabledEventUnit unit) : base(canvas, unit)
		{
			_switchIdInspectorConstructorList = new List<Func<Metadata, VariableNameInspector>>();
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (_switchIdInspectorConstructorList.Count() < unit.idCount) {
				for (var index = 0; index < unit.idCount - _switchIdInspectorConstructorList.Count(); index++) {
					_switchIdInspectorConstructorList.Add(meta => new VariableNameInspector(meta, GetNameSuggestions));
				}
			}

			for (var index = 0; index < unit.idCount; index++) {
				if (unit.Ids[index] == port) {
					VariableNameInspector switchIdInspector = new VariableNameInspector(meta, GetNameSuggestions);
					InspectorProvider.instance.Renew(ref switchIdInspector, meta, _switchIdInspectorConstructorList[index]);

					return switchIdInspector;
				}
			}

			return base.GetPortInspector(port, meta);
		}

		private IEnumerable<string> GetNameSuggestions()
		{
			return !GleAvailable
				? new List<string>()
				: Gle.RequestedSwitches.Select(sw => sw.Id).ToList();
		}
	}
}
