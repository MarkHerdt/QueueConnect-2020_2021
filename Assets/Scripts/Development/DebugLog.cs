using UnityEngine;

namespace QueueConnect.Development
{
    /// <summary>
    /// Displays "Debug.Log" sections in different Colors
    /// </summary>
    public static class DebugLog
    {
        /// <summary>
        /// Type of DebugLog
        /// </summary>
        public enum LogMethod
        {
            Normal,
            Warning,
            Error
        }
        
        /// <summary>
        /// Colors for the DebugLog Text
        /// </summary>
        private enum LogColor
        {
            Black,
            Blue,
            Brown,
            Green,
            Grey,
            Orange,
            Purple,
            Red,
            Silver,
            White,
            Yellow
        }
        
        /// <summary>
        /// Displays the passed Strings in the specified colors
        /// </summary>
        /// <param name="_Color1">First Color</param>
        /// <param name="_Text1">First String</param>
        /// <param name="_Color2">Second Color</param>
        /// <param name="_Text2">Second String</param>
        /// <param name="_Color3">Third Color</param>
        /// <param name="_Text3">Third String</param>
        /// <param name="_Color4">Fourth Color</param>
        /// <param name="_Text4">Fourth String</param>
        /// <param name="_Color5">Fifth Color</param>
        /// <param name="_Text5">Fifth String</param>
        /// <param name="_Color6">Sixth Color</param>
        /// <param name="_Text6">Sixth String</param>
        /// <param name="_Color7">Seventh Color</param>
        /// <param name="_Text7">Seventh String</param>
        /// <param name="_Color8">Eight Color</param>
        /// <param name="_Text8">Eight String</param>
        /// <param name="_Color9">Ninth Color</param>
        /// <param name="_Text9">Ninth String</param>
        /// <param name="_Color10">Tenth Color</param>
        /// <param name="_Text10">Tenth String</param>
        private static void LogWarning(LogColor _Color1  = LogColor.White, string _Text1  = "",
                                       LogColor _Color2  = LogColor.White, string _Text2  = "",
                                       LogColor _Color3  = LogColor.White, string _Text3  = "",
                                       LogColor _Color4  = LogColor.White, string _Text4  = "",
                                       LogColor _Color5  = LogColor.White, string _Text5  = "",
                                       LogColor _Color6  = LogColor.White, string _Text6  = "",
                                       LogColor _Color7  = LogColor.White, string _Text7  = "",
                                       LogColor _Color8  = LogColor.White, string _Text8  = "",
                                       LogColor _Color9  = LogColor.White, string _Text9  = "",
                                       LogColor _Color10 = LogColor.White, string _Text10 = "")
        {
            var _text = $"<color={_Color1}>{_Text1}</color> <color={_Color2}>{_Text2}</color> <color={_Color3}>{_Text3}</color> <color={_Color4}>{_Text4}</color> <color={_Color5}>{_Text5}</color> <color={_Color6}>{_Text6}</color> <color={_Color7}>{_Text7}</color> <color={_Color8}>{_Text8}</color> <color={_Color9}>{_Text9}</color> <color={_Color10}>{_Text10}</color>";

            Debug.LogWarning(_text);
        }

        /// <summary>
        /// Displays the passed Strings in the specified colors
        /// </summary>
        /// <param name="_Color1">First Color</param>
        /// <param name="_Text1">First String</param>
        /// <param name="_Color2">Second Color</param>
        /// <param name="_Text2">Second String</param>
        /// <param name="_Color3">Third Color</param>
        /// <param name="_Text3">Third String</param>
        /// <param name="_Color4">Fourth Color</param>
        /// <param name="_Text4">Fourth String</param>
        /// <param name="_Color5">Fifth Color</param>
        /// <param name="_Text5">Fifth String</param>
        /// <param name="_Color6">Sixth Color</param>
        /// <param name="_Text6">Sixth String</param>
        /// <param name="_Color7">Seventh Color</param>
        /// <param name="_Text7">Seventh String</param>
        /// <param name="_Color8">Eight Color</param>
        /// <param name="_Text8">Eight String</param>
        /// <param name="_Color9">Ninth Color</param>
        /// <param name="_Text9">Ninth String</param>
        /// <param name="_Color10">Tenth Color</param>
        /// <param name="_Text10">Tenth String</param>
        private static void LogError(LogColor _Color1  = LogColor.White, string _Text1  = "",
                                     LogColor _Color2  = LogColor.White, string _Text2  = "",
                                     LogColor _Color3  = LogColor.White, string _Text3  = "",
                                     LogColor _Color4  = LogColor.White, string _Text4  = "",
                                     LogColor _Color5  = LogColor.White, string _Text5  = "",
                                     LogColor _Color6  = LogColor.White, string _Text6  = "",
                                     LogColor _Color7  = LogColor.White, string _Text7  = "",
                                     LogColor _Color8  = LogColor.White, string _Text8  = "",
                                     LogColor _Color9  = LogColor.White, string _Text9  = "",
                                     LogColor _Color10 = LogColor.White, string _Text10 = "")
        {
            var _text = $"<color={_Color1}>{_Text1}</color> <color={_Color2}>{_Text2}</color> <color={_Color3}>{_Text3}</color> <color={_Color4}>{_Text4}</color> <color={_Color5}>{_Text5}</color> <color={_Color6}>{_Text6}</color> <color={_Color7}>{_Text7}</color> <color={_Color8}>{_Text8}</color> <color={_Color9}>{_Text9}</color> <color={_Color10}>{_Text10}</color>";

            Debug.LogError(_text);
        }
        
