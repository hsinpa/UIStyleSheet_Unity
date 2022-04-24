using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Hsinpa.UIStyle
{
    public class UIStyleEditorUtility
    {

        public static void DrawDropdown<T>(Rect position, string label, T[] options, System.Action<int> callback)
        {
            if (!EditorGUILayout.DropdownButton(new GUIContent(label), FocusType.Passive))
            {
                return;
            }

            void handleItemClicked(object parameter)
            {
                callback((int)parameter);
            }

            GenericMenu menu = new GenericMenu();

            int optionLength = options.Length;

            for (int i = 0; i < optionLength; i++)
            {
                menu.AddItem(new GUIContent(options[i].ToString()), false, handleItemClicked, i);
            }

            menu.DropDown(position);
        }

    }
}