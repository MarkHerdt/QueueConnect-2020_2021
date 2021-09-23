using System;

namespace Ganymed.Utils.Helper
{
    public static class Converter
    {
        /// <summary>
        /// Convert UnityEventType to InvokeOrigin
        /// </summary>
        /// <param name="unityEventType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static InvokeOrigin ToOrigin(this UnityEventType unityEventType)
        {
            switch (unityEventType)
            {
                case UnityEventType.OnDisable:
                    return InvokeOrigin.Termination;

                case UnityEventType.BuildPlayer:
                    return InvokeOrigin.BuildPlayer;
              
                case UnityEventType.Recompile:
                    return InvokeOrigin.Recompile;
                
                
                case UnityEventType.EditorApplicationStart:
                case UnityEventType.OnEnable:
                    return InvokeOrigin.Initialization;

                case UnityEventType.EditorApplicationQuit:
                case UnityEventType.ApplicationQuit:
                    return InvokeOrigin.ApplicationQuit;
                
                case UnityEventType.Awake:
                case UnityEventType.Start:
                case UnityEventType.Update:
                case UnityEventType.LateUpdate:
                    return InvokeOrigin.UnityMessage;

                case UnityEventType.EnteredEditMode:
                case UnityEventType.ExitingEditMode:
                case UnityEventType.EnteredPlayMode:
                case UnityEventType.ExitingPlayMode:
                case UnityEventType.TransitionEditPlayMode:
                    return InvokeOrigin.ApplicationStateChanged;
            }
            return InvokeOrigin.Unknown;
        }
    }
}