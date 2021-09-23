using System;
using System.Collections;
using QueueConnect.Config;
using QueueConnect.Environment;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Ganymed.Utils.ExtensionMethods;
using System.Threading.Tasks;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
namespace QueueConnect.GameSystem
{
    public enum GameState
    {
        Menu,
        Playing,
    }
    
    [HideMonoScript]
    public class GameController : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("Camera in scene")]
            [SceneObjectsOnly][BoxGroup("References", ShowLabel = false)]
            #pragma warning disable 109
            [SerializeField] private new Camera camera;
            #pragma warning restore 109
            [Tooltip("Map GameObject in scene")]
            [SceneObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private Transform map;
            [Tooltip("Collider on the BackGround")]
            [BoxGroup("References", ShowLabel = false)]
            [SerializeField] private Collider2D mapExit;
            [Tooltip("Tutorial GameObject in Scene")]
            [SceneObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private GameObject tutorial;
            [Tooltip("Speed the robots move with")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float currentRobotSpeed;
            [Tooltip("Is gradually added to the RobotSpeed over the course of a Minute")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField][ReadOnly] private float speedIncrement = 400f;
            [Tooltip("How much the speed should increase per minute (for easy difficulty)")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField][ReadOnly] private float easySpeedPerMinute = 1000f;
            [Tooltip("How much the speed should increase per minute (for medium difficulty)")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField][ReadOnly] private float mediumSpeedPerMinute = 1500f;
            [Tooltip("How much the speed should increase per minute (for hard difficulty)")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField][ReadOnly] private float hardSpeedPerMinute = 2000f;
            [Tooltip("How much the current speed increases per minute")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField][ReadOnly] private float currentSpeedPerMinute = 1000f;
        #endregion

        #region Privates
            private float robotSpawnTick;
            private bool robotPartsShuffled;
        #endregion
        
        #region Properties
            /// <summary>
            /// Singleton of "GameController"
            /// </summary>
            public static GameController Instance { get; private set; }
            /// <summary>
            /// What GameState the Player is currently in
            /// </summary>
            public static GameState GameState { get; private set; }
            /// <summary>
            /// Whether the Game is currently in the Tutorial
            /// </summary>
            public static bool IsTutorial { get; set; }
            
            /// <summary>
            /// Collider on the BackGround
            /// </summary>
            public static Collider2D MapExit => Instance.mapExit;
            /// <summary>
            /// Tick for the RobotSpawn
            /// </summary>

            public static float RobotSpawnTick { get => Instance.robotSpawnTick; set => Instance.robotSpawnTick = value; }
            /// <summary>
            /// Speed the robots move with
            /// </summary>

            public static float CurrentRobotSpeed => Instance.currentRobotSpeed;
            
            /// <summary>
            /// Is gradually added to the RobotSpeed over the course of a Minute
            /// </summary>

            public static float SpeedIncrement => Instance.speedIncrement;


            public static bool IsPaused { get; private set; }
            /// <summary>
            /// Whether the Method the clean the ConveyorBelt is running or not
            /// </summary>
            public static bool CleaningConveyorBelt { get; private set; }
            
        #endregion

        #region Events
            /// <summary>
            /// Is fired when the GameState is changed
            /// </summary>
            public static event Action<GameState> OnGameStateChanged;
            /// <summary>
            /// Is fired when the RobotSpeed changes
            /// </summary>
            public static event Action<float> OnRobotSpeedChanged;
            
            /// <summary>
            /// Event is invoked when the custom timescale is altered. Multiply velocity values with the passed single
            /// to create slow motion effects without affecting unities time scale.
            /// </summary>
            public static event Action<float> OnCustomTimeScaleChanged; 
            
            public static event Action<float> OnConveyorSpeedChanged;
            
            /// <summary>
            /// Is fired when the RobotSpawn changes
            /// </summary>
            public static event Action<bool> OnRobotSpawnChanged;

            public static event Action<bool> OnGamePausePlay;

            public static event Action OnSpeedIncrementChanged;
            
        #endregion
        
        private void Awake()
        {
            if (camera == null)
            {
                camera = FindObjectOfType<Camera>();
            }
            if (map == null)
            {
                var _objectsInScene = FindObjectsOfType<Transform>();

                foreach (var _object in _objectsInScene)
                {
                    if (!_object.Tag(Tags.Map)) continue;
                    
                        map = _object;
                        break;
                }
            }
            if (mapExit == null)
            {
                mapExit = FindObjectsOfType<BoxCollider2D>().First(_Collider => _Collider.gameObject.Tag(Tags.MapExit));
            }
            if (tutorial == null)
            {
                tutorial = FindObjectOfType<Tutorial.Tutorial>().gameObject;
            }
            
            Instance = Singleton.Persistent(this);
        }

        private void OnEnable()
        {
            EventController.OnGameEnded += GameEnded;
            OnGameStateChanged += ResetRobotSpawnTick;
            ShuffleCountdown.OnPartsShuffle += delegate { robotPartsShuffled = true; };
        }

        private void OnDisable()
        {
            EventController.OnGameEnded -= GameEnded;
            OnGameStateChanged -= ResetRobotSpawnTick;
        }
        
        private static void GameEnded(bool aborted) => CancelTimeSlow();
        
        private void Start()
        {
            PoolController.Instance.CreatePools();
            
            RobotSpawner.SelectRobotTypesOnMap(PoolController.RobotPools);
            GameConfig.SetRobotSpawn(true);
            
            SetGameState(GameState.Menu);

            currentRobotSpeed = GameConfig.BaseRobotSpeed;
            speedIncrement = GameConfig.BaseSpeedIncrement;
        }

        private void Update()
        {
            IncreaseRobotSpawnTick();
            IncreaseRobotSpeed();
        }
        
        /// <summary>
        /// Increases the RobotSpawnTick while the Game is not in PauseMode
        /// </summary>
        private void IncreaseRobotSpawnTick()
        {
            if(IsPaused) return;
            robotSpawnTick += Time.deltaTime;
        }

        /// <summary>
        /// Resets the RobotSpawnTick back to 0
        /// </summary>
        /// <param name="_State"></param>
        private void ResetRobotSpawnTick(GameState _State)
        {
            if (_State == GameState.Menu && !IsPaused)
            {
                robotSpawnTick = 0;
            }
        }
        
        public static float GetCurrentSpeed()
        {
            return CurrentRobotSpeed * Time.fixedDeltaTime;
        }
        
        /// <summary>
        /// Changes the GameState
        /// </summary>
        /// <param name="_NewState">Targeted state</param>
        public static void SetGameState(GameState _NewState)
        {
            if(GameState == _NewState) return;
            
            GameState = _NewState;
            OnGameStateChanged?.Invoke(GameState);
        }
        
        /// <summary>
        /// Gradually increases the Robot speed over time
        /// </summary>
        private void IncreaseRobotSpeed()
        {
            if (GameState == GameState.Playing && currentRobotSpeed < GameConfig.MaxRobotSpeed && !IsTutorial) 
                ChangeRobotSpeed(currentRobotSpeed + (speedIncrement / 60) * Time.deltaTime);
        }

        /// <summary>
        /// Adds the passed amount to the current RobotSpeed
        /// </summary>
        /// <param name="_Speed">Amount to increase the RobotSpeed with</param>
        public static void AddRobotSpeed(float _Speed)
        {
            ChangeRobotSpeed(Instance.currentRobotSpeed + _Speed);
        }

        /// <summary>
        /// Sets the amount that is gradually added to the RobotSpeed
        /// </summary>
        /// <param name="_AmountPerMinute">Amount that is added per minute</param>
        public static void SetSpeedIncrement(float _AmountPerMinute)
        {
            Instance.speedIncrement = _AmountPerMinute;
            OnSpeedIncrementChanged?.Invoke();
        }
        
        #region --- [CUSTOM TIMESCALE / TIME SLOW] ---

        /// <summary>
        /// Delegate for callbacks after a time slow process has finished.
        /// </summary>
        /// <param name="cancelled">when true, the process was cancelled</param>
        public delegate void TimeSlowCallback(bool cancelled);
        
        private static readonly WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        private static Coroutine timeSlowCoroutine = null;
        private static TimeSlowCallback callbackCache = null;
        private static float cachedSpeed;
        private static float customTimeScale = 1f;


        /// <summary>
        /// The scale at which time passed. 1 by default.
        /// </summary>
        public static float CustomTimeScale
        {
            get => customTimeScale;
            set
            {
                OnCustomTimeScaleChanged?.Invoke(value);
                customTimeScale = value;
            }
        }

        
        /// <summary>
        /// Initialize a process that will slow time temporarily depending on passed values.
        /// </summary>
        /// <param name="duration">The duration of the effect</param>
        /// <param name="progress">The progress / value curve of the effect</param>
        /// <param name="callback">Optional callback when the process has finished</param>
        /// <returns></returns>
        public static bool SlowTime(float duration, AnimationCurve progress, TimeSlowCallback callback = null)
        {
            cachedSpeed = CurrentRobotSpeed;
            callbackCache = callback;
            if (timeSlowCoroutine != null) return false;
            timeSlowCoroutine = Instance.StartCoroutine(SlowTimeCoroutine(duration, progress, callback));
            return true;
        }


        /// <summary>
        /// Cancel a time slow process and reset affected speed / time values back to their pre time slow conditions.
        /// </summary>
        public static void CancelTimeSlow() => CancelTimeSlow(null);
        
        /// <summary>
        /// Cancel a time slow process and reset affected speed / time values back to their pre time slow conditions.
        /// </summary>
        /// <param name="robotSpeed">When not null robotSpeed will be set to this value instead of the cached speed value</param>
        public static void CancelTimeSlow(float? robotSpeed)
        {
            if (timeSlowCoroutine == null) return;
            
            Instance.StopCoroutine(timeSlowCoroutine);
            ChangeRobotSpeed(robotSpeed ?? cachedSpeed);
            callbackCache?.Invoke(false);
        }

        /// <summary>
        /// Coroutine executing a time slow effect.
        /// </summary>
        /// <param name="duration">The duration of the effect</param>
        /// <param name="curve">The progress / value curve of the effect</param>
        /// <param name="callback">Optional callback when the process has finished</param>
        /// <returns></returns>
        private static IEnumerator SlowTimeCoroutine(float duration, AnimationCurve curve, TimeSlowCallback callback)
        {
            var timer = 0f;

            var initialSpeed = CurrentRobotSpeed;
            var targetSpeed = initialSpeed / 100f;

            while (timer < duration)
            {
                if (GameState == GameState.Playing)
                {
                    timer += Time.deltaTime;
                    CustomTimeScale = curve.Evaluate(timer / duration);
                    ChangeRobotSpeed(initialSpeed * CustomTimeScale);
                }
                
                //we wait until the end of frame to prevent that our progress will be overriden. 
                yield return endOfFrame; 
            }

            CustomTimeScale = 1f;
            ChangeRobotSpeed(initialSpeed);
            timeSlowCoroutine = null;
            callbackCache = null;
            callback?.Invoke(true);
        }

        #endregion
        
        
        /// <summary>
        /// Sets the speed the Robots move with to the passed value
        /// </summary>
        /// <param name="_Speed">Targeted speed</param>
        public static void ChangeRobotSpeed(float _Speed)
        {
            Instance.currentRobotSpeed = _Speed > GameConfig.MaxRobotSpeed ? GameConfig.MaxRobotSpeed : _Speed;
            OnConveyorSpeedChanged?.Invoke(_Speed);
        }
        
        
        private static bool previousRobotSpawn;
        private static bool previousCountDownState;
        
        private static float previousRobotSpeed;
        
        
        public static void PauseGame()
        {
            if(IsPaused) return;
                IsPaused = true;
                
                AudioSystem.PlayVFX(VFX.OnGamePaused);
                AudioSystem.StopAmbience(null);
                AudioSystem.PlayAmbience(Ambience.FactoryPause);
                AudioSystem.PlayMusic(Music.FactoryPause);
                
                previousRobotSpawn = GameConfig.SpawnRobots;
                GameConfig.SetRobotSpawn(false);

                EventController.NoRobotTargeted();

                previousCountDownState = ShuffleCountdown.StartCountDown;
                ShuffleCountdown.Instance.EnableDisableCountdown(false);
                
                previousRobotSpeed = CurrentRobotSpeed;
                ChangeRobotSpeed(0);
                
                OnGamePausePlay?.Invoke(IsPaused);
        }
        
        
        public static void ResumeGame()
        {
            if(!IsPaused) return;
            IsPaused = false;

            AudioSystem.PlayVFX(VFX.OnGamePlay);
            AudioSystem.StopAmbience(null);
            AudioSystem.PlayAmbience(Ambience.FactoryPlay);
            AudioSystem.PlayMusic(Music.FactoryPlay);
            
            SetGameState(GameState.Playing);
            
            EventController.RobotTargeted();
            
            ShuffleCountdown.Instance.EnableDisableCountdown(previousCountDownState);
            ChangeRobotSpeed(previousRobotSpeed);
            GameConfig.SetRobotSpawn(previousRobotSpawn);
            
            OnGamePausePlay?.Invoke(IsPaused);
        }
        
        /// <summary>
        /// Is called when the RobotSpeed changes
        /// </summary>
        /// <param name="_Speed">The new Robot speed</param>
        /// <returns>Returns the new Robot speed</returns>
        private static void RobotSpeedChanged(float _Speed)
        {
            previousRobotSpeed = _Speed;
        }

        /// <summary>
        /// Is called when the Robot spawn changes
        /// </summary>
        /// <param name="_Spawn">The current Robot spawn state</param>
        private static void RobotSpawnChanged(bool _Spawn)
        {
            previousRobotSpawn = _Spawn;
        }
        

        /// <summary>
        /// Start a new game
        /// </summary>
        public static void StartGame()
        {
            AudioSystem.StopAmbience(null);
            AudioSystem.PlayAmbience(Ambience.FactoryPlay);
            AudioSystem.PlayMusic(Music.FactoryPlay);
            AudioSystem.PlayVFX(VFX.OnLevelStart);
            
            Instance.CleanConveyorBelt(true);
            SetGameState(GameState.Playing);
        }

        /// <summary>
        /// Starts the Tutorial
        /// </summary>
        public static void StartTutorial()
        {
            IsTutorial = true;
            
            StartGame();
            Instance.tutorial.SetActive(true);
        }
        
        /// <summary>
        /// Exit the current game
        /// </summary>
        public static void ExitPlayMode(bool quitGame)
        {
            Instance.CleanConveyorBelt(false);
            IsPaused = false;
            
            EventController.GameEnded(quitGame);

            AudioSystem.StopAmbience(null);
            AudioSystem.PlayAmbience(Ambience.MainMenu);
            AudioSystem.PlayMusic(Music.MainMenu);
            
            RobotScanner.DestroyAllRobots(true);
            PlayerHealthHandler.ResetLife(0);
            ShuffleCountdown.Instance.EnableDisableCountdown(false);
            
            ChangeRobotSpeed(GameConfig.BaseRobotSpeed);
            GameConfig.SetRobotSpawn(true);
        }
        
        

        /// <summary>
        /// Speeds up the ConveyorBelt, to move all Robots that are currently active out of screen
        /// <param name="_StartGame">Is the game starting?</param>
        /// </summary>
        public async void CleanConveyorBelt(bool _StartGame)
        {
            CleaningConveyorBelt = true;
            RobotScanner.DestroyAllRobots(false);

            GameConfig.Invincible = true;
            
            await Task.Delay((int)(GameConfig.ShakeDuration * 2) / 1000);
            
            ChangeRobotSpeed(GameConfig.MaxRobotSpeed);

            try
            {
                if (!await TaskHelper.AwaitConditionSuccess(() => RobotPool.ActiveRobots != 0, 100)) return;

                ChangeRobotSpeed(GameConfig.BaseRobotSpeed);
                OnRobotSpeedChanged?.Invoke(currentRobotSpeed);
                RobotSpeedChanged(currentRobotSpeed);

                GameConfig.Invincible = false;
                    
                ResetRobots(PoolController.RobotPools, _StartGame);
            }
            catch
            {
                return;
            }
            
            CleaningConveyorBelt = false;
        }
        
        /// <summary>
        /// Starts/Resets the Game
        /// </summary>
        /// <param name="_RobotPools">What RobotTypes to spawn</param>
        /// <param name="_StartGame">Is the game starting?</param>
        private async void ResetRobots(IEnumerable<RobotPool> _RobotPools, bool _StartGame)
        {
            RobotSpawner.SelectRobotTypesOnMap(_RobotPools);
            
            PlayerHealthHandler.ResetLife(3, true);
            ScoreHandler.ResetScore();

            robotPartsShuffled = false;
            ShuffleCountdown.Instance.EnableDisableCountdown(true);
            
            await TaskHelper.AwaitConditionSuccess(() => !robotPartsShuffled);
            
            GameConfig.SetRobotSpawn(true);
            OnRobotSpawnChanged?.Invoke(GameConfig.SpawnRobots);
            RobotSpawnChanged(GameConfig.SpawnRobots);
            
            if (_StartGame)
                EventController.GameStarted();
        }
        
        /// <summary>
        /// Speed per minute for easy difficulty
        /// </summary>
        public void EasySpm()
        {
            currentSpeedPerMinute = easySpeedPerMinute;
            speedIncrement = currentSpeedPerMinute / 60;
        }

        /// <summary>
        /// Speed per minute for medium difficulty
        /// </summary>
        public void MediumSpm()
        {
            currentSpeedPerMinute = mediumSpeedPerMinute;
            speedIncrement = currentSpeedPerMinute / 60;
        }

        /// <summary>
        /// Speed per minute for hard difficulty
        /// </summary>
        public void HardSpm()
        {
            currentSpeedPerMinute = hardSpeedPerMinute;
            speedIncrement = currentSpeedPerMinute / 60;
        }
    }
}