        /// <summary>
        /// Displays the passed Strings in the specified colors
        /// </summary>
        /// <param name="_Color1">First Color</param>
        /// <param name="_Text1">First String</param>
        /// <param name="_Color2">Second Color</param>
        /// <param name="_Text2">Second String</param>
        /// <param name="_Color3">Third Color</param>
        /// <param name="_Text3">Third String</param>
        /// <param name="_Color4">Fourth Color</param>
        /// <param name="_Text4">Fourth String</param>
        /// <param name="_Color5">Fifth Color</param>
        /// <param name="_Text5">Fifth String</param>
        /// <param name="_Color6">Sixth Color</param>
        /// <param name="_Text6">Sixth String</param>
        /// <param name="_Color7">Seventh Color</param>
        /// <param name="_Text7">Seventh String</param>
        /// <param name="_Color8">Eight Color</param>
        /// <param name="_Text8">Eight String</param>
        /// <param name="_Color9">Ninth Color</param>
        /// <param name="_Text9">Ninth String</param>
        /// <param name="_Color10">Tenth Color</param>
        /// <param name="_Text10">Tenth String</param>
        private static void Log(LogColor _Color1  = LogColor.White, string _Text1  = "",
                                LogColor _Color2  = LogColor.White, string _Text2  = "",
                                LogColor _Color3  = LogColor.White, string _Text3  = "",
                                LogColor _Color4  = LogColor.White, string _Text4  = "",
                                LogColor _Color5  = LogColor.White, string _Text5  = "",
                                LogColor _Color6  = LogColor.White, string _Text6  = "",
                                LogColor _Color7  = LogColor.White, string _Text7  = "",
                                LogColor _Color8  = LogColor.White, string _Text8  = "",
                                LogColor _Color9  = LogColor.White, string _Text9  = "",
                                LogColor _Color10 = LogColor.White, string _Text10 = "")
        {
            var _text = $"<color={_Color1}>{_Text1}</color> <color={_Color2}>{_Text2}</color> <color={_Color3}>{_Text3}</color> <color={_Color4}>{_Text4}</color> <color={_Color5}>{_Text5}</color> <color={_Color6}>{_Text6}</color> <color={_Color7}>{_Text7}</color> <color={_Color8}>{_Text8}</color> <color={_Color9}>{_Text9}</color> <color={_Color10}>{_Text10}</color>";

            Debug.Log(_text);
        }
                
        /// <summary>
        /// Displays the string Black
        /// </summary>
        /// <param name="_Text">Black localizedText</param>
        public static void Black(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Black, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Black, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Black, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the string Blue
        /// </summary>
        /// <param name="_Text">Blue localizedText</param>
        public static void Blue(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Blue, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Blue, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Blue, _Text);
                    break;
            }
        }
        
