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

namespace VisualPinball.Unity.VisualScripting.Editor
{
	public abstract class GleUnitWidget<TUnit> : UnitWidget<TUnit> where TUnit : Unit, IGleUnit
	{
		protected override NodeColorMix baseColor => GleAvailable ? NodeColorMix.TealReadable : new NodeColorMix { red = 1f, green = 0f, blue = 0f };
		protected bool GameObjectAvailable => reference != null && reference.gameObject != null;
		protected IGamelogicEngine Gle => reference.gameObject.GetComponentInParent<IGamelogicEngine>();
		protected VisualScriptingGamelogicEngine VsGle => reference.gameObject.GetComponentInParent<VisualScriptingGamelogicEngine>();
		private bool GleAvailable => GameObjectAvailable && Gle != null;
		private bool VsGleAvailable => GameObjectAvailable && VsGle != null;

		protected GleUnitWidget(FlowCanvas canvas, TUnit unit) : base(canvas, unit)
		{
			if (!GameObjectAvailable) {
				unit.Errors.Add("Not attached to GameObject. You need to attach this graph to a flow machine sitting on a GameObject in order to use it.");

			} else if (!GleAvailable) {
				unit.Errors.Add("No gamelogic engine found. One of the GameObject's parents must have a gamelogic engine component.");
			}
		}
	}
}
