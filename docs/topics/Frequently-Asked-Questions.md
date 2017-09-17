Answers to frequently asked questions!


## What is this for?

For adding reorderable list controls to your custom editor interfaces with buttons for
adding and removing items.



## Where can I use list field controls?

They can be used in almost any editor interface:

 - Editor windows.
 - Custom inspectors.
 - Custom property drawers (see note below).
 - User preferences window.



## Can this library be used with C#?

Yes! The library is itself implemented in C#.



## Can this library be used with UnityScript?

Yes, absolutely!



## Can reordering be disabled?

Yes, in fact there are a number of flags which can be used to enable and disable things.
Refer to the `ReorderableListFlags` enumeration for available flags.



## Can list items have varying heights?

List items are presented via the `IReorderableListAdaptor` interface which allows list
items to have varying heights.

The serialized property version of this control supports this since custom property
drawers provide a provision for height calculation. Though a fixed item height can be
explicitly specified for improved drawing performance.

The provided `IList<T>` adaptor only supports fixed item heights. See API reference for
example implementation of a custom adaptor with custom height calculation.



## Can this list control work with other collection types?

Custom adaptors can be created by implementing the `IReorderableListAdaptor` interface.
Like mentioned above, list items can be of varying heights if desired.



## Is undo and redo supported?

The serialized property version of this control provides automatic support for undo and
redo. However, undo and redo support must be custom implemented if required for all other
versions of this control.



## Can the context menu be customized?

Yes, you can subclass the `ReorderableListControl` class and override the method
`AddItemsToMenu` to add or replace menu items.



## Can list items be selected?

Currently there isn't an included API for this, however in many cases it is possible to
implement some form of selection using custom item drawers.



## Can this library be included within asset store packages?

Yes, but change the namespace from `Rotorz.Games` to something of your own.



## What is a custom inspector?

Custom inspectors can be implemented for behaviour scripts and scriptable objects by
subclassing the [Editor] class.



## What is a property drawer?

Custom controls can be presented for properties by subclassing the [PropertyDrawer] class.
Property drawers can be associated by value type or with a custom attribute. The latter
can be used to feed additional parameters to the property drawer.




[Editor]: http://docs.unity3d.com/Documentation/ScriptReference/Editor.html
[PropertyDrawer]: http://docs.unity3d.com/Documentation/ScriptReference/PropertyDrawer.html
