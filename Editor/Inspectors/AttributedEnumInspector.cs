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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	public abstract class AttributedEnumInspector<TEnum> : Inspector
	{
		private List<TEnum> Enums;
		private string[] EnumDescriptions;
   
		public AttributedEnumInspector(Metadata metadata) : base(metadata) {
		}

		public override void Initialize()
		{
			Enums = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
			EnumDescriptions = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => GetEnumDescription(x)).ToArray();

			metadata.instantiate = true;

			base.Initialize();
		}

		protected override float GetHeight(float width, GUIContent label)
		{
			return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
		}

		protected override void OnGUI(Rect position, GUIContent label)
		{
			position = BeginLabeledBlock(metadata, position, label);

			var fieldPosition = new Rect(
				position.x,
				position.y,
				position.width,
				EditorGUIUtility.singleLineHeight);

			var index = Enums.FindIndex(c => c.Equals(metadata.value));
			var newIndex = EditorGUI.Popup(fieldPosition, index, EnumDescriptions);

			if (EndBlock(metadata)) {
				metadata.RecordUndo();
				metadata.value = Enums[newIndex];
			}
		}

		public override float GetAdaptiveWidth()
		{
			return Mathf.Max(18, EditorStyles.popup.CalcSize(new GUIContent(GetEnumDescription((TEnum)metadata.value))).x);
		}

		private string GetEnumDescription(TEnum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

			return (attribute == null ? value.ToString() : attribute.Description).Replace('/', '\u2215');
		}
	}
}
