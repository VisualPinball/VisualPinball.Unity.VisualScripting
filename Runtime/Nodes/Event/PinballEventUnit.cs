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
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting
{
	/// <summary>
	/// A special named event with any amount of parameters called manually with the 'Trigger Custom Event' unit.
	/// </summary>
	[UnitTitle("On Pinball Event")]
	[UnitCategory("Events/Visual Pinball")]
	public class PinballEventUnit : GameObjectEventUnit<PinballEventArgs>
	{
		[Serialize, Inspectable, UnitHeaderInspectable]
		public EventDefinition Event { get; set; }

		public override Type MessageListenerType => null;
		protected override string hookName => VisualScriptingEventNames.PinballEvent;

		[SerializeAs(nameof(argumentCount))]
		private int _argumentCount;

		[DoNotSerialize]
		[Inspectable, UnitHeaderInspectable("Arguments")]
		public int argumentCount
		{
			get => _argumentCount;
			set => _argumentCount = Mathf.Clamp(value, 0, 10);
		}

		[DoNotSerialize]
		public List<ValueOutput> argumentPorts { get; } = new List<ValueOutput>();

		protected override void Definition()
		{
			base.Definition();
			argumentPorts.Clear();
			for (var i = 0; i < argumentCount; i++) {
				argumentPorts.Add(ValueOutput<object>("argument_" + i));
			}
		}

		protected override bool ShouldTrigger(Flow flow, PinballEventArgs args)
		{
			return Event.Id.Equals(args.Id);
		}

		protected override void AssignArguments(Flow flow, PinballEventArgs args)
		{
			for (var i = 0; i < argumentCount; i++) {
				flow.SetValue(argumentPorts[i], args.Args[i]);
			}
		}

		public static void Trigger(GameObject target, string name, params object[] args)
		{
			EventBus.Trigger(VisualScriptingEventNames.PinballEvent, target, new PinballEventArgs(name, args));
		}
	}

	public readonly struct PinballEventArgs
	{
		public readonly string Id;

		public readonly object[] Args;

		public PinballEventArgs(string id, params object[] args)
		{
			Id = id;
			Args = args;
		}
	}
}
