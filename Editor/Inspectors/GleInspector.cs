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
using VisualPinball.Unity;
using VisualPinball.Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	public abstract class GleInspector : Inspector
	{
		protected GleInspector(Metadata metadata) : base(metadata) { }

		private const string NoGleError = "Unable to find Gamelogic Engine in scene.";

		protected VisualScriptingGamelogicEngine Gle {
			get {
				if (_gle != null) {
					return _gle;
				}
				var tableComponent = TableSelector.Instance.SelectedOrFirstTable;
				if (tableComponent != null) {
					_gle = tableComponent.GetComponentInChildren<VisualScriptingGamelogicEngine>();
				}
				if (_gle == null && ErrorMessage == null) {
					ErrorMessage = NoGleError;

				} else if (_gle != null && ErrorMessage == NoGleError) {
					ErrorMessage = null;
				}
				return _gle;
			}
		}
		protected string ErrorMessage;

		private VisualScriptingGamelogicEngine _gle;
	}
}
