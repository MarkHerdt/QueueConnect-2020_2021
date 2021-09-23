using System;

namespace Ganymed.Utils
{
    [Flags]
    public enum ApplicationState
    {
        PlayMode = 1,
        EditMode = 2,
        EditAndPlayMode = 3
    }
}