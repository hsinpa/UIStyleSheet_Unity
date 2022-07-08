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
        SerializedProperty uiStylesheetSRP_property;
        SerializedProperty styleLength_property;
        SerializedProperty styleLists_property;


        Texture _expandTex;
        Texture _closedTex;
        Color _alertColor;

        string[] toolbarArray = new string[0];
        List<UIStyleStruct.StateStruct> CurrentStateStructList => uiStyleStruct.m_char_list[style_index].stateStructs;
        int style_index = 0;

        void OnEnable()
        {
            uiStyleStruct = (UIStylesheet) target;
            uiStyleStruct.transition = UnityEngine.UI.Selectable.Transition.None;

            stateStruct_property = serializedObject.FindProperty("_stateStructs");
            interactable_property = serializedObject.FindProperty("m_Interactable");
            targetGraphic_property = serializedObject.FindProperty("m_TargetGraphic");
            uiStylesheetSRP_property = serializedObject.FindProperty("m_uiStylesheetSRP");
            styleLength_property = serializedObject.FindProperty("m_styleLength");
            styleLists_property = serializedObject.FindProperty("m_styleLists");

            this._expandTex = (Texture)Resources.Load("Texture/sort-down");
            this._closedTex = (Texture)Resources.Load("Texture/sort-up");
            this._alertColor = new Color32(231, 76, 60, 255);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(interactable_property);
            EditorGUILayout.PropertyField(targetGraphic_property);
            EditorGUILayout.PropertyField(uiStylesheetSRP_property);
            EditorGUILayout.PropertyField(styleLength_property);

            if (uiStyleStruct.targetGraphic != null) {

                if (toolbarArray.Length != uiStyleStruct.StyleLength)
                    toolbarArray = UIStyleEditorUtility.CreateStyleLabelArray(uiStyleStruct.StyleLength);

                while (uiStyleStruct.m_char_list.Count > uiStyleStruct.StyleLength) {
                    uiStyleStruct.m_char_list.RemoveAt(uiStyleStruct.m_char_list.Count - 1);
                }

                while (uiStyleStruct.m_char_list.Count < uiStyleStruct.StyleLength)
                {
                    uiStyleStruct.m_char_list.Add(new UIStyleStruct.Characteristics());
                    UIStyleEditorUtility.CreateDefaultStateLayout(uiStyleStruct, uiStyleStruct.m_char_list.Count - 1);
                }

                style_index = GUILayout.Toolbar(style_index, toolbarArray);
                style_index = Mathf.Clamp(style_index, 0, uiStyleStruct.StyleLength-1);
                CreateStateGUILayout();

            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(uiStyleStruct);
        }

        /// <summary>
        /// Create new State
        /// </summary>
        private void CreateStateGUILayout() {
            Rect outterLayout = EditorGUILayout.BeginVertical();


            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("States", new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 12 });

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                CurrentStateStructList.Add(new UIStyleStruct.StateStruct());
            }

            EditorGUILayout.EndHorizontal();

            CreateStatesGUILayout(outterLayout, CurrentStateStructList);

            GUILayout.Space(20);

            EditorGUILayout.EndVertical();
        }

        //Create Composition or remove selected state
        private void CreateStatesGUILayout(Rect rect, List<UIStyleStruct.StateStruct> stateStructs)
        {
            for (int i = 0; i < stateStructs.Count; i++)
            {
                UIStyleEditorUtility.DrawUILine(Color.gray, thickness: 1);
                int styleIndex = i;
                Rect composition_layout = EditorGUILayout.BeginVertical();

                var stateStruct = stateStructs[i];
                int d  = EditorGUILayout.Popup(UIStyleStatic.States[(int)stateStruct.state],
                                                (int)stateStruct.state,
                                                UIStyleStatic.States);
                stateStruct.SetState((UIStyleStruct.Trigger) d);

                CreateComposition(stateStruct.compositions);

                Rect footer_gui = EditorGUILayout.BeginHorizontal();
                
                    if (GUILayout.Button("Append Composition"))
                    {
                        OnCompositionAppend(styleIndex);
                    }

                    //make button red and text white
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
                styleComp.target =(UnityEngine.UI.Graphic)EditorGUILayout.ObjectField(mobject, type, allowSceneObjects: true);

                //Assign value to new assign ui graphics
                if (mobject == null && styleComp.target != null) {
                    UIStyleEditorUtility.AssignDefaultPropertyToGraphicsObject(styleComp.target, styleComp);
                }

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

                if (type == typeof(UnityEngine.UI.Text))
                    styleComp.styles.font = (UnityEngine.Font)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.font, typeof(UnityEngine.Font), allowSceneObjects: true);

                if (type == typeof(TMPro.TextMeshProUGUI))
                    styleComp.styles.font_asset = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.font_asset, typeof(TMPro.TMP_FontAsset), allowSceneObjects: true);
            }

            //Image
            if (type == typeof(UnityEngine.UI.Image) || type == typeof(UnityEngine.UI.RawImage))
            {
                styleComp.styles.scale = EditorGUILayout.FloatField("Texture Scale", styleComp.styles.scale);
                styleComp.styles.sprite = (UnityEngine.Sprite)EditorGUILayout.ObjectField((UnityEngine.Object)styleComp.styles.sprite, typeof(UnityEngine.Sprite), allowSceneObjects: true);
            }

        }

        private void OnCompositionAppend(int styleStructIndex) {
            var stateStruct = CurrentStateStructList[styleStructIndex];

            stateStruct.compositions.Add(new UIStyleStruct.StyleComposition());

            //uiStyleStruct.StateStructs[styleStructIndex] = stateStruct;
        }
    }
}