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

// ReSharper disable InconsistentNaming

using System;

namespace VisualPinball.Unity.VisualScripting
{

	[Serializable]
	public class VisualScriptingPlayerStatePropertyDefinition
	{
		public string Name;
		public VisualScriptingPropertyType Type;

		public string StringDefaultValue;
		public int IntegerDefaultValue;
		public float FloatDefaultValue;
		public bool BooleanDefaultValue;

		public VisualScriptingPlayerStateProperty Instantiate()
		{
			switch (Type) {
				case VisualScriptingPropertyType.String:
					return new VisualScriptingPlayerStateProperty(Name, StringDefaultValue);
				case VisualScriptingPropertyType.Integer:
					return new VisualScriptingPlayerStateProperty(Name, IntegerDefaultValue);
				case VisualScriptingPropertyType.Float:
					return new VisualScriptingPlayerStateProperty(Name, FloatDefaultValue);
				case VisualScriptingPropertyType.Boolean:
					return new VisualScriptingPlayerStateProperty(Name, BooleanDefaultValue);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public enum VisualScriptingPropertyType
	{
		String, Integer, Float, Boolean
	}
}
