// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList.Internal {

	public enum ReorderableListTexture {
		Icon_Add_Normal = 0,
		Icon_Add_Active,
		Icon_Remove_Normal,
		Icon_Remove_Active,
		Button_Normal,
		Button_Active,
		Button2_Normal,
		Button2_Active,
		TitleBackground,
		ContainerBackground,
		GrabHandle,
	}

	/// <summary>
	/// Resources to assist with reorderable list control.
	/// </summary>
	public static class ReorderableListResources {

		static ReorderableListResources() {
			GenerateSpecialTextures();
			LoadResourceAssets();
		}

		#region Texture Resources

		/// <summary>
		/// Resource assets for light skin.
		/// </summary>
		/// <remarks>
		/// <para>Resource assets are PNG images which have been encoded using a base-64
		/// string so that actual asset files are not necessary.</para>
		/// </remarks>
		private static string[] s_LightSkin = {
			"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",
		};
		/// <summary>
		/// Resource assets for dark skin.
		/// </summary>
		/// <remarks>
		/// <para>Resource assets are PNG images which have been encoded using a base-64
		/// string so that actual asset files are not necessary.</para>
		/// </remarks>
		private static string[] s_DarkSkin = {
			"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",
		};

		/// <summary>
		/// Gets light or dark version of the specified texture.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Texture2D GetTexture(ReorderableListTexture name) {
			return s_Cached[(int)name];
		}

		#endregion

		#region Generated Resources

		public static Texture2D texItemSplitter { get; private set; }

		/// <summary>
		/// Generate special textures.
		/// </summary>
		private static void GenerateSpecialTextures() {
			var splitterColor = EditorGUIUtility.isProSkin
				? new Color(1f, 1f, 1f, 0.14f)
				: new Color(0.59f, 0.59f, 0.59f, 0.55f)
				;
			texItemSplitter = CreatePixelTexture("(Generated) Item Splitter", splitterColor);
		}

		/// <summary>
		/// Create 1x1 pixel texture of specified color.
		/// </summary>
		/// <param name="name">Name for texture object.</param>
		/// <param name="color">Pixel color.</param>
		/// <returns>
		/// The new <c>Texture2D</c> instance.
		/// </returns>
		public static Texture2D CreatePixelTexture(string name, Color color) {
			var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
			tex.name = name;
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.filterMode = FilterMode.Point;
			tex.SetPixel(0, 0, color);
			tex.Apply();
			return tex;
		}

		#endregion

		#region Load PNG from Base-64 Encoded String

		private static Texture2D[] s_Cached;

		/// <summary>
		/// Read textures from base-64 encoded strings. Automatically selects assets based
		/// upon whether the light or dark (pro) skin is active.
		/// </summary>
		private static void LoadResourceAssets() {
			var skin = EditorGUIUtility.isProSkin ? s_DarkSkin : s_LightSkin;
			s_Cached = new Texture2D[skin.Length];

			for (int i = 0; i < s_Cached.Length; ++i) {
				// Get image data (PNG) from base64 encoded strings.
				byte[] imageData = Convert.FromBase64String(skin[i]);

				// Gather image size from image data.
				int texWidth, texHeight;
				GetImageSize(imageData, out texWidth, out texHeight);

				// Generate texture asset.
				var tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, true);
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.name = "(Generated) ReorderableList:" + i;
				tex.filterMode = FilterMode.Point;
				tex.LoadImage(imageData);

				s_Cached[i] = tex;
			}

			s_LightSkin = null;
			s_DarkSkin = null;
		}

		/// <summary>
		/// Read width and height if PNG file in pixels.
		/// </summary>
		/// <param name="imageData">PNG image data.</param>
		/// <param name="width">Width of image in pixels.</param>
		/// <param name="height">Height of image in pixels.</param>
		private static void GetImageSize(byte[] imageData, out int width, out int height) {
			width = ReadInt(imageData, 3 + 15);
			height = ReadInt(imageData, 3 + 15 + 2 + 2);
		}

		private static int ReadInt(byte[] imageData, int offset) {
			return (imageData[offset] << 8) | imageData[offset + 1];
		}

		#endregion

	}

}
