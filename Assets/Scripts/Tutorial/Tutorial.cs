using System;
using System.Collections;
using QueueConnect.CollectableSystem;
using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.Environment;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Tutorial
{
    [HideMonoScript]
    public class Tutorial : MonoBehaviour, ILocalizationCallback
    {
        #region Inspector Fields
            [Tooltip("Time in seconds between each Shuffle")]
            [SerializeField] private float shuffleWaitTime = .1f;
            [Tooltip("Number of times, the Parts will be shuffled")]
            [SerializeField] private float shuffleCount = 10;

            #pragma warning disable 649
            [SerializeField] private string[] tutorialTextKeys;
            #pragma warning restore 649
        #endregion
        
        #region Privates
            
        private string[] tutorialText;
            
            private bool paused;
            private IEnumerator setSpeed;
            private WaitForSeconds pauseWait;
            /// <summary>
            /// Whether the first Robot has been repaired
            /// </summary>
            private bool firstRobotRepaired;
            /// <summary>
            /// Whether the first PowerUp has been collected
            /// </summary>
            private bool firstPowerUpCollected;
            /// <summary>
            /// Have the Parts been shuffled before?
            /// </summary>
            private bool firstShuffle;
            /// <summary>
            /// Whether to enable all Buttons in the Displays
            /// </summary>
            private bool enableAllButtons;
            /// <summary>
            /// The first PowerUp that is collected
            /// </summary>
            private CollectableItem firstPowerUp;
            /// <summary>
            /// Time in seconds between each Shuffle
            /// </summary>
            private WaitForSeconds waitTime;
        #endregion

        private void Awake()
        {
            LocalizationManager.AddCallbackListener(this);
            pauseWait = new WaitForSeconds(.1f);
            waitTime = new WaitForSeconds(shuffleWaitTime);
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventController.OnRobotSpawned += RobotSpawned;
            EventController.OnRobotReleased += RobotReleased;
            EventController.OnRobotTargeted += DisableButtons;
            EventController.OnRobotRepaired += RobotRepaired;
            CollectableItem.OnItemCollectedEvent += PowerUpCollected;
            ShuffleCountdown.OnPartsShuffle += PartsShuffled;
            GameController.OnGameStateChanged += EndTutorial;
        
            ItemSpawner.AutoSpawnEnabled = false;
            
            paused = false;
            setSpeed = null;
            firstRobotRepaired = false;
            firstPowerUpCollected = false;
            firstShuffle = false;
            firstPowerUp = null;
            enableAllButtons = false;
        }
        
        private void OnDisable()
        {
            EventController.OnRobotSpawned -= RobotSpawned;
            EventController.OnRobotReleased -= RobotReleased;
            EventController.OnRobotTargeted -= DisableButtons;
            EventController.OnRobotRepaired -= RobotRepaired;
            CollectableItem.OnItemCollectedEvent -= PowerUpCollected;
            ShuffleCountdown.OnPartsShuffle -= PartsShuffled;
            GameController.OnGameStateChanged -= EndTutorial;
        }

        private void Update()
        {
            CheckRobotPosition();
        }

        private void CheckRobotPosition()
        {
            if (paused || firstShuffle) return;

            if ((RobotScanner.Robots.SafePeek()?.transform.position.x > GameConfig.MapCenter.x - 25 && RobotScanner.Robots.SafePeek()?.transform.position.x < GameConfig.MapCenter.x) || 
                (!firstPowerUpCollected  && firstPowerUp != null && firstPowerUp.transform.position.x > GameConfig.MapCenter.x - 25 && firstPowerUp.transform.position.x < GameConfig.MapCenter.x))
            {
                PauseGame(true);
            }
        }

        /// <summary>
        /// Pauses the Tutorial
        /// </summary>
        /// <param name="_Pause">Whether to pause/resume</param>
        private void PauseGame(bool _Pause)
        {
            if (setSpeed != null)
            {
                StopCoroutine(setSpeed);
            }

            setSpeed = Pause(_Pause);
            StartCoroutine(setSpeed);
            ShuffleCountdown.Instance.EnableDisableCountdown(!_Pause);
        }

        private IEnumerator Pause(bool _Pause)
        {
            var _robot = RobotScanner.Robots.SafePeek();
            // Paused
            if (_Pause)
            {
                paused = true;
                while (GameController.CurrentRobotSpeed > 10)
                {
                    GameController.ChangeRobotSpeed(Mathf.SmoothStep(GameController.CurrentRobotSpeed, 0, 25f * Time.deltaTime));
                    if ((_robot != null && _robot.transform.position.x > GameConfig.MapCenter.x) || (firstPowerUp != null && firstPowerUp.transform.position.x > GameConfig.MapCenter.x))
                    {
                        break;
                    }
                    yield return pauseWait;
                }
                GameController.ChangeRobotSpeed(0);

            }
            // Resume
            else
            {
                paused = false;
                while (GameController.CurrentRobotSpeed < GameConfig.BaseRobotSpeed)
                {
                    GameController.ChangeRobotSpeed(Mathf.SmoothStep(GameController.CurrentRobotSpeed, GameConfig.BaseRobotSpeed, 25f * Time.deltaTime));
                    yield return pauseWait;
                }
                GameController.ChangeRobotSpeed(GameConfig.BaseRobotSpeed);
            }

            setSpeed = null;
        }

        private void SetSpeed(float _TargetSpeed)
        {
            if (setSpeed != null)
            {
                StopCoroutine(setSpeed);
            }

            setSpeed = ConveyorSpeed(_TargetSpeed);
            StartCoroutine(setSpeed);
        }

        private IEnumerator ConveyorSpeed(float _TargetSpeed)
        {
            while (Math.Abs(GameController.CurrentRobotSpeed - _TargetSpeed) > 10)
            {
                GameController.ChangeRobotSpeed(Mathf.SmoothStep(GameController.CurrentRobotSpeed, _TargetSpeed, _TargetSpeed > 2000 ? .5f : 25 * Time.deltaTime));
                yield return pauseWait;
            }
            GameController.ChangeRobotSpeed(_TargetSpeed);
        }
        
        /// <summary>
        /// Disables the RobotSpawn after a Robot is spawned
        /// </summary>
        private void RobotSpawned()
        {
            if (!GameController.IsTutorial || GameController.CleaningConveyorBelt) return;

            if (!firstRobotRepaired || !firstPowerUpCollected || !firstShuffle)
            {
                GameConfig.SetRobotSpawn(false);
                SetSpeed(7500);
                //GameController.ChangeRobotSpeed(7500f);   
            }
        }

        /// <summary>
        /// Changes the speed back to normal
        /// </summary>
        private void RobotReleased()
        {
            if (!GameController.IsTutorial || GameController.CleaningConveyorBelt) return;
            
                SetSpeed(GameConfig.BaseRobotSpeed);
                //GameController.ChangeRobotSpeed(GameConfig.BaseRobotSpeed);
                if (!firstRobotRepaired)
                {
                    TooltipMonitor.ShowText(tutorialText[0]);
                }
        }

        /// <summary>
        /// Only enables the Buttons with the currently targeted Robots missing Parts
        /// </summary>
        private void DisableButtons()
        {
            if (!RobotScanner.RobotTargeted || enableAllButtons) return;
            
                var _robot = RobotScanner.Robots.SafePeek();
                    
                foreach (var _button in PartsDisplay.RobotPartButtons)
                {
                    // Disables every Button that doesn't have a Part for the currently targeted Robot
                    if (!(_robot.Type == _button.Part.Type && _robot.MissingParts[_button.Part.RobotIndex]))
                    {
                        _button.DisableButton();
                    }
                }
        }

        /// <summary>
        /// Sets which buttons will be enabled after the parts were shuffled
        /// </summary>
        private void PartsShuffled()
        {
            foreach (var _button in PartsDisplay.RobotPartButtons)
            {
                _button.EnableButton();
            }

            if (firstRobotRepaired && !firstShuffle)
            {
                firstShuffle = true;
                PauseGame(true);
                foreach (var _button in PartsDisplay.RobotPartButtons)
                {
                    _button.DisableButton();
                }
                
                TooltipMonitor.ShowText(tutorialText[1]);
                StartCoroutine(ShuffleParts());
                return;
            }
            
            DisableButtons();
        }

        /// <summary>
        /// Shuffles the Parts inside the Displays over a period of time
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShuffleParts()
        {
            for (byte i = 0; i < shuffleCount; i++)
            {
                EventController.ShuffleParts(ShuffleCountdown.PartsDisplayList.ShuffleList());
                yield return waitTime;
            }
            
            foreach (var _button in PartsDisplay.RobotPartButtons)
            {
                _button.EnableButton();
            }
            
            TooltipMonitor.HideDisplay(1000);
            DisableButtons();
            PauseGame(false);
            SpawnNewRobot();
            ItemSpawner.AutoSpawnEnabled = true;
        }
        
        /// <summary>
        /// Is called when a Robot has been repaired
        /// </summary>
        /// <param name="_Robot">The Robot that has been repaired</param>
        private void RobotRepaired(RobotBehaviour _Robot)
        {
            firstRobotRepaired = true;
            enableAllButtons = true;
            if (!firstRobotRepaired || !firstPowerUpCollected || !firstShuffle)
            {
                SpawnNewRobot();
            }
        }
        
        /// <summary>
        /// Spawn a new Robot 
        /// </summary>
        private void SpawnNewRobot()
        {
            if (firstRobotRepaired && !firstPowerUpCollected)
            {
                SpawnPowerUp();
                PauseGame(false);
                return;
            }
            
            GameConfig.SetRobotSpawn(true);
            // Calculates the time, so there is no delay
            if (RobotSpawner.LastRobotSpawn + RobotSpawner.CurrentSpawnDelay > GameController.RobotSpawnTick)
                GameController.RobotSpawnTick += RobotSpawner.LastRobotSpawn + RobotSpawner.CurrentSpawnDelay - GameController.RobotSpawnTick;
            
            PauseGame(false);
        }
        
        /// <summary>
        /// Manually spawns a PowerUp
        /// </summary>
        private void SpawnPowerUp()
        {
            TooltipMonitor.ShowText(tutorialText[2]);
            
            firstPowerUp = ItemSpawner.SpawnItem(ItemID.Armor, 1);
            ItemSpawner.PlayPulsingAnimation();
        }

        /// <summary>
        /// Is called when a PowerUp has been collected
        /// </summary>
        /// <param name="_Item">The PowerUp that has been collected</param>
        private void PowerUpCollected(CollectableItem _Item)
        {
            TooltipMonitor.HideDisplay(0);
            
            firstPowerUpCollected = true;
            
            if (firstRobotRepaired && firstPowerUpCollected)
            {
                CollectableItem.OnItemCollectedEvent -= PowerUpCollected;
            }
            
            PauseGame(false);
            SpawnNewRobot();
        }
        
        /// <summary>
        /// Ends the Tutorial
        /// </summary>
        private void EndTutorial(GameState _State)
        {
            if (_State != GameState.Menu || GameController.IsPaused) return;
            
                ItemSpawner.AutoSpawnEnabled = true;
                GameController.IsTutorial = false;
                gameObject.SetActive(false);
        }

        public void OnLanguageLoaded(Language language)
        {
            tutorialText = new string[tutorialTextKeys.Length];
            for (int i = 0; i < tutorialTextKeys.Length; i++)
            {
                tutorialText[i] = LocalizationManager.GetText(tutorialTextKeys[i]);
            }
        }
    }
}