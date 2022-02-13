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

using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Descriptor(typeof(AllSwitchesEnabledEventUnit))]
	public class AllSwitchesEnabledEventUnitDescriptor : MultiUnitDescriptor<AllSwitchesEnabledEventUnit>
	{
		public AllSwitchesEnabledEventUnitDescriptor(AllSwitchesEnabledEventUnit target) : base(target) { }
		protected override string ItemLabel(int id) => $"Switch ID {id}";
		protected override string ItemDescription(int id) => $"Switch ID {id} to look for enabled status.";
		protected override string DefinedSummary() => "This node triggers an event when the last switch in the list gets enabled.";
		protected override EditorTexture DefinedIcon() => EditorTexture.Single(Unity.Editor.Icons.SwitchEvent);
	}

	public abstract class MultiUnitDescriptor<TUnit> : UnitDescriptor<TUnit> where TUnit : class, IUnit
	{
		protected abstract string ItemLabel(int id);
		protected abstract string ItemDescription(int id);

		protected MultiUnitDescriptor(TUnit target) : base(target)
		{
		}

		protected override void DefinedPort(IUnitPort port, UnitPortDescription desc)
		{
			base.DefinedPort(port, desc);
			var match = new Regex("^(item)([0-9]+)$").Match(port.key);
			if (match.Success) {
				var id = int.Parse(match.Groups[2].Value) + 1;
				desc.label = ItemLabel(id);
				desc.summary = ItemDescription(id);
			}
		}
	}
}
