using System;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class Cooldown : ICooldown
    {
        public Cooldown(string name, float duration, float callbackWhenLeft = 0f)
        {
            Name = name;
            Duration = duration;
            this.callbackWhenLeft = callbackWhenLeft;
        }

        private readonly float callbackWhenLeft;
        private bool didInvoke = false;

        public event Action onBeforeExpiration;
        
        public string Name { get; }
        public float Duration { get; }

        public event Action onCooldownStart;
        public event Action onCooldownEnd;
        public event Action onCooldownCanceled;
        public event Action<float,float> onCooldownChanged;

        public void OnCooldownChanged(float oldValue, float newValue)
        {
            onCooldownChanged?.Invoke(oldValue, newValue);
            if (didInvoke || !(newValue <= callbackWhenLeft)) return;
            onBeforeExpiration?.Invoke();
            didInvoke = true;
        }

        public void OnCooldownEnd()
        {
            didInvoke = false;
            onCooldownEnd?.Invoke();
        }

        public void OnCooldownStart() => onCooldownStart?.Invoke();

        public void OnCooldownCancel()
        {
            didInvoke = false;
            onCooldownCanceled?.Invoke();
        }
    }
}