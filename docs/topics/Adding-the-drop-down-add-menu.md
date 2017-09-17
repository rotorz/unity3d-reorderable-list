The drop-down add menu button is automatically displayed when there is at least one
subscriber to the `ReorderableListControl.AddMenuClicked` event. The presentation of the
button varies depending upon whether the regular add button is also shown.

![](../img/drop-down-add-menu.png)

The drop-down add button is a general purpose button with no default behavior which can be
used to display a menu or a drop-down window.

In this example a simple menu is constructed and shown upon clicking the add menu button:

```csharp
using Rotorz.Games.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExampleWindow : EditorWindow
{
    private ReorderableListControl listControl;
    private IReorderableListAdaptor listAdaptor;

    private List<string> someList = new List<string>();


    private void OnEnable()
    {
        // Create list control and pass flag into constructor so that the
        // regular add button is not displayed.
        this.listControl = new ReorderableListControl(ReorderableListFlags.HideAddButton);

        // Subscribe to event for when add menu button is clicked which will
        // also indicate that the add menu button is to be presented.
        this.listControl.AddMenuClicked += OnAddMenuClicked;

        // Create adaptor for example list.
        this.listAdaptor = new GenericListAdaptor<string>(this.someList);
    }

    private void OnDisable()
    {
        // Unsubscribe from event, good practice.
        if (this.listControl != null) {
            this.listControl.AddMenuClicked -= OnAddMenuClicked;
        }
    }

    private void OnAddMenuClicked(object sender, AddMenuClickedEventArgs args)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Tree"), false, OnSelectAddMenuItem, "Tree");
        menu.AddItem(new GUIContent("Bush"), false, OnSelectAddMenuItem, "Bush");
        menu.AddItem(new GUIContent("Grass"), false, OnSelectAddMenuItem, "Grass");
        menu.DropDown(args.ButtonPosition);
    }

    private void OnSelectAddMenuItem(object userData)
    {
        Debug.Log(userData);
    }

    private void OnGUI()
    {
        // Draw layout version of reorderable list control.
        this.listControl.Draw(this.listAdaptor);
    }
}
```
