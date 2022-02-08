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

namespace VisualPinball.Unity.VisualScripting
{
	public static class ScoreConverter
	{
		public static byte[] Convert(int score, int displaySize)
		{
			return Text(score.ToString().PadLeft(displaySize, ' ')[..displaySize], displaySize);
		}

		public static byte[] Text(string text, int displaySize)
		{
			var source = GenerateAlphaNumeric(text);
			const int size = sizeof(short);
			var target = new byte[displaySize * size];
			Buffer.BlockCopy(source, 0, target, 0, System.Math.Min(source.Length, displaySize) * size);
			return target;
		}

		private static ushort[] GenerateAlphaNumeric(string text)
		{
			var data = new ushort[text.Length];
			for (var i = 0; i < text.Length; i++) {
				if (AlphaNumericMap.ContainsKey(text[i])) {
					data[i] = AlphaNumericMap[text[i]];
				} else {
					data[i] = AlphaNumericMap[' '];
				}
			}
			return data;
		}

		private static readonly Dictionary<char, ushort> AlphaNumericMap = new Dictionary<char, ushort> {
			{ '0', 0x443f },
			{ '1', 0x406 },
			{ '2', 0x85b },
			{ '3', 0x80f },
			{ '4', 0x866 },
			{ '5', 0x1069 },
			{ '6', 0x87d },
			{ '7', 0x7 },
			{ '8', 0x87f },
			{ '9', 0x86f },
			{ ' ', 0x0 },
			{ '!', 0x86 },
			{ '"', 0x202 },
			{ '#', 0x2a4e },
			{ '$', 0x2a6d },
			{ '%', 0x7f64 },
			{ '&', 0x1359 },
			{ '\'', 0x200 },
			{ '(', 0x1400 },
			{ ')', 0x4100 },
			{ '*', 0x7f40 },
			{ '+', 0x2a40 },
			{ ',', 0x4000 },
			{ '-', 0x840 },
			{ '.', 0x80 },
			{ '/', 0x4400 },
			{ ':', 0x2200 },
			{ ';', 0x4200 },
			{ '<', 0x1440 },
			{ '=', 0x848 },
			{ '>', 0x4900 },
			{ '?', 0x2883 },
			{ '@', 0xa3b },
			{ 'A', 0x877 },
			{ 'B', 0x2a0f },
			{ 'C', 0x39 },
			{ 'D', 0x220f },
			{ 'E', 0x79 },
			{ 'F', 0x71 },
			{ 'G', 0x83d },
			{ 'H', 0x876 },
			{ 'I', 0x2209 },
			{ 'J', 0x1e },
			{ 'K', 0x1470 },
			{ 'L', 0x38 },
			{ 'M', 0x536 },
			{ 'N', 0x1136 },
			{ 'O', 0x3f },
			{ 'P', 0x873 },
			{ 'Q', 0x103f },
			{ 'R', 0x1873 },
			{ 'S', 0x86d },
			{ 'T', 0x2201 },
			{ 'U', 0x3e },
			{ 'V', 0x4430 },
			{ 'W', 0x5036 },
			{ 'X', 0x5500 },
			{ 'Y', 0x86e },
			{ 'Z', 0x4409 },
			{ '[', 0x39 },
			{ '\\', 0x1100 },
			{ ']', 0xf },
			{ '^', 0x5000 },
			{ '_', 0x8 },
			{ '`', 0x100 },
			{ 'a', 0x2058 },
			{ 'b', 0x1078 },
			{ 'c', 0x858 },
			{ 'd', 0x480e },
			{ 'e', 0x4058 },
			{ 'f', 0x2c40 },
			{ 'g', 0xc0e },
			{ 'h', 0x2070 },
			{ 'i', 0x2000 },
			{ 'j', 0x4210 },
			{ 'k', 0x3600 },
			{ 'l', 0x30 },
			{ 'm', 0x2854 },
			{ 'n', 0x2050 },
			{ 'o', 0x85c },
			{ 'p', 0x170 },
			{ 'q', 0xc06 },
			{ 'r', 0x50 },
			{ 's', 0x1808 },
			{ 't', 0x78 },
			{ 'u', 0x1c },
			{ 'v', 0x4010 },
			{ 'w', 0x5014 },
			{ 'x', 0x5500 },
			{ 'y', 0xa0e },
			{ 'z', 0x4048 },
			{ '{', 0x4149 },
			{ '|', 0x2200 },
			{ '}', 0x1c09 },
			{ '~', 0x4c40 },
		};
	}
}
