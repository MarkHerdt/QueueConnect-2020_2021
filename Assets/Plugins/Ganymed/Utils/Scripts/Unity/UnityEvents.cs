using System;
using UnityEngine.Events;

namespace Ganymed.Utils.Unity
{
    #region --- [BOOL] ---

    [Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    [Serializable]
    public class BoolBoolEvent : UnityEvent<bool, bool>
    {
    }

    #endregion
    
    //------------------------------------------------------------------------------------------------------------------

    #region --- [STRING] ---

    [Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }

    #endregion
    
    //------------------------------------------------------------------------------------------------------------------

    #region --- [INT] ---
    
    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public class IntIntEvent : UnityEvent<int, int>
    {
    }
    
    [Serializable]
    public class IntIntIntEvent : UnityEvent<int, int, int>
    {
    }
    
    #endregion
}