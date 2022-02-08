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
	public class DisplayDefinition
	{
		public string Id = "display0";
		public int Width = 128;
		public int Height = 32;

		public bool SupportsNumericInput;
		public bool SupportsTextInput;
		public bool SupportsImageInput = true;

		public DisplayConfig DisplayConfig => new(Id, Width, Height);
	}
}
