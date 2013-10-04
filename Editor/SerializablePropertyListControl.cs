// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// List control for serialized property arrays which can be used within custom editor
	/// windows and inspectors with support for drag and drop reordering of list items.
	/// </summary>
	[Serializable]
	public class SerializablePropertyListControl : ReorderableListControl {

		#region SerializedProperty Abstraction

		/// <summary>
		/// Implementation of reorderable list data for serialized properties.
		/// </summary>
		private sealed class SerializedPropertyListData : IReorderableListData {

			/// <summary>
			/// Style for right-aligned label for element number prefix.
			/// </summary>
			private static GUIStyle s_RightAlignedLabelStyle;

			/// <summary>
			/// The serialized array property.
			/// </summary>
			public SerializedProperty arrayProperty;
			/// <summary>
			/// Width of element number prefix.
			/// </summary>
			private float _labelWidth;

			/// <summary>
			/// Initializes a new instance of <see cref="SerializedPropertyListData"/>.
			/// </summary>
			/// <param name="arrayProperty">Serialized property for entire array.</param>
			public SerializedPropertyListData(SerializedProperty arrayProperty) {
				this.arrayProperty = arrayProperty;

				int digitCount = Mathf.Max(2, Mathf.CeilToInt(Mathf.Log10((float)Count)));
				_labelWidth = digitCount * 8 + 8;

				if (s_RightAlignedLabelStyle == null) {
					s_RightAlignedLabelStyle = new GUIStyle(GUI.skin.label);
					s_RightAlignedLabelStyle.alignment = TextAnchor.UpperRight;
					s_RightAlignedLabelStyle.padding.right = 4;
				}
			}

			#region IReorderableListData - Implementation

			/// <inheritdoc/>
			public int Count {
				get { return arrayProperty.arraySize; }
			}

			/// <inheritdoc/>
			public void AddNew() {
				Insert(arrayProperty.arraySize);
			}
			/// <inheritdoc/>
			public void Insert(int index) {
				arrayProperty.InsertArrayElementAtIndex(index);
				ResetValue(arrayProperty.GetArrayElementAtIndex(index));
			}
			/// <inheritdoc/>
			public void Duplicate(int index) {
				arrayProperty.InsertArrayElementAtIndex(index);
			}
			/// <inheritdoc/>
			public void Remove(int index) {
				arrayProperty.DeleteArrayElementAtIndex(index);
			}
			/// <inheritdoc/>
			public void Move(int sourceIndex, int destIndex) {
				if (destIndex > sourceIndex)
					--destIndex;
				arrayProperty.MoveArrayElement(sourceIndex, destIndex);
			}
			/// <inheritdoc/>
			public void Clear() {
				arrayProperty.ClearArray();
			}

			/// <inheritdoc/>
			public void DrawItem(Rect position, int index) {
				if (Event.current.type == EventType.repaint)
					s_RightAlignedLabelStyle.Draw(new Rect(position.x, position.y, _labelWidth, position.height), index + ":", false, false, false, false);

				position.x += _labelWidth;
				position.width -= _labelWidth;

				EditorGUI.PropertyField(position, arrayProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
			}

			#endregion

			#region Methods

			/// <summary>
			/// Reset value of array element.
			/// </summary>
			/// <param name="element">Serializd property for array element.</param>
			private void ResetValue(SerializedProperty element) {
				switch (element.type) {
					case "string":
						element.stringValue = "";
						break;
					case "Vector2f":
						element.vector2Value = Vector2.zero;
						break;
					case "Vector3f":
						element.vector3Value = Vector3.zero;
						break;
					case "Rectf":
						element.rectValue = new Rect();
						break;
					case "Quaternionf":
						element.quaternionValue = Quaternion.identity;
						break;
					case "int":
						element.intValue = 0;
						break;
					case "float":
						element.floatValue = 0f;
						break;
					case "UInt8":
						element.boolValue = false;
						break;
					case "ColorRGBA":
						element.colorValue = Color.black;
						break;

					default:
						if (element.type.StartsWith("PPtr"))
							element.objectReferenceValue = null;
						break;
				}
			}

			#endregion

		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="ReorderableListControl"/>.
		/// </summary>
		public SerializablePropertyListControl() : base() {
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ReorderableListControl"/>.
		/// </summary>
		/// <param name="flags">Optional flags which affect behavior of control.</param>
		public SerializablePropertyListControl(ReorderableListFlags flags) : base(flags) {
		}

		#endregion

		#region Methods

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <c>null</c> was specified for serialized property.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown is specified serialized property does not represent an array.
		/// </exception>
		public void Draw(SerializedProperty arrayProperty, DrawEmpty drawEmpty) {
			if (arrayProperty == null)
				throw new ArgumentNullException("Array property was null.");
			if (!arrayProperty.isArray)
				throw new InvalidOperationException("Specified serialized propery is not an array.");

			float itemHeight = (arrayProperty.arraySize > 0)
				? EditorGUI.GetPropertyHeight(arrayProperty.GetArrayElementAtIndex(0), GUIContent.none, true)
				: 0f;

			DoListField(new SerializedPropertyListData(arrayProperty), drawEmpty, itemHeight);
		}

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		public void Draw(SerializedProperty arrayProperty) {
			Draw(arrayProperty, null);
		}

		#endregion

	}

}