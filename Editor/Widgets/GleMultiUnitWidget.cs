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

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	public abstract class GleMultiUnitWidget<TUnit> : GleUnitWidget<TUnit> where TUnit : Unit, IGleUnit, IMultiInputUnit
	{
		protected abstract IEnumerable<string> IdSuggestions(IGamelogicEngine gle);

		private readonly List<Func<Metadata, VariableNameInspector>> _idInspectorConstructorList;

		protected GleMultiUnitWidget(FlowCanvas canvas, TUnit unit) : base(canvas, unit)
		{
			_idInspectorConstructorList = new List<Func<Metadata, VariableNameInspector>>();
		}

		public override Inspector GetPortInspector(IUnitPort port, Metadata meta)
		{
			if (_idInspectorConstructorList.Count() < unit.inputCount) {
				for (var index = 0; index < unit.inputCount - _idInspectorConstructorList.Count(); index++) {
					_idInspectorConstructorList.Add(m => new VariableNameInspector(m, GetNameSuggestions));
				}
			}

			for (var index = 0; index < unit.inputCount; index++) {
				if (unit.multiInputs[index] == port) {
					var idInspector = new VariableNameInspector(meta, GetNameSuggestions);
					InspectorProvider.instance.Renew(ref idInspector, meta, _idInspectorConstructorList[index]);

					return idInspector;
				}
			}

			return base.GetPortInspector(port, meta);
		}

		private IEnumerable<string> GetNameSuggestions()
		{
			return !GleAvailable ? new List<string>() : IdSuggestions(Gle).ToList();
		}
	}
}
