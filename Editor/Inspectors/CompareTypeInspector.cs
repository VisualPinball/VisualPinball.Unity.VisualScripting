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
    [Inspector(typeof(CompareType))]
    public class CompareTypeInspector : Inspector
    {
        private List<CompareType> CompareTypes = Enum.GetValues(typeof(CompareType)).Cast<CompareType>().ToList();
        private string[] CompareTypeDescriptions = Enum.GetValues(typeof(CompareType)).Cast<CompareType>().Select(x => GetEnumDescription(x)).ToArray();
        
        public CompareTypeInspector(Metadata metadata) : base(metadata) {
        }

        public override void Initialize()
        {
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

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var index = CompareTypes.FindIndex(c => c == (CompareType)metadata.value);
            var newIndex = EditorGUI.Popup(fieldPosition, index, CompareTypeDescriptions);

            if (EndBlock(metadata))
			{
                metadata.RecordUndo();
                metadata.value = CompareTypes[newIndex];
            }
        }

        public override float GetAdaptiveWidth()
        {
            return Mathf.Max(18, EditorStyles.popup.CalcSize(new GUIContent(GetEnumDescription((CompareType)metadata.value))).x + Styles.popup.fixedWidth);
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return (attribute == null ? value.ToString() : attribute.Description).Replace('/', '\u2215');
        }
    }

    public static class Styles
    {
        static Styles()
        {
            popup = new GUIStyle("TextFieldDropDown");
            popup.fixedWidth = 10;
            popup.clipping = TextClipping.Clip;
            popup.normal.textColor = ColorPalette.transparent;
            popup.active.textColor = ColorPalette.transparent;
            popup.hover.textColor = ColorPalette.transparent;
            popup.focused.textColor = ColorPalette.transparent;
            popup.onNormal.textColor = ColorPalette.transparent;
            popup.onActive.textColor = ColorPalette.transparent;
            popup.onHover.textColor = ColorPalette.transparent;
            popup.onFocused.textColor = ColorPalette.transparent;
        }

        public static readonly GUIStyle textField;
        public static readonly GUIStyle popup;
    }
}
