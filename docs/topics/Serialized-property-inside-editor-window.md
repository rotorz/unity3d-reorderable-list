The serialized property version of this field can also be used in editor windows. This
allows you to take advantage of the automatic undo and redo capabilities which Unity
provides.

In this example a reorderable list field is shown when an object containing the following
behaviour is selected:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class SomeBehaviour : MonoBehaviour
{
    public List<string> wishlist = new List<string>();
}
```

Before we can interact with the wishlist property we must create an instance of [SerializedObject].
This is done each time the user selection changes.
        
```csharp
using Rotorz.Games.Collections;
using UnityEditor;
using UnityEngine;

public class ArrayPropertyWindow : EditorWindow
{
    private SerializedObject serializedObject;
    private SerializedProperty wishlistProperty;


    private void OnEnable()
	{
        // Consider selection when window is first shown.
        this.OnSelectionChange();
    }

    private void OnSelectionChange()
	{
        // Get editable `SomeBehaviour` objects from selection.
        var filtered = Selection.GetFiltered(typeof(SomeBehaviour), SelectionMode.Editable);
        if (filtered.Length == 0) {
            this.serializedObject = null;
            this.wishlistProperty = null;
        }
        else {
            // Let's work with the first filtered result.
            this.serializedObject = new SerializedObject(filtered[0]);
            this.wishlistProperty = this.serializedObject.FindProperty("wishlist");
        }

        Repaint();
    }

    private void OnGUI()
	{
        if (this.serializedObject == null) {
            return;
		}
        this.serializedObject.Update();

        ReorderableListGUI.ListField(this.wishlistProperty);

        this.serializedObject.ApplyModifiedProperties();
    }
}
```



[SerializedObject]: http://docs.unity3d.com/Documentation/ScriptReference/SerializedObject.html
