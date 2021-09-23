using System.Collections;
using QueueConnect.Environment;
using QueueConnect.Plugins.SoundSystem;

using Ganymed.Utils.ExtensionMethods;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableMultiplier : CollectableItem
    {
        [SerializeField] private Vector2Int multiplier = new Vector2Int(5,15);

        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            ItemSpawner.Instance.StartCoroutine(IncreaseMultiplierRoutine());
            return true;
        }

        private IEnumerator IncreaseMultiplierRoutine()
        {
            var amount = multiplier.RandomValue();
            for (var i = 0; i < amount; i++)
            {
                ScoreHandler.IncreaseMultiplier();
                AudioSystem.PlayVFX(VFX.OnItemMotivationCollected);
                yield return ItemSpawner.WaitForPointOne;
            }
        }
    }
}