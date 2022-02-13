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
	[Serializable]
	public class PlayerState : State
	{
		public readonly int Id;

		public PlayerState(int id)
		{
			Id = id;
		}
		protected override string VariableChangedEventName => VisualScriptingEventNames.PlayerVariableChanged;
	}

	[Serializable]
	public class TableState : State
	{
		protected override string VariableChangedEventName => VisualScriptingEventNames.TableVariableChanged;
	}

	public abstract class State
	{
		protected abstract string VariableChangedEventName { get; }

		private readonly Dictionary<string, StateVariable> _variables = new();

		public void AddProperty(StateVariable variable)
		{
			_variables[variable.Id] = variable;
		}

		public T Get<T>(string variableId) where T : class
		{
			if (!_variables.ContainsKey(variableId)) {
				throw new ArgumentException($"No such variable ID ({variableId}).", nameof(variableId));
			}
			if (_variables[variableId].Type != typeof(T)) {
				throw new InvalidOperationException($"Variable \"{variableId}\" is of type {_variables[variableId].Type}, but you asked for a {typeof(T)}.");
			}

			return _variables[variableId].Get<T>();
		}

		public StateVariable GetVariable(string variableId) => _variables[variableId];

		public object Get(string variableId)
		{
			if (!_variables.ContainsKey(variableId)) {
				throw new ArgumentException($"No such variable ID ({variableId}).", nameof(variableId));
			}

			if (_variables[variableId].Type == typeof(string)) {
				return Get<string>(variableId);
			}
			if (_variables[variableId].Type == typeof(Integer)) {
				return Get<Integer>(variableId);
			}
			if (_variables[variableId].Type == typeof(Float)) {
				return Get<Float>(variableId);
			}
			if (_variables[variableId].Type == typeof(Bool)) {
				return Get<Bool>(variableId);
			}

			throw new InvalidOperationException($"Unknown type of variable {_variables[variableId].Name}.");
		}

		public void Set<T>(string variableId, T value) where T : class
		{
			if (!_variables.ContainsKey(variableId)) {
				throw new ArgumentException($"No such variable ID ({variableId}).", nameof(variableId));
			}
			if (_variables[variableId].Type != value.GetType()) {
				throw new ArgumentException($"Variable \"{variableId}\" is of type {_variables[variableId].Type}, but you provided a {value.GetType()}.", nameof(value));
			}

			var currentValue = _variables[variableId].Get<T>();
			_variables[variableId].Set(value);

			if (currentValue != value) {
				EventBus.Trigger(
					VariableChangedEventName,
					new VariableChangedArgs(variableId, currentValue, value)
				);
			}
		}
	}

	[Serializable]
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
		public override string ToString() => _value.ToString();
	}

	[Serializable]
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
		public override string ToString() => _value.ToString();
	}

	[Serializable]
	public class Bool
	{
		private readonly bool _value;
		public Bool(bool value)
		{
			_value = value;
		}
		public static implicit operator bool(Bool num) => num._value;
		public static implicit operator Bool(bool num) => new(num);
		public override string ToString() => _value.ToString();
	}

	public readonly struct VariableChangedArgs
	{
		public readonly string VariableId;

		public readonly object OldValue;
		public readonly object NewValue;

		public VariableChangedArgs(string variableId, object oldValue, object newValue)
		{
			VariableId = variableId;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
