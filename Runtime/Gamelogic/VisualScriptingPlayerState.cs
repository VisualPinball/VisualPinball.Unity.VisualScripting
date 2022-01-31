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
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	public class VisualScriptingPlayerState
	{
		public readonly int Id;

		private readonly Dictionary<string, VisualScriptingPlayerStateProperty> _properties = new();

		public VisualScriptingPlayerState(int id)
		{
			Id = id;
		}

		public void AddProperty(VisualScriptingPlayerStateProperty property)
		{
			_properties[property.Name] = property;
		}

		public T Get<T>(string propertyName) where T : class
		{
			if (!_properties.ContainsKey(propertyName)) {
				throw new ArgumentException($"No such property name ({propertyName}).", nameof(propertyName));
			}
			if (_properties[propertyName].Type != typeof(T)) {
				throw new InvalidOperationException($"Property \"{propertyName}\" is of type {_properties[propertyName].Type}, but you asked for a {typeof(T)}.");
			}

			return _properties[propertyName].Get<T>();
		}

		public void Set<T>(string propertyName, T value) where T : class
		{
			if (!_properties.ContainsKey(propertyName)) {
				throw new ArgumentException($"No such property name ({propertyName}).", nameof(propertyName));
			}
			if (_properties[propertyName].Type != value.GetType()) {
				throw new ArgumentException($"Property \"{propertyName}\" is of type {_properties[propertyName].Type}, but you provided a {value.GetType()}.", nameof(value));
			}

			var currentValue = _properties[propertyName].Get<T>();
			_properties[propertyName].Set(value);

			if (currentValue != value) {
				EventBus.Trigger(VisualScriptingEventNames.CurrentPlayerStateChanged, new PlayerStateChangedArgs(propertyName));
			}
		}

		internal bool Has<T>(string propertyName) => _properties.ContainsKey(propertyName) && typeof(T) == _properties[propertyName].Type;
	}

	public class Integer
	{
		private readonly int _value;
		public Integer(int value)
		{
			_value = value;
		}
		public static Integer operator +(Integer lhs, Integer rhs) => new(lhs._value + rhs._value);
		public static Integer operator -(Integer lhs, Integer rhs) => new(lhs._value - rhs._value);
		public static Integer operator *(Integer lhs, Integer rhs) => new(lhs._value * rhs._value);
		public static Integer operator /(Integer lhs, Integer rhs) => new(lhs._value / rhs._value);
		public static implicit operator int(Integer num) => num._value;
		public static implicit operator Integer(int num) => new(num);
	}

	public class Float
	{
		private readonly float _value;
		public Float(float value)
		{
			_value = value;
		}
		public static Float operator +(Float lhs, Float rhs) => new(lhs._value + rhs._value);
		public static Float operator -(Float lhs, Float rhs) => new(lhs._value - rhs._value);
		public static Float operator *(Float lhs, Float rhs) => new(lhs._value * rhs._value);
		public static Float operator /(Float lhs, Float rhs) => new(lhs._value / rhs._value);
		public static implicit operator float(Float num) => num._value;
		public static implicit operator Float(float num) => new(num);
	}

	public class Bool
	{
		private readonly bool _value;
		public Bool(bool value)
		{
			_value = value;
		}
		public static implicit operator bool(Bool num) => num._value;
		public static implicit operator Bool(bool num) => new(num);
	}

	public readonly struct PlayerStateChangedArgs
	{
		public readonly string PropertyName;

		public PlayerStateChangedArgs(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
