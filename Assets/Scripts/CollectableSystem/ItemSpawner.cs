using System;
using System.Collections.Generic;
using System.Linq;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Singleton;
using QueueConnect.GameSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QueueConnect.CollectableSystem
{
    public class ItemSpawner : MonoSingleton<ItemSpawner>, ICooldown
    {
        #region --- [INSPECTOR] ---

        [SerializeField] private bool disableSpawnOnLoad = false;
        [SerializeField] private ItemSpawn[] spawnPositions = new ItemSpawn[10];

        #endregion

        #region --- [ITEM COROUTINE HELPER ---
        
        public static readonly WaitForSeconds WaitForPointTwo = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds WaitForPointOne = new WaitForSeconds(0.1f);
        
        #endregion

        #region --- [PROPERTIES] ---

        /// <summary>
        /// When enabled, id will be spawned.
        /// </summary>
        public static bool AutoSpawnEnabled { get; set; } = true;

        /// <summary>
        /// Removes all entries from the List of spawnable Items
        /// </summary>
        public static void ClearSpawnableItemsList() => SpawnableItems.Clear();

        /// <summary>
        /// Set the availability of the passed item type. 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="available">when true, items of the set type can be spawned.</param>
        public static void SetItemSpawnAvailability(ItemID itemID, bool available)
        {
            if (available) SpawnableItems.Add(itemID);
            else SpawnableItems.Remove(itemID);
            OnAllowedItemsChanged?.Invoke(SpawnableItems.ToList());
        }

        private static readonly HashSet<ItemID> SpawnableItems = new HashSet<ItemID>();

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [FIELDS] ---

        private static readonly List<CollectableItem> Collectables = new List<CollectableItem>();
        
        private static readonly List<CollectableItem> AvailableCollectables = new List<CollectableItem>();

        private bool isOnCooldown = false;
        private bool isPlaying = false;

        #endregion

        #region --- [PROCESSOR] ---

        private static string AvailableCollectablesProcessor(CollectableItem item)
        {
            return $"{item.Name} | {item.SpawnValue} | {item.Duration}";
        }
        
        private static string AvailableItemIdsProcessor(ItemID id)
        {
            return $"{id}";
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [EVENTS] ---
        
        private static event Action OnAvailableChangedHandler;

        /// <summary>
        /// Is fired when the List of Items that are allowed to spawn has changed
        /// </summary>
        public static event Action<List<ItemID>> OnAllowedItemsChanged;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INITIALIZATION] ---

        private void OnEnable()
        {
            foreach (ItemID itemId in Enum.GetValues(typeof(ItemID)))
            {
                SpawnableItems.Add(itemId);
            }
            
            if (disableSpawnOnLoad)
                AutoSpawnEnabled = false;
            
            EventController.OnGameStarted += OnGameStart;
            EventController.OnGameEnded += OnGameStop;
            
            foreach (var item in CollectableSettings.Collectables.Select(prefab => Instantiate(prefab, transform)))
            {
                Collectables.Add(item);
                AvailableCollectables.Add(item);
                item.onCooldownStart += () => { AvailableCollectables.Remove(item); OnAvailableChangedHandler?.Invoke(); };
                item.onCooldownEnd += () => { AvailableCollectables.AddIfNotInList(item); OnAvailableChangedHandler?.Invoke(); };
                item.onCooldownCanceled += () => { AvailableCollectables.AddIfNotInList(item); OnAvailableChangedHandler?.Invoke(); };
            }

            OnAllowedItemsChanged?.Invoke(SpawnableItems.ToList());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [CALLS] ---

        private void Update()
        {
            SpawnCollectableObject();
        }
        
        private void OnGameStart()
        {
            CooldownHandler.CancelCooldown(this);
            CooldownHandler.StartCooldown(this, Random.Range(CollectableSettings.InitialCooldown.x, CollectableSettings.InitialCooldown.y));
            
            foreach (var collectable in Collectables)
            {
                if (!collectable.UseInitialCooldown) continue;
                
                CooldownHandler.StartCooldown(collectable, collectable.InitialCooldown);
            }
            
            isPlaying = true;
        }

        private void OnGameStop(bool wasAborted)
        {
            StopAllCoroutines();
            foreach (var collectable in Collectables)
            {
                CooldownHandler.CancelCooldown(collectable);
            }
            CooldownHandler.CancelCooldown(this);
            
            isPlaying = false;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [TUTORIAL] ---

        /// <summary>
        /// Play a "pulsing" animation on all active id.
        /// </summary>
        public static void PlayPulsingAnimation()
        {
            foreach (var item in AvailableCollectables)
            {
                item.PlayPulsingAnimation();
            }
        }

        #endregion

        #region --- [MANUAL SPAWN] ---

        /// <summary>
        /// Spawn an item at a random <see cref="ItemSpawn"/> or at the <see cref="ItemSpawn"/> with the passed index.
        /// To spawn an item at the center use an index of 5
        /// </summary>
        /// <param name="itemID">the type of item to spawn</param>
        /// <param name="index">index of the spawner</param>
        public static CollectableItem SpawnItem(ItemID itemID, int index = -1)
        {
            var item = Collectables.FirstOrDefault(collectable => 
                (collectable.ItemID == itemID &&
                 !collectable.gameObject.activeSelf &&
                 SpawnableItems.Contains(collectable.ItemID)));
            
            if (!item) return null;
            
            ItemSpawn itemSpawn;
            if (index < Instance.spawnPositions.Length && index > -1)
            {
                itemSpawn = Instance.spawnPositions[index];
            }
            else
            {
                itemSpawn = Instance.GetSpawnPosition();
            }
            
            CooldownHandler.CancelCooldown(item);
            if (item != null) item.Spawn(itemSpawn);
            CooldownHandler.CancelCooldown(Instance);
            CooldownHandler.StartCooldown(Instance);

            return item;
        }

        #endregion
        
        #region --- [SPAWN] ---
        
        private ItemSpawn GetSpawnPosition()
        {
            ushort spawnValueSum = 0;
            foreach (var spawn in spawnPositions)
            {
                spawnValueSum += spawn.SpawnValue;
            }

            var rolledChance = Random.Range(0, spawnValueSum);
            ushort value = 0;

            foreach (var spawn in spawnPositions)
            {
                if (rolledChance <= spawn.SpawnValue + value)
                {
                    return spawn;
                }
                value += spawn.SpawnValue;
            }
            return null;
        }

        private void SpawnCollectableObject()
        {
            if(!isPlaying || isOnCooldown || !AutoSpawnEnabled ||  AvailableCollectables.Count <= 0) return;
            var collectableItem = GetCollectableItemToSpawn();

            if (!collectableItem) return;
            
            collectableItem.Spawn(GetSpawnPosition());
            CooldownHandler.StartCooldown(this);
        }

        private static CollectableItem GetCollectableItemToSpawn()
        {
            uint spawnValueSum = 0;
            foreach (var item in AvailableCollectables)
            {
                if (item.CanSpawn && SpawnableItems.Contains(item.ItemID))
                    spawnValueSum += item.SpawnValue;
            }

            var rolledChance = Random.Range(0, spawnValueSum);

            uint value = 0;

            foreach (var item in AvailableCollectables)
            {
                if (item.CanSpawn  && SpawnableItems.Contains(item.ItemID))
                {
                    if (rolledChance <= item.SpawnValue + value)
                    {
                        return item;
                    }

                    value += item.SpawnValue;
                }
            }

            return null;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [COOLDOWN INTERFACE] ---
        
        
        public event Action onCooldownEnd;
        public event Action onCooldownStart;
        public event Action onCooldownCanceled;

        public string Name => "Collectable Handler";

        public float Duration
        {
            get
            {
                var period = CollectableSettings.SpawnPeriod;
                return Random.Range(period.x, period.y);
            }
        }


        public void OnCooldownStart()
        {
            isOnCooldown = true;
            onCooldownStart?.Invoke();
        }

        public void OnCooldownChanged(float oldValue, float newValue)
        {
            
        }

        public void OnCooldownEnd()
        {
            isOnCooldown = false;
            onCooldownEnd?.Invoke();
        }

        public void OnCooldownCancel()
        {
            isOnCooldown = false;
            onCooldownCanceled?.Invoke();
        }

        #endregion
    }
}