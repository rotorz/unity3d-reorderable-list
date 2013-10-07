// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// List control for generic lists which can be used within custom editor windows
	/// and inspectors with support for drag and drop reordering of list items.
	/// </summary>
	[Serializable]
	public class GenericListControl : ReorderableListControl {
		
		#region Generic List Abstraction

		/// <summary>
		/// Implementation of reorderable list data for generic lists.
		/// </summary>
		private sealed class GenericListData<T> : IReorderableListData {

			public List<T> list;

			public ReorderableListControl.ItemDrawer<T> itemDrawer;
			public float itemHeight;

			/// <summary>
			/// Initializes a new instance of <see cref="GenericListData{T}"/>.
			/// </summary>
			/// <param name="list">The list which can be reordered.</param>
			/// <param name="itemDrawer">Callback to draw list item.</param>
			/// <param name="itemHeight">Height of list item in pixels.</param>
			public GenericListData(List<T> list, ReorderableListControl.ItemDrawer<T> itemDrawer, float itemHeight) {
				this.list = list;
				this.itemDrawer = itemDrawer ?? ReorderableListGUI.DefaultItemDrawer;
				this.itemHeight = itemHeight;
			}

			#region IReorderableListData - Implementation

			/// <inheritdoc/>
			public object Raw {
				get { return list; }
			}

			/// <inheritdoc/>
			public int Count {
				get { return list.Count; }
			}

			/// <inheritdoc/>
			public void Add() {
				list.Add(default(T));
			}
			/// <inheritdoc/>
			public void Insert(int index) {
				list.Insert(index, default(T));
			}
			/// <inheritdoc/>
			public void Duplicate(int index) {
				list.Insert(index + 1, list[index]);
			}
			/// <inheritdoc/>
			public void Remove(int index) {
				list.RemoveAt(index);
			}
			/// <inheritdoc/>
			public void Move(int sourceIndex, int destIndex) {
				if (destIndex > sourceIndex)
					--destIndex;

				T item = list[sourceIndex];
				list.RemoveAt(sourceIndex);
				list.Insert(destIndex, item);
			}
			/// <inheritdoc/>
			public void Clear() {
				list.Clear();
			}

			/// <inheritdoc/>
			public void DrawItem(Rect position, int index) {
				list[index] = itemDrawer(position, list[index]);
			}

			/// <inheritdoc/>
			public float GetItemHeight(int index) {
				return itemHeight;
			}

			#endregion

		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="GenericListControl"/>.
		/// </summary>
		public GenericListControl() : base() {
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GenericListControl"/>.
		/// </summary>
		/// <param name="flags">Optional flags which affect behavior of control.</param>
		public GenericListControl(ReorderableListFlags flags) : base(flags) {
		}

		#endregion

		#region Methods
		
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public void Draw<T>(List<T> list, ItemDrawer<T> drawItem, DrawEmpty drawEmpty, float itemHeight) {
			DoListField(new GenericListData<T>(list, drawItem, itemHeight), drawEmpty);
		}
		/// <summary>
		/// Draw list field control.
		/// </summary>
		/// <param name="position">Position of control.</param>
		/// <param name="list">The list which can be reordered.</param>
		/// <param name="drawItem">Callback to draw list item.</param>
		/// <param name="drawEmpty">Callback to draw custom content for empty list (optional).</param>
		/// <param name="itemHeight">Height of a single list item.</param>
		/// <typeparam name="T">Type of list item.</typeparam>
		public void Draw<T>(Rect position, List<T> list, ItemDrawer<T> drawItem, DrawEmptyAbsolute drawEmpty, float itemHeight) {
			DoListField(position, new GenericListData<T>(list, drawItem, itemHeight), drawEmpty);
		}

		#endregion

	}

}