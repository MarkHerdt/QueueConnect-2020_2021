using System;
using Ganymed.UISystem;
using QueueConnect.Robot;
using UnityEngine;

namespace QueueConnect.Platform
{
    public static class VibrationHandler
    {
        public static bool EnableVibrations
        {
            get => PlayerPrefs.GetInt(nameof(EnableVibrations), 0) > 0;
            set => PlayerPrefs.SetInt(nameof(EnableVibrations), value ? 1 : 0);
        }
        
        #if !UNITY_STANDALONE
        
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            RobotBehaviour.OnRobotDestroyed += (instance, only) => VibrateHandheld();
        }

        private static void VibrateHandheld()
        {
            if (EnableVibrations)
            {
                Handheld.Vibrate();
            }
        }

        #endif
    }
}
