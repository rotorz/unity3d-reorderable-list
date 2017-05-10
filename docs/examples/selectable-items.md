# Item selection with a custom adaptor

Item selection can be added to a reorderable list control by creating a custom reorderable
list adaptor. Selection state can be efficiently represented using a hash set collection.

![](img/selectable-items.png)
        
```csharp
using Rotorz.Games.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItemAdaptor : GenericListAdaptor<string>
{
    private HashSet<int> selectedIndices = new HashSet<int>();


    public SelectableItemAdaptor(List<string> list, float itemHeight)
        : base(list, null, itemHeight)
    {
    }


    public override void DrawItemBackground(Rect position, int index)
    {
        if (this.selectedIndices.Contains(index)) {
            ReorderableListStyles.SelectedItem.Draw(position, GUIContent.none, false, false, false, false);
        }
    }

    public override void DrawItem(Rect position, int index)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.MouseDown:
                if (Event.current.button == 0 && position.Contains(Event.current.mousePosition)) {
                    if (Event.current.control) {
                        // Toggle selection of this item if control key is held.
                        if (this.selectedIndices.Contains(index)) {
                            this.selectedIndices.Remove(index);
                        }
                        else {
                            this.selectedIndices.Add(index);
                        }
                    }
                    else {
                        // Deselect all other items and select this one instead.
                        this.selectedIndices.Clear();
                        this.selectedIndices.Add(index);
                    }
                    Event.current.Use();
                }
                break;

            case EventType.Repaint:
                GUI.skin.label.Draw(position, this[index], false, false, false, false);
                break;
        }
    }
}
```
