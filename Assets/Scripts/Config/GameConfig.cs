using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using QueueConnect.Robot;
using UnityEditor;
using UnityEngine;

namespace QueueConnect.Config
{
    /// <summary>
    /// Manages the game settings
    /// </summary>
    [HideMonoScript]
    [CreateAssetMenu(menuName = "ConfigFiles/GameConfig", fileName = "GameConfig_SOB")]
    public class GameConfig : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Spawn", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning restore 649
            #endif
            [Tooltip("Whether the spawning of Robots is disabled/enabled")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField] private bool spawnRobots = true;
            [Tooltip("Delay in between each Robot spawn")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField] private float baseSpawnDelay = 5f;
            [Tooltip("How many Robot Prefabs of each type are instantiated at start")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField] private byte robotPoolStartCount = 5;
            [Header("Parts")] 
            [Tooltip("Default amount of RobotParts to disable on each Robot")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField] private Vector2 defaultAmountToDisable = new Vector2(1, 2);
            [Tooltip("Minimum number of disabled Sprites (Use the \"PartAmountToDisable\"-Method from the \"GameController.cs\" to change the value)")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField][ReadOnly] private byte minAmountToDisable = 2;
            [Tooltip("Maximum number of disabled Sprites (Use the \"PartAmountToDisable\"-Method from the \"GameController.cs\" to change the value)")]
            [BoxGroup("Spawn", ShowLabel = false)]
            [SerializeField][ReadOnly] private byte maxAmountToDisable = 2;
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Robots", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title3;
                #pragma warning restore 649
            #endif
            [Tooltip("Value at game start")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float baseRobotSpeed = 2000f;
            [Tooltip("Is gradually added to the RobotSpeed over the course of a Minute")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float baseSpeedIncrement = 266.6667f;
            [Tooltip("Speed the robots move with when the Game is restarting")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float maxRobotSpeed = 10000f;
            [Tooltip("Linear Drag of the Robots")]
            [BoxGroup("Robots", ShowLabel = false)]
            #if UNITY_EDITOR
                [OnValueChanged(nameof(SetRobotDrag))]
            #endif
            [SerializeField] private float robotDrag = 100f;
            [Tooltip("How fast the Robots accelerate when they're released by the Lift")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float accelerationStep = 10f;
            [Tooltip("Gravity of the Robots")]
            [BoxGroup("Robots", ShowLabel = false)]
            #if UNITY_EDITOR
                [OnValueChanged(nameof(SetRobotGravity))]
            #endif
            [SerializeField] private float robotGravity = 2000f;
            [Header("Explosion")]
            [Tooltip("How long the Robot shakes, before it explodes")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float shakeDuration = .25f;
            [Tooltip("How far the Parts move from its origin position")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private Vector2 shakeAmount = new Vector2(-1.25f, 1.25f);
            [BoxGroup("Robots", ShowLabel = false)]
            [Tooltip("Mass of each Robot Part")]
            [SerializeField] private float partMass = 1f;
            [BoxGroup("Robots", ShowLabel = false)]
            [Tooltip("Gravity of each Robot Part")]
            [SerializeField] private float partGravity = 125f;
            [Tooltip("Force/Speed of the explosion")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private float explosionForce = 450f;
            [Tooltip("Speed, the exploded Parts rotate with")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private Vector2 rotationSpeed = new Vector2(-.025f, .025f);
            [Tooltip("What \"ForceMode\" to use for the explosion")]
            [BoxGroup("Robots", ShowLabel = false)]
            [SerializeField] private ForceMode2D forceMode = ForceMode2D.Impulse;
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("ShuffleCountdown", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning restore 649
            #endif
            [Tooltip("Time in seconds, until the next Robot Part shuffle")]
            [BoxGroup("ShuffleCountdown", ShowLabel = false)]
            [SerializeField] private float defaultShuffleCountdownDuration = 15f;
            [Tooltip("Time in seconds, the Slider needs to deplete during a reset")]
            [BoxGroup("ShuffleCountdown", ShowLabel = false)]
            [SerializeField] private float resetCountdownDuration = .28125f;
            [Tooltip("Will be added to the Countdown when a Robot Part has been repaired")]
            [BoxGroup("ShuffleCountdown", ShowLabel = false)]
            [SerializeField] private float addToCountdown = .5f;
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Robot Scanner", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title4;
                #pragma warning restore 649
            #endif
            [Tooltip("Speed the Scanner moves with")]
            [BoxGroup("Robot Scanner", ShowLabel = false)]
            [SerializeField] private float scannerSpeed = 10f;
            [Tooltip("X: min delay | Y: max delay, before the Scanner moves again")]
            [BoxGroup("Robot Scanner", ShowLabel = false)]
            [SerializeField] private Vector2 randomMovementDelay = new Vector2(1.25f, 2.5f);
            [Tooltip("How fast the LightBeam changes its color (Higher number = faster)")]
            [BoxGroup("Robot Scanner", ShowLabel = false)]
            [SerializeField] private float fadeToError = .2f;
            [Tooltip("How fast the LightBeam changes its color (Higher number = faster)")]
            [BoxGroup("Robot Scanner", ShowLabel = false)]
            [SerializeField] private float fadeToNormal = .1f;
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Repair Arm", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title5;
                #pragma warning restore 649
            #endif
            [Tooltip("Speed the RepairArm moves with")]
            [BoxGroup("RepairArm", ShowLabel = false)]
            #pragma warning disable 649
            [SerializeField] private float repairArmSpeed = 400f;
            #pragma warning restore 649
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("PlayerLife", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title6;
                #pragma warning restore 649
            #endif
            [Tooltip("Player won't lose any life if set to \"true\"")]
            [BoxGroup("PlayerLife", ShowLabel = false)]
            #pragma warning disable 649
            [SerializeField] private bool invincible;
            [Tooltip("Player won't lose any life when a wrong Robot part is selected")]
            [BoxGroup("PlayerLife", ShowLabel = false)]
            [SerializeField] private bool allowMistakes;
            #pragma warning restore 649
            [Tooltip("How often the Light will flicker, smaller range = faster flickering")]
            [BoxGroup("PlayerLife", ShowLabel = false)]
            [SerializeField] private Vector2 flickerSteps = new Vector2(-100f, 100f);
            [Tooltip("Duration of the flicker")]
            [BoxGroup("PlayerLife", ShowLabel = false)]
            [SerializeField] private float flickerDuration = 25f;
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("FilePath", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title7;
                #pragma warning restore 649
            #endif
            [Tooltip("FilePath to the \"GameConfig\"-ScriptableObject (Is set inside the Constructor)"), LabelText("GameConfig FilePath")]
            [FilePath(ParentFolder = "Assets/Resources", Extensions = "asset", RequireExistingPath = true)]
            [ShowInInspector][ReadOnly] private static readonly string GAME_CONFIG_FILEPATH;
        #endregion

        #region Privates
            private static GameConfig instance;
        #endregion

        #region Properties
            /// <summary>
            /// Width of the visible Map
            /// </summary>
            public static float MapWidth { get; set; }
            /// <summary>
            /// Height of the visible Map
            /// </summary>
            public static float MapHeight { get; set; }
            /// <summary>
            /// Center point of the Map
            /// </summary>
            public static Vector3 MapCenter { get; set; }
            /// <summary>
            /// Whether the spawning of Robots is disabled/enabled
            /// </summary>
            public static bool SpawnRobots => instance.spawnRobots;
            /// <summary>
            /// Delay in between each Robot spawn
            /// </summary>
            public static float BaseSpawnDelay => instance.baseSpawnDelay;
            /// <summary>
            /// How many Robot Prefabs of each type are instantiated at start
            /// </summary>
            public static byte RobotPoolStartCount => instance.robotPoolStartCount;
            /// <summary>
            /// Time in seconds until the next Robot Part shuffle
            /// </summary>
            public static float DefaultShuffleCountdownDuration => instance.defaultShuffleCountdownDuration;
            /// <summary>
            /// Time in seconds, the Slider needs to deplete during a reset
            /// </summary>
            public static float ResetCountdownDuration => instance.resetCountdownDuration;
            /// <summary>
            /// Will be added to the Countdown when a Robot Part has been repaired
            /// </summary>
            public static float AddToCountdown => instance.addToCountdown;
            /// <summary>
            /// Default amount of RobotParts to disable on each Robot
            /// </summary>
            public static Vector2 DefaultAmountToDisable => instance.defaultAmountToDisable;
            /// <summary>
            /// Minimum number of disabled Sprites (Use the "PartAMountToDisable"-Method from the "GameController.cs" to change the value)
            /// </summary>
            public static byte MinAmountToDisable => instance.minAmountToDisable;
            /// <summary>
            /// Maximum number of disabled Sprites (Use the "PartAMountToDisable"-Method from the "GameController.cs" to change the value)
            /// </summary>
            public static byte MaxAmountToDisable => instance.maxAmountToDisable;
            /// <summary>
            /// Value at game start
            /// </summary>
            public static float BaseRobotSpeed => instance.baseRobotSpeed;
            /// <summary>
            /// Is gradually added to the RobotSpeed over the course of a Minute
            /// </summary>
            public static float BaseSpeedIncrement => instance.baseSpeedIncrement; 
            /// <summary>
            /// Speed limit for easy difficulty
            /// </summary>
            public static float EasySpeedLimit { get; private set; }
            /// <summary>
            /// Speed limit for medium difficulty
            /// </summary>
            public static float MediumSpeedLimit { get; private set; }
            /// <summary>
            /// Speed limit for hard difficulty
            /// </summary>
            public static float HardSpeedLimit { get; private set; }
            /// <summary>
            /// Speed the Robots move with when the Game is restarting
            /// </summary>
            public static float MaxRobotSpeed => instance.maxRobotSpeed;
            /// <summary>
            /// Linear Drag of the Robots
            /// </summary>
            public static float RobotDrag => instance.robotDrag;
            /// <summary>
            /// How fast the Robots accelerate when they're released by the Lift
            /// </summary>
            public static float AccelerationStep => instance.accelerationStep;
            /// <summary>
            /// Gravity of the Robots
            /// </summary>
            public static float RobotGravity => instance.robotGravity;
            /// <summary>
            /// How long the Robot shakes before it explodes
            /// </summary>
            public static float ShakeDuration => instance.shakeDuration;
            /// <summary>
            /// How far the Parts move from its origin position
            /// </summary>
            public static Vector2 ShakeAmount => instance.shakeAmount;
            /// <summary>
            /// Mass of each Robot Part
            /// </summary>
            public static float PartMass => instance.partMass;
            /// <summary>
            /// Gravity of each Robot Part
            /// </summary>
            public static float PartGravity => instance.partGravity;
            /// <summary>
            /// Force/Speed of the explosion
            /// </summary>
            public static float ExplosionForce => instance.explosionForce;
            /// <summary>
            /// What "ForceMode" to use for the explosion
            /// </summary>
            public static ForceMode2D ForceMode => instance.forceMode;
            /// <summary>
            /// Force of the rotation
            /// </summary>
            public static Vector2 RotationSpeed => instance.rotationSpeed;
            /// <summary>
            /// Speed the Scanner moves with
            /// </summary>
            public static float ScannerSpeed => instance.scannerSpeed;
            /// <summary>
            /// X: min delay | Y: max delay, before the Scanner moves again
            /// </summary>
            public static Vector2 RandomMovementDelay => instance.randomMovementDelay;
            /// <summary>
            /// How fast the LightBeam changes its color (Higher number = faster)
            /// </summary>
            public static float FadeToError => instance.fadeToError;
            /// <summary>
            /// How fast the LightBeam changes its color (Higher number = faster)
            /// </summary>
            public static float FadeToNormal => instance.fadeToNormal;
            /// <summary>
            /// Speed the RepairArm moves with
            /// </summary>
            public static float RepairArmSpeed => instance.repairArmSpeed;
            /// <summary>
            /// Player won't lose any life if set to "true"
            /// </summary>
            public static bool Invincible { get => instance.invincible; set => instance.invincible = value;}
            /// <summary>
            /// Player won't lose any life when a wrong Robot part is selected
            /// </summary>
            public static bool AllowMistakes => instance.allowMistakes;
            /// <summary>
            /// How often the Light will flicker
            /// </summary>
            public static Vector2 FlickerStep => instance.flickerSteps;
            /// <summary>
            /// Duration of the flicker
            /// </summary>
            public static float FlickerDuration => instance.flickerDuration;
        #endregion

        static GameConfig()
        {
            GAME_CONFIG_FILEPATH = "Config/GameConfig_SOB.asset";
        }

        private GameConfig()
        {
            EasySpeedLimit = baseRobotSpeed + ((maxRobotSpeed - baseRobotSpeed) / 3);
            MediumSpeedLimit = baseRobotSpeed + (((maxRobotSpeed - baseRobotSpeed) / 3) * 2);
            HardSpeedLimit = baseRobotSpeed + (((maxRobotSpeed - baseRobotSpeed) / 3) * 3);
        }

        private void OnEnable()
        {
            minAmountToDisable = (byte)defaultAmountToDisable.x;
            maxAmountToDisable = (byte)defaultAmountToDisable.y;
        }

        /// <summary>
        /// Initializes this ScriptableObject<br/>
        /// Is called after the "OnEnable"-Method and before the "Start"-Method <br/>
        /// <b>Method must be static!</b>
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            instance = Singleton.Persistent(instance, GAME_CONFIG_FILEPATH.Substring(0, GAME_CONFIG_FILEPATH.IndexOf('.')));
        }
        
        /// <summary>
        /// Enables/Disabled the Robot spawn
        /// </summary>
        /// <param name="_Value">"true" = enabled, "false" = disabled</param>
        public static void SetRobotSpawn(bool _Value)
        {
            instance.spawnRobots = _Value;
        }
        
        /// <summary>
        /// Sets a new Min/Max amount of Robot Parts to be disabled (Actual disabled Part amount depends on how many Parts of that Robot Type are on the Displays!)
        /// </summary>
        /// <param name="_MinAmount">Minimum amount to be disabled</param>
        /// <param name="_MaxAmount">Maximum amount to be disabled</param>
        public static void RobotPartsToDisable(byte? _MinAmount = null, byte? _MaxAmount = null)
        {
            instance.minAmountToDisable = _MinAmount ?? instance.minAmountToDisable;
            instance.maxAmountToDisable = _MaxAmount != null ? (_MaxAmount.Value < instance.minAmountToDisable ? instance.minAmountToDisable : _MaxAmount.Value) : instance.maxAmountToDisable;

            EventController.PartsToDisableChanged();
        }
        
        #if UNITY_EDITOR
            /// <summary>
            /// Sets the LinearDrag of the Robots
            /// </summary>
            private void SetRobotDrag()
            {
                if (!EditorApplication.isPlaying) return;
                
                    foreach (var _robot in PoolController.RobotPools.SelectMany(_Pool => _Pool.Pool.AllObjects))
                    {
                        if (((RobotBehaviour)_robot.Component).EditorRigidbody2D.drag != 0)
                        {
                            ((RobotBehaviour)_robot.Component).EditorRigidbody2D.drag = robotDrag;   
                        }
                    }
            }
            
            /// <summary>
            /// Sets the Gravity of the Robots
            /// </summary>
            private void SetRobotGravity()
            {
                if (!EditorApplication.isPlaying) return;
                
                    foreach (var _robot in PoolController.RobotPools.SelectMany(_Pool => _Pool.Pool.AllObjects))
                    {
                        ((RobotBehaviour)_robot.Component).EditorRigidbody2D.gravityScale = robotGravity;
                    }
            }
        #endif
    }

    #if UNITY_EDITOR
        /// <summary>
        /// Empty class to display the "Title"-Attribute <br/>
        /// Works only in Editor!
        /// </summary>
        [Serializable]
        public class Title : MonoBehaviour 
        {
            // Copy this to create a Title
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Title", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title;
                #pragma warning restore 649
            #endif
        }
    #endif
}