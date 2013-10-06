// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

using Rotorz.ReorderableList.Internal;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Base class for custom reorderable list control.
	/// </summary>
	[Serializable]
	public abstract class ReorderableListControl {

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
		///     public List<string> wishlist = new List<string>();
		/// 
		///     private void OnGUI() {
		///         ReorderableListGUI.ListField(wishlist, DrawListItem);
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
		///     var wishlist:List.<String>;
		/// 
		///     function OnGUI() {
		///         ReorderableListGUI.ListField(wishlist, DrawListItem);
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
		public delegate T ItemDrawer<T>(Rect position, T item);

		/// <summary>
		/// Invoked to draw content for empty list.
		/// </summary>
		/// <remarks>
		/// <para>Callback should make use of <c>GUILayout</c> to present controls.</para>
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
		///         ReorderableListGUI.ListField(_list, ReorderableListGUI.TextFieldItemDrawer, DrawEmptyMessage);
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
		///         ReorderableListGUI.ListField(_list, ReorderableListGUI.TextFieldItemDrawer, DrawEmptyMessage);
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
		/// Invoked to draw content for empty list with absolute positioning.
		/// </summary>
		/// <param name="position">Position of empty content.</param>
		public delegate void DrawEmptyAbsolute(Rect position);

		#region Custom Styles

		/// <summary>
		/// Background color of anchor list item.
		/// </summary>
		public static readonly Color AnchorBackgroundColor;
		/// <summary>
		/// Background color of target slot when dragging list item.
		/// </summary>
		public static readonly Color TargetBackgroundColor;

		private static GUIContent RemoveButtonNormalContent { get; set; }
		private static GUIContent RemoveButtonActiveContent { get; set; }

		static ReorderableListControl() {
			if (EditorGUIUtility.isProSkin) {
				AnchorBackgroundColor = new Color(85f / 255f, 85f / 255f, 85f / 255f, 0.85f);
				TargetBackgroundColor = new Color(0, 0, 0, 0.5f);
			}
			else {
				AnchorBackgroundColor = new Color(225f / 255f, 225f / 255f, 225f / 255f, 0.85f);
				TargetBackgroundColor = new Color(0, 0, 0, 0.5f);
			}

			RemoveButtonNormalContent = new GUIContent(ReorderableListResources.texRemoveButton);
			RemoveButtonActiveContent = new GUIContent(ReorderableListResources.texRemoveButtonActive);
		}

		#endregion

		/// <summary>
		/// Position of mouse upon anchoring item for drag.
		/// </summary>
		private static float s_AnchorMouseOffset;
		/// <summary>
		/// Zero-based index of anchored list item.
		/// </summary>
		private static int s_AnchorIndex = -1;
		/// <summary>
		/// Zero-based index of target list item for reordering.
		/// </summary>
		private static int s_TargetIndex = -1;

		/// <summary>
		/// Unique ID of list control which should be automatically focused. A value
		/// of zero indicates that no control is to be focused.
		/// </summary>
		private static int s_AutoFocusControlID = 0;
		/// <summary>
		/// Zero-based index of item which should be focused.
		/// </summary>
		private static int s_AutoFocusIndex = -1;

		/// <summary>
		/// Zero-based index of list item which is currently being drawn.
		/// </summary>
		private static int s_CurrentItemIndex;
		/// <summary>
		/// Gets zero-based index of list item which is currently being drawn;
		/// or a value of -1 if no item is currently being drawn.
		/// </summary>
		/// <remarks>
		/// <para>Use <see cref="ReorderableListGUI.currentItemIndex"/> instead.</para>
		/// </remarks>
		internal static int currentItemIndex {
			get { return s_CurrentItemIndex; }
		}

		#region Properties

		[SerializeField]
		private ReorderableListFlags _flags;

		/// <summary>
		/// Gets or sets flags which affect behavior of control.
		/// </summary>
		public ReorderableListFlags flags {
			get { return _flags; }
			set { _flags = value; }
		}

		/// <summary>
		/// Gets a value indicating whether add button is shown.
		/// </summary>
		protected bool hasAddButton {
			get { return (_flags & ReorderableListFlags.HideAddButton) == 0; }
		}
		/// <summary>
		/// Gets a value indicating whether remove buttons are shown.
		/// </summary>
		protected bool hasRemoveButtons {
			get { return (_flags & ReorderableListFlags.HideRemoveButtons) == 0; }
		}

		[SerializeField]
		private GUIStyle _containerStyle;
		[SerializeField]
		private GUIStyle _addButtonStyle;
		[SerializeField]
		private GUIStyle _removeButtonStyle;

		/// <summary>
		/// Gets or sets style used to draw background of list control.
		/// </summary>
		/// <seealso cref="ReorderableListGUI.containerStyle"/>
		public GUIStyle containerStyle {
			get { return _containerStyle; }
			set { _containerStyle = value; }
		}
		/// <summary>
		/// Gets or sets style used to draw add button.
		/// </summary>
		/// <seealso cref="ReorderableListGUI.addButtonStyle"/>
		public GUIStyle addButtonStyle {
			get { return _addButtonStyle; }
			set { _addButtonStyle = value; }
		}
		/// <summary>
		/// Gets or sets style used to draw remove button.
		/// </summary>
		/// <seealso cref="ReorderableListGUI.removeButtonStyle"/>
		public GUIStyle removeButtonStyle {
			get { return _removeButtonStyle; }
			set { _removeButtonStyle = value; }
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="ReorderableListControl"/>.
		/// </summary>
		public ReorderableListControl() {
			_containerStyle = ReorderableListGUI.containerStyle;
			_addButtonStyle = ReorderableListGUI.addButtonStyle;
			_removeButtonStyle = ReorderableListGUI.removeButtonStyle;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ReorderableListControl"/>.
		/// </summary>
		/// <param name="flags">Optional flags which affect behavior of control.</param>
		public ReorderableListControl(ReorderableListFlags flags)
			: this() {
			this.flags = flags;
		}

		#endregion

		#region Event Handling

		// Keep track of previously known mouse position.
		private static Vector2 s_MousePosition;

		/// <summary>
		/// Indicate that first control of list item should be automatically focused
		/// if possible.
		/// </summary>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="itemIndex">Zero-based index of list item.</param>
		private void AutoFocusItem(int controlID, int itemIndex) {
			if ((flags & ReorderableListFlags.DisableAutoFocus) == 0) {
				s_AutoFocusControlID = controlID;
				s_AutoFocusIndex = itemIndex;
			}
		}

		/// <summary>
		/// Draw add item button.
		/// </summary>
		/// <param name="position">Position of button.</param>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="list">The list which can be reordered.</param>
		private void DoAddButton(Rect position, int controlID, IReorderableListData list) {
			if (GUI.Button(position, GUIContent.none, addButtonStyle)) {
				// Append item to list.
				GUIUtility.keyboardControl = 0;
				list.AddNew();

				GUI.changed = true;
				ReorderableListGUI.indexOfChangedItem = -1;

				AutoFocusItem(controlID, list.Count - 1);
			}
		}

		/// <summary>
		/// Draw remove button.
		/// </summary>
		/// <param name="position">Position of button.</param>
		/// <returns>
		/// A value of <c>true</c> if clicked; otherwise <c>false</c>.
		/// </returns>
		private bool DoRemoveButton(Rect position) {
			int controlID = GUIUtility.GetControlID(FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID)) {
				case EventType.MouseDown:
					// Do not allow button to be pressed using right mouse button since
					// context menu should be shown instead!
					if (GUI.enabled && Event.current.button != 1 && position.Contains(s_MousePosition)) {
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

						if (position.Contains(s_MousePosition)) {
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
					var content = (GUIUtility.hotControl == controlID && position.Contains(s_MousePosition))
						? RemoveButtonActiveContent
						: RemoveButtonNormalContent;
					removeButtonStyle.Draw(position, content, controlID);
					break;
			}

			return false;
		}

		private static bool s_TrackingCancelBlockContext;

		/// <summary>
		/// Begin tracking drag and drop within list.
		/// </summary>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="itemIndex">Zero-based index of item which is going to be dragged.</param>
		private static void BeginTrackingReorderDrag(int controlID, int itemIndex) {
			GUIUtility.hotControl = controlID;
			GUIUtility.keyboardControl = 0;
			s_AnchorIndex = itemIndex;
			s_TargetIndex = itemIndex;
			s_TrackingCancelBlockContext = false;
		}

		/// <summary>
		/// Stop tracking drag and drop.
		/// </summary>
		private static void StopTrackingReorderDrag() {
			GUIUtility.hotControl = 0;
			s_AnchorIndex = -1;
			s_TargetIndex = -1;
		}

		/// <summary>
		/// Gets a value indicating whether item in current list is currently being tracked.
		/// </summary>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <returns>
		/// A value of <c>true</c> if item is being tracked; otherwise <c>false</c>.
		/// </returns>
		private static bool IsTrackingControl(int controlID) {
			return !s_TrackingCancelBlockContext && GUIUtility.hotControl == controlID;
		}

		/// <summary>
		/// Accept reordering.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		private static void AcceptReorderDrag(IReorderableListData list) {
			try {
				// Reorder list as needed!
				s_TargetIndex = Mathf.Clamp(s_TargetIndex, 0, list.Count + 1);
				if (s_TargetIndex != s_AnchorIndex && s_TargetIndex != s_AnchorIndex + 1) {
					list.Move(s_AnchorIndex, s_TargetIndex);
					GUI.changed = true;
				}
			}
			finally {
				StopTrackingReorderDrag();
			}
		}

		private static Rect s_DragItemPosition;

		// Micro-optimisation to avoid repeated construction.
		private static Rect s_ItemContentPosition;
		private static Rect s_RemoveButtonPosition;
		
		private void DrawListItem(EventType eventType, Rect position, int controlID, IReorderableListData list, int itemIndex) {
			bool allowReordering = (flags & ReorderableListFlags.DisableReordering) == 0;
			bool canDragItem = GUI.enabled && allowReordering;
			bool trackingControl = IsTrackingControl(controlID);

			float halfItemOffset = position.height / 2f;

			s_ItemContentPosition = position;
			s_ItemContentPosition.x = position.x + 2;
			s_ItemContentPosition.y += 1;
			s_ItemContentPosition.width = position.width - 4;
			s_ItemContentPosition.height = position.height - 4;

			// Make space for grab handle?
			if (allowReordering) {
				s_ItemContentPosition.x += 20;
				s_ItemContentPosition.width -= 20;
			}

			// Make space for remove button?
			if (hasRemoveButtons)
				s_ItemContentPosition.width -= removeButtonStyle.fixedWidth;

			switch (eventType) {
				case EventType.Repaint:
					// Draw grab handle?
					if (allowReordering)
						GUI.DrawTexture(new Rect(position.x + 6, position.y + halfItemOffset - 3, 9, 5), ReorderableListResources.texGrabHandle);

					// Draw splitter between list items.
					if (!trackingControl || itemIndex != s_AnchorIndex)
						GUI.DrawTexture(new Rect(position.x, position.y - 1, position.width, 1), ReorderableListResources.texItemSplitter);
					break;
			}

			// Allow control to be automatically focused.
			if (s_AutoFocusIndex == itemIndex)
				GUI.SetNextControlName("AutoFocus_" + controlID + "_" + itemIndex);

			// Present actual control.
			EditorGUI.BeginChangeCheck();
			{
				s_CurrentItemIndex = itemIndex;
				list.DrawItem(s_ItemContentPosition, itemIndex);
			}
			if (EditorGUI.EndChangeCheck())
				ReorderableListGUI.indexOfChangedItem = itemIndex;

			// Draw remove button?
			if (hasRemoveButtons) {
				s_RemoveButtonPosition = position;
				s_RemoveButtonPosition.width = removeButtonStyle.fixedWidth;
				s_RemoveButtonPosition.x = s_ItemContentPosition.xMax + 2;
				s_RemoveButtonPosition.height -= 2;

				if (DoRemoveButton(s_RemoveButtonPosition)) {
					// Remove last entry in list.
					list.Remove(itemIndex);

					GUI.changed = true;
					ReorderableListGUI.indexOfChangedItem = -1;
				}
			}

			// Check for context click?
			if (eventType == EventType.ContextClick && position.Contains(Event.current.mousePosition)) {
				ShowContextMenu(controlID, itemIndex, list);
				Event.current.Use();
			}
		}

		private void DrawFloatingListItem(EventType eventType, int controlID, IReorderableListData list, float targetSlotPosition) {
			if (eventType == EventType.Repaint) {
				Color restoreColor = GUI.color;

				// Fill background of target area.
				Rect targetPosition = s_DragItemPosition;
				targetPosition.y = targetSlotPosition - 1;
				targetPosition.height = 1;

				GUI.DrawTexture(targetPosition, ReorderableListResources.texItemSplitter);

				--targetPosition.x;
				++targetPosition.y;
				targetPosition.width += 2;
				targetPosition.height = s_DragItemPosition.height - 1;

				GUI.color = TargetBackgroundColor;
				GUI.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				// Fill background of item which is being dragged.
				--s_DragItemPosition.x;
				s_DragItemPosition.width += 2;
				--s_DragItemPosition.height;

				GUI.color = AnchorBackgroundColor;
				GUI.DrawTexture(s_DragItemPosition, EditorGUIUtility.whiteTexture);

				++s_DragItemPosition.x;
				s_DragItemPosition.width -= 2;
				++s_DragItemPosition.height;

				// Draw horizontal splitter above and below.
				GUI.color = new Color(0f, 0f, 0f, 0.6f);
				targetPosition.y = s_DragItemPosition.y - 1;
				targetPosition.height = 1;
				GUI.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				targetPosition.y += s_DragItemPosition.height;
				GUI.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				GUI.color = restoreColor;
			}

			DrawListItem(eventType, s_DragItemPosition, controlID, list, s_AnchorIndex);
		}

		/// <summary>
		/// Draw list container and items.
		/// </summary>
		/// <param name="position">Position of list control in GUI.</param>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="list">Abstracted representation of list.</param>
		private void DrawListContainerAndItems(Rect position, int controlID, IReorderableListData list) {
			bool allowReordering = (flags & ReorderableListFlags.DisableReordering) == 0;
			bool trackingControl = IsTrackingControl(controlID);

			// Get local copy of event information for efficiency.
			EventType eventType = Event.current.GetTypeForControl(controlID);
			if (Event.current.isMouse)
				s_MousePosition = Event.current.mousePosition;

			int newTargetIndex = s_TargetIndex;

			// Position of first item in list.
			float firstItemY = position.y + containerStyle.padding.top;

			switch (eventType) {
				case EventType.MouseDown:
					if (trackingControl) {
						// Cancel drag when other mouse button is pressed.
						s_TrackingCancelBlockContext = true;
						Event.current.Use();
					}
					break;

				case EventType.MouseDrag:
					if (trackingControl) {
						// Reset target index and adjust when looping through list items.
						if (s_MousePosition.y < firstItemY)
							newTargetIndex = 0;
						else if (s_MousePosition.y >= position.yMax)
							newTargetIndex = list.Count;
					}
					break;

				case EventType.MouseUp:
					if (controlID == GUIUtility.hotControl) {
						// Allow user code to change control over reordering during drag.
						if (!s_TrackingCancelBlockContext && allowReordering)
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

				case EventType.ExecuteCommand:
					if (s_ContextControlID == controlID) {
						int itemIndex = s_ContextItemIndex;
						try {
							DoCommand(s_ContextCommandName, itemIndex, list);
							Event.current.Use();
						}
						finally {
							s_ContextControlID = 0;
							s_ContextItemIndex = 0;
						}
					}
					break;

				case EventType.Repaint:
					// Draw caption area of list.
					containerStyle.Draw(position, GUIContent.none, false, false, false, false);
					break;
			}

			ReorderableListGUI.indexOfChangedItem = -1;

			// Draw list items!
			Rect itemPosition = new Rect(position.x + 2, firstItemY, position.width - 4, 0);
			float targetSlotPosition = position.yMax - s_DragItemPosition.height - 1;

			int count = list.Count;
			for (int i = 0; i < count; ++i) {
				itemPosition.y = itemPosition.yMax;
				itemPosition.height = 0;

				if (trackingControl) {
					// Does this represent the target index?
					if (i == s_TargetIndex) {
						targetSlotPosition = itemPosition.y;
						itemPosition.y += s_DragItemPosition.height;
					}

					// Do not draw item if it is currently being dragged.
					// Draw later so that it is shown in front of other controls.
					if (i == s_AnchorIndex)
						continue;
				}

				// Update position for current item.
				itemPosition.height = list.GetItemHeight(i) + 4;

				// Draw list item.
				DrawListItem(eventType, itemPosition, controlID, list, i);

				// Did list count change (i.e. item removed)?
				if (list.Count < count) {
					// We assume that it was this item which was removed, so --i allows us
					// to process the next item as usual.
					count = list.Count;
					--i;
					continue;
				}
				
				// Event has already been used, skip to next item.
				if (Event.current.type != EventType.Used) {
					switch (eventType) {
						case EventType.MouseDown:
							if (GUI.enabled && itemPosition.Contains(s_MousePosition)) {
								// Remove input focus from control before attempting a context click or drag.
								GUIUtility.keyboardControl = 0;

								if (allowReordering && Event.current.button == 0) {
									s_DragItemPosition = itemPosition;

									BeginTrackingReorderDrag(controlID, i);
									s_AnchorMouseOffset = itemPosition.y - s_MousePosition.y;
									s_TargetIndex = i;
								}

								Event.current.Use();
							}
							break;

						case EventType.MouseDrag:
							float midpoint = itemPosition.y + itemPosition.height / 2f;
							if (trackingControl) {
								if (s_DragItemPosition.y > itemPosition.y && s_DragItemPosition.y <= midpoint)
									newTargetIndex = i;
								else if (s_DragItemPosition.yMax > midpoint && s_DragItemPosition.yMax <= itemPosition.yMax)
									newTargetIndex = i + 1;
							}
							break;
					}
				}
			}

			// Item which is being dragged should be shown on top of other controls!
			if (IsTrackingControl(controlID)) {
				if (eventType == EventType.MouseDrag) {
					// Force repaint to occur so that dragging rectangle is visible.
					s_TargetIndex = newTargetIndex;
					Event.current.Use();
				}

				s_DragItemPosition.y = Mathf.Clamp(s_MousePosition.y + s_AnchorMouseOffset, firstItemY, position.yMax - s_DragItemPosition.height - 1);
				DrawFloatingListItem(eventType, controlID, list, targetSlotPosition);
			}
			
			s_CurrentItemIndex = -1;

			// Fake control to catch input focus if auto focus was not possible.
			// Note: Replicated in layout version of `DoListField`.
			GUIUtility.GetControlID(FocusType.Keyboard);
		}

		/// <summary>
		/// Checks to see if list control needs to be automatically focused.
		/// </summary>
		/// <param name="controlID">Unique ID of list control.</param>
		private void CheckForAutoFocusControl(int controlID) {
			if (Event.current.type == EventType.Used)
				return;

			// Automatically focus control!
			if (s_AutoFocusControlID == controlID) {
				s_AutoFocusControlID = 0;
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2)
				GUI.FocusControl("AutoFocus_" + controlID + "_" + s_AutoFocusIndex);
#else
				EditorGUI.FocusTextInControl("AutoFocus_" + controlID + "_" + s_AutoFocusIndex);
#endif
				s_AutoFocusIndex = -1;
			}
		}
		
		/// <summary>
		/// Draw additional controls below list control and highlight drop target.
		/// </summary>
		/// <param name="position">Position of list control in GUI.</param>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="list">Abstracted representation of list.</param>
		private void DrawFooterControls(Rect position, int controlID, IReorderableListData list) {
			if (hasAddButton) {
				Rect addButtonRect = new Rect(
					position.xMax - addButtonStyle.fixedWidth,
					position.yMax - 1,
					addButtonStyle.fixedWidth,
					addButtonStyle.fixedHeight
				);
				DoAddButton(addButtonRect, controlID, list);
			}
		}

		/// <summary>
		/// Cache of container heights mapped by control ID.
		/// </summary>
		private static Dictionary<int, float> s_ContainerHeightCache = new Dictionary<int, float>();

		/// <summary>
		/// Do layout version of list field.
		/// </summary>
		/// <param name="controlID">Unique ID of list control.</param>
		/// <param name="list">Abstracted representation of list.</param>
		/// <returns>
		/// Position of list container area in GUI (excludes footer area).
		/// </returns>
		private Rect DrawLayoutListField(int controlID, IReorderableListData list) {
			float totalHeight;

			// Calculate position of list field using layout engine.
			if (Event.current.type == EventType.Layout) {
				totalHeight = CalculateListHeight(list);
				s_ContainerHeightCache[controlID] = totalHeight;
			}
			else {
				totalHeight = s_ContainerHeightCache.ContainsKey(controlID)
					? s_ContainerHeightCache[controlID]
					: 0;
			}

			Rect position = GUILayoutUtility.GetRect(GUIContent.none, containerStyle, GUILayout.Height(totalHeight));

			// Make room for add button?
			if (hasAddButton)
				position.height -= addButtonStyle.fixedHeight;

			if (Event.current.type != EventType.Layout) {
				// Draw list as normal.
				DrawListContainerAndItems(position, controlID, list);
			}
			else {
				// Layout events are still needed to avoid breaking control ID's in remaining
				// interface. Let's keep this as simple as possible!

				Rect itemPosition = default(Rect);

				int count = list.Count;
				for (int i = 0; i < count; ++i) {
					itemPosition.height = list.GetItemHeight(i);

					if (s_AutoFocusIndex == i)
						GUI.SetNextControlName("AutoFocus_" + controlID + "_" + i);

					list.DrawItem(itemPosition, i);

					if (hasRemoveButtons)
						DoRemoveButton(default(Rect));
				}

				// Fake control to catch input focus if auto focus was not possible.
				// Note: Still needed, copied from absolute version of `DoListField`.
				GUIUtility.GetControlID(FocusType.Keyboard);
			}

			CheckForAutoFocusControl(controlID);

			return position;
		}

		/// <summary>
		/// Draw content for empty list (layout version).
		/// </summary>
		/// <param name="drawEmpty">Callback to draw empty content.</param>
		/// <returns>
		/// Position of list container area in GUI (excludes footer area).
		/// </returns>
		private Rect DrawLayoutEmptyList(DrawEmpty drawEmpty) {
			Rect r = EditorGUILayout.BeginVertical(containerStyle);
			{
				if (drawEmpty != null)
					drawEmpty();
				else
					GUILayout.Space(5);
			}
			EditorGUILayout.EndVertical();

			// Allow room for add button.
			GUILayoutUtility.GetRect(0, addButtonStyle.fixedHeight - 1);

			return r;
		}

		/// <summary>
		/// Draw content for empty list (layout version).
		/// </summary>
		/// <param name="position">Position of list control in GUI.</param>
		/// <param name="drawEmpty">Callback to draw empty content.</param>
		private void DrawEmptyListControl(Rect position, DrawEmptyAbsolute drawEmpty) {
			if (Event.current.type == EventType.Repaint)
				containerStyle.Draw(position, GUIContent.none, false, false, false, false);

			// Take padding into consideration when drawing empty content.
			position.x += containerStyle.padding.left;
			position.y += containerStyle.padding.top;
			position.width -= containerStyle.padding.horizontal;
			position.height -= containerStyle.padding.vertical;

			if (drawEmpty != null)
				drawEmpty(position);
		}

		/// <summary>
		/// Correct if for some reason one or more styles are missing!
		/// </summary>
		private void FixStyles() {
			containerStyle = containerStyle ?? ReorderableListGUI.containerStyle;
			addButtonStyle = addButtonStyle ?? ReorderableListGUI.addButtonStyle;
			removeButtonStyle = removeButtonStyle ?? ReorderableListGUI.removeButtonStyle;
		}

		/// <summary>
		/// Draw layout version of list control.
		/// </summary>
		/// <param name="list">Abstracted representation of list.</param>
		/// <param name="drawEmpty">Delegate for drawing empty list.</param>
		protected void DoListField(IReorderableListData list, DrawEmpty drawEmpty) {
			int controlID = GUIUtility.GetControlID(FocusType.Passive);

			FixStyles();

			Rect position;

			if (list.Count > 0)
				position = DrawLayoutListField(controlID, list);
			else
				position = DrawLayoutEmptyList(drawEmpty);

			DrawFooterControls(position, controlID, list);
		}

		/// <summary>
		/// Draw list control with absolute positioning.
		/// </summary>
		/// <param name="position">Position of list control in GUI.</param>
		/// <param name="list">Abstracted representation of list.</param>
		/// <param name="drawEmpty">Delegate for drawing empty list.</param>
		protected void DoListField(Rect position, IReorderableListData list, DrawEmptyAbsolute drawEmpty) {
			int controlID = GUIUtility.GetControlID(FocusType.Passive);

			FixStyles();

			// Allow for footer area.
			if (hasAddButton)
				position.height -= addButtonStyle.fixedHeight;

			if (list.Count > 0) {
				DrawListContainerAndItems(position, controlID, list);
				CheckForAutoFocusControl(controlID);
			}
			else {
				DrawEmptyListControl(position, drawEmpty);
			}

			DrawFooterControls(position, controlID, list);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Calculate height of list control in pixels.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		protected float CalculateListHeight(IReorderableListData list) {
			FixStyles();

			float totalHeight = containerStyle.padding.vertical - 1;

			// Take list items into consideration.
			int count = list.Count;
			for (int i = 0; i < count; ++i)
				totalHeight += list.GetItemHeight(i);
			// Add spacing between list items.
			totalHeight += 4 * count;

			// Add height of add button.
			if (hasAddButton)
				totalHeight += addButtonStyle.fixedHeight;

			return totalHeight;
		}

		/// <summary>
		/// Calculate height of list control in pixels.
		/// </summary>
		/// <param name="itemCount">Count of items in list.</param>
		/// <param name="itemHeight">Fixed height of list item.</param>
		/// <returns>
		/// Required list height in pixels.
		/// </returns>
		public float CalculateListHeight(int itemCount, float itemHeight) {
			FixStyles();

			float totalHeight = containerStyle.padding.vertical - 1;

			// Take list items into consideration.
			totalHeight += (itemHeight + 4) * itemCount;

			// Add height of add button.
			if (hasAddButton)
				totalHeight += addButtonStyle.fixedHeight;

			return totalHeight;
		}

		#endregion

		#region Context Menu

		/// <summary>
		/// Content for "Move to Top" command.
		/// </summary>
		protected static readonly GUIContent commandMoveToTop = new GUIContent("Move to Top");
		/// <summary>
		/// Content for "Move to Bottom" command.
		/// </summary>
		protected static readonly GUIContent commandMoveToBottom = new GUIContent("Move to Bottom");
		/// <summary>
		/// Content for "Insert Above" command.
		/// </summary>
		protected static readonly GUIContent commandInsertAbove = new GUIContent("Insert Above");
		/// <summary>
		/// Content for "Insert Below" command.
		/// </summary>
		protected static readonly GUIContent commandInsertBelow = new GUIContent("Insert Below");
		/// <summary>
		/// Content for "Duplicate" command.
		/// </summary>
		protected static readonly GUIContent commandDuplicate = new GUIContent("Duplicate");
		/// <summary>
		/// Content for "Remove" command.
		/// </summary>
		protected static readonly GUIContent commandRemove = new GUIContent("Remove");
		/// <summary>
		/// Content for "Clear All" command.
		/// </summary>
		protected static readonly GUIContent commandClearAll = new GUIContent("Clear All");

		// Command control id and item index are assigned when context menu is shown.
		private static int s_ContextControlID;
		private static int s_ContextItemIndex;

		// Command name is assigned by default context menu handler.
		private static string s_ContextCommandName;

		private void ShowContextMenu(int controlID, int itemIndex, IReorderableListData list) {
			GenericMenu menu = new GenericMenu();

			s_ContextControlID = controlID;
			s_ContextItemIndex = itemIndex;

			AddItemsToMenu(menu, itemIndex, list);

			if (menu.GetItemCount() > 0)
				menu.ShowAsContext();
		}

		/// <summary>
		/// Default functionality to handle context command.
		/// </summary>
		/// <example>
		/// <para>Can be used when adding custom items to the context menu:</para>
		/// <code language="csharp"><![CDATA[
		/// protected override void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListData list) {
		///     var specialCommand = new GUIContent("Special Command");
		///     menu.AddItem(specialCommand, false, defaultContextHandler, specialCommand);
		/// }
		/// ]]></code>
		/// <code language="unityscript"><![CDATA[
		/// function AddItemsToMenu(menu:GenericMenu, itemIndex:int, list:IReorderableListData) {
		///     var specialCommand = new GUIContent('Special Command');
		///     menu.AddItem(specialCommand, false, defaultContextHandler, specialCommand);
		/// }
		/// ]]></code>
		/// </example>
		/// <seealso cref="AddItemsToMenu"/>
		protected static GenericMenu.MenuFunction2 defaultContextHandler = DefaultContextMenuHandler;

		private static void DefaultContextMenuHandler(object userData) {
			var commandContent = userData as GUIContent;
			if (commandContent == null || string.IsNullOrEmpty(commandContent.text))
				return;

			s_ContextCommandName = commandContent.text;

			var e = EditorGUIUtility.CommandEvent("ReorderableListContextCommand");
			EditorWindow.focusedWindow.SendEvent(e);
		}

		/// <summary>
		/// Invoked to generate context menu for list item.
		/// </summary>
		/// <param name="menu">Menu which can be populated.</param>
		/// <param name="itemIndex">Zero-based index of item which was right-clicked.</param>
		/// <param name="list">The list which can be reordered.</param>
		protected virtual void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListData list) {
			if ((flags & ReorderableListFlags.DisableReordering) == 0) {
				if (itemIndex > 0)
					menu.AddItem(commandMoveToTop, false, defaultContextHandler, commandMoveToTop);
				else
					menu.AddDisabledItem(commandMoveToTop);

				if (itemIndex + 1 < list.Count)
					menu.AddItem(commandMoveToBottom, false, defaultContextHandler, commandMoveToBottom);
				else
					menu.AddDisabledItem(commandMoveToBottom);

				if (hasAddButton) {
					menu.AddSeparator("");

					menu.AddItem(commandInsertAbove, false, defaultContextHandler, commandInsertAbove);
					menu.AddItem(commandInsertBelow, false, defaultContextHandler, commandInsertBelow);

					if ((flags & ReorderableListFlags.DisableDuplicateCommand) == 0)
						menu.AddItem(commandDuplicate, false, defaultContextHandler, commandDuplicate);
				}
			}

			if (hasRemoveButtons) {
				if (menu.GetItemCount() > 0)
					menu.AddSeparator("");

				menu.AddItem(commandRemove, false, defaultContextHandler, commandRemove);
				menu.AddSeparator("");
				menu.AddItem(commandClearAll, false, defaultContextHandler, commandClearAll);
			}
		}

		#endregion

		#region Command Handling

		/// <summary>
		/// Invoked to handle command.
		/// </summary>
		/// <remarks>
		/// <para>It is important to set the value of <c>GUI.changed</c> to <c>true</c> if any
		/// changes are made by command handler.</para>
		/// <para>Default command handling functionality can be inherited:</para>
		/// <code language="csharp"><![CDATA[
		/// protected override bool HandleCommand<T>(string commandName, int itemIndex, List<T> list) {
		///     if (base.HandleContextCommand(itemIndex, list))
		///         return true;
		///     
		///     // Place default command handling code here...
		///     switch (commandName) {
		///         case "Your Command":
		///             break;
		///     }
		/// 
		///     return false;
		/// }
		/// ]]></code>
		/// <code language="unityscript"><![CDATA[
		/// function HandleCommand<T>(commandName:String, itemIndex:int, list:List.<T>):boolean {
		///     if (base.HandleContextCommand(itemIndex, list))
		///         return true;
		///     
		///     // Place default command handling code here...
		///     switch (commandName) {
		///         case 'Your Command':
		///             break;
		///     }
		/// 
		///     return false;
		/// }
		/// ]]></code>
		/// </remarks>
		/// <param name="commandName">Name of command. This is the text shown in the context menu.</param>
		/// <param name="itemIndex">Zero-based index of item which was right-clicked.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <returns>
		/// A value of <c>true</c> if command was known; otherwise <c>false</c>.
		/// </returns>
		protected virtual bool HandleCommand(string commandName, int itemIndex, IReorderableListData list) {
			switch (commandName) {
				case "Move to Top":
					list.Move(itemIndex, 0);
					break;
				case "Move to Bottom":
					list.Move(itemIndex, list.Count);
					break;
				case "Insert Above":
					list.Insert(itemIndex);
					AutoFocusItem(s_ContextControlID, itemIndex);
					break;
				case "Insert Below":
					list.Insert(itemIndex + 1);
					AutoFocusItem(s_ContextControlID, itemIndex + 1);
					break;
				case "Duplicate":
					list.Duplicate(itemIndex);
					AutoFocusItem(s_ContextControlID, itemIndex + 1);
					break;
				case "Remove":
					list.Remove(itemIndex);
					break;
				case "Clear All":
					list.Clear();
					break;

				default:
					return false;
			}

			GUI.changed = true;
			ReorderableListGUI.indexOfChangedItem = -1;

			return true;
		}

		/// <summary>
		/// Call to manually perform command.
		/// </summary>
		/// <remarks>
		/// <para>Warning message is logged to console if attempted to execute unknown command.</para>
		/// </remarks>
		/// <param name="commandName">Name of command. This is the text shown in the context menu.</param>
		/// <param name="itemIndex">Zero-based index of item which was right-clicked.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <returns>
		/// A value of <c>true</c> if command was known; otherwise <c>false</c>.
		/// </returns>
		public bool DoCommand(string commandName, int itemIndex, IReorderableListData list) {
			if (!HandleCommand(s_ContextCommandName, itemIndex, list)) {
				Debug.LogWarning("Unknown context command.");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Call to manually perform command.
		/// </summary>
		/// <remarks>
		/// <para>Warning message is logged to console if attempted to execute unknown command.</para>
		/// </remarks>
		/// <param name="command">Content representing command.</param>
		/// <param name="itemIndex">Zero-based index of item which was right-clicked.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <returns>
		/// A value of <c>true</c> if command was known; otherwise <c>false</c>.
		/// </returns>
		public bool DoCommand<T>(GUIContent command, int itemIndex, IReorderableListData list) {
			return DoCommand(command.text, itemIndex, list);
		}

		#endregion

	}

}