namespace Hsinpa.UIStyle
{
    public class UIStyleStatic
    {

        public static readonly string[] States = new string[5] { UIStyleStatic.FixedState.Idle,
                                                                UIStyleStatic.FixedState.Hover,
                                                                UIStyleStatic.FixedState.Pressed,
                                                                UIStyleStatic.FixedState.Disable,
                                                                UIStyleStatic.FixedState.Custom,
        };

        public static readonly string[] Compositions = new string[2] { UIStyleStatic.FixedComposition.Text,
                                                                UIStyleStatic.FixedComposition.Image};

        public class ColorTable {
            public static UnityEngine.Color IdleColor = UnityEngine.Color.white;
            public static UnityEngine.Color HoverColor = new UnityEngine.Color(0.9f, 0.9f, 0.9f);
            public static UnityEngine.Color PressedColor = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
            public static UnityEngine.Color DisableColor = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
        }

        public enum UIEventEnum {
            Idle = 0,
            Hover = 1,
            Pressed = 257,
            Pressed_OutofBorder = 256
        }

        public class FixedState {
            public const string Idle = "Idle";
            public const string Hover = "Hover";
            public const string Pressed = "Pressed";
            public const string Disable = "Disabled";
            public const string Custom = "Custom";
        }

        public class FixedComposition
        {
            public const string Text = "Text";
            public const string Image = "Image";
        }
    }
}