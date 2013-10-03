// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

public class ReorderableListTest : EditorWindow {

	[MenuItem("Window/Dynamic List Test")]
	static void ShowWindow() {
		GetWindow<ReorderableListTest>("Dynamic List Test");
	}

	private List<string> _list;
	private List<string> _list2;
	
	#region Message
	
	private void OnEnable() {
		_list = new List<string>();
		_list.Add("Test 1");
		_list.Add("Test 2");
		_list.Add("Test 3");
		_list.Add("Test 4");
		_list.Add("Test 5");
		_list.Add("Test 6");

		_list2 = new List<string>();
		_list2.Add("Test 7");
		_list2.Add("Test 8");
	}

	private Vector2 _scrollPosition;

	private void OnGUI() {
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		{
			ReorderableListGUI.Title("Test List #1");
			ReorderableListGUI.ListField(_list, CustomListFieldItem, DrawEmpty, 42);
			ReorderableListGUI.ListField(_list2, CustomListFieldItem, DrawEmpty, 22, ReorderableListFlag.HideAddButton | ReorderableListFlag.DisableReordering);
			ReorderableListGUI.ListField(_list2, CustomListFieldItem, DrawEmpty, 22);
		}
		GUILayout.EndScrollView();
	}
	
	private string CustomListFieldItem(Rect position, string itemValue) {
		// Text fields do not like null values!
		if (itemValue == null)
			itemValue = "";
		
		position.width -= 30;
		itemValue = EditorGUI.TextField(position, itemValue);
		
		position.x = position.xMax + 5;
		position.width = 25;
		if (GUI.Button(position, "?")) {
		}
		
		return itemValue;
	}

	private void DrawEmpty() {
		GUILayout.Label("No items in list.", EditorStyles.miniLabel);
	}
	
	#endregion
	
}