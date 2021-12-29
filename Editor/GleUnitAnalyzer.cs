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

using System.Collections.Generic;
using Unity.VisualScripting;
using VisualPinball.Unity.VisualScripting;

namespace Editor
{

	[Analyser(typeof(SwitchEventUnit))]
	public class SwitchEventUnitAnalyzer : GleUnitAnalyser<VisualScriptingScriptEvent>
	{
		public SwitchEventUnitAnalyzer(GraphReference reference, SwitchEventUnit target) : base(reference, target)
		{
		}
	}

	public abstract class GleUnitAnalyser<TArgs> : UnitAnalyser<GleEventUnit<TArgs>>
	{
		protected GleUnitAnalyser(GraphReference reference, GleEventUnit<TArgs> target) : base(reference, target) { }

		protected override IEnumerable<Warning> Warnings()
		{
			foreach (var baseWarning in base.Warnings()) {
				yield return baseWarning;
			}

			foreach (var warning in unit.Errors) {
				yield return Warning.Error(warning);
			}
		}
	}
}
