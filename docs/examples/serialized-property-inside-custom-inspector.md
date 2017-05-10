# Serialized property inside custom inspector

Reorderable list fields can be added to custom inspector interfaces with automatic support
for undo/redo when using serialized properties. Serialized properties support native
arrays as well as generic lists.
        
In this example we will implement an editor for the following behaviour class:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class SomeBehaviour : MonoBehaviour
{
    public List<string> wishlist = new List<string>();
}
```

Custom inspectors can be implemented by extending the [Editor Class](http://docs.unity3d.com/Documentation/ScriptReference/Editor.html).
The serialized property for our "wishlist" field can then be accessed via the serialize
object representation of `SomeBehaviour`. We can override the method [OnInspectorGUI](http://docs.unity3d.com/Documentation/ScriptReference/Editor.OnInspectorGUI.html)
to present the reorderable list.

```csharp
using Rotorz.Games.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SomeBehaviour))]
public class SomeBehaviourEditor : Editor
{
    private SerializedProperty wishlistProperty;


    private void OnEnable()
	{
        this.wishlistProperty = this.serializedObject.FindProperty("wishlist");
    }

    public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

        ReorderableListGUI.ListField(this.wishlistProperty);

        this.serializedObject.ApplyModifiedProperties();
    }
}
```
