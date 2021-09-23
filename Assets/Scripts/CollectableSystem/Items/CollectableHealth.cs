using QueueConnect.Environment;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableHealth : CollectableItem
    {
        private void Start()
        {
            PlayerHealthHandler.OnHealthLost += (amount, reset) => SetItemIntractable(!reset);
        }

        protected override bool Use()
        {
            if (!PlayerHealthHandler.AddLife(false)) return false;
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            return true;
        }
        
        protected override bool CheckItemSpawnCondition()
        {
            return PlayerHealthHandler.CurrentHealth < 3;
        }

    }
}