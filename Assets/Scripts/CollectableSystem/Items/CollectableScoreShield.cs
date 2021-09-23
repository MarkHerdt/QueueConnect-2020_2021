using QueueConnect.Environment;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;

namespace QueueConnect.CollectableSystem
{
    public class CollectableScoreShield : CollectableItem
    {
        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            AudioSystem.PlayVFX(VFX.OnItemScoreShieldCollected);
            ScoreHandler.HasScoreShield = true;
            return true;
        }

        protected override bool CheckItemSpawnCondition()
        {
            return !ScoreHandler.HasScoreShield;
        }
    }
}