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

using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	public static class GleUnitWidget
	{
		public static readonly NodeColorMix Color = new() {
			red = 0.92549019607843137254901960784314f,
			green = 0.51764705882352941176470588235294f,
			blue = 0.23921568627450980392156862745098f
		};
	}

	public abstract class GleUnitWidget<TUnit> : UnitWidget<TUnit> where TUnit : Unit, IGleUnit
	{
		protected override NodeColorMix baseColor => GleAvailable ? GleUnitWidget.Color : NodeColor.Red;
		protected IGamelogicEngine Gle;
		protected VisualScriptingGamelogicEngine VsGle;
		protected bool GleAvailable => Gle != null;
		protected bool VsGleAvailable => VsGle != null;

		protected GleUnitWidget(FlowCanvas canvas, TUnit unit) : base(canvas, unit)
		{
			var table = TableSelector.Instance.SelectedOrFirstTable;
			if (table != null) {
				Gle = table.GetComponentInChildren<IGamelogicEngine>();
				VsGle = table.GetComponentInChildren<VisualScriptingGamelogicEngine>();
			}
			if (!GleAvailable) {
				Debug.LogError($"Cannot find GLE for {GetType()}.");
			}
		}
	}
}
