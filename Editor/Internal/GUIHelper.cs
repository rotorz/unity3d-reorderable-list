// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;

using System;
using System.Reflection;

namespace Rotorz.ReorderableList.Internal {

	/// <summary>
	/// Utility functions to assist with GUIs.
	/// </summary>
	internal static class GUIHelper {

		static GUIHelper() {
			Type tyGUIClip = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
			if (tyGUIClip != null) {
				PropertyInfo piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
				if (piVisibleRect != null)
					VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), piVisibleRect.GetGetMethod());
			}
		}

		/// <summary>
		/// Gets visible rectangle within GUI.
		/// </summary>
		/// <remarks>
		/// <para>VisibleRect = TopmostRect + scrollViewOffsets</para>
		/// </remarks>
		public static Func<Rect> VisibleRect;

	}

}