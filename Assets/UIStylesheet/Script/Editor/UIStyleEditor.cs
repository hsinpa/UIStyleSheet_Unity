using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Hsinpa.UIStyle
{
    [CustomEditor(typeof(UIStylesheet))]
    public class UIStyleEditor : Editor
    {
        UIStylesheet uiStyleStruct;
        SerializedProperty stateStruct_property;

        SerializedProperty interactable_property;
        SerializedProperty targetGraphic_property;


        Texture _expandTex;
        Texture _closedTex;
        Color _alertColor;

        void OnEnable()
        {
            uiStyleStruct = (UIStylesheet) target;
            uiStyleStruct.transition = UnityEngine.UI.Selectable.Transition.None;

            stateStruct_property = serializedObject.FindProperty("_stateStructs");
            interactable_property = serializedObject.FindProperty("m_Interactable");
            targetGraphic_property = serializedObject.FindProperty("m_TargetGraphic");

            this._expandTex = (Texture)Resources.Load("Texture/sort-down");
            this._closedTex = (Texture)Resources.Load("Texture/sort-up");
            this._alertColor = new Color32(231, 76, 60, 255);

            Debug.Log(this._closedTex.name);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(interactable_property);
            EditorGUILayout.PropertyField(targetGraphic_property);

            if (uiStyleStruct.targetGraphic != null) {

                if (uiStyleStruct.StateStructs.Count == 0)
                    CreateDefaultStateLayout();

                CreateStateGUILayout();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateStateGUILayout() {
            Rect outterLayout = EditorGUILayout.BeginVertical();


            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("States", new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 14 });

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct());
            }

            EditorGUILayout.EndHorizontal();

            CreateStatesGUILayout(outterLayout, uiStyleStruct.StateStructs);

            GUILayout.Space(20);

            EditorGUILayout.EndVertical();
        }


        private void CreateStatesGUILayout(Rect rect, List<UIStyleStruct.StateStruct> stateStructs)
        {

            for (int i = 0; i < stateStructs.Count; i++)
            {

                DrawUILine(Color.gray, thickness: 1);
                int styleIndex = i;
                Rect composition_layout = EditorGUILayout.BeginVertical();

                var stateStruct = stateStructs[i];
                int d  = EditorGUILayout.Popup(UIStyleStatic.States[(int)stateStruct.state],
                                                (int)stateStruct.state,
                                                UIStyleStatic.States);
                stateStruct.SetState((UIStyleStruct.Trigger) d);

                if (d == (int)UIStyle.UIStyleStruct.Trigger.Custom) {
                    stateStruct.id = EditorGUILayout.TextField("ID", stateStruct.id);
                }

                CreateComposition(stateStruct.compositions);


                Rect footer_gui = EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Append Composition"))
                {
                    OnCompositionAppend(styleIndex);
                }

                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = this._alertColor;
                GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
                guiStyle.normal.textColor = Color.white;
                guiStyle.fixedWidth = 20;
                if (GUILayout.Button("-", guiStyle))
                {
                    stateStructs.RemoveAt(styleIndex);
                }
                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();

                //UIStyleEditorUtility.DrawDropdown(composition_layout, "Select Composition", UIStyleStatic.Compositions, (int x) => {
                //    OnCompositionAppend(styleIndex, x);
                //});

                //stateStructs[i] = stateStruct;
                EditorGUILayout.EndVertical();
            }

        }

        private void CreateComposition(List<UIStyleStruct.StyleComposition> compositionStructs) {
            if (compositionStructs == null) return;
            int compLens = compositionStructs.Count;



            for (int i = 0; i < compLens; i++) {
                int compositeIndex = i;
                UIStyleStruct.StyleComposition styleComp = compositionStructs[i];


                EditorGUILayout.BeginHorizontal();
                var type = typeof(UnityEngine.UI.Graphic);

                if (GUILayout.Button((styleComp.is_expanded) ? this._closedTex : this._expandTex,
                    GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    styleComp.is_expanded = !styleComp.is_expanded;
                }

                UnityEngine.Object mobject = (UnityEngine.Object) styleComp.target;
                styleComp.target =(UnityEngine.UI.Graphic)EditorGUILayout.ObjectField(mobject, type);

                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                {
                    compositionStructs.RemoveAt(compositeIndex);
                }

                EditorGUILayout.EndHorizontal();

                if (styleComp.is_expanded) {
                    EditorGUILayout.BeginVertical();
                    DecorateComposition(styleComp);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void DecorateComposition(UIStyleStruct.StyleComposition styleComp) {
            if (styleComp.target == null) return;

            System.Type type = styleComp.target.GetType();

            styleComp.styles.color = EditorGUILayout.ColorField("Tint", styleComp.styles.color);
            styleComp.styles.rotation = EditorGUILayout.FloatField("Rotation", styleComp.styles.rotation);

            //Text
            if (type == typeof(UnityEngine.UI.Text) || type == typeof(TMPro.TextMeshProUGUI)) {
                styleComp.styles.size = EditorGUILayout.IntField("Text Size", styleComp.styles.size);
                styleComp.styles.font = (UnityEngine.Font)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.font, typeof(UnityEngine.Font));
            }

            //Image
            if (type == typeof(UnityEngine.UI.Image) || type == typeof(UnityEngine.UI.RawImage))
            {
                styleComp.styles.scale = EditorGUILayout.FloatField("Texture Scale", styleComp.styles.scale);
                styleComp.styles.sprite = (UnityEngine.Sprite)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.sprite, typeof(UnityEngine.Sprite));
            }

        }

        private void CreateDefaultStateLayout() {
            if (uiStyleStruct.targetGraphic == null)
            {
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Idle });
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Hover });
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Pressed });
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct() { state = UIStyleStruct.Trigger.Disabled });
                return;
            }

            uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Idle, UIStyleStatic.ColorTable.IdleColor));
            uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Hover, UIStyleStatic.ColorTable.HoverColor));
            uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Pressed, UIStyleStatic.ColorTable.PressedColor));
            uiStyleStruct.StateStructs.Add(UIStyleStruct.StateStruct.SetDefaultComposition(uiStyleStruct.targetGraphic, UIStyleStruct.Trigger.Disabled, UIStyleStatic.ColorTable.DisableColor));
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

        private void OnCompositionAppend(int styleStructIndex) {
            var stateStruct = uiStyleStruct.StateStructs[styleStructIndex];

            stateStruct.compositions.Add(new UIStyleStruct.StyleComposition());

            //uiStyleStruct.StateStructs[styleStructIndex] = stateStruct;
        }



    }
}