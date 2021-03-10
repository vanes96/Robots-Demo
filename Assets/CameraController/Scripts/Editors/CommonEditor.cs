using ССP.Tools.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ССP.Editors
{
    public abstract class CommonEditor : Editor
    {
        protected static readonly Dictionary<string, Tuple<SerializedProperty, string, string>> PropertiesDictionary = 
            new Dictionary<string, Tuple<SerializedProperty, string, string>>();

        protected static SerializedObject _target;
        protected const float SmallSpace = 5, MediumSpace = 10, LargeSpace = 15;
        protected const string Speed = "Speed", Smoothness = "Smoothness", Player = "Player", Border = "Border",
                               Width = "Width", Height = "Height", Size = "Size", Position = "Position", Min = "Min", Max = "Max",
                               Drag = "Drag", Key = "Key", Offset = "Offset", Target = "Target", Control = "Control", Limits_ = "Limits",
                               Minimum = "Minimum", Maximum = "Maximum", Overlay = "Overlay", Environment = "Environment", Main = "Main", 
                               Space = " ", Object = "Object", Transform = "Transform";

        protected virtual void OnEnable()
        {
            if (_target == null)
                _target = new SerializedObject(target);
        }

        protected void BeginGroup(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
            _target.Update();
            EditorGUI.BeginChangeCheck();
        }

        protected virtual void EndGroup()
        {
            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();
        }

        protected void SetProperty(ref SerializedProperty property, string name)
        {
            property = _target.FindProperty(name);
        }

        protected SerializedProperty GetProperty(string name)
        {
            return _target.FindProperty(name);
        }

        protected void ShowPropertyField(Tuple<SerializedProperty, string, string> propertyData, float spaceSize = SmallSpace)
        {
            ShowSpace(spaceSize);
            EditorGUILayout.PropertyField(propertyData.Item1, new GUIContent(propertyData.Item2 ?? propertyData.Item1.displayName, propertyData.Item3));
        }

        protected void ShowLabel(Tuple<string, string> label, bool onTop = false)
        {
            if (!onTop)
                ShowSpace(LargeSpace);

            EditorGUILayout.LabelField(new GUIContent(label.Item1, label.Item2), new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.BoldAndItalic });
            ShowSpace();
        }

        protected void ShowImage(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(image, style, options);
        }

        protected void ShowSlider(Tuple<SerializedProperty, string, string> propertyData, float spaceSize = SmallSpace, float minSliderValue = Limits.MinSliderValue, float maxSliderValue = Limits.MaxSliderValue, bool inFoldout = false)
        {
            ShowSpace(spaceSize);
            EditorGUILayout.Slider(propertyData.Item1, minSliderValue, maxSliderValue,
                new GUIContent((inFoldout ? "     " : "") + (propertyData.Item2 ?? propertyData.Item1.displayName), propertyData.Item3));
        }

        protected void ShowIntSlider(Tuple<SerializedProperty, string, string> propertyData, float spaceSize = SmallSpace, int minSliderValue = (int)Limits.MinSliderValue, int maxSliderValue = (int)Limits.MaxSliderValue, bool inFoldout = false)
        {
            ShowSpace(spaceSize);
            EditorGUILayout.IntSlider(propertyData.Item1, minSliderValue, maxSliderValue,
                new GUIContent((inFoldout ? "     " : "") + (propertyData.Item2 ?? propertyData.Item1.displayName), propertyData.Item3));
        }

        protected void ShowToolbar(ref int currentIndex, Tuple<string, string>[] texts, params GUILayoutOption[] options)
        {
            int textLength = texts.Length;
            GUIContent[] contents = new GUIContent[textLength];
            for (int i = 0; i < textLength; i++)
                contents[i] = new GUIContent(texts[i].Item1, texts[i].Item2);
            currentIndex = GUILayout.Toolbar(currentIndex, contents, options);
        }

        protected virtual void ShowList(SerializedProperty list, IEnumerable<string> columnsNames)
        {
            EditorGUILayout.PropertyField(list);

            if (list.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                EditorGUI.indentLevel--;
            }
        }

        protected void ShowSpace(float pixels = SmallSpace)
        {
            GUILayout.Space(pixels);
        }

        protected bool IsPropertyValuePositive(SerializedProperty property)
        {
            return property.floatValue > Limits.MinSliderValue; // && INT or FLOAT
        }

        protected void ShowFoldout(ref bool value, string label, GUIStyle style = null, bool toggle = true)
        {
            ShowSpace();
            value = EditorGUILayout.Foldout(value, label, toggle, style);
        }

        protected void DestroyGameObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(_pixelPrefab);
#endif
        }

    }
}
