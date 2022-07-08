using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    public class UIStyleStruct
    {
        public enum Trigger { Idle, Hover, Pressed, Disabled, None };

        public struct TriggerStruct {
            public Trigger trigger;
            public Trigger fallback;
        }

        public readonly static Dictionary<Trigger, TriggerStruct> TRIGGER_TABLE = new System.Collections.Generic.Dictionary<Trigger, TriggerStruct>() {
            {Trigger.Disabled, new TriggerStruct() {trigger = Trigger.Disabled, fallback = Trigger.Idle } },
            {Trigger.Pressed, new TriggerStruct() {trigger = Trigger.Pressed, fallback = Trigger.Hover } },
            {Trigger.Hover, new TriggerStruct() {trigger = Trigger.Hover, fallback = Trigger.Idle } },
            {Trigger.Idle, new TriggerStruct() {trigger = Trigger.Idle, fallback = Trigger.None } },
        }; 

        [System.Serializable]
        public class StateStruct {
            public Trigger state;
            public List<StyleComposition> compositions;

            public bool IsValid => compositions == null;

            public void SetState(Trigger p_trigger) {
                if (p_trigger >= 0 && (int)p_trigger < UIStyleStatic.States.Length) {
                    state = p_trigger;
                }
            }

            public StateStruct() {
                this.state = Trigger.Idle;
                this.compositions = new List<StyleComposition>();
            }

            public static StateStruct SetDefaultComposition(UnityEngine.UI.Graphic target, Trigger trigger, Color color) {
                if (target == null) return null;

                StateStruct state = new StateStruct() { state = trigger };

                state.compositions = new List<StyleComposition>();

                StyleComposition newStyleComposition = new StyleComposition() { target = target };
                newStyleComposition.styles.color = color;

                state.compositions.Add(newStyleComposition);

                return state;
            }
        }

        [System.Serializable]
        public class StyleComposition
        {
            public UnityEngine.UI.Graphic target;

            public StyleStruct styles;

            public Color OriginalColor { get {
                    if (_originalColor == Color.clear && target != null)
                        _originalColor = target.color;

                    return _originalColor;
                } 
            }
            private Color _originalColor = Color.clear;

            public bool is_expanded; // Accordion effect Editor UI

            public StyleComposition() {
                styles = new StyleStruct();
                styles.color = Color.white;
            }
        }

        #region Entity Style Struct
        [System.Serializable]
        public class StyleStruct {
            public Color color = Color.white;
            public float scale = 1;
            public float rotation = 0;

            //Text / TMPro Material
            public Font font;
            public TMPro.TMP_FontAsset font_asset;

            public int size;

            //Image / Raw Image
            public Sprite sprite;
        }

        [System.Serializable]
        public class Characteristics {
            public List<UIStyleStruct.StateStruct> stateStructs = new List<StateStruct>();
        }
        #endregion

    }
}