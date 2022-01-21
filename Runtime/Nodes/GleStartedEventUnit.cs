// Visual Pinball Engine
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

using System;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	[UnitTitle("On Gamelogic Engine Started Event")]
	[UnitCategory("Events\\Visual Pinball")]
	public sealed class GleStartedEventUnit : EventUnit<EventArgs>
	{
		protected override bool register => true;

		public override EventHook GetHook(GraphReference reference)
		{
			return new EventHook(VisualScriptingEventNames.GleStartedEvent);
		}
	}
}
