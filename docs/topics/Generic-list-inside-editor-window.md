Items from generic lists can be presented using custom item drawers. A custom item drawer
is essentially a delegate which is called to draw each list item. The generic list adaptor
can be subclassed instead if items of varying heights are needed (see [Custom list adaptor](custom-list-adaptor.md)).

>
> **Tip** - Consider using [serialized properties]() instead if undo/redo support is needed.
>

```csharp
using Rotorz.Games.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenericListWindow : EditorWindow
{
    private List<string> nameList;


    private void OnEnable()
    {
        this.nameList = new List<string>();
    }

    private void OnGUI()
    {
        ReorderableListGUI.ListField(this.nameList, DrawListItem);
    }


    private string DrawListItem(Rect position, string value)
    {
        // Text fields do not like null values!
        if (value == null) {
            value = "";
        }
        return EditorGUI.TextField(position, value);
    }
}
```
