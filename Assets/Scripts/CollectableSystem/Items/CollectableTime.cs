using QueueConnect.Environment;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableTime : CollectableItem
    {
        [SerializeField] private float timeGained = 5f;

        protected override bool Use()
        {
            if (!ShuffleCountdown.AddToCountdown(timeGained)) return false;
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            AudioSystem.PlayVFX(VFX.OnShuffleCountdownReset);
            return true;
        }
    }
}