using QueueConnect.Development;
using QueueConnect.GameSystem;
using QueueConnect.Environment;
using QueueConnect.Robot;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ganymed.Utils.ExtensionMethods;
using UnityEngine;

namespace QueueConnect.Config
{
    /// <summary>
    /// Manages Robot types/skins
    /// </summary>
    [HideMonoScript]
    [CreateAssetMenu(menuName = "ConfigFiles/RobotPartConfig", fileName = "RobotPartConfig_SOB")]
    public class RobotPartConfig : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Robot Types", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning restore 649
            #endif
            [Tooltip("Set the Spawn-values for each RobotType (Only for Editor)")]
            [DictionaryDrawerSettings(KeyLabel = "Robot Type", ValueLabel = "Spawn-values")]
            [SerializeField] public RobotTypeSpawn robotSpawn = new RobotTypeSpawn();
            [Tooltip("All robot types (Prefabs)")]
            [SerializeField][ReadOnly] private List<RobotTypePrefab> robotTypePrefabs = new List<RobotTypePrefab>();
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Skins", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning restore 649
                [InlineButton(nameof(ChangeSkin),
                              "                                                                                                                                                                                                                                                                                                                                      " + 
                                   "ChangeSkin" +
                                   "                                                                                                                                                                                                                                                                                                                                      ")]
            #endif
            #pragma warning disable 649
            [SerializeField, LabelText(""), LabelWidth(25)] private RobotType skinTypeToChange;
            #pragma warning restore 649
            [Tooltip("The currently active skin for each Robot Type")]
            [DictionaryDrawerSettings(KeyLabel = "Robot Type", ValueLabel = "Skin ID", IsReadOnly = true)]
            [SerializeField] private ActiveSkin activeSkins = new ActiveSkin();
            [Tooltip("All available robot skins")]
            [DictionaryDrawerSettings(KeyLabel = "Robot Type", ValueLabel = "Skins", IsReadOnly = true)]
            [SerializeField] private SkinsDictionary skins = new SkinsDictionary();
            [Tooltip("All unlocked robot skins")]
            [DictionaryDrawerSettings(KeyLabel = "Robot Type", ValueLabel = "Skins", IsReadOnly = true)]
            [SerializeField] private SkinsDictionary unlockedSkins = new SkinsDictionary();
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("FilePaths", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title3;
                #pragma warning restore 649
            #endif
        
            [Tooltip("FilePath to the \"RobotPartConfig\"-ScriptableObject (Is set inside the Constructor)"), LabelText("RobotPartConfig FilePath")]
            [FilePath(ParentFolder = "Assets/Resources", Extensions = "asset", RequireExistingPath = true), BoxGroup("FilePaths", ShowLabel = false)]
            [ShowInInspector][ReadOnly] private static readonly string ROBOT_PART_CONFIG_FILEPATH;
            [Tooltip("FolderPath to the \"RobotTypePrefabs\"")]
            #pragma warning disable 649
            [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true), BoxGroup("FilePaths", ShowLabel = false)]
            [SerializeField] private string robotTypePrefabsFolderPath;
            
