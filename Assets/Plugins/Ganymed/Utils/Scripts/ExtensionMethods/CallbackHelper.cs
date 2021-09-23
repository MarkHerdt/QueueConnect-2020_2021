using System;

namespace Ganymed.Utils.ExtensionMethods
{
   
    public static class CallbackHelper
    {
#if UNITY_EDITOR
        public static UnityEventType GetUnityEventType(this UnityEditor.PlayModeStateChange state)
        {
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredEditMode:
                    return UnityEventType.EnteredEditMode;
                case UnityEditor.PlayModeStateChange.ExitingEditMode:
                    return UnityEventType.ExitingEditMode;
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    return UnityEventType.EnteredPlayMode;
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    return UnityEventType.ExitingPlayMode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
#endif
    }
}