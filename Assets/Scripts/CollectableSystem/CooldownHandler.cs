using System.Collections.Generic;
using Ganymed.Utils;
using Ganymed.Utils.Callbacks;
using QueueConnect.GameSystem;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public static class CooldownHandler
    {
        #region --- [FILEDS] ---

        private static readonly Dictionary<ICooldown, Timer> Cooldowns = new Dictionary<ICooldown, Timer>();

        private static readonly Stack<ICooldown> ExpiredCooldown = new Stack<ICooldown>();
        
        private static bool isPlaying = false;

        #endregion

        #region --- [OTHER] ---
        
        private class Timer
        {
            public float TimeLeft;

            public Timer(float duration)
            {
                TimeLeft = duration;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [ACCESS] ---

        /// <summary>
        /// Start a cooldown of the passed ICooldown object. Returns true if operation is successful. 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cooldown"></param>
        public static bool StartCooldown(ICooldown obj, float? cooldown = null)
        {
            var timer = (cooldown ?? obj.Duration);
            if (timer <= 0)
            {
                Debug.LogWarning("Cooldown is <= 0!");
                return false;
            }
            if (obj.IsOnCooldown())
            {
                Debug.LogWarning("Cannot add object to cooldown that is already on cooldown");
                return false;
            }
            Cooldowns.Add(obj, new Timer(timer));
            obj.OnCooldownStart();
            return true;
        }

        //hello 
        
        /// <summary>
        /// Try cancel the passed cooldown. Returns true if operation is successful. 
        /// </summary>
        /// <param name="obj">ICooldown object</param>
        /// <returns></returns>
        public static bool CancelCooldown(ICooldown obj)
        {
            if (!Cooldowns.ContainsKey(obj)) return false;
            Cooldowns.Remove(obj);
            obj.OnCooldownCancel();
            return true;
        }
        

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [UPDATE LOGIC] ---


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            UnityEventCallbacks.AddEventListener(Update, ApplicationState.PlayMode, UnityEventType.Update);
            GameController.OnGameStateChanged += (state) => { isPlaying = state == GameState.Playing; };
        }
        
        /// <summary>
        /// Static method that will update cooldowns every frame.
        /// </summary>
        private static void Update()
        {
            if(!isPlaying) return;
            
            foreach (var cooldown in Cooldowns)
            {
                var deltaTime = Time.deltaTime;
                var left = cooldown.Value.TimeLeft;
                cooldown.Key.OnCooldownChanged(left, left - deltaTime);
                cooldown.Value.TimeLeft -= deltaTime;
                if (cooldown.Value.TimeLeft > 0) continue;
                
                cooldown.Key.OnCooldownEnd();
                ExpiredCooldown.Push(cooldown.Key);
            }
            
            if(ExpiredCooldown.Count <= 0) return;
            for (var i = ExpiredCooldown.Count - 1; i >= 0; i--)
            {
                Cooldowns.Remove(ExpiredCooldown.Pop());
            }
            ExpiredCooldown.Clear();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ICOOLDOWN EXTENSIONS] ---

        public static bool IsOnCooldown(this ICooldown cooldown)
            => Cooldowns.ContainsKey(cooldown);
        
        public static void IncrementCD(this ICooldown cooldown, float increment)
        {
            if (Cooldowns.TryGetValue(cooldown, out var timer))
            {
                var timeLeft = timer.TimeLeft;
                cooldown.OnCooldownChanged(timeLeft, timeLeft + increment);
                timer.TimeLeft += increment;
            }
        }
        
        public static void DecrementCD(this ICooldown cooldown, float decrement)
        {
            if (Cooldowns.TryGetValue(cooldown, out var timer))
            {
                var timeLeft = timer.TimeLeft;
                cooldown.OnCooldownChanged(timeLeft, timeLeft - decrement);
                timer.TimeLeft -= decrement;
            }
        }

        #endregion
    }
}