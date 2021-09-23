using System.Collections;
using System.Threading.Tasks;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableItemBurst : CollectableItem
    {
        [Range(0,5)]
        [SerializeField] private int itemAmount = 3;
        [SerializeField] private ItemID[] spawnableItems = default;

        protected override void Awake()
        {
            base.Awake();
            itemAmount = Mathf.Min(itemAmount, spawnableItems.Length);
        }

        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            ItemSpawner.Instance.StartCoroutine(SpawnItemsRoutine());
            return true;
        }

        private IEnumerator SpawnItemsRoutine()
        {
            for (var i = 0; i < itemAmount; i++)
            {
                ItemSpawner.SpawnItem(spawnableItems.GetRandomArrayElement());
                yield return ItemSpawner.WaitForPointTwo;
            }
        }
    }
}