// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

/// <summary>
/// Additional flags which can be passed into reorderable list field.
/// </summary>
[Flags]
public enum ReorderableListFlag {
	/// <summary>
	/// Hide grab handles and disable reordering of list items.
	/// </summary>
	DisableReordering = 1,
	/// <summary>
	/// Hide add button at base of control.
	/// </summary>
	HideAddButton = 2,
	/// <summary>
	/// Hide remove buttons from list items.
	/// </summary>
	HideRemoveButtons = 4,
}

/// <summary>
/// Utility class for drawing dynamically orderable list.
/// </summary>
public static class ReorderableListGUI {

	/// <summary>
	/// Invoked to draw list item.
	/// </summary>
	/// <remarks>
	/// <para>GUI controls must be positioned absolutely within the given rectangle since
	/// list items must be sized consistently.</para>
	/// </remarks>
	/// <example>
	/// <para>The following listing presents a text field for each list item:</para>
	/// <code language="csharp"><![CDATA[
	/// using UnityEngine;
	/// using UnityEditor;
	/// 
	/// using System.Collections.Generic;
	/// 
	/// public class ExampleWindow : EditorWindow {
	///     private List<string> _list;
	/// 
	///     private void OnEnable() {
	///         _list = new List<string>();
	///     }
	///     private void OnGUI() {
	///         ReorderableListGUI.ListField(_list, DrawListItem);
	///     }
	/// 
	///     private string DrawListItem(Rect position, string value) {
	///         // Text fields do not like `null` values!
	///         if (value == null)
	///             value = "";
	///         return EditorGUI.TextField(position, value);
	///     }
	/// }
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// import System.Collections.Generic;
	/// 
	/// class ExampleWindow extends EditorWindow {
	///     private var _list:List.<String>;
	/// 
	///     function OnEnable() {
	///         _list = new List.<String>();
	///     }
	///     function OnGUI() {
	///         ReorderableListGUI.ListField(_list, DrawListItem);
	///     }
	/// 
	///     function DrawListItem(position:Rect, value:String):String {
	///         // Text fields do not like `null` values!
	///         if (value == null)
	///             value = '';
	///         return EditorGUI.TextField(position, value);
	///     }
	/// }
	/// ]]></code>
	/// </example>
	/// <typeparam name="T">Type of item list.</typeparam>
	/// <param name="position">Position of list item.</param>
	/// <param name="item">The list item.</param>
	/// <returns>
	/// The modified value.
	/// </returns>
	public delegate T DrawItem<T>(Rect position, T item);

	/// <summary>
	/// Invoked to draw content for empty list.
	/// </summary>
	/// <remarks>
	/// <para>Callback should make use of <c>GUILayout</c> for to present controls.</para>
	/// </remarks>
	/// <example>
	/// <para>The following listing displays a label for empty list control:</para>
	/// <code language="csharp"><![CDATA[
	/// using UnityEngine;
	/// using UnityEditor;
	/// 
	/// using System.Collections.Generic;
	/// 
	/// public class ExampleWindow : EditorWindow {
	///     private List<string> _list;
	/// 
	///     private void OnEnable() {
	///         _list = new List<string>();
	///     }
	///     private void OnGUI() {
	///         ReorderableListGUI.ListField(_list, ReorderableListGUI.DrawTextField, DrawEmptyMessage);
	///     }
	/// 
	///     private string DrawEmptyMessage() {
	///         GUILayout.Label("List is empty!", EditorStyles.miniLabel);
	///     }
	/// }
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// import System.Collections.Generic;
	/// 
	/// class ExampleWindow extends EditorWindow {
	///     private var _list:List.<String>;
	/// 
	///     function OnEnable() {
	///         _list = new List.<String>();
	///     }
	///     function OnGUI() {
	///         ReorderableListGUI.ListField(_list, ReorderableListGUI.DrawTextField, DrawEmptyMessage);
	///     }
	/// 
	///     function DrawEmptyMessage() {
	///         GUILayout.Label('List is empty!', EditorStyles.miniLabel);
	///     }
	/// }
	/// ]]></code>
	/// </example>
	public delegate void DrawEmpty();

	/// <summary>
	/// Default list item height.
	/// </summary>
	public const float DefaultItemHeight = 18;

	/// <summary>
	/// Background color of anchor list item.
	/// </summary>
	public static readonly Color AnchorBackgroundColor;

	static ReorderableListGUI() {
		InitStyles();

		AnchorBackgroundColor = EditorGUIUtility.isProSkin
			? new Color(0, 0, 0, 0.5f)
			: new Color(0, 0, 0, 0.3f);
	}

