// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace Rotorz.Games.Collections
{
    internal sealed class GenericElementAdderMenu : IElementAdderMenu
    {
        private readonly GenericMenu innerMenu = new GenericMenu();


        public GenericElementAdderMenu()
        {
        }


        public void AddItem(GUIContent content, GenericMenu.MenuFunction handler)
        {
            this.innerMenu.AddItem(content, false, handler);
        }

        public void AddDisabledItem(GUIContent content)
        {
            this.innerMenu.AddDisabledItem(content);
        }

        public void AddSeparator(string path = "")
        {
            this.innerMenu.AddSeparator(path);
        }

        public bool IsEmpty {
            get { return this.innerMenu.GetItemCount() == 0; }
        }

        public void DropDown(Rect position)
        {
            this.innerMenu.DropDown(position);
        }
    }
}
