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
	public class PlayerVariableDefinition : VariableDefinition
	{
	}

	[Serializable]
	public class TableVariableDefinition : VariableDefinition
	{
	}

	public abstract class VariableDefinition
	{
		public string Id;
		public string Name;
		public VariableType Type;

		public string StringDefaultValue;
		public int IntegerDefaultValue;
		public float FloatDefaultValue;
		public bool BooleanDefaultValue;

		public StateVariable Instantiate()
		{
			switch (Type) {
				case VariableType.String:
					return new StateVariable(Id, Name, StringDefaultValue);
				case VariableType.Integer:
					return new StateVariable(Id, Name, IntegerDefaultValue);
				case VariableType.Float:
					return new StateVariable(Id, Name, FloatDefaultValue);
				case VariableType.Boolean:
					return new StateVariable(Id, Name, BooleanDefaultValue);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public bool HasId => !string.IsNullOrEmpty(Id);
		public void GenerateId() => Id = Guid.NewGuid().ToString()[..13];
	}

	public enum VariableType
	{
		String, Integer, Float, Boolean
	}
}
