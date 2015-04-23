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
			Title = new GUIStyle();
			Title.border = new RectOffset(2, 2, 2, 1);
			Title.margin = new RectOffset(5, 5, 5, 0);
			Title.padding = new RectOffset(5, 5, 0, 0);
			Title.alignment = TextAnchor.MiddleLeft;
			Title.normal.background = ReorderableListResources.texTitleBackground;
			Title.normal.textColor = EditorGUIUtility.isProSkin
				? new Color(0.8f, 0.8f, 0.8f)
				: new Color(0.2f, 0.2f, 0.2f);

			Container = new GUIStyle();
			Container.border = new RectOffset(2, 2, 1, 2);
			Container.margin = new RectOffset(5, 5, 5, 5);
			Container.padding = new RectOffset(1, 1, 2, 2);
			Container.normal.background = ReorderableListResources.texContainerBackground;

			AddButton = new GUIStyle();
			AddButton.fixedWidth = 30;
			AddButton.fixedHeight = 16;
			AddButton.normal.background = ReorderableListResources.texAddButton;
			AddButton.active.background = ReorderableListResources.texAddButtonActive;

			AddButton2 = new GUIStyle();
			AddButton2.fixedWidth = 30;
			AddButton2.fixedHeight = 18;
			AddButton2.normal.background = ReorderableListResources.texAddButton2;
			AddButton2.active.background = ReorderableListResources.texAddButton2Active;

			RemoveButton = new GUIStyle();
			RemoveButton.fixedWidth = 27;
			RemoveButton.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
			RemoveButton.imagePosition = ImagePosition.ImageOnly;
			RemoveButton.alignment = TextAnchor.MiddleCenter;
		}

		/// <summary>
		/// Gets default style for title header.
		/// </summary>
		public static GUIStyle Title { get; private set; }

		/// <summary>
		/// Gets default style for background of list control.
		/// </summary>
		public static GUIStyle Container { get; private set; }
		/// <summary>
		/// Gets default style for add item button.
		/// </summary>
		public static GUIStyle AddButton { get; private set; }
		/// <summary>
		/// Gets default style for add item button 2.
		/// </summary>
		public static GUIStyle AddButton2 { get; private set; }
		/// <summary>
		/// Gets default style for remove item button.
		/// </summary>
		public static GUIStyle RemoveButton { get; private set; }

	}

}
