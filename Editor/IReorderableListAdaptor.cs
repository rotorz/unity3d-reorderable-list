// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Adaptor allowing reorderable list control to interface with list data.
	/// </summary>
	public interface IReorderableListAdaptor {

		/// <summary>
		/// Gets count of elements in list.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Add new element at end of list.
		/// </summary>
		void Add();
		/// <summary>
		/// Insert new element at specified index.
		/// </summary>
		/// <param name="index">Zero-based index for list element.</param>
		void Insert(int index);
		/// <summary>
		/// Duplicate existing element.
		/// </summary>
		/// <param name="index">Zero-based index of list element.</param>
		void Duplicate(int index);
		/// <summary>
		/// Remove element at specified index.
		/// </summary>
		/// <param name="index">Zero-based index of list element.</param>
		void Remove(int index);
		/// <summary>
		/// Move element from source index to destination index.
		/// </summary>
		/// <param name="sourceIndex">Zero-based index of source element.</param>
		/// <param name="destIndex">Zero-based index of destination element.</param>
		void Move(int sourceIndex, int destIndex);
		/// <summary>
		/// Clear all elements from list.
		/// </summary>
		void Clear();

		/// <summary>
		/// Draw interface for list element.
		/// </summary>
		/// <param name="position">Position in GUI.</param>
		/// <param name="index">Zero-based index of array element.</param>
		void DrawItem(Rect position, int index);
		/// <summary>
		/// Gets height of list item in pixels.
		/// </summary>
		/// <param name="index">Zero-based index of array element.</param>
		/// <returns>
		/// Measurement in pixels.
		/// </returns>
		float GetItemHeight(int index);

	}

}