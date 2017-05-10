# Customize appearance of list field

Style of list container, add button and remove buttons can be customized by providing
custom styles. This example demonstrates a custom inspector with custom styles which are
based upon the default styles.


**Tip** - Another option is to subclass `ReorderableListControl` and initialise custom
styles there instead. A subclass can override other behaviour such as providing custom
context menu items.


```csharp
using Rotorz.Games.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SomeBehaviour))]
class SomeBehaviourEditor : Editor
{
    // Custom instance of reorderable list control.
    private ReorderableListControl listControl;
    // Serialized property adaptor for wishlist.
    private SerializedListAdaptor wishlistAdaptor;


    private void OnEnable()
    {
        // Prepare custom styles as needed.
        var style = new GUIStyle(ReorderableListStyles.Container);
        style.normal.background = AssetDatabase.LoadAssetAtPath("Assets/custom_container.png") as Texture2D;

        // Assign custom style to instance of list control.
        this.listControl = new ReorderableListControl();
        this.listControl.ContainerStyle = style;

        // Create adaptor for wishlist using serialized property.
        var wishlist = serializedObject.FindProperty("wishlist");
        this.wishlistAdaptor = new SerializedListAdaptor(wishlist);
    }

    public override void OnInspectorGUI()
    {
        this.listControl.Draw(this.wishlistAdaptor);
    }
}
```
