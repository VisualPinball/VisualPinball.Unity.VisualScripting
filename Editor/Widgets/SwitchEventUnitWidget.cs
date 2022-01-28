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
	[Widget(typeof(SwitchEventUnit))]
	public sealed class SwitchEventUnitWidget : GleUnitWidget<SwitchEventUnit>
	{
		private VariableNameInspector _switchIdInspector;
		private readonly Func<Metadata, VariableNameInspector> _switchIdInspectorConstructor;

		public SwitchEventUnitWidget(FlowCanvas canvas, SwitchEventUnit unit) : base(canvas, unit)
		{
			_switchIdInspectorConstructor = meta => new VariableNameInspector(meta, GetNameSuggestions);
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (port == unit.Id) {
				InspectorProvider.instance.Renew(ref _switchIdInspector, meta, _switchIdInspectorConstructor);

				return _switchIdInspector;
			}

			return base.GetPortInspector(port, meta);
		}

		private IEnumerable<string> GetNameSuggestions()
		{
			if (!GameObjectAvailable) {
				return new List<string>();
			}
			var gle = Gle;
			return gle == null ? new List<string>() : gle.AvailableSwitches.Select(sw => sw.Id).ToList();
		}
	}
}
