README
======

This repository contains a class which allows editor developers to easily add reorderable
lists to their GUIs.

Use of this source code is governed by a BSD-style license that can be found in
the LICENSE file.

Installing scripts
------------------

Scripts must be placed within an "Editor" folder somewhere within your "Assets"
folder.

Usage Example
-------------

![screenshot](https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity/raw/master/screenshot.png)

**C#:**

    :::csharp
    List<string> yourList = new List<string>();
    
    void OnGUI() {
        ReorderableListGUI.ListField(yourList, CustomListItem, DrawEmpty);
    }
	
	string CustomListItem(Rect position, string itemValue) {
		// Text fields do not like null values!
		if (itemValue == null)
			itemValue = "";
		return EditorGUI.TextField(position, itemValue);
	}
	
	void DrawEmpty() {
		GUILayout.Label("No items in list.", EditorStyles.miniLabel);
	}

**UnityScript:**

    :::javascript
    var yourList:List.<String> = new List.<String>();
    
    function OnGUI() {
        ReorderableListGUI.ListField(yourList, CustomListItem, DrawEmpty);
    }
	
	function CustomListItem(position:Rect, itemValue:String):String {
		// Text fields do not like null values!
		if (itemValue == null)
			itemValue = '';
		return EditorGUI.TextField(position, itemValue);
	}
	
	function DrawEmpty() {
		GUILayout.Label('No items in list.', EditorStyles.miniLabel);
	}

Useful links
------------

- [Rotorz Website](<http://rotorz.com>)

Contribution Agreement
----------------------

This project is licensed under the BSD license (see LICENSE). To be in the best
position to enforce these licenses the copyright status of this project needs to
be as simple as possible. To achieve this the following terms and conditions
must be met:

- All contributed content (including but not limited to source code, text,
  image, videos, bug reports, suggestions, ideas, etc.) must be the
  contributors own work.

- The contributor disclaims all copyright and accepts that their contributed
  content will be released to the [public domain](<http://en.wikipedia.org/wiki/Public_domain>).

- The act of submitting a contribution indicates that the contributor agrees
  with this agreement. This includes (but is not limited to) pull requests, issues,
  tickets, e-mails, newsgroups, blogs, forums, etc.

### Disclaimer

External content linked in the above text are for convienence purposes only and
do not contribute to the agreement in any way. Linked content should be digested
under the readers discretion.