            [Tooltip("FolderPath to the \"Cyclops Robot Skins\"")]
            [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true), BoxGroup("FilePaths", ShowLabel = false)]
            [SerializeField] private string cyclopsSkinFolderPath;
           
            [Tooltip("FolderPath to the \"Aqua Robot Skins\"")]
            [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true), BoxGroup("FilePaths", ShowLabel = false)]
            [SerializeField] private string aquaSkinFolderPath;
            
            [Tooltip("FolderPath to the \"Aqua Robot Skins\"")]
            [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true), BoxGroup("FilePaths", ShowLabel = false)]
            [SerializeField] private string faunSkinFolderPath;
            #pragma warning restore 649
        #endregion
        
        #region Properties
            /// <summary>
            /// Singleton of "RobotPartConfig"
            /// </summary>
            public static RobotPartConfig Instance { get; private set; }
            /// <summary>
            /// List of all robot types (Prefabs)
            /// </summary>
            public static List<RobotTypePrefab> RobotTypePrefabs => Instance.robotTypePrefabs;
            /// <summary>
            /// All unlocked robot skins
            /// </summary>
            public static SkinsDictionary UnlockedSkins => Instance.unlockedSkins;
        #endregion

        static RobotPartConfig()
        {
            ROBOT_PART_CONFIG_FILEPATH = "Config/RobotPartConfig_SOB.asset";
        }

        #if UNITY_EDITOR
            private void OnEnable()
            {
                foreach (var _type in Enum.GetValues(typeof(RobotType)).Cast<RobotType>())
                {
                    if (!robotSpawn.ContainsKey(_type))
                    {
                        robotSpawn.Add(_type, new RobotTypePrefab.RobotSpawn());
                    }
                }
            }        
        #endif

        /// <summary>
        /// Initializes this ScriptableObject<br/>
        /// Is called after the "OnEnable"-Method and before the "Start"-Method<br/>
        /// <b>Method must be static!</b>
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Instance = Singleton.Persistent(Instance, ROBOT_PART_CONFIG_FILEPATH.Substring(0, ROBOT_PART_CONFIG_FILEPATH.IndexOf('.')));
            LoadRobotSkins();
            Instance.LoadRobotTypes();
            Instance.InitializeRobotSkins();
        }

        /// <summary>
        /// Loads all robot types (Prefabs) from the Resources folder and saves them in the "robotTypePrefabs"-List
        /// </summary>
        private void LoadRobotTypes()
        {
            Sprite[] _tmpDisplayParts = null;
            ushort _tmpSkinIndex = 0;
            robotTypePrefabs = new List<RobotTypePrefab>();

            var _robots = Resources.LoadAll<RobotBehaviour>(robotTypePrefabsFolderPath);

            foreach (var _robot in _robots)
            {
                for (ushort i = 0; i < unlockedSkins[_robot.Type].skinList.Count; i++)
                {
                    if (unlockedSkins[_robot.Type].skinList[i].SkinID != activeSkins[_robot.Type]) continue;

                        _tmpSkinIndex = i;
                        _tmpDisplayParts = unlockedSkins[_robot.Type].skinList[i].DisplaySprites;
                        break;
                }
                
                robotTypePrefabs.Add(new RobotTypePrefab(_Type:         _robot.Type,
                                                              _Prefab:       _robot.gameObject,
                                                              _RobotScript:  _robot,
                                                              _Parts:        _robot.Parts,
                                                              _SkinIndex:    _tmpSkinIndex,
                                                              _DisplayParts: _tmpDisplayParts));
                
                // Index, this Robot Type has in the "robotTypePrefabs"-List
                _robot.TypeIndex = (short)(robotTypePrefabs.Count - 1);
            }
        }

        /// <summary>
        /// Loads all available robot skins and adds the unlocked skins to the "unlockedSkins" list
        /// </summary>
        public static async void LoadRobotSkins()
        {
            if (!Instance)
            {
                var source = new CancellationTokenSource();
                var ct = source.Token;

                var getInstance = Task.Run(delegate
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        while (Instance == null)
                        {
                        }
                    }
                    catch
                    {
                        Debug.Log($"{nameof(LoadRobotSkins)} Timeout");
                    }
                }, ct);
                var timeOut = Task.Delay(1000, ct);
                
                var completed = await Task.WhenAny(getInstance, timeOut);

                await completed.Then(delegate
                {
                    source.Cancel();
                    source.Dispose();
                });
                if(!Instance) return;
            }

            // Paths to the Skins of all Robot Types concatenate
            var _allSkins = 
                Resources.LoadAll<RobotParts>(Instance.cyclopsSkinFolderPath).Concat(
                Resources.LoadAll<RobotParts>(Instance.aquaSkinFolderPath)).Concat(
                Resources.LoadAll<RobotParts>(Instance.faunSkinFolderPath));

            Instance.skins = new SkinsDictionary();
            Instance.unlockedSkins = new SkinsDictionary();

            foreach (var _skin in _allSkins)
            {
                // All skins
                AddSkinToDictionary(Instance.skins, _skin);

                // TODO: Load unlocked skins from users saved data

                // Unlocked skins
                if (_skin.Unlocked)
                {
                    AddSkinToDictionary(Instance.unlockedSkins, _skin);
                }
                
                UpdateActiveSkin(_skin.Type, 1);
            }
        }

        /// <summary>
        /// Adds a Robot skin to the respective Dictionary if it's not already inside
        /// </summary>
        /// <param name="_Dictionary">The Dictionary to add the skin to</param>
        /// <param name="_Skin">The Robot skin</param>
        private static void AddSkinToDictionary(SkinsDictionary _Dictionary, RobotParts _Skin)
        {
            // When the Robot Type already exists in the Dictionary
            if (_Dictionary.ContainsKey(_Skin.Type))
            {
                _Dictionary[_Skin.Type].skinList.Add(_Skin);
            }
            // When the Robot Type doesn't exist in the Dictionary
            else
            {
                _Dictionary.Add(_Skin.Type, new SkinList());
                _Dictionary[_Skin.Type].skinList.Add(_Skin);
            }
        }

        /// <summary>
        /// Updates the Skin of each Robot Type to the currently active one when the game loads
        /// </summary>
        private void InitializeRobotSkins()
        {
            foreach (var _prefab in robotTypePrefabs)
            {
                ChangeRobotSkin(_prefab, activeSkins[_prefab.Type]);
            }
        }

        /// <summary>
        /// Changes the Skin of the passed Robot
        /// </summary>
        /// <param name="_Prefab">Robot Type to change the Skin of</param>
        /// <param name="_SkinId">Id of the targeted skin</param>
        private static void ChangeRobotSkin(RobotTypePrefab _Prefab, ushort _SkinId, bool gameStart = false)
        {
            List<RobotParts> _skinList = null;
            short _skinIndex = -1;

            // TODO: Check if the player has unlocked a skin before this "ChangeRobotSkin()"-Method

            if (Instance.unlockedSkins.ContainsKey(_Prefab.Type))
            {
                _skinList = Instance.unlockedSkins[_Prefab.Type].skinList;

                // Checks if the player has unlocked the skin of the passed type
                for (ushort i = 0; i < _skinList.Count; i++)
                {
                    if (_skinList[i].SkinID != _SkinId) continue;
                        _skinIndex = (short)i;
                        break;
                }
            }

            // Won't change the skin if it's not unlocked
            if (_skinIndex == -1)
            {
                DebugLog.Red_White_Red_White("Couldn't find SkinId:", $"{_SkinId.ToString()}", "for RobotType:", $"{_Prefab.Type}");
            }
            else
            {
                var _previousValue = true;
                
                if (gameStart)
                {
                    _previousValue = GameConfig.SpawnRobots;

                    RobotScanner.DestroyAllRobots(false);   
                }

                for (byte i = 0; i < _Prefab.RobotParts.Length; i++)
                {
                    // Changes the Reference Skin for this Robot Type
                    _Prefab.RobotParts[i] = _skinList?[_skinIndex].RobotSprites[i];
                    Instance.robotTypePrefabs[_Prefab.RobotScript.TypeIndex].SkinIndex = (ushort)_skinIndex;

                    if (gameStart)
                    {
                        // Only changes the Skins of the Robots inside the Pool, if it already exists
                        if (GameController.Instance == null || PoolController.RobotPools.Count < _Prefab.RobotScript.PoolIndex + 1) continue;
                        
                            foreach (var _robot in PoolController.RobotPools[_Prefab.RobotScript.PoolIndex].Pool.AllObjects)
                            {
                                if (((RobotBehaviour)_robot.Component).Parts[i].sprite != null)
                                {
                                    ((RobotBehaviour)_robot.Component).Parts[i].sprite = _skinList?[_skinIndex].RobotSprites[i];
                                }
                            }   
                    }
                }

                UpdateActiveSkin(_Prefab.Type, _SkinId);

                if (gameStart)
                {
                    GameConfig.SetRobotSpawn(_previousValue);
                }
            }
        }

        /// <summary>
        /// Updates the "activeSkins"-Dictionary with the currently active skin for the passed Robot Type
        /// </summary>
        /// <param name="_Type">Robot Type</param>
        /// <param name="_SkinId">Id of the targeted skin</param>
        /// <param name="_ConstructorCall">Set this to "true" when the Method is called from a "RobotTypePrefab"-Constructor</param>
        public static void UpdateActiveSkin(RobotType _Type, ushort _SkinId, bool _ConstructorCall = false)
        {
            // Adds the new Robot Type to the Dictionary
            if (!Instance.activeSkins.ContainsKey(_Type))
            {
                Instance.activeSkins.Add(_Type, _SkinId);
            }
            // Updates the Dictionary
            else
            {
                if (!_ConstructorCall)
                {
                    Instance.activeSkins[_Type] = _SkinId;
                }
            }
        }

        /// <summary>
        /// Dictionary for all Robot skins of a specific Robot Type
        /// </summary>
        [Serializable]
        public class SkinsDictionary : SerializableDictionaryBase<RobotType, SkinList> { }

        /// <summary>
        /// Dataclass for the "SkinsDictionary"-Class
        /// </summary>
        [Serializable]
        public class SkinList
        {
            [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
            [SerializeField] public List<RobotParts> skinList = new List<RobotParts>();

            private void Add_Remove_Skin()
            {
                DebugLog.Black($"{this}");
            }
        }

        /// <summary>
        /// Dictionary for the currently active skin of each Robot Type
        /// </summary>
        [Serializable]
        private class ActiveSkin : SerializableDictionaryBase<RobotType, ushort> { }
        
        /// <summary>
        /// Set the Spawn-values for each RobotType (Only for Editor)
        /// </summary>
        [Serializable]
        public class RobotTypeSpawn : SerializableDictionaryBase<RobotType, RobotTypePrefab.RobotSpawn> { }

        /// <summary>
        /// Spawn-values for the Robot Types (Editor only)
        /// </summary>
        public static RobotTypeSpawn RobotSpawn => Instance.robotSpawn;
        
        /// <summary>
        /// Changes the Skin of a Robot Type (Works only in Editor)
        /// </summary>
        private void ChangeSkin()
        {
            if (!Application.isPlaying) return;
            
                ushort _skinId = 1;
                RobotTypePrefab _tmpRobotTypePrefab = null;
                
                foreach (var _robotTypePrefab in robotTypePrefabs)
                {
                    if (_robotTypePrefab.Type != skinTypeToChange) continue;
                        _tmpRobotTypePrefab = _robotTypePrefab;
                        break;
                }

                foreach (var _robotType in activeSkins)
                {
                    if (_robotType.Key == skinTypeToChange)
                    {
                        _skinId = _robotType.Value;
                    }
                }

                ChangeRobotSkin(_tmpRobotTypePrefab, _skinId);
                
                RobotSpawner.SelectRobotTypesOnMap(PoolController.RobotPools);
        }
    }

    /// <summary>
    /// Dataclass for the different robot types (Prefabs) 
    /// </summary>
    [Serializable]
    public class RobotTypePrefab
    {
        #region Inspector Fields
            [Tooltip("Robot Type of this Prefab")]
            [SerializeField][ReadOnly] private RobotType type;
            [Tooltip("Prefab of this robot type")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private GameObject prefab;
            [Tooltip("\"RobotBehaviour\" script of this Prefab")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private RobotBehaviour robotScript;
            [Tooltip("Additional spawn chance for this Robot Type")]
            [SerializeField][ReadOnly] private RobotSpawn spawnChance = new RobotSpawn();
            [Tooltip("Index the currently active skin of that Type has in the List of unlocked Skins")]
            [SerializeField][ReadOnly] private ushort skinIndex;
            [Tooltip("All Parts of this Robot")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private Sprite[] robotParts;
            [Tooltip("Sprites for the Displays")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private Sprite[] displayParts;
        #endregion

        #region Properties
            /// <summary>
            /// "RobotType" of this prefab
            /// </summary>
            public RobotType Type { get => type; private set => type = value; }
            /// <summary>
            /// Prefab of that robot type
            /// </summary>
            public GameObject Prefab { get => prefab; private set => prefab = value; }
            /// <summary>
            /// "RobotBehaviour" script of this prefab
            /// </summary>
            public RobotBehaviour RobotScript { get => robotScript; private set => robotScript = value; }
            /// <summary>
            /// Additional spawn chance for this Robot Type
            /// </summary>
            public RobotSpawn SpawnChance => spawnChance;
            /// <summary>
            /// Index the currently active skin of that Type has in the List of unlocked Skins
            /// </summary>
            public ushort SkinIndex { get => skinIndex; set => skinIndex = value; }
            /// <summary>
            /// All Parts of this Robot
            /// </summary>
            public Sprite[] RobotParts { get => robotParts; private set => robotParts = value; }
            /// <summary>
            /// Sprites for the Displays
            /// </summary>
            public Sprite[] DisplayParts { get => displayParts; private set => displayParts = value; }
        #endregion

        /// <param name="_Type">"RobotType" of this prefab</param>
        /// <param name="_Prefab">Prefab of this robot</param>
        /// <param name="_RobotScript">"RobotBehaviour" script of the prefab</param>
        /// <param name="_SkinIndex">Index the currently active skin of that Type has in the List of unlocked Skins</param>
        /// <param name="_Parts">SpriteRenderer of the Robot Parts</param>
        /// <param name="_DisplayParts">Sprites for the Displays</param>
        public RobotTypePrefab(RobotType _Type, GameObject _Prefab, RobotBehaviour _RobotScript, ushort _SkinIndex, IReadOnlyList<SpriteRenderer> _Parts, Sprite[] _DisplayParts)
        {
            this.type = _Type;
            this.prefab = _Prefab;
            this.robotScript = _RobotScript;
            this.skinIndex = _SkinIndex;

            // So the Sprite names are shown in the Inspector instead of the SpriteRenderers
            this.robotParts = new Sprite[_Parts.Count];
            for (byte i = 0; i < _Parts.Count; i++)
            {
                this.robotParts[i] = _Parts[i].sprite;
            }

            this.displayParts = _DisplayParts;

            RobotPartConfig.UpdateActiveSkin(_Type, 1, true);
        }

        /// <summary>
        /// Needs to be in a separate Class so the value is passed by reference
        /// </summary>
        [Serializable]
        public class RobotSpawn
        {
            #region Inspector Fields
                [Tooltip("Is this RobotType allowed to be spawned on the current map")]
                [SerializeField] public bool allowedOnMap = true;
                [Tooltip("Is this RobotType currently active")]
                #if UNITY_EDITOR
                    [OnValueChanged(nameof(UpdateRobotSpawn))]
                #endif
                [SerializeField] private bool currentlyActive = true;
                [Tooltip("Additional spawn chance for this Robot Type")]
                #if UNITY_EDITOR
                    [OnValueChanged(nameof(UpdateRobotSpawn))]
                #endif
                [SerializeField] private ushort spawnChance = 100;
            #endregion

            #region Properties
                /// <summary>
                /// Is this RobotType currently active
                /// </summary>
                public bool CurrentlyActive
                {
                    get => currentlyActive;
                    set
                    {
                        currentlyActive = value;
                        UpdateRobotSpawn();
                    }
                }
                /// <summary>
                /// Additional spawn chance for this Robot Type
                /// </summary>
                public ushort SpawnChance
                {
                    get => spawnChance;
                    set
                    {
                        spawnChance = value;
                        UpdateRobotSpawn();
                    }
                }
            #endregion

            /// <summary>
            /// Updates the SpawnValues of the Robot Types 
            /// </summary>
            private void UpdateRobotSpawn()
            {
                if (!Application.isPlaying) return;
                
                    foreach (var _kvp in RobotPartConfig.Instance.robotSpawn)
                    {
                        foreach (var _robotType in RobotPartConfig.RobotTypePrefabs)
                        {
                            if (_kvp.Key != _robotType.Type) continue;
                            _robotType.SpawnChance.currentlyActive = _kvp.Value.currentlyActive;
                            _robotType.SpawnChance.spawnChance = _kvp.Value.spawnChance;
                        }
                    }

                    RobotSpawner.UpdateRobotSpawn();
            }
        }
    }

    /// <summary>
    /// Dataclass for references to Robot Parts
    /// </summary>
    [Serializable]
    public struct Part
    {
        #region Inspector Fields
            [Tooltip("To which Robot Type this part belongs")]
            [SerializeField][ReadOnly] private RobotType type;
            [Tooltip("The Sprite of the Robot Part inside the Robot")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private Sprite robotSprite;
            [Tooltip("The Sprite of the Robot Part inside the Display")]
            [AssetsOnly]
            [SerializeField][ReadOnly] private Sprite displaySprite;
            [Tooltip("Index this Sprite has in this Robot Type")]
            [SerializeField][ReadOnly] private sbyte robotIndex;
        #endregion

        #region Properties
            /// <summary>
            /// To which Robot Type this part belongs
            /// </summary>
            public RobotType Type { get => type; private set => type = value; }
            /// <summary>
            /// The Sprite of the Robot Part inside the Robot
            /// </summary>
            public Sprite RobotSprite { get => robotSprite; set => robotSprite = value; }
            /// <summary>
            /// The Sprite of the Robot Part inside the Display
            /// </summary>
            public Sprite DisplaySprite { get => displaySprite; set => displaySprite = value; }
            /// <summary>
            /// Index this Sprite has in this Robot Type
            /// </summary>
            public sbyte RobotIndex { get => robotIndex; private set => robotIndex = value; }
            /// <summary>
            /// Index this Part has in the "PartsList"
            /// </summary>
            public byte PartsListIndex { get; set; }
        #endregion

        /// <param name="_Type">To which Robot Type this part belongs</param>
        /// <param name="_RobotSprite">The Sprite of the Robot Part inside the Robot</param>
        /// <param name="_DisplaySprite">The Sprite of the Robot Part inside the Display</param>
        /// <param name="_RobotIndex">Index this Sprite has in this Robot Type</param>
        public Part(RobotType _Type, Sprite _RobotSprite, Sprite _DisplaySprite, sbyte _RobotIndex) : this()
        {
            this.type = _Type;
            this.robotSprite = _RobotSprite;
            this.displaySprite = _DisplaySprite;
            this.robotIndex = _RobotIndex;
            this.PartsListIndex = 0;
        }

        /// <param name="_Type">To which Robot Type this part belongs</param>
        /// <param name="_RobotSprite">The Sprite of the Robot Part</param>
        /// <param name="_RobotIndex">Index this Part has in its Robot Type</param>
        /// <param name="_PartsListIndex">Index this Part has in the "PartsList"</param>
        public Part(RobotType _Type, Sprite _RobotSprite, sbyte _RobotIndex, byte _PartsListIndex) : this()
        {
            this.type = _Type;
            this.robotSprite = _RobotSprite;
            this.robotIndex = _RobotIndex;
            this.PartsListIndex = _PartsListIndex;
        }
    }
}