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

namespace VisualPinball.Unity.VisualScripting
{
	public class StateVariable
	{
		public string Id;
		public string Name;
		public Type Type; // always *object* type (string, Integer, Float, Bool)
		private object _value;

		public StateVariable(string id, string name, string initialValue) : this(id, name, typeof(string), initialValue)
		{
		}
		public StateVariable(string id, string name, int initialValue) : this(id, name, typeof(Integer),new Integer(initialValue))
		{
		}
		public StateVariable(string id, string name, float initialValue) : this(id, name, typeof(Float), new Float(initialValue))
		{
		}
		public StateVariable(string id, string name, bool initialValue) : this(id, name, typeof(Bool), new Bool(initialValue))
		{
		}

		private StateVariable(string id, string name, Type type, object initialValue)
		{
			Id = id;
			Name = name;
			Type = type;
			_value = initialValue;
		}

		public T Get<T>() where T : class
		{
			return _value as T;
		}

		public void Set<T>(T value)
		{
			_value = value;
		}
	}
}
