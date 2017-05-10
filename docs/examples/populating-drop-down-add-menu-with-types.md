# Populating the drop-down add menu with types

Utility functionality is provided to assist when building menus that contain a number of
addable element types that implement some interface or base type; this is referred to as
the element contract type.

Let's begin by defining the base class that each of our elements must be derived from:

```csharp
// ExampleNode.cs
using UnityEngine;

public abstract class ExampleNode : ScriptableObject
{
    [SerializeField]
    private string displayName;


    public string DisplayName {
        get { return this.displayName; }
        set { this.displayName = value; }
    }
}
```

We then need some sort of container wherein our node instances will be stored. In the case
of this example this will be another custom `ScriptableObject` implementation; although
the same principle can also be applied to a collection type.

```csharp
// ExampleGraph.cs
using System.Collections.Generic;
using UnityEngine;

public abstract class ExampleGraph : ScriptableObject
{
    [SerializeField]
    private List<ExampleNode> nodes = new List<ExampleNode>();


    public void AddNode(ExampleNode node)
    {
        this.nodes.Add(node);
    }
}
```

We will also need to implement at least one type of node so that the add menu will contain
at least one type to select from!

```csharp
// NodeTypeA.cs
public class NodeTypeA : ExampleNode
{
}

// NodeTypeB.cs
public class NodeTypeB : ExampleNode
{
}
```

An `IElementAdder<TContext>` implementation is also needed since this defines how nodes
are to be created    and how they are to be associated with their context object. In the
case of    this example the context object will be an instance of **ExampleGraph**.

```csharp
using Rotorz.Games.Collections;
using System;
using UnityEngine;

public class ExampleNodeElementAdder : IElementAdder<ExampleGraph>
{
    public ExampleNodeElementAdder(ExampleGraph graph)
    {
        this.Object = graph;
    }


    public ExampleGraph Object { get; private set; }


    public bool CanAddElement(Type type)
    {
        return true;
    }

    public object AddElement(Type type)
    {
        var node = (ExampleNode)ScriptableObject.CreateInstance(type);
        this.Object.AddNode(node);
        return node;
    }
}
```

The drop-down add menu can then be defined like shown below where we make use of an adder
element menu builder to populate the menu with the relevant element types:

```csharp
using Rotorz.Games.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExampleGraph))]
public class ExampleGraphEditor : Editor
{
    private ReorderableListControl listControl;
    private IReorderableListAdaptor listAdaptor;


    private void OnEnable()
    {
        // Create list control and pass flag into constructor so that the
        // regular add button is not displayed.
        this.listControl = new ReorderableListControl(ReorderableListFlags.HideAddButton);

        // Subscribe to event for when add menu button is clicked which will
        // also indicate that the add menu button is to be presented.
        this.listControl.AddMenuClicked += this.OnAddMenuClicked;

        // Create adaptor for example list.
        var nodesProperty = serializedObject.FindProperty("nodes");
        this.listAdaptor = new SerializedPropertyAdaptor(nodesProperty);
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
        var graph = target as ExampleGraph;
        var elementAdder = new ExampleNodeElementAdder(graph);

        var builder = ElementAdderMenuBuilder.For<ExampleGraph>(typeof(ExampleNode));
        builder.SetElementAdder(elementAdder);

        var menu = builder.GetMenu();
        menu.DropDown(args.ButtonPosition);
    }

    private void OnGUI()
    {
        // Draw layout version of reorderable list control.
        this.listControl.Draw(this.listAdaptor);
    }
}
```

With this approach custom commands can also be included by adding them directly using the
menu builder.

Adder menus can also be extended with custom commands without needing to directly interact
with the menu builder. This can be achieved by annotating custom command implementations
with an attribute which defines the context in which the command will be included:

```csharp
using Rotorz.Games.Collections;
using UnityEngine;

[ElementAdderMenuCommand(typeof(ExampleNode))]
public class SpecialCommand : IElementAdderMenuCommand<ExampleGraph>
{
    public SpecialCommand()
    {
        this.Content = new GUIContent("Special Command");
    }


    public GUIContent Content { get; private set; }


    public bool CanExecute(IElementAdder<ExampleGraph> elementAdder)
    {
        return true;
    }

    public void Execute(IElementAdder<ExampleGraph> elementAdder)
    {
        // Execute some custom command here!
        // Such as bulk adding nodes!
    }
}
```
