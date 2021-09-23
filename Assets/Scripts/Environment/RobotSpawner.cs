using System;
using QueueConnect.GameSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using QueueConnect.Config;
using QueueConnect.Robot;
using Sirenix.OdinInspector;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Robot spawn behaviour
    /// </summary>
    [HideMonoScript]
    public class RobotSpawner : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("The current delay in seconds, between each Robot spawn")]
            [SerializeField][ReadOnly] private float currentSpawnDelay;
            [Tooltip("Gradually subtracts this value from \"currentSpawnDelay\" over the duration of 60 seconds")]
            [SerializeField] private float spawnDelayDecrement = .4f;
            [Tooltip("Minimum value for the SpawnDelay")]
            [SerializeField] private float minSpawnDelay = 1;
        #endregion
        
        #region Privates
            private List<RobotPool> activeRobotPools = new List<RobotPool>();
            private List<RobotPool> inactiveRobotPools = new List<RobotPool>();
            private ushort randomNumber;
            private ushort spawnChance;
            private float lastRobotSpawn;
        #endregion

        #region Properties
            /// <summary>
            /// Singleton of "RobotSpawner"
            /// </summary>
            public static RobotSpawner Instance { get; private set; }
            /// <summary>
            /// Time when the last Robot was spawned
            /// </summary>
            public static float LastRobotSpawn => Instance.lastRobotSpawn;
            /// <summary>
            /// The current delay in seconds, between each Robot spawn
            /// </summary>
            public static float CurrentSpawnDelay { get => Instance.currentSpawnDelay;
                set
                {
                    Instance.currentSpawnDelay = value < Instance.minSpawnDelay ? Instance.minSpawnDelay : value;
                    OnSpawnDelayChanged?.Invoke(Instance.currentSpawnDelay);
                } 
            }
        #endregion

        #region Events
            /// <summary>
            /// Is fired when the SpawnDelay changes
            /// </summary>
            public static event Action<float> OnSpawnDelayChanged; 
        #endregion
        
        private void Awake()
        {
            Instance = Singleton.Persistent(this);
        }

        private void OnEnable()
        {
            GameController.OnGameStateChanged += ResetLastRobotSpawn;
        }

        private void OnDisable()
        {
            GameController.OnGameStateChanged -= ResetLastRobotSpawn;
        }

        private void Start()
        { 
            OnSpawnDelayChanged?.Invoke(Instance.currentSpawnDelay);
        }

        private void Update()
        {
            SpawnRobot();
            SetSpawnDelay();
        }

        /// <summary>
        /// Calculates the spawn chance for each robot type
        /// </summary>
        private static void CalculateSpawnChance()
        {
            Instance.spawnChance = 0;

            foreach (var _pool in Instance.activeRobotPools)
            {
                Instance.spawnChance += _pool.Spawn.SpawnChance;
            }
        }

        /// <summary>
        /// Sets the Spawn-values of each RobotType before the Map starts
        /// </summary>
        private static void GetRobotSpawn()
        {
            #if UNITY_EDITOR
                foreach (var _entry in RobotPartConfig.RobotSpawn)
                {
                    foreach (var _robotType in RobotPartConfig.RobotTypePrefabs)
                    {
                        if (_robotType.Type != _entry.Key) continue;
                            _robotType.SpawnChance.allowedOnMap = _entry.Value.allowedOnMap;
                            _robotType.SpawnChance.CurrentlyActive = _entry.Value.CurrentlyActive;
                            _robotType.SpawnChance.SpawnChance = _entry.Value.SpawnChance;

                            break;
                    }
                }
            #else
                // For build
            #endif
        }

        /// <summary>
        /// Allows the Robots from the passed pool to be spawned on the Map <br/>
        /// Must only be called once at Map start! <br/>
        /// All possible Robot Types for a Map must be included in the passed List subsequent entries are not allowed!
        /// </summary>
        /// <param name="_RobotPools">Robots that are allowed to spawn on this Map</param>
        public static void SelectRobotTypesOnMap(IEnumerable<RobotPool> _RobotPools)
        {
            GetRobotSpawn();

            // Gets every RobotPool that is allowed on the current Map
            var _tmpPool = _RobotPools.Where(_Pool => _Pool.Spawn.allowedOnMap).ToList();
            
            // Must create a new List, so it doesn't take the passed ObjectPool as a reference and changes its values
            Instance.activeRobotPools = new List<RobotPool>(_tmpPool);
            Instance.inactiveRobotPools = new List<RobotPool>();

            ShuffleCountdown.GetRobotParts(Instance.activeRobotPools);
            
            UpdateRobotSpawn();

            Instance.lastRobotSpawn = -CurrentSpawnDelay;
        }

        /// <summary>
        /// Activates/Deactivates a Robot Type
        /// </summary>
        public static void UpdateRobotSpawn()
        {
            // Activates a Robot Type
            for (byte i = 0; i < Instance.inactiveRobotPools.Count; i++)
            {
                if (!Instance.inactiveRobotPools[i].Spawn.CurrentlyActive) continue;
                
                    Instance.activeRobotPools.Add(Instance.inactiveRobotPools[i]);
                    Instance.inactiveRobotPools.RemoveAt(i);

                break;
            }
            // Deactivates a Robot Type
            for (byte i = 0; i < Instance.activeRobotPools.Count; i++)
            {
                if (Instance.activeRobotPools[i].Spawn.CurrentlyActive) continue;
                
                    Instance.inactiveRobotPools.Add(Instance.activeRobotPools[i]);
                    Instance.activeRobotPools.RemoveAt(i);

                    break;
            }

            EventController.AllowedRobotTypesChanged(Instance.activeRobotPools.Select(_Pool => _Pool.Type).ToList());
            CalculateSpawnChance();
        }

        /// <summary>
        /// Spawns the robots
        /// </summary>
        private void SpawnRobot()
        {
            if (lastRobotSpawn + currentSpawnDelay > GameController.RobotSpawnTick) return;

                if (currentSpawnDelay == 0) CurrentSpawnDelay = GameConfig.BaseSpawnDelay;
                
                // Only spawns Robots when Robot spawn is not disabled<
                if (!GameConfig.SpawnRobots) return;
                
                    lastRobotSpawn = GameController.RobotSpawnTick;
                
                    // Which Robot Type to spawn
                    randomNumber = (ushort)Random.Range(0, spawnChance);

                    for (byte i = 0; i < activeRobotPools.Count; i++)
                    {
                        if (randomNumber <= activeRobotPools[i].Spawn.SpawnChance)
                        {
                            var _liftElement = ConveyorLift.LiftElements[0];
                            var _tmp = activeRobotPools[i].Pool.GetObject(_liftElement.transform);
                            RobotPool.ActiveRobots++;
                            _liftElement.Robot = (RobotBehaviour)_tmp.Component;
                            
                            EventController.RobotSpawned();

                            break;
                        }

                        randomNumber -= activeRobotPools[i].Spawn.SpawnChance;
                    }
        }

        /// <summary>
        /// Resets the value when the last Robot was spawned to 0
        /// </summary>
        /// <param name="_State">The current GameState</param>
        private void ResetLastRobotSpawn(GameState _State)
        {
            if (_State == GameState.Menu && !GameController.IsPaused)
            {
                lastRobotSpawn = -currentSpawnDelay;   
            }
        }

        /// <summary>
        /// Sets a new value for the current SpawnDelay
        /// </summary>
        private void SetSpawnDelay()
        {
            if (GameController.GameState == GameState.Playing && currentSpawnDelay > minSpawnDelay && !GameController.IsTutorial) 
                CurrentSpawnDelay -= (spawnDelayDecrement / 60) * Time.deltaTime;
        }
    }
}