// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.
#pragma strict

import Rotorz.ReorderableList;
import System.Collections.Generic;

class ReorderableListUnityScriptDemo extends EditorWindow {

	@MenuItem('Window/List Demo (UnityScript)')
	static function ShowWindow() {
		GetWindow.<ReorderableListUnityScriptDemo>('List Demo');
	}

	var shoppingList:List.<String>;
	var purchaseList:List.<String>;
	
	function OnEnable() {
		shoppingList = new List.<String>();
		shoppingList.Add('Bread');
		shoppingList.Add('Carrots');
		shoppingList.Add('Beans');
		shoppingList.Add('Steak');
		shoppingList.Add('Coffee');
		shoppingList.Add('Fries');

		purchaseList = new List.<String>();
		purchaseList.Add('Cheese');
		purchaseList.Add('Crackers');
	}

	var _scrollPosition:Vector2;

	function OnGUI() {
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		
		ReorderableListGUI.Title('Shopping List');
		ReorderableListGUI.ListField(shoppingList, PendingItemDrawer, DrawEmpty);

		ReorderableListGUI.Title('Purchased Items');
		ReorderableListGUI.ListField(purchaseList, PurchasedItemDrawer, DrawEmpty, ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableReordering);
	
		GUILayout.EndScrollView();
	}
	
	function PendingItemDrawer(position:Rect, itemValue:String):String {
		// Text fields do not like null values!
		if (itemValue == null)
			itemValue = '';
		
		position.width -= 50;
		itemValue = EditorGUI.TextField(position, itemValue);
		
		position.x = position.xMax + 5;
		position.width = 45;
		if (GUI.Button(position, 'Info')) {
		}
		
		return itemValue;
	}

	function PurchasedItemDrawer(position:Rect, itemValue:String):String {
		position.width -= 50;
		GUI.Label(position, itemValue);

		position.x = position.xMax + 5;
		position.width = 45;
		if (GUI.Button(position, 'Info')) {
		}

		return itemValue;
	}

	function DrawEmpty() {
		GUILayout.Label('No items in list.', EditorStyles.miniLabel);
	}
	
}