# Customize context menu

This example demonstrates how to add to, or even replace entirely, the list field's
context menu.
        
First we need to implement a subclass of `ReorderableListControl` so that we can override
the default context menu and add some custom items.
        
```csharp
using Rotorz.Games.Collections;
using UnityEditor;

public class CustomContextMenuList : ReorderableListControl
{
    private static readonly GUIContent s_MenuItem1 = new GUIContent("MenuItem1");
    private static readonly GUIContent s_MenuItem2 = new GUIContent("MenuItem2");


    protected override void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor)
    {
        // Remove if default menu items are not wanted.
        base.AddItemsToMenu(menu, itemIndex, adaptor);

        menu.AddSeparator("");

        // Custom menu item the usual way:
        menu.AddItem(s_MenuItem1, false, () => Debug.Log("You selected menu item #1!"));
        // Or... implement as command:
        menu.AddItem(s_MenuItem2, false, defaultContextHandler, s_MenuItem2);
    }

    protected override bool HandleCommand(string commandName, int itemIndex,IReorderableListAdaptor adaptor)
    {
        // Remove if default commands are not wanted.
        if (base.HandleCommand(commandName, itemIndex, adaptor)) {
            return true;
        }

        // Place custom command handler here...
        switch (commandName) {
            case "MenuItem2":
                Debug.Log("You selected menu item #2!");
                return true;
        }

        return false;
    }

}
```

In order to use our custom reorderable list we will also need to instantiate an adaptor
for our list. We can cache this adaptor alongside our custom list control instance.

```csharp
private SerializedProperty someListProperty;

private CustomContextMenuList customListControl;
private IReorderableListAdaptor someListAdaptor;


private void OnEnable()
{
    this.someListProperty = this.serializedObject.FindProperty("someList");

    this.customListControl = new CustomContextMenuList();
    this.someListAdaptor = new SerializedPropertyAdaptor(someListProperty);
}

public override void OnInspectorGUI()
{
    this.serializedObject.Update();

    this.customListControl.Draw(this.someListAdaptor);

    this.serializedObject.ApplyModifiedProperties();
}
```
