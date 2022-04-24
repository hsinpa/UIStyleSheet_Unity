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

        void OnEnable()
        {
            uiStyleStruct = (UIStylesheet) target;

            stateStruct_property = serializedObject.FindProperty("_stateStructs");
            interactable_property = serializedObject.FindProperty("m_Interactable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(interactable_property);
            CreateStateGUILayout();
            //EditorGUILayout.PropertyField(stateStruct_property);



            serializedObject.ApplyModifiedProperties();
        }

        private void CreateStateGUILayout() {
            Rect outterLayout = EditorGUILayout.BeginVertical();
            GUILayout.Label("States");

            CreateStatesGUILayout(outterLayout, uiStyleStruct.StateStructs);

            GUILayout.Space(20);

            // Add / Minus bar
            Rect states_ctrlbar = EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(20))) {
                uiStyleStruct.StateStructs.Add(new UIStyleStruct.StateStruct());
                Debug.Log("+");
            }

            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20))) {
                if (uiStyleStruct.StateStructs.Count > 0) uiStyleStruct.StateStructs.RemoveAt(uiStyleStruct.StateStructs.Count - 1);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }


        private void CreateStatesGUILayout(Rect rect, List<UIStyleStruct.StateStruct> stateStructs)
        {
            int stateLens = stateStructs.Count;

            for (int i = 0; i < stateLens; i++)
            {
                int styleIndex = i;
                Rect composition_layout = EditorGUILayout.BeginVertical();

                var stateStruct = stateStructs[i];
                int d  = EditorGUILayout.Popup(UIStyleStatic.States[stateStruct.state], stateStruct.state, UIStyleStatic.States);
                stateStruct.SetState(d);

                CreateComposition(stateStruct.compositions);

                if (GUILayout.Button("Append Composition"))
                {
                    OnCompositionAppend(styleIndex);
                }

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
                UnityEngine.Object mobject = (UnityEngine.Object) styleComp.target;
                styleComp.target =(UnityEngine.UI.Graphic)EditorGUILayout.ObjectField(mobject, type);

                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                {
                    compositionStructs.RemoveAt(compositeIndex);
                }

                EditorGUILayout.EndHorizontal();

                DecorateComposition(styleComp);

            }
        }

        private void DecorateComposition(UIStyleStruct.StyleComposition styleComp) {
            if (styleComp.target == null) return;

            System.Type type = styleComp.target.GetType();

            styleComp.styles.color = EditorGUILayout.ColorField("Tint", styleComp.styles.color);

            if (type == typeof(UnityEngine.UI.Text)) {
                styleComp.styles.size = EditorGUILayout.IntField("Text Size", styleComp.styles.size);
                styleComp.styles.font = (UnityEngine.Font)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.font, typeof(UnityEngine.Font));
            }

            if (type == typeof(UnityEngine.UI.Image))
            {
                styleComp.styles.scale = EditorGUILayout.FloatField("Texture Size", styleComp.styles.scale);
                styleComp.styles.sprite = (UnityEngine.Sprite)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.sprite, typeof(UnityEngine.Sprite));
            }

        }

        private void OnCompositionAppend(int styleStructIndex) {
            var stateStruct = uiStyleStruct.StateStructs[styleStructIndex];

            stateStruct.compositions.Add(new UIStyleStruct.StyleComposition());

            //uiStyleStruct.StateStructs[styleStructIndex] = stateStruct;
        }

    }
}