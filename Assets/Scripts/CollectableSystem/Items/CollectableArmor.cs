using QueueConnect.Environment;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;

namespace QueueConnect.CollectableSystem
{
    public class CollectableArmor : CollectableItem
    {
        private void Start()
        {
            PlayerHealthHandler.OnArmorLost += (amount, reset) => SetItemIntractable(!reset);
        }
        
        protected override bool Use()
        {
            if (!PlayerHealthHandler.AddArmor(false)) return false;
            AudioSystem.PlayVFX(VFX.OnItemArmorCollected);
            return true;
        }

        protected override bool CheckItemSpawnCondition()
        {
            return PlayerHealthHandler.CurrentArmor < 3;
        }
    }
}