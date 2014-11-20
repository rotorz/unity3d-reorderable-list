// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.
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