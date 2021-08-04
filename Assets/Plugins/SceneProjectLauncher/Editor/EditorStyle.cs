using UnityEngine;
using UnityEditor;

namespace SPL
{
    public class EditorStyle
    {
        private static EditorStyle style;

        public readonly GUIStyle Area;
        public readonly GUIStyle AreaPadded;

        public readonly GUIStyle MenuButton;
        public readonly GUIStyle MenuButtonSelected;

        public readonly GUIStyle Heading;
        public readonly GUIStyle Subheading;
        public readonly GUIStyle Subheading2;

        public readonly GUIStyle Toggle;

        public static EditorStyle Get => style ??= new EditorStyle();

        private EditorStyle()
        {
            Area = new GUIStyle
            {
                padding = new RectOffset(10, 10, 10, 10),
                wordWrap = true
            };

            AreaPadded = new GUIStyle
            {
                padding = new RectOffset(20, 20, 20, 20),
                wordWrap = true
            };

            MenuButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Normal,
                fontSize = 14,
                fixedHeight = 24
            };

            MenuButtonSelected = new GUIStyle(MenuButton)
            {
                fontStyle = FontStyle.Bold
            };

            Heading = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 24
            };

            Subheading = new GUIStyle(Heading)
            {
                fontSize = 18
            };

            Subheading2 = new GUIStyle(Heading)
            {
                fontSize = 14
            };

            Toggle = new GUIStyle(EditorStyles.toggle)
            {
                stretchWidth = false
            };
        }
    }
}
