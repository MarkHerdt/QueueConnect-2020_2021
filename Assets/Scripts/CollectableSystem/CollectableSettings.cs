using System.Collections.Generic;
using Ganymed.Utils.Singleton;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableSettings : Settings<CollectableSettings>
    {
        #region --- [INSPECTOR] ---

        [SerializeField] [MinMaxSlider(.1f,100f, true)] private Vector2  initialCooldown = new Vector2(10,20);
        [SerializeField] [MinMaxSlider(.1f,100f, true)] private Vector2 timeBetweenSpawns = new Vector2(10,20);

        [SerializeField] private List<CollectableItem> collectableItems = new List<CollectableItem>();

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [PROPERTIES] ---

        public static IEnumerable<CollectableItem> Collectables => Instance.collectableItems;

        public static Vector2 InitialCooldown => Instance.initialCooldown;
        public static Vector2 SpawnPeriod => Instance.timeBetweenSpawns;
       
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [EDITOR] ---

#if UNITY_EDITOR
        [MenuItem("Game/Edit Collectable Settings", priority = 40)]
        public static void EditSettings()
        {
            SelectObject(Instance);
        }
#endif

        #endregion
    }
}