using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;

namespace QueueConnect.CollectableSystem
{
    public class CollectableRandom : CollectableItem
    {
        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            return false;
        }
    }
}