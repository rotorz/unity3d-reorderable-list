The reorderable list field watches for changes surrounding individual items allowing you
to pinpoint the item which was actually modified. Any changes made outside of list item
drawers are considered changes made by the list control itself.
        
The property [GUI.changed] is set to `true` if changes are made using the list control. If
a list item drawer reports changes then the property `ReorderableListGUI.indexOfChangedItem`
is set to the zero-based index of that item. A value of `-1` indicates that changes were
reported by the list control itself.

```csharp
// Begin checking for changes to `GUI.changed`.
EditorGUI.BeginChangeCheck();

ReorderableListGUI.ListField(wishlist);

// Were any changes made to the state of `GUI.changed`?
if (EditorGUI.EndChangeCheck()) {
    // Determine whether changes were made to a specific list item.
    if (ReorderableListGUI.IndexOfChangedItem != -1) {
        // We know the index of the item which was modified!
    }
    else {
        // Changes were made outside of an item drawer.
        // for example, an item was added, removed, moved, etc.
    }
}
```



[GUI.changed]: http://docs.unity3d.com/Documentation/ScriptReference/GUI-changed.html
