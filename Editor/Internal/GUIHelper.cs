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
	internal static class GUIHelper {

		static GUIHelper() {
			var tyGUIClip = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
			if (tyGUIClip != null) {
				var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
				if (piVisibleRect != null) {
					var getGetMethod = piVisibleRect.GetGetMethod();
					VisibleRect = () => (Rect)getGetMethod.Invoke(null, null);
				}
			}

			var miFocusTextInControl = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);
			if (miFocusTextInControl == null)
				miFocusTextInControl = typeof(GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);

			FocusTextInControl = str => miFocusTextInControl.Invoke(null, new object[] { str });
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

	}

}