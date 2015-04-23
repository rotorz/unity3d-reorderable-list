// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Rotorz.ReorderableList.Internal;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList {
	
	/// <summary>
	/// Styles for the <see cref="ReorderableListControl"/>.
	/// </summary>
	public static class ReorderableListStyles {

		static ReorderableListStyles() {
		}

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
		/// Gets default style for add item button 2.
		/// </summary>
		public static GUIStyle defaultAddButton2Style { get; private set; }
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

			defaultAddButton2Style = new GUIStyle();
			defaultAddButton2Style.fixedWidth = 30;
			defaultAddButton2Style.fixedHeight = 18;
			defaultAddButton2Style.normal.background = ReorderableListResources.texAddButton2;
			defaultAddButton2Style.active.background = ReorderableListResources.texAddButton2Active;

			defaultRemoveButtonStyle = new GUIStyle();
			defaultRemoveButtonStyle.fixedWidth = 27;
			defaultRemoveButtonStyle.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
			defaultRemoveButtonStyle.imagePosition = ImagePosition.ImageOnly;
			defaultRemoveButtonStyle.alignment = TextAnchor.MiddleCenter;

		}

	}

}
