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

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static UIStyleStruct.StyleComposition AssignDefaultPropertyToGraphicsObject(UnityEngine.UI.Graphic g_target, UIStyleStruct.StyleComposition styleComp)
        {
            styleComp.styles.color = g_target.color;
            var type = g_target.GetType();

            //Text
            if (type == typeof(UnityEngine.UI.Text))
            {
                styleComp.styles.size = ((UnityEngine.UI.Text)g_target).fontSize;
                styleComp.styles.font = ((UnityEngine.UI.Text)g_target).font;
            }

            if (type == typeof(TMPro.TextMeshProUGUI))
            {
                styleComp.styles.size = (int)((TMPro.TextMeshProUGUI)g_target).fontSize;
                styleComp.styles.font_asset = ((TMPro.TextMeshProUGUI)g_target).font;
            }

            //Image
            if (type == typeof(UnityEngine.UI.Image) || type == typeof(UnityEngine.UI.RawImage))
            {
                styleComp.styles.scale = EditorGUILayout.FloatField("Texture Scale", styleComp.styles.scale);
                styleComp.styles.sprite = (UnityEngine.Sprite)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.sprite, typeof(UnityEngine.Sprite), allowSceneObjects: true);
            }

            return styleComp;
        }


        public static void CreateDefaultStateLayout(UIStylesheet uiStyleStruct, int styleIndex)
        {
            if (uiStyleStruct.targetGraphic == null)
            {
                uiStyleStruct.m_char_list[styleIndex].stateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Idle });
                //uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Hover });
                //uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Pressed });
                //uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Disabled });
                return;
            }

            uiStyleStruct.m_char_list[styleIndex].stateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Idle, UIStyleStatic.ColorTable.IdleColor));
            //uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Hover, UIStyleStatic.ColorTable.HoverColor));
            //uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Pressed, UIStyleStatic.ColorTable.PressedColor));
            //uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Disabled, UIStyleStatic.ColorTable.DisableColor));
        }

        public static string[] CreateStyleLabelArray(int style_count) {
            string toolbarStringPrefix = "Style {0}";
            string[] labels = new string[style_count];

            for (int i = 0; i < style_count; i++) {
                labels[i] = string.Format(toolbarStringPrefix, i+1);
            }

            return labels;
        }

    }
}