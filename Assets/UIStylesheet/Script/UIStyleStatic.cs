namespace Hsinpa.UIStyle
{
    public class UIStyleStatic
    {

        public static readonly string[] States = new string[4] { UIStyleStatic.FixedState.Idle,
                                                                UIStyleStatic.FixedState.Hover,
                                                                UIStyleStatic.FixedState.Selected,
                                                                UIStyleStatic.FixedState.Disable };

        public static readonly string[] Compositions = new string[2] { UIStyleStatic.FixedComposition.Text,
                                                                UIStyleStatic.FixedComposition.Image};


        public class FixedState {
            public const string Idle = "Idle";
            public const string Hover = "Hover";
            public const string Selected = "Selected";
            public const string Disable = "Disabled";
        }


        public class FixedComposition
        {
            public const string Text = "Text";
            public const string Image = "Image";
        }
    }
}