# Subscribing to item inserted and removing events

This example demonstrates how to subscribe to events by creating a local instance of the
list control. It is also necessary to instantiate the appropriate list adaptor which can
be cached alongside list control instance.
        
For this example we will subscribe to item added and removing events so that we can echo
these to the Unity console.
        
```csharp
using Rotorz.Games.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ExampleWindow : EditorWindow
{
    private ReorderableListControl listControl;
    private IReorderableListAdaptor listAdaptor;

    private List<string> someList = new List<string>();


    private void OnEnable()
	{
        // Create list control and optionally pass flags into constructor.
        this.listControl = new ReorderableListControl();

        // Subscribe to events for item insertion and removal.
        this.listControl.ItemInserted += this.OnItemInserted;
        this.listControl.ItemRemoving += this.OnItemRemoving;

        // Create adaptor for example list.
        this.listAdaptor = new GenericListAdaptor(this.someList);
    }

    private void OnDisable()
	{
        // Unsubscribe from events, good practice.
        if (this.listControl != null) {
            this.listControl.ItemInserted -= this.OnItemInserted;
            this.listControl.ItemRemoving -= this.OnItemRemoving;
        }
    }

    private void OnItemInserted(object sender, ItemInsertedEventArgs args)
	{
        string item = this.someList[args.ItemIndex];
        if (args.WasDuplicated) {
            Debug.Log("Duplicated: " + item);
		}
        else {
            Debug.Log("Inserted: " + item);
		}
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
	{
        string item = this.someList[args.ItemIndex];
        Debug.Log("Removing: " + item);

        // You can cancel item removal at this stage!
        if (item == "Keep Me!") {
            args.Cancel = true;
		}
    }

    private void OnGUI()
	{
        // Draw layout version of reorderable list control.
        this.listControl.Draw(this.listAdaptor);

        // OR
        
        // Draw absolute version of reorderable list control.
        Rect position = default(Rect);
        position.x = 100;
        position.y = 100;
        position.width = 200;
        position.height = this.listControl.CalculateListHeight(this.listAdaptor);
        this.listControl.Draw(position, this.listAdaptor);
    }
}
```
