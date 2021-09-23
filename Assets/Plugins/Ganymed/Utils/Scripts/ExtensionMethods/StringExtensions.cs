using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ganymed.Utils;
using UnityEngine;

namespace QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string Dehumanize(this string text)
        {
            var chars = new List<char>();
            var first = true;

                
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if (!first)
                    {
                        chars.Add('_');
                    }
                    
                    first = false;
                    chars.Add(char.ToLower(text[i]));
                }
                else
                {
                    chars.Add(text[i]);
                }
            }

            return new string(chars.ToArray());
        }


        public static string GetFolder(this string input)
        {
            var split = input.Split('/');
            var ret = split[split.Length - 1];

            return ret;
        }

        public static string GetFolderPath(this string input)
        {
            var split = input.Split('/');
            split = split.Take(split.Length - 1).ToArray();


            var empty = string.Empty;
            for (var i = 0; i < split.Length; i++)
            {
                empty = $"{empty}{(i > 0 ? "/" : "")}{split[i]}";
            }

            return empty;
        }


        /// <summary>
        /// Use this to format the name of a variable to automatically add breaks before a upper case character. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string AsLabel(this string name, string prefix = "")
        {
            var chars = new List<char>();

            for (var i = 0; i < name.Length; i++)
                if (i == 0)
                {
                    chars.Add(char.ToUpper(name[i]));
                }
                else
                {
                    if (i < name.Length - 1)
                        if (char.IsUpper(name[i]) && !char.IsUpper(name[i + 1])
                            || char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                            if (i > 1)
                                chars.Add(' ');

                    chars.Add(name[i]);
                }

            return $"{prefix}" +
                   $"{(prefix.IsNullOrWhiteSpace() ? "" : " ")}" +
                   $"{chars.Aggregate(string.Empty, (current, character) => current + character)}";
        }


        /// <summary>
        /// Get an array of boolean values for each character of a string. Each boolean value will be true if the
        /// associated character is uppercase and false if it is lowercase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool[] GetUpperLowerConfig(this string input)
        {
            var config = new bool[input.Length];

            for (var i = 0; i < config.Length; i++) config[i] = input[i].ToString() == input[i].ToString().ToUpper();

            return config;
        }


        /// <summary>
        /// Copy the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str, bool removeRichText = true)
        {
            GUIUtility.systemCopyBuffer = removeRichText ? str.RemoveRichText() : str;
        }


        public static string RemoveRichText(this string input)
        {
            input = RemoveRichTextDynamicTag(input, "color");

            input = RemoveRichTextTag(input, "b");
            input = RemoveRichTextTag(input, "i");


            // TMP
            input = RemoveRichTextDynamicTag(input, "align");
            input = RemoveRichTextDynamicTag(input, "size");
            input = RemoveRichTextDynamicTag(input, "cspace");
            input = RemoveRichTextDynamicTag(input, "font");
            input = RemoveRichTextDynamicTag(input, "indent");
            input = RemoveRichTextDynamicTag(input, "line-height");
            input = RemoveRichTextDynamicTag(input, "line-indent");
            input = RemoveRichTextDynamicTag(input, "link");
            input = RemoveRichTextDynamicTag(input, "margin");
            input = RemoveRichTextDynamicTag(input, "margin-left");
            input = RemoveRichTextDynamicTag(input, "margin-right");
            input = RemoveRichTextDynamicTag(input, "mark");
            input = RemoveRichTextDynamicTag(input, "mspace");
            input = RemoveRichTextDynamicTag(input, "noparse");
            input = RemoveRichTextDynamicTag(input, "nobr");
            input = RemoveRichTextDynamicTag(input, "page");
            input = RemoveRichTextDynamicTag(input, "pos");
            input = RemoveRichTextDynamicTag(input, "space");
            input = RemoveRichTextDynamicTag(input, "sprite index");
            input = RemoveRichTextDynamicTag(input, "sprite name");
            input = RemoveRichTextDynamicTag(input, "sprite");
            input = RemoveRichTextDynamicTag(input, "style");
            input = RemoveRichTextDynamicTag(input, "voffset");
            input = RemoveRichTextDynamicTag(input, "width");

            input = RemoveRichTextTag(input, "u");
            input = RemoveRichTextTag(input, "s");
            input = RemoveRichTextTag(input, "sup");
            input = RemoveRichTextTag(input, "sub");
            input = RemoveRichTextTag(input, "allcaps");
            input = RemoveRichTextTag(input, "smallcaps");
            input = RemoveRichTextTag(input, "uppercase");
            // TMP end


            return input;
        }


        private static string RemoveRichTextDynamicTag(this string input, string tag)
        {
            var index = -1;
            while (true)
            {
                index = input.IndexOf($"<{tag}=", StringComparison.Ordinal);
                if (index != -1)
                {
                    var endIndex = input.Substring(index, input.Length - index).IndexOf('>');
                    if (endIndex > 0)
                        input = input.Remove(index, endIndex + 1);
                    continue;
                }

                input = RemoveRichTextTag(input, tag, false);
                return input;
            }
        }

        private static string RemoveRichTextTag(this string input, string tag, bool isStart = true)
        {
            while (true)
            {
                var index = input.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>", StringComparison.Ordinal);
                if (index != -1)
                {
                    input = input.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                    continue;
                }

                if (isStart)
                    input = RemoveRichTextTag(input, tag, false);
                return input;
            }
        }


        /// <summary>
        /// Set an array of boolean values for each character of a string. Each boolean value represents if the
        /// associated character is uppercase (true) or lowercase (false). The default value if the configuration is
        /// shorter than the string is lowercase (false)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="caseConfiguration"></param>
        /// <returns></returns>
        public static string SetUpperLowerConfig(this string input, bool[] caseConfiguration)
        {
            var returnValue = string.Empty;

            for (var i = 0; i < input.Length; i++)
                if (caseConfiguration.Length - 1 >= i)
                    returnValue += caseConfiguration[i] ? input[i].ToString().ToUpper() : input[i].ToString().ToLower();
                else
                    returnValue += input[i].ToString().ToLower();

            return returnValue;
        }


        /// <summary>
        /// Remove breaks (\n) from the target string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveBreaks(this string input)
        {
            return input.Replace("\n", string.Empty);
        }


        /// <summary>
        /// Cut either the start/end or both of a string removing unnecessary characters.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cut"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string Cut(this string input, StartEnd cut = StartEnd.StartAndEnd, char character = ' ')
        {
            switch (cut)
            {
                case StartEnd.Start:
                    while (input.StartsWith(character.ToString())) input = input.Remove(0, 1);

                    return input;

                case StartEnd.End:
                    while (input.EndsWith(character.ToString())) input = input.Remove(input.Length - 1, 1);

                    return input;

                case StartEnd.StartAndEnd:
                    while (input.StartsWith(character.ToString())) input = input.Remove(0, 1);

                    while (input.EndsWith(character.ToString())) input = input.Remove(input.Length - 1, 1);

                    return input;

                default:
                    return input;
            }
        }

        /// <summary>
        /// Remove a given string form the beginning of the target string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static string RemoveFormBeginning(this string input, string remove)
        {
            return input.StartsWith(remove, StringComparison.OrdinalIgnoreCase)
                ? input.Remove(0, remove.Length)
                : input;
        }

        /// <summary>
        /// remove the given length from the beginning of the target string. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static string RemoveFormBeginning(this string input, int remove)
        {
            return input.Length > remove ? input.Remove(0, remove) : input;
        }


        /// <summary>
        /// remove the given length from the end of the target string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static string TryRemoveFromEnd(this string input, char remove)
        {
            return input.EndsWith(remove.ToString()) ? input.Remove(input.Length - 1, 1) : input;
        }

        /// <summary>
        /// remove the given length from the end of the target string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RemoveFormEnd(this string input, int length)
        {
            return input.Length > length ? input.Remove(input.Length - length, length) : input;
        }

        /// <summary>
        /// remove the given string from the target string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static string Delete(this string input, string remove)
        {
            return input.Replace(remove, "");
        }


        /// <summary>
        /// Repeat the given string for target values times.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string Repeat(this int value, string character = " ")
        {
            var returnValue = string.Empty;

            for (var i = 0; i < value; i++) returnValue += character;

            return returnValue;
        }

        public static bool IsNullOrWhiteSpace(this string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }
    }
}