	#region Custom Styles

	public static GUIStyle TitleStyle { get; private set; }
	public static GUIStyle ContainerStyle { get; private set; }
	public static GUIStyle AddButtonStyle { get; private set; }
	public static GUIStyle RemoveButtonStyle { get; private set; }

	private static GUIContent RemoveButtonNormalContent { get; set; }
	private static GUIContent RemoveButtonActiveContent { get; set; }

	private static void InitStyles() {
		TitleStyle = new GUIStyle();
		TitleStyle.border = new RectOffset(2, 2, 2, 1);
		TitleStyle.margin = new RectOffset(5, 5, 5, 0);
		TitleStyle.padding = new RectOffset(5, 5, 0, 0);
		TitleStyle.alignment = TextAnchor.MiddleLeft;
		TitleStyle.normal.background = ReorderableListResources.texTitleBackground;
		TitleStyle.normal.textColor = EditorGUIUtility.isProSkin
			? new Color(0.8f, 0.8f, 0.8f)
			: new Color(0.2f, 0.2f, 0.2f);

		ContainerStyle = new GUIStyle();
		ContainerStyle.border = new RectOffset(2, 2, 1, 2);
		ContainerStyle.margin = new RectOffset(5, 5, 5, 5);
		ContainerStyle.padding = new RectOffset(1, 1, 1, 2);
		ContainerStyle.normal.background = ReorderableListResources.texContainerBackground;

		AddButtonStyle = new GUIStyle();
		AddButtonStyle.fixedWidth = 30;
		AddButtonStyle.fixedHeight = 16;
		AddButtonStyle.normal.background = ReorderableListResources.texAddButton;
		AddButtonStyle.active.background = ReorderableListResources.texAddButtonActive;

		RemoveButtonStyle = new GUIStyle();
		RemoveButtonStyle.fixedWidth = 27;
		RemoveButtonStyle.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
		RemoveButtonStyle.imagePosition = ImagePosition.ImageOnly;
		RemoveButtonStyle.alignment = TextAnchor.MiddleCenter;

		RemoveButtonNormalContent = new GUIContent(ReorderableListResources.texRemoveButton);
		RemoveButtonActiveContent = new GUIContent(ReorderableListResources.texRemoveButtonActive);
	}

	#endregion

	#region Basic List Items

	private static T DrawDefaultField<T>(Rect position, T item) {
		GUI.Label(position, "Item drawer not implemented.");
		return item;
	}

	public static string DrawTextField(Rect position, string item) {
		if (item == null)
			item = "";
		return EditorGUI.TextField(position, item);
	}

	#endregion

	private static GUIContent _temp = new GUIContent();

	/// <summary>
	/// Position of rectangle which is shown to higlight target position when dragging.
	/// </summary>
	private static Rect dragHighlighter;

	/// <summary>
	/// Mouse button which was used when beginning to drag item.
	/// </summary>
	private static int _anchorMouseButton;
	/// <summary>
	/// Zero-based index of anchored list item.
	/// </summary>
	private static int _anchorIndex = -1;
	/// <summary>
	/// Zero-based index of target list item for reordering.
	/// </summary>
	private static int _targetIndex = -1;

	/// <summary>
	/// Unique ID of list control which should be automatically focused. A value
	/// of zero indicates that no control is to be focused.
	/// </summary>
	private static int _autoFocusControlID = 0;
	/// <summary>
	/// Zero-based index of item which should be focused.
	/// </summary>
	private static int _autoFocusIndex;

