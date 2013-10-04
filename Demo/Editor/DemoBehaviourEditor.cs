// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEditor;

using Rotorz.ReorderableList;

[CustomEditor(typeof(DemoBehaviour))]
public class DemoBehaviourEditor : Editor {

	private SerializedProperty _wishlistProperty;
	private SerializedProperty _pointsProperty;

	private void OnEnable() {
		_wishlistProperty = serializedObject.FindProperty("wishlist");
		_pointsProperty = serializedObject.FindProperty("points");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		ReorderableListGUI.Title("Wishlist");
		ReorderableListGUI.ListField(_wishlistProperty);

		ReorderableListGUI.Title("Points");
		ReorderableListGUI.ListField(_pointsProperty);

		serializedObject.ApplyModifiedProperties();
	}

}