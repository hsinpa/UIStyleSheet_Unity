using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    public class UIStyleStruct
    {
        public enum Trigger { Idle, Enter, Leave, Selected, Disabled };

        [System.Serializable]
        public class StateStruct {
            public int state;
            public List<StyleComposition> compositions;

            public void SetState(int index) {
                if (index >= 0 && index < UIStyleStatic.States.Length) {
                    state = index;
                }
            }

            public StateStruct() {
                this.state = (int)Trigger.Idle;
                this.compositions = new List<StyleComposition>();
            }
        }

        [System.Serializable]
        public class StyleComposition
        {
            public UnityEngine.UI.Graphic target;

            public StyleStruct styles;

            public StyleComposition() {
                styles = new StyleStruct();
                styles.color = Color.white;
            }
        }

        #region Entity Style Struct
        [System.Serializable]
        public class StyleStruct {
            public Color color;
            public Font font;
            public int size;
            public Sprite sprite;
            public float scale = 1;
        }
        #endregion

    }
}