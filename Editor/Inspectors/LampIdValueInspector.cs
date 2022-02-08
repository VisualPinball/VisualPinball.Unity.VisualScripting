using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{

    public sealed class LampIdValueInspector : Inspector
    {
        public LampIdValueInspector(Metadata metadata, Func<IEnumerable<string>> getSuggestions) : base(metadata)
        {
            Ensure.That(nameof(getSuggestions)).IsNotNull(getSuggestions);

            this.getSuggestions = getSuggestions;
        }

        public Func<IEnumerable<string>> getSuggestions { get; }


        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, GetFieldHeight(width, label), label);
        }

        private float GetFieldHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private List<string> suggestions = new List<string>();

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var fieldPosition = position.VerticalSection(ref y, GetFieldHeight(position.width, GUIContent.none));

            LampIdValue lampIdValue = (LampIdValue)metadata.value;

            var valueWidth = LudiqGUI.GetTextFieldAdaptiveWidth(lampIdValue.value);

            var textFieldPosition = new Rect
                (
                fieldPosition.x,
                fieldPosition.y,
                fieldPosition.width - Styles.popup.fixedWidth - valueWidth - 60,
                fieldPosition.height
                );

            var popupPosition = new Rect
                (
                textFieldPosition.xMax,
                fieldPosition.y,
                Styles.popup.fixedWidth,
                fieldPosition.height
                );

            var newIdValue = EditorGUI.TextField(textFieldPosition, lampIdValue.id, Styles.textField);

            // Micro optimizing memory here because it's a pretty substantial alloc

            suggestions.Clear();
            suggestions.AddRange(getSuggestions());

            EditorGUI.BeginDisabledGroup(suggestions.Count == 0);

            var suggestionsArray = getSuggestions().ToArray();
            var currentSuggestionIndex = Array.IndexOf(suggestionsArray, lampIdValue.id);

            EditorGUI.BeginChangeCheck();

            var newSuggestionIndex = EditorGUI.Popup(popupPosition, currentSuggestionIndex, suggestionsArray, Styles.popup);

            if (EditorGUI.EndChangeCheck())
            {
                newIdValue = suggestions[newSuggestionIndex];
            }

            EditorGUI.EndDisabledGroup();

            var valueFieldPosition = new Rect
                (
                fieldPosition.x + fieldPosition.width - valueWidth,
                fieldPosition.y,
                valueWidth,
                fieldPosition.height
                );

            var valueLabelFieldPosition = new Rect
             (
             fieldPosition.x + fieldPosition.width - valueWidth - 50,
             fieldPosition.y,
             50,
             fieldPosition.height
             );


            EditorGUI.LabelField(valueLabelFieldPosition, "Value");

            var newValue = LudiqGUI.DraggableIntField(valueFieldPosition, lampIdValue.value);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = new LampIdValue
                {
                    id = newIdValue,
                    value = newValue
                };
            }
        }

        public override float GetAdaptiveWidth()
        {
            LampIdValue lampIdValue = (LampIdValue)metadata.value;

            return Mathf.Max(30, EditorStyles.textField.CalcSize(new GUIContent(lampIdValue.id)).x + 1 + Styles.popup.fixedWidth) +
                LudiqGUI.GetTextFieldAdaptiveWidth(lampIdValue.value) + 70;
        }

        public static class Styles
        {
            static Styles()
            {
                textField = new GUIStyle(EditorStyles.textField);

                popup = new GUIStyle("TextFieldDropDown");
                popup.fixedWidth = 18;
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
}
