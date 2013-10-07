// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Rotorz.ReorderableList.Internal;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Utility class for drawing reorderable lists.
	/// </summary>
	public static class ReorderableListGUI {

		/// <summary>
		/// Default list item height.
		/// </summary>
		public const float DefaultItemHeight = 18;

		/// <summary>
		/// Gets or sets zero-based index of last item which was changed. A value of -1
		/// indicates that no item was changed by list.
		/// </summary>
		/// <remarks>
		/// <para>This property should not be set when items are added or removed.</para>
		/// </remarks>
		public static int indexOfChangedItem { get; internal set; }

		/// <summary>
		/// Gets zero-based index of list item which is currently being drawn;
		/// or a value of -1 if no item is currently being drawn.
		/// </summary>
		public static int currentItemIndex {
			get { return ReorderableListControl.currentItemIndex; }
		}

		#region Basic Item Drawers

		/// <summary>
		/// Default list item drawer implementation.
		/// </summary>
		/// <remarks>
		/// <para>Always presents the label "Item drawer not implemented.".</para>
		/// </remarks>
		/// <param name="position">Position to draw list item control(s).</param>
		/// <param name="item">Value of list item.</param>
		/// <returns>
		/// Unmodified value of list item.
		/// </returns>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static T DefaultItemDrawer<T>(Rect position, T item) {
			GUI.Label(position, "Item drawer not implemented.");
			return item;
		}

		/// <summary>
		/// Draws text field allowing list items to be edited.
		/// </summary>
		/// <remarks>
		/// <para>Null values are automatically changed to empty strings since null
		/// values cannot be edited using a text field.</para>
		/// <para>Value of <c>GUI.changed</c> is set to <c>true</c> if value of item
		/// is modified.</para>
		/// </remarks>
		/// <param name="position">Position to draw list item control(s).</param>
		/// <param name="item">Value of list item.</param>
		/// <returns>
		/// Modified value of list item.
		/// </returns>
		public static string TextFieldItemDrawer(Rect position, string item) {
			if (item == null) {
				item = "";
				GUI.changed = true;
			}
			return EditorGUI.TextField(position, item);
		}

		#endregion

		/// <summary>
		/// Gets the default list control implementation.
		/// </summary>
		private static ReorderableListControl defaultListControl { get; set; }

		static ReorderableListGUI() {
			defaultListControl = new ReorderableListControl();

			indexOfChangedItem = -1;

			InitStyles();
		}

		#region Custom Styles

		/// <summary>
		/// Gets default style for title header.
		/// </summary>
		public static GUIStyle defaultTitleStyle { get; private set; }

		/// <summary>
		/// Gets default style for background of list control.
		/// </summary>
		public static GUIStyle defaultContainerStyle { get; private set; }
		/// <summary>
		/// Gets default style for add item button.
		/// </summary>
		public static GUIStyle defaultAddButtonStyle { get; private set; }
		/// <summary>
		/// Gets default style for remove item button.
		/// </summary>
		public static GUIStyle defaultRemoveButtonStyle { get; private set; }

		private static void InitStyles() {
			defaultTitleStyle = new GUIStyle();
			defaultTitleStyle.border = new RectOffset(2, 2, 2, 1);
			defaultTitleStyle.margin = new RectOffset(5, 5, 5, 0);
			defaultTitleStyle.padding = new RectOffset(5, 5, 0, 0);
			defaultTitleStyle.alignment = TextAnchor.MiddleLeft;
			defaultTitleStyle.normal.background = ReorderableListResources.texTitleBackground;
			defaultTitleStyle.normal.textColor = EditorGUIUtility.isProSkin
				? new Color(0.8f, 0.8f, 0.8f)
				: new Color(0.2f, 0.2f, 0.2f);

			defaultContainerStyle = new GUIStyle();
			defaultContainerStyle.border = new RectOffset(2, 2, 1, 2);
			defaultContainerStyle.margin = new RectOffset(5, 5, 5, 5);
			defaultContainerStyle.padding = new RectOffset(1, 1, 2, 2);
			defaultContainerStyle.normal.background = ReorderableListResources.texContainerBackground;

			defaultAddButtonStyle = new GUIStyle();
			defaultAddButtonStyle.fixedWidth = 30;
			defaultAddButtonStyle.fixedHeight = 16;
			defaultAddButtonStyle.normal.background = ReorderableListResources.texAddButton;
			defaultAddButtonStyle.active.background = ReorderableListResources.texAddButtonActive;

			defaultRemoveButtonStyle = new GUIStyle();
			defaultRemoveButtonStyle.fixedWidth = 27;
			defaultRemoveButtonStyle.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
			defaultRemoveButtonStyle.imagePosition = ImagePosition.ImageOnly;
			defaultRemoveButtonStyle.alignment = TextAnchor.MiddleCenter;

		}

		#endregion

		private static GUIContent s_Temp = new GUIContent();

		#region Title Control

		/// <summary>
		/// Draw title control for list field.
		/// </summary>
		/// <remarks>
		/// <para>When needed, should be shown immediately before list field.</para>
		/// </remarks>
		/// <example>
		/// <code language="csharp"><![CDATA[
		/// ReorderableListGUI.Title(titleContent);
		/// ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer);
		/// ]]></code>
		/// <code language="unityscript"><![CDATA[
		/// ReorderableListGUI.Title(titleContent);
		/// ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer);
		/// ]]></code>
		/// </example>
		/// <param name="caption">Caption for list control.</param>
		public static void Title(GUIContent caption) {
			Rect position = GUILayoutUtility.GetRect(caption, defaultTitleStyle);
			position.height += 6;
			Title(position, caption);
		}

		/// <summary>
		/// Draw title control for list field.
		/// </summary>
		/// <remarks>
		/// <para>When needed, should be shown immediately before list field.</para>
		/// </remarks>
		/// <example>
		/// <code language="csharp"><![CDATA[
		/// ReorderableListGUI.Title("Your Title");
		/// ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer);
		/// ]]></code>
		/// <code language="unityscript"><![CDATA[
		/// ReorderableListGUI.Title('Your Title');
		/// ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer);
		/// ]]></code>
		/// </example>
		/// <param name="caption">Caption for list control.</param>
		public static void Title(string caption) {
			s_Temp.text = caption;
			Title(s_Temp);
		}

		/// <summary>
		/// Draw title control for list field.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="caption">Caption for list control.</param>
		public static void Title(Rect position, GUIContent caption) {
			if (Event.current.type == EventType.Repaint)
				defaultTitleStyle.Draw(position, caption, false, false, false, false);
		}

		/// <summary>
		/// Draw title control for list field.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="caption">Caption for list control.</param>
		public static void Title(Rect position, string caption) {
			s_Temp.text = caption;
			Title(position, s_Temp);
		}

		#endregion

		#region List<T> Control

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty, float itemHeight, ReorderableListFlags flags = 0) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.DrawEmptyAbsolute drawEmpty, float itemHeight, ReorderableListFlags flags = 0) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, DefaultItemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, DefaultItemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, float itemHeight) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, null, 0);
		}
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, float itemHeight) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, null, 0);
		}

		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, DefaultItemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, null, 0);
		}
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public static void ListFieldAbsolute<T>(Rect position, List<T> list, ReorderableListControl.ItemDrawer<T> drawItem) {
			var adaptor = new GenericListAdaptor<T>(list, drawItem, DefaultItemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, null, 0);
		}

		/// <summary>
		/// Calculate height of list field for absolute positioning.
		/// </summary>
		/// <param name="itemCount">Count of items in list.</param>
		/// <param name="itemHeight">Fixed height of list item.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		public static float CalculateListFieldHeight(int itemCount, float itemHeight, ReorderableListFlags flags = 0) {
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = defaultListControl.flags;
			try {
				defaultListControl.flags = flags;
				return defaultListControl.CalculateListHeight(itemCount, itemHeight);
			}
			finally {
				defaultListControl.flags = restoreFlags;
			}
		}
		/// <summary>
		/// Calculate height of list field for absolute positioning.
		/// </summary>
		/// <param name="itemCount">Count of items in list.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		public static float CalculateListFieldHeight(int itemCount, ReorderableListFlags flags = 0) {
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = defaultListControl.flags;
			try {
				defaultListControl.flags = flags;
				return defaultListControl.CalculateListHeight(itemCount, DefaultItemHeight);
			}
			finally {
				defaultListControl.flags = restoreFlags;
			}
		}

		#endregion

		#region SerializedProperty Control

		/// <summary>
		/// Draw list field control for serializable property array.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(SerializedProperty arrayProperty, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}
		/// <summary>
		/// Draw list field control for serializable property array.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		/// <summary>
		/// Draw list field control for serializable property array.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(SerializedProperty arrayProperty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty);
			ReorderableListControl.DrawControlFromState(adaptor, null, flags);
		}
		/// <summary>
		/// Draw list field control for serializable property array.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty);
			ReorderableListControl.DrawControlFromState(position, adaptor, null, flags);
		}

		/// <summary>
		/// Calculate height of list field for absolute positioning.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		public static float CalculateListFieldHeight(SerializedProperty arrayProperty, ReorderableListFlags flags = 0) {
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = defaultListControl.flags;
			try {
				defaultListControl.flags = flags;
				return defaultListControl.CalculateListHeight(new SerializedPropertyAdaptor(arrayProperty));
			}
			finally {
				defaultListControl.flags = restoreFlags;
			}
		}

		#region Fixed Item Heights

		/// <summary>
		/// Draw list field control for serializable property array with fixed item heights.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="fixedItemHeight">Use fixed height for items rather than <see cref="UnityEditor.EditorGUI.GetPropertyHeight"/>.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}
		/// <summary>
		/// Draw list field control for serializable property array with fixed item heights.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="fixedItemHeight">Use fixed height for items rather than <see cref="UnityEditor.EditorGUI.GetPropertyHeight"/>.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		/// <summary>
		/// Draw list field control for serializable property array with fixed item heights.
		/// </summary>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="fixedItemHeight">Use fixed height for items rather than <see cref="UnityEditor.EditorGUI.GetPropertyHeight"/>.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, null, flags);
		}
		/// <summary>
		/// Draw list field control for serializable property array with fixed item heights.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="fixedItemHeight">Use fixed height for items rather than <see cref="UnityEditor.EditorGUI.GetPropertyHeight"/>.</param>
		/// <param name="arrayProperty">Serializable property.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight, ReorderableListFlags flags = 0) {
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, null, flags);
		}

		#endregion

		#endregion

		#region Adaptor Control

		/// <summary>
		/// Draw list field control for adapted collection.
		/// </summary>
		/// <param name="adaptor">Reorderable list adaptor.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags = 0) {
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}
		/// <summary>
		/// Draw list field control for adapted collection.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="adaptor">Reorderable list adaptor.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags = 0) {
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		/// <summary>
		/// Draw list field control for adapted collection.
		/// </summary>
		/// <param name="adaptor">Reorderable list adaptor.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListField(IReorderableListAdaptor adaptor, ReorderableListFlags flags = 0) {
			ReorderableListControl.DrawControlFromState(adaptor, null, flags);
		}
		/// <summary>
		/// Draw list field control for adapted collection.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="adaptor">Reorderable list adaptor.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListFlags flags = 0) {
			ReorderableListControl.DrawControlFromState(position, adaptor, null, flags);
		}

		/// <summary>
		/// Calculate height of list field for adapted collection.
		/// </summary>
		/// <param name="adaptor">Reorderable list adaptor.</param>
		/// <param name="flags">Optional flags to pass into list field.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor, ReorderableListFlags flags = 0) {
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = defaultListControl.flags;
			try {
				defaultListControl.flags = flags;
				return defaultListControl.CalculateListHeight(adaptor);
			}
			finally {
				defaultListControl.flags = restoreFlags;
			}
		}

		#endregion

	}

}