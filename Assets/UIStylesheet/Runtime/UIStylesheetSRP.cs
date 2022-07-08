using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    [CreateAssetMenu(fileName = "UIStyleSRP", menuName = "SRP/UIStyle", order = 1)]
    public class UIStylesheetSRP : ScriptableObject
    {
        public UnityEngine.UI.ColorBlock colorBlock;

        public Color FilterColorByTrigger(UIStyleStruct.Trigger trigger) {

            switch (trigger)
            {
                case UIStyleStruct.Trigger.Idle:
                    return colorBlock.normalColor;

                case UIStyleStruct.Trigger.Hover:
                    return colorBlock.highlightedColor;

                case UIStyleStruct.Trigger.Pressed:
                    return colorBlock.pressedColor;

                case UIStyleStruct.Trigger.Disabled:
                    return colorBlock.disabledColor;
            }

            return Color.white;
        }

    }
}