        /// <summary>
        /// Displays the string Green
        /// </summary>
        /// <param name="_Text">Green localizedText</param>
        public static void Green(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Green, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Green, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Green, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the string Orange
        /// </summary>
        /// <param name="_Text">Orange localizedText</param>
        public static void Orange(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Orange, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Orange, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Orange, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the string Red
        /// </summary>
        /// <param name="_Text">Red localizedText</param>
        public static void Red(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the string White
        /// </summary>
        /// <param name="_Text">White localizedText</param>
        public static void White(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the string Yellow
        /// </summary>
        /// <param name="_Text">Yellow localizedText</param>
        public static void Yellow(string _Text, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Yellow, _Text);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Yellow, _Text);
                    break;
                case LogMethod.Error: LogError(LogColor.Yellow, _Text);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String Redand the second String White
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        public static void Red_White(string _Text1, string _Text2, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White and the second String Green
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Green localizedText</param>
        public static void White_Green(string _Text1, string _Text2, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Green, _Text2);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Green, _Text2);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Green, _Text2);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White and the second String Red
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        public static void White_Red(string _Text1, string _Text2, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String Red, the second String White and the third String Red
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        /// <param name="_Text3">Red localizedText</param>
        public static void Red_White_Red(string _Text1, string _Text2, string _Text3, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Green and the third String White
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Green localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        public static void White_Green_White(string _Text1, string _Text2, string _Text3, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red and the third String White
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        public static void White_Red_White(string _Text1, string _Text2, string _Text3, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String Red, the second String White, the third String Red and the fourth String Green
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        /// <param name="_Text3">Red localizedText</param>
        /// <param name="_Text4">Green localizedText</param>
        public static void Red_White_Red_Green(string _Text1, string _Text2, string _Text3, string _Text4, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.Green, _Text4);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String Red, the second String White, the third String Red and the fourth String White
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        /// <param name="_Text3">Red localizedText</param>
        /// <param name="_Text4">White localizedText</param>
        public static void Red_White_Red_White(string _Text1, string _Text2, string _Text3, string _Text4, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Green, the third String White and the fourth String Green
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Green localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        /// <param name="_Text4">Green localizedText</param>
        public static void White_Green_White_Green(string _Text1, string _Text2, string _Text3, string _Text4, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Green, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White and the fourth String Green
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        /// <param name="_Text4">Green localizedText</param>
        public static void White_Red_White_Green(string _Text1, string _Text2, string _Text3, string _Text4, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White and the fourth String Red
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        /// <param name="_Text4">Red localizedText</param>
        public static void White_Red_White_Red(string _Text1, string _Text2, string _Text3, string _Text4, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White, the fourth String Red and the fifth String White
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        /// <param name="_Text3">Red localizedText</param>
        /// <param name="_Text4">White localizedText</param>
        /// <param name="_Text5">Red localizedText</param>
        public static void Red_White_Red_White_Red(string _Text1, string _Text2, string _Text3, string _Text4, string _Text5, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White, the fourth String Red and the fifth String White
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        /// <param name="_Text4">Red localizedText</param>
        /// <param name="_Text5">White localizedText</param>
        public static void White_Red_White_Red_White(string _Text1, string _Text2, string _Text3, string _Text4, string _Text5, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4, LogColor.White, _Text5);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4, LogColor.White, _Text5);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Red, _Text4, LogColor.White, _Text5);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White, the fourth String Red, the fifth String White and the sixth String White
        /// </summary>
        /// <param name="_Text1">Red localizedText</param>
        /// <param name="_Text2">White localizedText</param>
        /// <param name="_Text3">Red localizedText</param>
        /// <param name="_Text4">White localizedText</param>
        /// <param name="_Text5">Red localizedText</param>
        /// <param name="_Text6">White localizedText</param>
        public static void Red_White_Red_White_Red_White(string _Text1, string _Text2, string _Text3, string _Text4, string _Text5, string _Text6, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5, LogColor.White, _Text6);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5, LogColor.White, _Text6);
                    break;
                case LogMethod.Error: LogError(LogColor.Red, _Text1, LogColor.White, _Text2, LogColor.Red, _Text3, LogColor.White, _Text4, LogColor.Red, _Text5, LogColor.White, _Text6);
                    break;
            }
        }

        /// <summary>
        /// Displays the first String White, the second String Red, the third String White, the fourth String Green, the fifth String White and the sixt String Blue
        /// </summary>
        /// <param name="_Text1">White localizedText</param>
        /// <param name="_Text2">Red localizedText</param>
        /// <param name="_Text3">White localizedText</param>
        /// <param name="_Text4">Green localizedText</param>
        /// <param name="_Text5">White localizedText</param>
        /// <param name="_Text6">Blue Text</param>
        public static void White_Red_White_Green_White_Blue(string _Text1, string _Text2, string _Text3, string _Text4, string _Text5, string _Text6, LogMethod _Method = LogMethod.Normal)
        {
            switch (_Method)
            {
                case LogMethod.Normal: Log(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4, LogColor.White, _Text5, LogColor.Blue, _Text6);
                    break;
                case LogMethod.Warning: LogWarning(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4, LogColor.White, _Text5, LogColor.Blue, _Text6);
                    break;
                case LogMethod.Error: LogError(LogColor.White, _Text1, LogColor.Red, _Text2, LogColor.White, _Text3, LogColor.Green, _Text4, LogColor.White, _Text5, LogColor.Blue, _Text6);
                    break;
            }
        }
    }
}