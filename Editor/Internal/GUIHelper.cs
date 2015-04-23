// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList.Internal {

	/// <summary>
	/// Utility functions to assist with GUIs.
	/// </summary>
	public static class GUIHelper {

		static GUIHelper() {
			var tyGUIClip = Type.GetType("UnityEngine.GUIClip,UnityEngine");
			if (tyGUIClip != null) {
				var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
				if (piVisibleRect != null)
					VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), piVisibleRect.GetGetMethod());
			}
			
			var miFocusTextInControl = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);
			if (miFocusTextInControl == null)
				miFocusTextInControl = typeof(GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);

			FocusTextInControl = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), miFocusTextInControl);
		}

		/// <summary>
		/// Gets visible rectangle within GUI.
		/// </summary>
		/// <remarks>
		/// <para>VisibleRect = TopmostRect + scrollViewOffsets</para>
		/// </remarks>
		public static Func<Rect> VisibleRect;

		/// <summary>
		/// Focus control and text editor where applicable.
		/// </summary>
		public static Action<string> FocusTextInControl;

		private static GUIStyle s_TempStyle = new GUIStyle();

		/// <summary>
		/// Draw texture using <see cref="GUIStyle"/> to workaround bug in Unity where
		/// <see cref="GUI.DrawTexture"/> flickers when embedded inside a property drawer.
		/// </summary>
		/// <param name="position">Position of which to draw texture in space of GUI.</param>
		/// <param name="texture">Texture.</param>
		public static void DrawTexture(Rect position, Texture2D texture) {
			if (Event.current.type != EventType.Repaint)
				return;

			s_TempStyle.normal.background = texture;

			s_TempStyle.Draw(position, GUIContent.none, false, false, false, false);
		}

		private static GUIContent s_TempIconContent = new GUIContent();

		internal static bool IconButton(Rect position, bool visible, Texture2D iconNormal, Texture2D iconActive, GUIStyle style) {
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Vector2 mousePosition = Event.current.mousePosition;

			switch (Event.current.GetTypeForControl(controlID)) {
				case EventType.MouseDown:
					// Do not allow button to be pressed using right mouse button since
					// context menu should be shown instead!
					if (GUI.enabled && Event.current.button != 1 && position.Contains(mousePosition)) {
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

						if (position.Contains(mousePosition)) {
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
					if (visible) {
						bool isActive = GUIUtility.hotControl == controlID && position.Contains(Event.current.mousePosition);
						s_TempIconContent.image = isActive ? iconActive : iconNormal;
						style.Draw(position, s_TempIconContent, controlID);
					}
					break;
			}

			return false;
		}

	}

}
