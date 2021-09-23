using System;
using System.Collections.Generic;
using System.Linq;
using QueueConnect.CollectableSystem;
using QueueConnect.Config;
using QueueConnect.Environment;
using QueueConnect.Robot;
using QueueConnect.Tutorial;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace QueueConnect.GameSystem
{
    [HideMonoScript]
    public class WaveController : MonoBehaviour
    {
        #region InspectorFields#
            [Header("WaveController Settings")]
            [BoxGroup("Settings", false)][Tooltip("The wave, the Player is currently at")]
            [SerializeField][ReadOnly] private ushort wave = 1;
            [BoxGroup("Settings", false)][Tooltip("Duration of the currently running game")]
            [SerializeField][ReadOnly] private float duration;
            [BoxGroup("Settings", false)][Tooltip("Duration in seconds this wave will last")]
            [SerializeField][ReadOnly] private float waveDuration;
            [BoxGroup("Settings", false)][Tooltip("The default duration for each wave")]
            [SerializeField] private float defaultDuration = 60;
            #if UNITY_EDITOR
                [FoldoutGroup("Debug")][Tooltip("Speed, the Robots currently move with")]
                [SerializeField][ReadOnly] private float currentRobotSpeed;
                [FoldoutGroup("Debug")][Tooltip("Amount that is added to the RobotSpeed per minute")]
                [SerializeField][ReadOnly] private float currentSpeedIncrement;
                [FoldoutGroup("Debug")][Tooltip("The current delay in seconds, between each Robot spawn")]
                [SerializeField][ReadOnly] private float currentSpawnDelay;
                [FoldoutGroup("Debug")][Tooltip("Minimum amount of RobotParts, that are disabled on each Robot")]
                [SerializeField][ReadOnly] private byte currentMinAmountToDisable;
                [FoldoutGroup("Debug")][Tooltip("Maximum amount of RobotParts, that are disabled on each Robot")]
                [SerializeField][ReadOnly] private byte currentMaxAmountToDisable;
                [FoldoutGroup("Debug")][Tooltip("Duration in seconds, between each Shuffle")]
                [SerializeField][ReadOnly] private float currentNextShuffleDuration;
                [FoldoutGroup("Debug")][Tooltip("RobotTypes that are currently allowed to spawn")]
                [SerializeField][ReadOnly] private List<RobotType> currentlyAllowedRobotTypes;
                [FoldoutGroup("Debug")][Tooltip("Items that are currently allowed to spawn")]
                [SerializeField][ReadOnly] private List<ItemID> currentlyAllowedItems;
            #endif
            [Tooltip("To enable functionalities after a specific Wave has been reached")]
            [SerializeField] private List<WaveSettings> waveSettings = new List<WaveSettings>();
        #endregion
        
        #region Privates
            /// <summary>
            /// Singleton of "WaveController"
            /// </summary>
            private static WaveController instance;
            /// <summary>
            /// Whether the game is currently running;
            /// </summary>
            private bool hasGameStarted;
            /// <summary>
            /// Whether a wave transition is currently running
            /// </summary>
            private bool waveTransition;
        #endregion

        #region Properties
            /// <summary>
            /// Returns the time left (in seconds), of the currently running Wave
            /// </summary>
            public static float WaveDurationLeft => Mathf.Abs(instance.waveDuration * instance.wave - instance.duration * instance.wave);
        #endregion

        #region Events
            /// <summary>
            /// Is fired, when another wave has started (ushort = The currently running Wave)
            /// </summary>
            public static event Action<int> OnNextWaveStarted;
        #endregion
        
        private void Awake()
        {
            instance = Singleton.Persistent(this);
        }

        private void OnEnable()
        {
            EventController.OnGameStarted += GameStarted;
            EventController.OnGameEnded += GameEnded;
            TooltipMonitor.OnWaveCountdownFinished += StartNextWave;
            #if UNITY_EDITOR
                // Debug
                GameController.OnConveyorSpeedChanged += delegate(float _CurrentSpeed) { currentRobotSpeed = _CurrentSpeed; };
                GameController.OnSpeedIncrementChanged += delegate { currentSpeedIncrement = GameController.SpeedIncrement; };
                RobotSpawner.OnSpawnDelayChanged += delegate(float _SpawnDelay) { currentSpawnDelay = _SpawnDelay; };
                EventController.OnPartsToDisableChanged += delegate { currentMinAmountToDisable = GameConfig.MinAmountToDisable; currentMaxAmountToDisable = GameConfig.MaxAmountToDisable; };
                EventController.OnShuffleCountdownChanged += delegate(float _Duration) { currentNextShuffleDuration = _Duration; };
                EventController.OnAllowedRobotTypesChanged += delegate(List<RobotType> _RobotTypes) { currentlyAllowedRobotTypes = _RobotTypes; };
                ItemSpawner.OnAllowedItemsChanged += delegate(List<ItemID> _Items) { currentlyAllowedItems = _Items; };
            #endif
        }

        private void OnDisable()
        {
            EventController.OnGameStarted -= GameStarted;
            EventController.OnGameEnded -= GameEnded;
            TooltipMonitor.OnWaveCountdownFinished -= StartNextWave;
        }

        private void Start()
        {
            RobotPartConfig.RobotSpawn.ForEach(_Robot => _Robot.Value.CurrentlyActive = true);
            #if UNITY_EDITOR
                currentRobotSpeed = GameController.CurrentRobotSpeed;
                currentSpeedIncrement = GameController.SpeedIncrement;
            #endif

        }

        private void Update()
        {
            WaveDuration();
        }

        /// <summary>
        /// Is called when a game has been started
        /// </summary>
        private void GameStarted()
        {
            if (GameController.IsTutorial) return;
            
                hasGameStarted = true;
                wave = 1;
                duration = 0;
                waveTransition = false;
                var _waveSetting = waveSettings.FirstOrDefault(_Wave => _Wave.Wave == wave);
                if (_waveSetting != null)
                    _waveSetting.ApplySettings();
                else
                    ApplyDefaultSettings();
        }
        
        /// <summary>
        /// Is called after a game has finished
        /// </summary>
        /// <param name="_WasAborted">"true", when the game has been ended through the Menu</param>
        private void GameEnded(bool _WasAborted)
        {
            hasGameStarted = false;
            ResetSettings();
        }

        /// <summary>
        /// Checks the duration of the current game and enables the next wave
        /// </summary>
        private void WaveDuration()
        {
            if (!hasGameStarted || GameController.IsPaused || waveTransition) return;
            
                duration += Time.deltaTime;

                if (duration >= waveDuration && !waveTransition)
                    WaveTransition();
        }
        
        /// <summary>
        /// Disables the RobotSpawn and starts the WaveTransition
        /// </summary>
        private void WaveTransition()
        {
            waveTransition = true;
            StartNextWave();
            //GameConfig.SetRobotSpawn(false);
            
            //TooltipMonitor.StartCountdownToWave(3, ++wave);
        }

        /// <summary>
        /// Starts the next Wave
        /// </summary>
        private void StartNextWave()
        {
            OnNextWaveStarted?.Invoke(++wave);
            var _waveSetting = waveSettings.FirstOrDefault(_Wave => _Wave.Wave == wave);
            if (_waveSetting != null)
                _waveSetting.ApplySettings();
            else
                ApplyDefaultSettings();

            waveTransition = false;
            //GameConfig.SetRobotSpawn(true);
        }

        /// <summary>
        /// Sets the duration how long the wave will last
        /// </summary>
        /// <param name="_Duration">The duration in seconds</param>
        private static void SetWaveDuration(float _Duration)
        {
            instance.waveDuration += _Duration;
        }
        
        /// <summary>
        /// Applies the default settings when no settings for this wave are specified
        /// </summary>
        private static void ApplyDefaultSettings()
        {
            SetWaveDuration(instance.defaultDuration);
        }
        
        /// <summary>
        /// Resets all Settings-Values that could've been changed, to their default values
        /// </summary>
        private static void ResetSettings()
        {
            GameController.AddRobotSpeed(-GameController.CurrentRobotSpeed + GameConfig.BaseRobotSpeed);
            GameController.SetSpeedIncrement(GameConfig.BaseSpeedIncrement);
            RobotSpawner.CurrentSpawnDelay = GameConfig.BaseSpawnDelay;
            GameConfig.RobotPartsToDisable((byte)GameConfig.DefaultAmountToDisable.x, (byte)GameConfig.DefaultAmountToDisable.y);
            ShuffleCountdown.CurrentShuffleCountDownDuration = GameConfig.DefaultShuffleCountdownDuration;
            RobotPartConfig.RobotSpawn.ForEach(_Robot => _Robot.Value.CurrentlyActive = true);
            ItemSpawner.ClearSpawnableItemsList();
            ((ItemID[])Enum.GetValues(typeof(ItemID))).ForEach(_Enum => ItemSpawner.SetItemSpawnAvailability(_Enum, true));
        }
        
        /// <summary>
        /// Settings that can be applied on a Wave change
        /// </summary>
        [Serializable]
        private class WaveSettings
        {
            #region Inspector Fields
                #pragma warning disable 649
                [Tooltip("The Wave at which these features should be activated")]
                [SerializeField] private ushort wave;
                #pragma warning restore 649
                [FoldoutGroup("Settings")][Tooltip("Sets the duration in seconds, how long this wave will be")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Duration")]
                [SerializeField] private FloatValue waveDuration = new FloatValue();
                [FoldoutGroup("Settings")][Tooltip("Adds the set value to the current RobotSpeed")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Speed")]
                [SerializeField] private FloatValue addRobotSpeed = new FloatValue();
                [FoldoutGroup("Settings")][Tooltip("RobotSpeed will increase by this amount per minute")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Amount")]
                [SerializeField] private FloatValue setSpeedIncrement = new FloatValue();
                [FoldoutGroup("Settings")][Tooltip("Delay in Seconds between each Robot spawn")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Seconds")]
                [SerializeField] private FloatValue setSpawnDelay = new FloatValue();
                [FoldoutGroup("Settings")][Tooltip("Minimum amount of RobotParts to disable on each Robot")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Amount")]
                [SerializeField] private ByteValue minAmountToDisable = new ByteValue();
                [FoldoutGroup("Settings")][Tooltip("Maximum amount of RobotParts to disable on each Robot")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Amount")]
                [SerializeField] private ByteValue maxAmountToDisable = new ByteValue();
                [FoldoutGroup("Settings")][Tooltip("Sets the time in seconds between each Shuffle")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Seconds")]
                [SerializeField] private FloatValue nextShuffleDuration = new FloatValue();
                [FoldoutGroup("Settings")][Tooltip("RobotTypes that are allowed to spawn")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "RobotTypes")]
                [SerializeField] private RobotTypeValue allowedRobotTypes;
                [FoldoutGroup("Settings")][Tooltip("Allowed Items to spawn after this Wave is reached")]
                [DictionaryDrawerSettings(KeyLabel = "InUse", ValueLabel = "Items")]
                [SerializeField] private ItemIDValue allowedItems;
            #endregion

            #region Properties
                /// <summary>
                /// The Wave at which these features should be activated
                /// </summary>
                public ushort Wave => wave;
            #endregion
            
            /// <summary>
            /// Applies all settings, that are marked to be used
            /// </summary>
            public void ApplySettings()
            {
                SetWaveDuration();
                AddRobotSpeed();
                SetSpeedIncrement();
                SetSpawnDelay();
                MinAmountToDisable();
                MaxAmountToDisable();
                NextShuffleDuration();
                AllowedRobotTypes();
                AllowedItems();
            }
            
            /// <summary>
            /// Sets the duration how long the wave will last
            /// </summary>
            private void SetWaveDuration()
            {
                if (waveDuration.Keys.Count > 0 && waveDuration.Keys.ElementAt(0))
                    WaveController.SetWaveDuration(waveDuration.Values.ElementAt(0));
            }
            
            /// <summary>
            /// Adds the set value to the current RobotSpeed
            /// </summary>
            private void AddRobotSpeed()
            {
                if (addRobotSpeed.Keys.Count > 0 && addRobotSpeed.Keys.ElementAt(0))
                    GameController.AddRobotSpeed(addRobotSpeed.Values.ElementAt(0));
            }

            /// <summary>
            /// Sets the value, by which the RobotSpeed increases over the duration of a minute
            /// </summary>
            private void SetSpeedIncrement()
            {
                if (setSpeedIncrement.Keys.Count > 0 && setSpeedIncrement.Keys.ElementAt(0))
                    GameController.SetSpeedIncrement(setSpeedIncrement.Values.ElementAt(0));
            }
            
            /// <summary>
            /// Sets delay in Seconds between each Robot spawn
            /// </summary>
            private void SetSpawnDelay()
            {
                if (setSpawnDelay.Keys.Count > 0 && setSpawnDelay.Keys.ElementAt(0))
                    RobotSpawner.CurrentSpawnDelay = setSpawnDelay.Values.ElementAt(0);
            }

            /// <summary>
            /// Sets the minimum amount of RobotParts to disable on each Robot
            /// </summary>
            private void MinAmountToDisable()
            {
                if (minAmountToDisable.Keys.Count > 0 && minAmountToDisable.Keys.ElementAt(0))
                    GameConfig.RobotPartsToDisable(minAmountToDisable.Values.ElementAt(0), null);                    
            }

            /// <summary>
            /// Sets the maximum amount of RobotParts to disable on each Robot
            /// </summary>
            private void MaxAmountToDisable()
            {
                if (maxAmountToDisable.Keys.Count > 0 && maxAmountToDisable.Keys.ElementAt(0))
                    GameConfig.RobotPartsToDisable(null, maxAmountToDisable.Values.ElementAt(0));
            }
            
            /// <summary>
            /// Sets the time in seconds between each Shuffle
            /// </summary>
            private void NextShuffleDuration()
            {
                if (nextShuffleDuration.Keys.Count > 0 && nextShuffleDuration.Keys.ElementAt(0))
                    ShuffleCountdown.CurrentShuffleCountDownDuration = nextShuffleDuration.Values.ElementAt(0);
            }

            /// <summary>
            /// Sets the RobotTypes that are allowed to spawn
            /// </summary>
            private void AllowedRobotTypes()
            {
                if (!allowedRobotTypes.InUse) return;
                
                    foreach (var _robotPrefab in RobotPartConfig.RobotSpawn)
                        _robotPrefab.Value.CurrentlyActive = allowedRobotTypes.RobotTypes.Contains(_robotPrefab.Key);
            }

            /// <summary>
            /// Sets the Items that are allowed to spawn
            /// </summary>
            private void AllowedItems()
            {
                if (!allowedItems.InUse) return;
                
                    ItemSpawner.ClearSpawnableItemsList();
                    foreach (var _item in allowedItems.Items)
                        ItemSpawner.SetItemSpawnAvailability(_item, true);
            }
        }
        
        [Serializable] private class FloatValue : SerializableDictionaryBase<bool, float> { }
        
        [Serializable] private class ByteValue : SerializableDictionaryBase<bool, byte> { }

        [Serializable]
        private struct RobotTypeValue
        {
            #region Inspector Fields
                #pragma warning disable 649
                [Tooltip("Whether to use this Setting or not")]
                [SerializeField] private bool inUse;
                [Tooltip("RobotTypes that are allowed to spawn")]
                [SerializeField] private List<RobotType> robotTypes;
                #pragma warning restore 649
            #endregion
            
            #region Properties
                /// <summary>
                /// Whether to use this Setting or not
                /// </summary>
                public bool InUse => inUse;
                /// <summary>
                /// RobotTypes that are allowed to spawn
                /// </summary>
                public List<RobotType> RobotTypes => robotTypes;
            #endregion
        }
        
        [Serializable]
        private struct ItemIDValue
        {
            #region Inspector Fields
                #pragma warning disable 649
                [Tooltip("Whether to use this Setting or not")]
                [SerializeField] private bool inUse;
                [Tooltip("Items that are allowed to spawn")]
                [SerializeField] private List<ItemID> items;
                #pragma warning restore 649
            #endregion

            #region Properties
                /// <summary>
                /// Whether to use this Setting or not
                /// </summary>
                public bool InUse => inUse;
                /// <summary>
                /// Items that are allowed to spawn
                /// </summary>
                public List<ItemID> Items => items;
            #endregion
        }
    }   
}