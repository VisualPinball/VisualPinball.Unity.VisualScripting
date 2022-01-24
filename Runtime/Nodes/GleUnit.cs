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

using System.Collections.Generic;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting
{
	public abstract class GleEventUnit<TArgs> : EventUnit<TArgs>, IGleUnit
	{
		[DoNotSerialize]
		public List<string> Errors { get; } = new();
	}

	public abstract class GleUnit : Unit, IGleUnit
	{
		[DoNotSerialize]
		public List<string> Errors { get; } = new();

		[DoNotSerialize]
		protected IGamelogicEngine Gle;

		[DoNotSerialize]
		protected Player Player;
		
		protected bool AssertGle(Flow flow)
		{
			if (!UnityObjectUtility.IsUnityNull(Gle)) {
				return true;
			}
			Gle = flow.stack.gameObject.GetComponentInParent<IGamelogicEngine>();
			return Gle != null;
		}

		protected bool AssertPlayer(Flow flow)
		{
			if (!UnityObjectUtility.IsUnityNull(Player)) {
				return true;
			}
			Player = flow.stack.gameObject.GetComponentInParent<Player>();
			return Player != null;
		}
	}

	public interface IGleUnit
	{
		List<string> Errors { get; }
	}
}
