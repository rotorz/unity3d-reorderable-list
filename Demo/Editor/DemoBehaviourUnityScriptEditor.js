// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
#pragma strict

import Rotorz.ReorderableList;

@CustomEditor(DemoBehaviourUnityScript)
class DemoBehaviourUnityScriptEditor extends Editor {

	var _wishlistProperty:SerializedProperty;
	var _pointsProperty:SerializedProperty;

	function OnEnable() {
		_wishlistProperty = serializedObject.FindProperty('wishlist');
		_pointsProperty = serializedObject.FindProperty('points');
	}

	function OnInspectorGUI() {
		serializedObject.Update();

		ReorderableListGUI.Title('Wishlist');
		ReorderableListGUI.ListField(_wishlistProperty);

		ReorderableListGUI.Title('Points');
		ReorderableListGUI.ListField(_pointsProperty);

		serializedObject.ApplyModifiedProperties();
	}

}