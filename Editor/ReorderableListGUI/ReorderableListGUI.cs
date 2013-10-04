// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

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
	public static ReorderableListControl defaultListControl { get; private set; }

	static ReorderableListGUI() {
		defaultListControl = new ReorderableListControl();

		indexOfChangedItem = -1;

		InitStyles();
	}

	#region Custom Styles

	/// <summary>
	/// Gets style for title header.
	/// </summary>
	public static GUIStyle titleStyle { get; private set; }

	/// <summary>
	/// Gets style for background of list control.
	/// </summary>
	public static GUIStyle containerStyle { get; private set; }
	/// <summary>
	/// Gets style for add item button.
	/// </summary>
	public static GUIStyle addButtonStyle { get; private set; }
	/// <summary>
	/// Gets style for remove item button.
	/// </summary>
	public static GUIStyle removeButtonStyle { get; private set; }

	private static void InitStyles() {
		titleStyle = new GUIStyle();
		titleStyle.border = new RectOffset(2, 2, 2, 1);
		titleStyle.margin = new RectOffset(5, 5, 5, 0);
		titleStyle.padding = new RectOffset(5, 5, 0, 0);
		titleStyle.alignment = TextAnchor.MiddleLeft;
		titleStyle.normal.background = ReorderableListResources.texTitleBackground;
		titleStyle.normal.textColor = EditorGUIUtility.isProSkin
			? new Color(0.8f, 0.8f, 0.8f)
			: new Color(0.2f, 0.2f, 0.2f);

		containerStyle = new GUIStyle();
		containerStyle.border = new RectOffset(2, 2, 1, 2);
		containerStyle.margin = new RectOffset(5, 5, 5, 5);
		containerStyle.padding = new RectOffset(1, 1, 1, 2);
		containerStyle.normal.background = ReorderableListResources.texContainerBackground;

		addButtonStyle = new GUIStyle();
		addButtonStyle.fixedWidth = 30;
		addButtonStyle.fixedHeight = 16;
		addButtonStyle.normal.background = ReorderableListResources.texAddButton;
		addButtonStyle.active.background = ReorderableListResources.texAddButtonActive;

		removeButtonStyle = new GUIStyle();
		removeButtonStyle.fixedWidth = 27;
		removeButtonStyle.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
		removeButtonStyle.imagePosition = ImagePosition.ImageOnly;
		removeButtonStyle.alignment = TextAnchor.MiddleCenter;

	}

	#endregion

	private static GUIContent s_Temp = new GUIContent();

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
	/// <param name="caption">Caption for list control.</para>
	public static void Title(GUIContent caption) {
		Rect position = GUILayoutUtility.GetRect(caption, titleStyle);
		if (Event.current.type == EventType.Repaint) {
			position.height += 6;
			titleStyle.Draw(position, caption, false, false, false, false);
		}
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
	/// <param name="caption">Caption for list control.</para>
	public static void Title(string caption) {
		s_Temp.text = caption;
		Title(s_Temp);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
	/// <param name="itemHeight">Height of a single list item.</param>
	/// <param name="flags">Optional flags to pass into list field.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty, float itemHeight, ReorderableListFlags flags) {
		defaultListControl.flags = flags;
		defaultListControl.Draw(list, drawItem, drawEmpty, itemHeight);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
	/// <param name="flags">Optional flags to pass into list field.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags) {
		defaultListControl.flags = flags;
		defaultListControl.Draw(list, drawItem, drawEmpty, DefaultItemHeight);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
	/// <param name="itemHeight">Height of a single list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty, float itemHeight) {
		defaultListControl.flags = 0;
		defaultListControl.Draw(list, drawItem, drawEmpty, itemHeight);
	}
	
	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="itemHeight">Height of a single list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem, float itemHeight) {
		defaultListControl.flags = 0;
		defaultListControl.Draw(list, drawItem, null, itemHeight);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem) {
		defaultListControl.flags = 0;
		defaultListControl.Draw(list, drawItem, null, DefaultItemHeight);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, ReorderableListControl.DrawItem<T> drawItem, ReorderableListControl.DrawEmpty drawEmpty) {
		defaultListControl.flags = 0;
		defaultListControl.Draw(list, drawItem, drawEmpty, DefaultItemHeight);
	}

}