using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    public class UIStyleStruct
    {
        public enum Trigger { Idle, Hover, Pressed, Disabled, Custom };

        [System.Serializable]
        public class StateStruct {
            public Trigger state;
            public string id; // Only used in custom state
            public List<StyleComposition> compositions;

            public bool IsValid => compositions == null;

            public void SetState(Trigger p_trigger) {
                if (p_trigger >= 0 && (int)p_trigger < UIStyleStatic.States.Length) {
                    state = p_trigger;
                }
            }

            public StateStruct() {
                this.state = Trigger.Custom;
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
            public int size;

            //Image / Raw Image
            public Sprite sprite;
        }
        #endregion

    }
}