	/// <summary>
	/// Draw add item button.
	/// </summary>
	/// <param name="position">Position of button.</param>
	/// <param name="controlID">Unique ID of list control.</param>
	/// <param name="list">The list which can be reordered.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	private static void DoAddButton<T>(Rect position, int controlID, List<T> list) {
		if (GUI.Button(position, GUIContent.none, AddButtonStyle)) {
			// Append item to list.
			GUIUtility.keyboardControl = 0;
			list.Add(default(T));
			GUI.changed = true;

			// Attempt to automatically focus list control.
			_autoFocusControlID = controlID;
			_autoFocusIndex = list.Count - 1;
		}
	}

	/// <summary>
	/// Draw remove button.
	/// </summary>
	/// <param name="position">Position of button.</param>
	/// <param name="forceActiveContent">Indicates if active content should be forced.</param>
	/// <returns>
	/// A value of <c>true</c> if clicked; otherwise <c>false</c>.
	/// </returns>
	private static bool DoRemoveButton(Rect position, bool forceActiveContent) {
		int controlID = GUIUtility.GetControlID(FocusType.Passive);

		switch (Event.current.GetTypeForControl(controlID)) {
			case EventType.MouseDown:
				if (GUI.enabled && position.Contains(Event.current.mousePosition)) {
					GUIUtility.hotControl = controlID;
					GUIUtility.keyboardControl = 0;
					Event.current.Use();
				}
				break;

			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
					Event.current.Use();
				break;

			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID) {
					GUIUtility.hotControl = 0;

					if (position.Contains(Event.current.mousePosition)) {
						Event.current.Use();
						return true;
					}
					else {
						Event.current.Use();
						return false;
					}
				}
				break;

			case EventType.Repaint:
				var content = (GUIUtility.hotControl == controlID || forceActiveContent)
					? RemoveButtonActiveContent
					: RemoveButtonNormalContent;
				RemoveButtonStyle.Draw(position, content, controlID);
				break;
		}

		return false;
	}

	/// <summary>
	/// Begin tracking drag and drop within list.
	/// </summary>
	/// <param name="controlID">Unique ID of list control.</param>
	/// <param name="itemIndex">Zero-based index of item which is going to be dragged.</param>
	private static void BeginTrackingReorderDrag(int controlID, int itemIndex) {
		GUIUtility.hotControl = controlID;
		GUIUtility.keyboardControl = 0;
		_anchorMouseButton = Event.current.button;
		_anchorIndex = itemIndex;
		_targetIndex = itemIndex;
	}

	/// <summary>
	/// Stop tracking drag and drop.
	/// </summary>
	private static void StopTrackingReorderDrag() {
		GUIUtility.hotControl = 0;
		_anchorIndex = -1;
		_targetIndex = -1;
	}

	/// <summary>
	/// Gets a value indicating whether item in current list is currently being tracked.
	/// </summary>
	/// <param name="controlID">Unique ID of list control.</param>
	/// <returns>
	/// A value of <c>true</c> if item is being tracked; otherwise <c>false</c>.
	/// </returns>
	private static bool IsTrackingControl(int controlID) {
		return GUIUtility.hotControl == controlID;
	}

	/// <summary>
	/// Accept reordering.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	private static void AcceptReorderDrag<T>(List<T> list) {
		try {
			// Reorder list as needed!
			_targetIndex = Mathf.Clamp(_targetIndex, 0, list.Count + 1);
			if (_targetIndex != _anchorIndex && _targetIndex != _anchorIndex + 1) {
				if (_targetIndex > _anchorIndex)
					--_targetIndex;

				T temp = list[_anchorIndex];
				list.RemoveAt(_anchorIndex);
				list.Insert(_targetIndex, temp);

				GUI.changed = true;
			}
		}
		finally {
			StopTrackingReorderDrag();
		}
	}

	private static Rect DoListField<T>(int controlID, List<T> list, DrawItem<T> drawItem, float itemHeight, ReorderableListFlag flags) {
		bool allowReordering = (flags & ReorderableListFlag.DisableReordering) == 0;
		bool includeRemoveButtons = (flags & ReorderableListFlag.HideRemoveButtons) == 0;

		RectOffset containerMargin = ContainerStyle.margin;
		bool trackingControl = IsTrackingControl(controlID);

		float itemOffset = itemHeight + 3;
		float halfItemOffset = Mathf.Ceil(itemOffset / 2f);

		float totalHeight = itemOffset * list.Count + 6 + containerMargin.top + containerMargin.bottom;
		Rect containerRect = GUILayoutUtility.GetRect(0, totalHeight);

		containerRect.x += containerMargin.left;
		containerRect.y += containerMargin.top;
		containerRect.width -= containerMargin.left + containerMargin.right;
		containerRect.height -= containerMargin.top + containerMargin.bottom;

		// Position of first item in list.
		float firstItemY = containerRect.y + 4;
		
		// Get local copy of event information for efficiency.
		EventType eventType = Event.current.GetTypeForControl(controlID);
		Vector2 mousePosition = Event.current.mousePosition;

		// We must put this back!
		Color restoreColor = GUI.color;
		
		switch (eventType) {
			case EventType.MouseDown:
			case EventType.MouseDrag:
				if (trackingControl) {
					// Cancel drag when other mouse button is pressed.
					if (Event.current.button != _anchorMouseButton)
						StopTrackingReorderDrag();

					// Reset target index and adjust when looping through list items.
					if (mousePosition.y < firstItemY)
						_targetIndex = 0;
					else
						_targetIndex = list.Count;

					// Force repaint to occur so that dragging rectangle is visible.
					if (trackingControl)
						Event.current.Use();
				}
				break;

			case EventType.MouseUp:
				if (trackingControl) {
					// Allow user code to change control over reordering during drag.
					if (allowReordering)
						AcceptReorderDrag(list);
					else
						StopTrackingReorderDrag();
					Event.current.Use();
				}
				break;

			case EventType.KeyDown:
				if (trackingControl && Event.current.keyCode == KeyCode.Escape) {
					StopTrackingReorderDrag();
					Event.current.Use();
				}
				break;

			case EventType.Repaint:
				// Draw caption area of list.
				ContainerStyle.Draw(containerRect, GUIContent.none, false, false, false, false);
				break;
		}
		
		// Draw list items!
		Rect itemPosition = new Rect(containerRect.x + 2, firstItemY - 1, containerRect.width - 4, itemOffset);
		Rect itemContentPosition = new Rect(itemPosition.x + 2, itemPosition.y + 1, itemPosition.width - 2, itemHeight);
		Rect handlePosition = new Rect(itemPosition.x + 6, itemPosition.y + 1 + halfItemOffset - 4, 9, 5);
		Rect handleResponsePosition = new Rect(itemPosition.x, itemPosition.y + 1, 20, itemOffset);

		// Make space for grab handle?
		if (allowReordering) {
			itemContentPosition.x += 20;
			itemContentPosition.width -= 20;
		}

		// Make space for remove buttons?
		Rect removeButtonPosition = default(Rect);
		if (includeRemoveButtons) {
			itemContentPosition.width -= RemoveButtonStyle.fixedWidth;
			removeButtonPosition = new Rect(itemContentPosition.xMax, itemContentPosition.y, RemoveButtonStyle.fixedWidth, itemHeight);
		}
		
		for (int i = 0; i < list.Count; ++i) {
			EditorGUIUtility.AddCursorRect(handleResponsePosition, MouseCursor.MoveArrow);

			// Draw grab handle.
			switch (eventType) {
				case EventType.Repaint:
					// Highlight background of anchor item.
					if (trackingControl && i == _anchorIndex) {
						GUI.color = AnchorBackgroundColor;
						GUI.DrawTexture(new Rect(itemPosition.x - 1, itemPosition.y - 1, itemPosition.width + 2, itemPosition.height + 1), EditorGUIUtility.whiteTexture);
						GUI.color = restoreColor;
					}

					if (allowReordering)
						GUI.DrawTexture(handlePosition, ReorderableListResources.texGrabHandle);

					if (i != 0 && (!trackingControl || (i != _anchorIndex && i != _anchorIndex + 1)))
						GUI.DrawTexture(new Rect(itemPosition.x, itemPosition.y - 1, itemPosition.width, 1), ReorderableListResources.texItemSplitter);
					break;

				case EventType.MouseDown:
					if (allowReordering && GUI.enabled && handleResponsePosition.Contains(mousePosition)) {
						BeginTrackingReorderDrag(controlID, i);

						// Is target index below anchor?
						if (mousePosition.y > itemPosition.yMax - halfItemOffset)
							_targetIndex = i + 1;

						Event.current.Use();
					}
					break;

				case EventType.MouseDrag:
					if (trackingControl) {
						if (mousePosition.y >= itemPosition.y - halfItemOffset && mousePosition.y <= itemPosition.yMax - halfItemOffset)
							_targetIndex = i;
					}
					break;
			}

			if (_autoFocusControlID == controlID && _autoFocusIndex == i)
				GUI.SetNextControlName("AutoFocus_" + controlID);

			// Draw the actual list item!
			list[i] = drawItem(itemContentPosition, list[i]);

			if (includeRemoveButtons)
				if (DoRemoveButton(removeButtonPosition, trackingControl && _anchorIndex == i)) {
					// Remove last entry in list.
					if (list.Count > 0) {
						list.RemoveAt(i);
						GUI.changed = true;
					}
				}

			// Offset position rectangles for next item.
			itemPosition.y += itemOffset;
			itemContentPosition.y += itemOffset;
			handlePosition.y += itemOffset;
			handleResponsePosition.y += itemOffset;
			removeButtonPosition.y += itemOffset;
		}

		// Automatically focus control!
		if (_autoFocusControlID == controlID) {
			_autoFocusControlID = 0;
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2)
			GUI.FocusControl("AutoFocus_" + controlID);
#else
			EditorGUI.FocusTextInControl("AutoFocus_" + controlID);
#endif
		}

		// Fake control to catch input focus if auto focus was not possible.
		GUIUtility.GetControlID(FocusType.Keyboard);

		// Update position of drag rectangle.
		switch (eventType) {
			case EventType.Repaint:
				// Highlight drag rectangle?
				// Note: Draw on top of other controls!
				if (IsTrackingControl(controlID)) {
					GUI.color = EditorGUIUtility.isProSkin
						? new Color(0.75f, 0.75f, 0.75f)
						: new Color(0.12f, 0.12f, 0.12f);
					GUI.DrawTexture(dragHighlighter, EditorGUIUtility.whiteTexture);
					GUI.color = restoreColor;
				}
				break;

			case EventType.MouseDown:
			case EventType.MouseDrag:
				if (IsTrackingControl(controlID)) {
					dragHighlighter = containerRect;
					dragHighlighter.y = firstItemY + _targetIndex * itemOffset - 3;
					dragHighlighter.height = 4;
				}
				break;
		}

		return containerRect;
	}

	private static Rect DoEmptyList<T>(List<T> list, DrawEmpty drawEmpty, ReorderableListFlag flags) {
		Rect r = EditorGUILayout.BeginVertical(ContainerStyle);
		{
			if (drawEmpty != null)
				drawEmpty();
			else
				GUILayout.Space(5);
		}
		EditorGUILayout.EndVertical();
		return r;
	}

	private static void DoListField<T>(List<T> list, DrawItem<T> drawItem, DrawEmpty drawEmpty, float itemHeight, ReorderableListFlag flags) {
		int controlID = GUIUtility.GetControlID(FocusType.Passive);

		Rect containerPosition;
		if (list.Count > 0)
			containerPosition = DoListField(controlID, list, drawItem ?? DrawDefaultField, itemHeight, flags);
		else
			containerPosition = DoEmptyList(list, drawEmpty, flags);

		if ((flags & ReorderableListFlag.HideAddButton) == 0) {
			Rect addButtonRect = GUILayoutUtility.GetRect(0, AddButtonStyle.fixedHeight);
			addButtonRect.width = AddButtonStyle.fixedWidth;
			addButtonRect.x = containerPosition.xMax - addButtonRect.width;
			addButtonRect.y -= ContainerStyle.margin.bottom + 1;

			DoAddButton(addButtonRect, controlID, list);
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
	/// ReorderableListGUI.Title(titleContent);
	/// ReorderableListGUI.ListField(list, DynamicListGU.DrawTextField);
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// ReorderableListGUI.Title(titleContent);
	/// ReorderableListGUI.ListField(list, DynamicListGU.DrawTextField);
	/// ]]></code>
	/// </example>
	/// <param name="caption">Caption for list control.</para>
	public static void Title(GUIContent caption) {
		Rect position = GUILayoutUtility.GetRect(caption, TitleStyle);
		if (Event.current.type == EventType.Repaint) {
			position.height += 6;
			TitleStyle.Draw(position, caption, false, false, false, false);
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
	/// ReorderableListGUI.ListField(list, DynamicListGU.DrawTextField);
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// ReorderableListGUI.Title('Your Title');
	/// ReorderableListGUI.ListField(list, DynamicListGU.DrawTextField);
	/// ]]></code>
	/// </example>
	/// <param name="caption">Caption for list control.</para>
	public static void Title(string caption) {
		_temp.text = caption;
		Title(_temp);
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
	public static void ListField<T>(List<T> list, DrawItem<T> drawItem, DrawEmpty drawEmpty, float itemHeight, ReorderableListFlag flags) {
		DoListField(list, drawItem, drawEmpty, itemHeight, flags);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
	/// <param name="itemHeight">Height of a single list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, DrawItem<T> drawItem, DrawEmpty drawEmpty, float itemHeight) {
		DoListField(list, drawItem, drawEmpty, itemHeight, 0);
	}
	
	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="itemHeight">Height of a single list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, DrawItem<T> drawItem, float itemHeight) {
		DoListField(list, drawItem, null, itemHeight, 0);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, DrawItem<T> drawItem) {
		DoListField(list, drawItem, null, DefaultItemHeight, 0);
	}

	/// <summary>
	/// Draw list field control.
	/// </summary>
	/// <param name="list">The list which can be reordered.</param>
	/// <param name="drawItem">Callback to draw list item.</param>
	/// <param name="drawEmpty">Callback to draw custom content for empty list.</param>
	/// <typeparam name="T">Type of list item.</typeparam>
	public static void ListField<T>(List<T> list, DrawItem<T> drawItem, DrawEmpty drawEmpty) {
		DoListField(list, drawItem, drawEmpty, DefaultItemHeight, 0);
	}

}