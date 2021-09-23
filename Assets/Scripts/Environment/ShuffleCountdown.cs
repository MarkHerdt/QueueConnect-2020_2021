using QueueConnect.Development;
using QueueConnect.GameSystem;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QueueConnect.Config;
using QueueConnect.ExtensionMethods;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SoundSystem.Core;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Shuffles the RobotParts in the Part Displays
    /// </summary>
    [HideMonoScript]
    public class ShuffleCountdown : MonoBehaviour
    {
        
        #region Inspector Fields
        [Tooltip("The current delay in seconds between each shuffle")]
        [SerializeField][ReadOnly] private float currentShuffleCountdownDuration;
        [Header("Indications")] 
        [SerializeField] private Color defaultColor = Color.green;
        [SerializeField] private Color littleTimeLeftColor = Color.red;
        [SerializeField] private Animation littleTimeLeftAnimation = null;
        [Space]
        [SerializeField] private Color defaultColorSlider = Color.green;
        [SerializeField] private Color littleTimeLeftColorSlider = Color.red;
        [SerializeField] private Color defaultColorSliderBackground = Color.green;
        [SerializeField] private Color littleTimeLeftColorSliderBackground = Color.red;
        [SerializeField] private Image sliderImage = null;
        [SerializeField] private SpriteRenderer sliderBackground = null;
        [Space]
        [SerializeField] [Range(1f, 7f)]
        private float timeLeftWhenSoundCueIsPlayed = 4f;
        
        [Space]
        [Tooltip("Slider Component")]
        [ChildGameObjectsOnly, BoxGroup("References", ShowLabel = false)]
        [SerializeField, Required]
        private Slider countdownSlider;

        [Tooltip("TextMeshPro Component")]
        [ChildGameObjectsOnly, BoxGroup("References", ShowLabel = false)]
        [SerializeField]
        private TextMeshProUGUI timeTextField;

        [Tooltip("List of all Robot Parts that are allowed to be disabled")] [SerializeField] [ReadOnly]
        private List<Part> partsList = new List<Part>();

        [Tooltip("All Robot Parts missing on the currently active Robots on the map")] [SerializeField] [ReadOnly]
        private MissingParts missingPartsOnMap = new MissingParts();


        [SerializeField] private UnityEvent OnLittleTimeLeftEvent = new UnityEvent();
        [SerializeField] private UnityEvent OnCounterResetEvent = new UnityEvent();
        
        #endregion

        #region Privates

        private bool gameStartedFirstTime = true;

        // Parts
        private readonly Dictionary<RobotType, List<Part>> freeParts = new Dictionary<RobotType, List<Part>>();
        private readonly Dictionary<RobotType, List<Part>> partsToDisable = new Dictionary<RobotType, List<Part>>();

        private List<Part> partsDisplayList = new List<Part>();

        // Random chance
        private const byte MIN_CHANCE = 1;
        private const byte MAX_CHANCE = 100;
        private float keyChance;
        private float valueChance;
        private byte random;
        private byte keyIndex;
        private byte valueIndex;
        private byte keyIterationCheck;

        private bool startCountDown;

        // ShuffleCountdown
        private float countDown;
        private bool fastReset;
        private float? reset;

        
        /// <summary>
        /// Was a audio cue played within the current reset cycle.
        /// </summary>
        private bool playedAudioCueThisCycle = true;
        
        #endregion

        #region Propteries

        /// <summary>
        /// Singleton of "ShuffleCountdown"
        /// </summary>
        public static ShuffleCountdown Instance { get; private set; }
        public static List<Part> PartsDisplayList => Instance.partsDisplayList;
        /// <summary>
        /// List of all Robot Parts that are allowed to be disabled
        /// </summary>
        public static Dictionary<RobotType, List<Part>> PartsToDisable => Instance.partsToDisable;
        /// <summary>
        /// Whether the Countdown is currently enabled or not
        /// </summary>
        public static bool StartCountDown => Instance.startCountDown;
        /// <summary>
        /// The current delay in seconds between each shuffle
        /// </summary>
        public static float CurrentShuffleCountDownDuration
        {
            get => Instance.currentShuffleCountdownDuration;
            set
            {
                Instance.currentShuffleCountdownDuration = value;
                EventController.ShuffleCountdownChanged(Instance.currentShuffleCountdownDuration);
            }
        }

        #endregion

        #region --- [EVENTS] ---

        public static event Action<float> OnTimeGained;
        public static event Action<float> OnTimeLost;

        
        public static event Action OnBeforePartsShuffle;
        public static event Action OnPartsShuffle;

        #endregion

        private void Awake()
        {
            if (countdownSlider == null)
            {
                countdownSlider = GetComponentInChildren<Slider>();
            }

            if (timeTextField == null)
            {
                timeTextField = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (GetComponentInChildren<Canvas>().worldCamera == null)
            {
                GetComponentInChildren<Canvas>().worldCamera = FindObjectOfType<Camera>();
            }
            
            Instance = Singleton.Persistent(this);
        }

        private void OnEnable()
        {
            EventController.OnMenuOpened += StopCountdown;
            EventController.OnGameEnded += StopAudioCue;
            EventController.OnRobotRepaired += RobotRepairedTimeGain;
        }
        
        private void OnDisable()
        {
            EventController.OnMenuOpened -= StopCountdown;
            EventController.OnGameEnded -= StopAudioCue;
            EventController.OnRobotRepaired -= RobotRepairedTimeGain;
        }

        private void Start()
        {
            currentShuffleCountdownDuration = GameConfig.DefaultShuffleCountdownDuration;
            EventController.ShuffleCountdownChanged(currentShuffleCountdownDuration);
        }

        private void Update()
        {
            CalculateCountdown();
        }

        private void StopCountdown()
        {
            EnableDisableCountdown(false);
        }

        /// <summary>
        /// Enables/Disables the ShuffleCountdown
        /// </summary>
        /// <param name="_Enable">"true" = enables the countdown, "false" = disables the countdown</param>
        public void EnableDisableCountdown(bool _Enable)
        {
            startCountDown = _Enable;
        }

        /// <summary>
        /// Calculates the Countdowns next value
        /// </summary>
        private void CalculateCountdown()
        {
            if (!startCountDown) return;

            if (fastReset)
            {
                reset = countDown / GameConfig.ResetCountdownDuration;
                fastReset = false;
                playedAudioCueThisCycle = true;
            }

            countDown -= reset == null ? Time.deltaTime : Time.deltaTime * reset.Value;
            countdownSlider.value = countDown;
            timeTextField.text = $"{((byte) countDown + 1).ToString()}";
            
            
            if (!playedAudioCueThisCycle && countDown <= timeLeftWhenSoundCueIsPlayed)
            {
                //Played once per cycle if the timer reached a certain threshold.
                AudioSystem.PlayVFX(VFX.OnBeforeShuffleTimerReset);
                OnBeforePartsShuffle?.Invoke();
                OnLittleTimeLeftEvent?.Invoke();
                playedAudioCueThisCycle = true;
                littleTimeLeftAnimation.Play();
                timeTextField.color = littleTimeLeftColor;
                sliderImage.color = littleTimeLeftColorSlider;
                sliderBackground.color = littleTimeLeftColorSliderBackground;
            }

            if (countDown > 0) return;

            reset = null;
            GetPartsToDisable();
            ResetCountdown();
            OnPartsShuffle?.Invoke();
        }

        

        private void OnValidate()
        {
            timeTextField.color = defaultColor; 
            sliderImage.color = defaultColorSlider;
            sliderBackground.color = defaultColorSliderBackground;
        }

        /// <summary>
        /// Resets the Countdown to its initial values
        /// </summary>
        /// <param name="_FastReset">Set to "true" to reset immediately</param>
        private void ResetCountdown(bool _FastReset = false)
        {
            OnCounterResetEvent?.Invoke();
            timeTextField.color = defaultColor; 
            sliderImage.color = defaultColorSlider;
            sliderBackground.color = defaultColorSliderBackground;
            timeTextField.transform.localScale = Vector3.one;
            littleTimeLeftAnimation.Stop();
            
            
            if (GameController.GameState == GameState.Playing && !_FastReset)
            {
                playedAudioCueThisCycle = false;
                AudioSystem.StopVFX(VFX.OnBeforeShuffleTimerReset);
                AudioSystem.PlayVFX(VFX.OnShuffleCountdownReset);
            }

            if (_FastReset)
            {
                fastReset = true;
            }
            else
            {
                countDown = currentShuffleCountdownDuration;
                countdownSlider.maxValue = countDown;
                countdownSlider.value = countDown;
            }
        }

        private static void RobotRepairedTimeGain(RobotBehaviour _Robot)
        {
            AddToCountdown();
        }
        
        /// <summary>
        /// Increases the time of the Countdown.
        /// </summary>
        public static bool AddToCountdown(float? gained = null)
        {
            var time = gained ?? (Instance.countDown + GameConfig.AddToCountdown > Instance.currentShuffleCountdownDuration
                ? Instance.currentShuffleCountdownDuration - Instance.countDown
                : GameConfig.AddToCountdown);

            Instance.countDown += time;


            // if the countdown exceeds the value of timeLeftWhenSoundCueIsPlayed and we already played an audio cue this
            // cycle we we stop the audio cue and reset the playedAudioCueThisCycle value.
            Instance.ValidateAudioCue();
            
            OnTimeGained?.Invoke(time);
            
            return true;
        }

        /// <summary>
        /// if the countdown exceeds the value of timeLeftWhenSoundCueIsPlayed and we already played an audio cue this
        /// cycle we we stop the audio cue and reset the playedAudioCueThisCycle value.
        /// </summary>
        private void ValidateAudioCue()
        {
            if (playedAudioCueThisCycle && (countDown > timeLeftWhenSoundCueIsPlayed))
            {
                StopAudioCue();
            }
        }

        private void StopAudioCue(bool x = false)
        {
            AudioSystem.StopVFX(VFX.OnBeforeShuffleTimerReset);
            playedAudioCueThisCycle = false;
            littleTimeLeftAnimation.Stop();
            timeTextField.color = defaultColor; 
            sliderImage.color = defaultColorSlider;
            sliderBackground.color = defaultColorSliderBackground;
            timeTextField.transform.localScale = Vector3.one;
        }
        

        /// <summary>
        /// Reduces the time of the Countdown when a wrong RobotPart was selected.
        /// </summary>
        public static void RemoveFromCountdown(float? lost = null)
        {
            // WIP!
            
            var currentCountdown = Instance.countDown;
            var newCountdown =  currentCountdown / 1.3f;
            var difference = currentCountdown - newCountdown;
            Instance.countDown = newCountdown;
            
            //TODO: add and use GameConfig.RemoveFromCountdown variable.
            // var lost = Instance.countDown + GameConfig.AddToCountdown > GameConfig.ShuffleCountdownDuration
            //     ? GameConfig.ShuffleCountdownDuration - Instance.countDown
            //     : GameConfig.AddToCountdown;
            
            //TODO: last time I used this another robot (Aqua spawned with only one part missing)
            //Instance.countDown -= .5f;
            
            OnTimeLost?.Invoke(difference);
        }

        /// <summary>
        /// Gets the Sprites of all Robot Types active on the Map <br/>
        /// Only call this Method once, when the Map is started!
        /// </summary>
        /// <param name="_RobotPools">List of active Pools on this Map</param>
        public static void GetRobotParts(IEnumerable<RobotPool> _RobotPools)
        {
            Instance.partsList.Clear();
            Instance.freeParts.Clear();
            Instance.partsToDisable.Clear();
            Instance.missingPartsOnMap.Clear();
            Instance.missingPartsOnMap.typeCountDict.Clear();

            byte _arraySize = 0;

            foreach (var _pool in _RobotPools)
            {
                // Must start at 1 so the Robot stands won't be copied to the List
                for (byte i = 1; i < RobotPartConfig.RobotTypePrefabs[_pool.TypeIndex].RobotParts.Length; i++)
                {
                    Instance.partsList.Add(new Part(_Type: _pool.Type,
                        _RobotSprite: RobotPartConfig.RobotTypePrefabs[_pool.TypeIndex].RobotParts[i],
                        _RobotIndex: (sbyte) i, _PartsListIndex: _arraySize));
                    _arraySize++;
                }

                var _partIndex = new PartIndex();

                Instance.freeParts.Add(_pool.Type, new List<Part>());
                Instance.partsToDisable.Add(_pool.Type, new List<Part>());
                Instance.missingPartsOnMap.Add(_pool.Type, _partIndex);
                Instance.missingPartsOnMap.typeCountDict.Add(_pool.Type, new TypeCount());

                // Creates a new Dictionary for every Part this RobotType has (Starts at 1 so the RobotStand won't be added)
                for (sbyte i = 1; i < RobotPartConfig.RobotTypePrefabs[_pool.TypeIndex].RobotParts.Length; i++)
                {
                    _partIndex.Add(i, new PartCount(RobotPartConfig.RobotTypePrefabs[_pool.TypeIndex].RobotParts[i]));
                }
            }

            Instance.partsDisplayList = new List<Part>();

            Instance.ResetCountdown(true);
            if (Instance.gameStartedFirstTime)
            {
                Instance.gameStartedFirstTime = false;
                Instance.GetPartsToDisable();
            }
        }

        /// <summary>
        /// Picks random Parts from the Robot Types on the map to allow them to be disabled
        /// </summary>
        private void GetPartsToDisable()
        {
            // TODO: Ensure a min amount of disabled Parts for each Robot Type

            // Clears the Lists from the previous ShuffleCountdown
            freeParts.ForEach(_Kvp => _Kvp.Value.Clear());
            partsToDisable.ForEach(_Kvp => _Kvp.Value.Clear());
            partsDisplayList.Clear();

            // Picks the Parts that are currently not missing on any Robot
            foreach (var _part in partsList)
            {
                if ((object) _part.RobotSprite != null)
                {
                    freeParts[_part.Type].Add(_part);
                }
            }

            // Gets how many Parts are currently missing on the Map
            var _uniqueDisabledPartsCount = 0;
            for (byte i = 0; i < missingPartsOnMap.typeCountDict.Count; i++)
            {
                missingPartsOnMap.typeCountDict.ElementAt(i).Value.UniqueCount -=
                    missingPartsOnMap.typeCountDict.ElementAt(i).Value.TmpCount;
                missingPartsOnMap.typeCountDict.ElementAt(i).Value.TmpCount = 0;

                _uniqueDisabledPartsCount += missingPartsOnMap.typeCountDict.ElementAt(i).Value.UniqueCount;
            }

            for (byte i = 0; i < 8 - _uniqueDisabledPartsCount; i++)
            {
                // Which type to disable
                random = (byte) UnityEngine.Random.Range(MIN_CHANCE > MAX_CHANCE ? MAX_CHANCE : MIN_CHANCE,
                    MAX_CHANCE < MIN_CHANCE ? MIN_CHANCE : MAX_CHANCE);
                // Chance for each Robot Type
                // "MAX_CHANCE" must be cast as a float!
                keyChance = (float) MAX_CHANCE / freeParts.Count;
                keyIndex = (byte) Mathf.CeilToInt(random / keyChance);

                CheckDisabledPartsCount();

                // Which part to disable
                random = (byte) UnityEngine.Random.Range(MIN_CHANCE > MAX_CHANCE ? MAX_CHANCE : MIN_CHANCE,
                    MAX_CHANCE < MIN_CHANCE ? MIN_CHANCE : MAX_CHANCE);
                // Chance for each Part inside the List
                // "MAX_CHANCE" must be cast as a float!
                valueChance = (float) MAX_CHANCE / freeParts.ElementAt(keyIndex - 1).Value.Count;
                valueIndex = (byte) Mathf.CeilToInt(random / valueChance);

                // Adds this Part to the List of Parts that are allowed to be disabled
                partsToDisable[freeParts.ElementAt(keyIndex - 1).Key]
                    .Add(freeParts.ElementAt(keyIndex - 1).Value[valueIndex - 1]);
                // Increases the counter for this Robot Type
                missingPartsOnMap.typeCountDict[freeParts.ElementAt(keyIndex - 1).Key].UniqueCount++;
                missingPartsOnMap.typeCountDict[freeParts.ElementAt(keyIndex - 1).Key].TmpCount++;
                // Removes this Part from the List of pickable Parts, so it won't be picked again in this loop
                freeParts.ElementAt(keyIndex - 1).Value.RemoveAt(valueIndex - 1);

                // When there are no more free Parts to disable
                if (freeParts.ElementAt(keyIndex - 1).Value.Count == 0)
                {
                    break;
                }
            }

            // Adds the Parts that are currently missing on the Map to the List
            foreach (var _partIndex in missingPartsOnMap.Values)
            {
                foreach (var _partCount in _partIndex.Values)
                {
                    if (_partCount.Count <= 0) continue;
                    partsToDisable[_partCount.Part.Type].Add(_partCount.Part);
                }
            }

            // Parts that will be displayed in the Parts Displays
            foreach (var _part in partsToDisable.Values.SelectMany(_List => _List))
            {
                partsDisplayList.Add(new Part(_Type: _part.Type,
                    _RobotSprite: _part.RobotSprite,
                    _DisplaySprite: RobotPartConfig.UnlockedSkins[_part.Type]
                        .skinList[RobotPartConfig.RobotTypePrefabs.First(_Robot => _Robot.Type == _part.Type).SkinIndex]
                        .DisplaySprites[_part.RobotIndex - 1],
                    _RobotIndex: _part.RobotIndex));
            }

            EventController.PartsToDisableChanged();
            EventController.ShuffleParts(partsDisplayList.ShuffleList());
        }

        /// <summary>
        /// Checks if a Robot Type has reached the maximum number of allowed disabled Parts for its Type
        /// </summary>
        private void CheckDisabledPartsCount()
        {
            keyIterationCheck = 0;

            while (missingPartsOnMap.typeCountDict[freeParts.ElementAt(keyIndex - 1).Key].UniqueCount >
                8 / freeParts.Count || freeParts.ElementAt(keyIndex - 1).Value.Count <= 0)
            {
                keyIndex++;
                keyIndex = keyIndex > freeParts.Count ? (byte) 1 : keyIndex;

                keyIterationCheck++;

                // Prevents infinite loops
                if (keyIterationCheck <= freeParts.Keys.Count) continue;

                DebugLog.Red("Break CheckDisabledPartsCount");
                break;
            }
        }

        /// <summary>
        /// Adds a Robot Part to the List of currently missing Parts on the Map
        /// </summary>
        public static void AddToMissingParts(Part _Part)
        {
            //Instance.missingPartsOnMap[_Part.Type][_Part.RobotIndex].AddMissingPart(_Part);

            // If this is the first Part 
            if (Instance.missingPartsOnMap[_Part.Type][_Part.RobotIndex].Count == 0)
            {
                Instance.missingPartsOnMap[_Part.Type][_Part.RobotIndex].Part = _Part;
                Instance.missingPartsOnMap.typeCountDict[_Part.Type].UniqueCount += 1;

                // Sets the Sprite to "null" so there won't be a duplicate of it in the Displays
                Instance.partsList[_Part.PartsListIndex] = new Part(_Type: _Part.Type, null,
                    _RobotIndex: _Part.RobotIndex, _PartsListIndex: _Part.PartsListIndex);
            }

            Instance.missingPartsOnMap[_Part.Type][_Part.RobotIndex].Count++;
        }

        /// <summary>
        /// Removes a Robot Part from the List of currently missing Parts on the Map
        /// </summary>
        /// <param name="_Type">Type of the Robot</param>
        /// <param name="_RobotIndex">Index of that Part inside the Robot</param>
        public static void RemoveFromMissingParts(RobotType _Type, sbyte _RobotIndex)
        {
            //Instance.missingPartsOnMap[_Type][_RobotIndex].RemoveMissingPart(_Type, _RobotIndex);

            Instance.missingPartsOnMap[_Type][_RobotIndex].Count--;

            if (Instance.missingPartsOnMap[_Type][_RobotIndex].Count != 0) return;
            Instance.missingPartsOnMap.typeCountDict[_Type].UniqueCount -= 1;

            // Enables the Part in the "partsList"
            Instance.partsList[Instance.missingPartsOnMap[_Type][_RobotIndex].Part.PartsListIndex] = new Part(
                _Type: _Type,
                _RobotSprite: Instance.missingPartsOnMap[_Type][_RobotIndex].Part.RobotSprite,
                _RobotIndex: _RobotIndex,
                _PartsListIndex: Instance.missingPartsOnMap[_Type][_RobotIndex].Part.PartsListIndex);
        }

        /// <summary>
        /// Dictionary for the missing Robot Parts on the map
        /// </summary>
        [Serializable]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout, KeyLabel = "Robot Type",
            ValueLabel = "Index Count")]
        public class MissingParts : SerializableDictionaryBase<RobotType, PartIndex>
        {
            [NonSerialized]
            public Dictionary<RobotType, TypeCount> typeCountDict = new Dictionary<RobotType, TypeCount>();
        }

        /// <summary>
        /// (sbyte)Key = index the Robot Parts has inside the Robot <br/>
        /// (byte)Value = How often this Part is in the Dictionary
        /// </summary>
        [Serializable]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Part Index",
            ValueLabel = "Part Count", IsReadOnly = true)]
        public class PartIndex : SerializableDictionaryBase<sbyte, PartCount>
        {
        }

        /// <summary>
        /// Count of how many Parts of each Robot Type are missing on the Map
        /// </summary>
        public class TypeCount
        {
            #region Properties

            /// <summary>
            /// How many unique Parts are missing
            /// </summary>
            public byte UniqueCount { get; set; }

            /// <summary>
            /// Is incremented for each Part that is allowed to be disabled
            /// </summary>
            public byte TmpCount { get; set; }

            #endregion
        }

        /// <summary>
        /// Count of how many Parts of one specific type are missing
        /// </summary>
        [Serializable]
        public class PartCount
        {
            #region Inspector Fields

            [Tooltip("Sprite of the Part")] [LabelWidth(40)] [SerializeField] [ReadOnly]
            private Sprite sprite;

            [Tooltip("How often it is missing on the Map")] [LabelWidth(40)] [SerializeField] [ReadOnly]
            private byte count;

            #endregion

            #region Properties

            /// <summary>
            /// How often its missing on the Map
            /// </summary>
            public byte Count
            {
                get => count;
                set => count = value;
            }

            /// <summary>
            /// The Robot Part
            /// </summary>
            public Part Part { get; set; }

            #endregion

            /// <param name="_Sprite">Sprite of this Part</param>
            public PartCount(Sprite _Sprite)
            {
                sprite = _Sprite;
            }
        }

#if UNITY_EDITOR
        [Button]
        private void CountdownReset() => ResetCountdown(true);
#endif
    }
}