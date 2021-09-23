using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ganymed.Utils.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static bool Contains(this int flags, int bindingFlags) =>
            (flags & bindingFlags) == bindingFlags;

        
        public static bool MatchesExactly(this int flags, int bindingFlags) =>
            flags == bindingFlags;

        
        public static bool MatchesPartly(this int flags, int bindingFlags) =>
            (flags & bindingFlags) != 0;
    }
}