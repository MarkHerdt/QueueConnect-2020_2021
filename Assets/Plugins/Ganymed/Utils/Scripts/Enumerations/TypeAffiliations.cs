using System;

namespace Ganymed.Utils
{
    [Flags]
    public enum TypeAffiliations
    {
        None = 0,
        Primitive = 1,
        String = 2,
        Enum = 4,
        Class = 8,
        Generic = 16,
        Interface = 32,
        Struct = 64,
        All = 127
    }
}