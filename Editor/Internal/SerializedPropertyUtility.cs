// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Utility functionality for <see cref="SerializedPropertyAdaptor"/> implementations.
	/// </summary>
	public static class SerializedPropertyUtility {

		/// <summary>
		/// Reset the value of a property.
		/// </summary>
		/// <param name="property">Serialized property for a serialized property.</param>
		public static void ResetValue(SerializedProperty property) {
			if (property == null)
				throw new ArgumentNullException("property");

			switch (property.propertyType) {
				case SerializedPropertyType.Integer:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Boolean:
					property.boolValue = false;
					break;
				case SerializedPropertyType.Float:
					property.floatValue = 0f;
					break;
				case SerializedPropertyType.String:
					property.stringValue = "";
					break;
				case SerializedPropertyType.Color:
					property.colorValue = Color.black;
					break;
				case SerializedPropertyType.ObjectReference:
					property.objectReferenceValue = null;
					break;
				case SerializedPropertyType.LayerMask:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Enum:
					property.enumValueIndex = 0;
					break;
				case SerializedPropertyType.Vector2:
					property.vector2Value = default(Vector2);
					break;
				case SerializedPropertyType.Vector3:
					property.vector3Value = default(Vector3);
					break;
				case SerializedPropertyType.Vector4:
					property.vector4Value = default(Vector4);
					break;
				case SerializedPropertyType.Rect:
					property.rectValue = default(Rect);
					break;
				case SerializedPropertyType.ArraySize:
					property.intValue = 0;
					break;
				case SerializedPropertyType.Character:
					property.intValue = 0;
					break;
				case SerializedPropertyType.AnimationCurve:
					property.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					break;
				case SerializedPropertyType.Bounds:
					property.boundsValue = default(Bounds);
					break;
				case SerializedPropertyType.Gradient:
					//!TODO: Amend when Unity add a public API for setting the gradient.
					break;
			}

			ResetChildPropertyValues(property);
		}

		private static void ResetChildPropertyValues(SerializedProperty element) {
			if (!element.hasChildren)
				return;

			var childProperty = element.Copy();
			int elementPropertyDepth = element.depth;
			bool enterChildren = true;

			while (childProperty.Next(enterChildren) && childProperty.depth > elementPropertyDepth) {
				enterChildren = false;
				ResetValue(childProperty);
			}
		}

	}

}
