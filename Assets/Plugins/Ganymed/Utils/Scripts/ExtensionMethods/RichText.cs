using UnityEngine;

namespace Ganymed.Utils.ExtensionMethods
{
    /// <summary>
    /// Class containing extension methods and helper for dealing with rich text.
    /// </summary>
    public static class RichText
    {
        #region --- [COLOR FIELDS] ---

        public static readonly Color _red = new Color(0.95f, 0.24f, 0.21f);
        public static readonly Color _green = new Color(0.6f, 0.91f, 0.62f);
        public static readonly Color _blue = new Color(0.59f, 0.66f, 1f);

        public static readonly Color _cyan = new Color(0.41f, 1f, 0.97f);
        public static readonly Color _magenta = new Color(0.93f, 0.27f, 1f);
        public static readonly Color _yellow = new Color(0.69f, 1f, 0.8f);
        public static readonly Color _black = Color.black;

        public static readonly Color _orange = new Color(0.87f, 0.67f, 0f);
        public static readonly Color _violet = new Color(0.58f, 0.34f, 0.89f);

        public static readonly Color _white = Color.white;
        public static readonly Color _lightGray = new Color(0.59f, 0.6f, 0.67f, 0.86f);
        public static readonly Color _gray = new Color(0.4f, 0.4f, 0.49f, 0.86f);
        public static readonly Color _darkGray = new Color(0.05f, 0.05f, 0.05f);

        #endregion
        
        #region --- [COLOR STRINGS] ---
        
        public const string ClearColor = "</color>";
        
        public static string Red => red ?? (red = _red.AsRichText());
        private static string red;
        
        public static string Green => green ?? (green = _green.AsRichText());
        private static string green;
        
        public static string Blue => blue ?? (blue = _blue.AsRichText());
        private static string blue;
        
        public static string Cyan => cyan ?? (cyan = _cyan.AsRichText());
        private static string cyan;
        
        public static string Magenta => magenta ?? (magenta = _magenta.AsRichText());
        private static string magenta;
        
        public static string Yellow => yellow ?? (yellow = _yellow.AsRichText());
        private static string yellow;
        
        public static string Black => black ?? (black = _black.AsRichText());
        private static string black;
        
        public static string Orange => orange ?? (orange = _orange.AsRichText());
        private static string orange;
        
        public static string Violet => violet ?? (violet = _violet.AsRichText());
        private static string violet;
        
        public static string White => white ?? (white = _white.AsRichText());
        private static string white;
        
        public static string LightGray => lightGray ?? (lightGray = _lightGray.AsRichText());
        private static string lightGray;
        
        public static string Gray => gray ?? (gray = _gray.AsRichText());
        private static string gray;
        
        public static string DarkGray => darkGray ?? (darkGray = _darkGray.AsRichText());
        private static string darkGray;

        #endregion
        
        
        #region --- [EXTENSION METHODS] ---
        
        public static string ToRichTextFontSize(this int num)
        {
            return $"<size={num.Min(0)}>";
        }

        public static string AsRichText(this Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        }
        public static string AsRichText(this Color32 color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        }

        public static string ToFontSize(this float value, float? defaultSize = null)
        {
            var calculatedSize = defaultSize / 100 * value;
            return $"<size=#{calculatedSize ?? value}>";
        }
        
                
        public static string AsLineHeight(this int value) 
            => $"<line-height={value}%>";

        #endregion
    }
}
