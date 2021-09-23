using QueueConnect.Environment;
using QueueConnect.GameSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueConnect.Attributes;
using UnityEngine;
using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SoundSystem;
using SoundSystem.Core;
using Random = UnityEngine.Random;

namespace QueueConnect.Robot
{
    /// <summary>
    /// Robot behaviour
    /// </summary>
    [HideMonoScript]
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(RepairParticleMoveDirection))]
    public class RobotBehaviour : MonoBehaviour
    {
        #region Inspector Fields

        [SerializeField] private RobotRepairedAnimationHandler repairedAnimationHandler = null; 

#pragma warning  disable
        [Tooltip("Type of the robot")] [BoxGroup("Robot", ShowLabel = false)] [SerializeField]
        private RobotType type;
#pragma warning  enabled
        [Tooltip("RigidBody2D component on this Object")]
        [BoxGroup("Robot", ShowLabel = false), ChildGameObjectsOnly]
        [SerializeField]
        private Rigidbody2D rigidBody2D;

        [Tooltip("BoxCollider2D component on the Robot")]
        [BoxGroup("Robot", ShowLabel = false), ChildGameObjectsOnly]
        [SerializeField]
        private BoxCollider2D robotCollider;

        [Tooltip("BoxCollider2D component on the RobotStand")]
        [BoxGroup("Robot", ShowLabel = false), ChildGameObjectsOnly]
        [SerializeField]
        private BoxCollider2D standCollider;

        [Tooltip("Animator Component")] [BoxGroup("Robot", ShowLabel = false), ChildGameObjectsOnly] [SerializeField]
        private Animator animator;

        [Tooltip("RepairParticleMoveDirection-Script")]
        [BoxGroup("Robot", ShowLabel = false), ChildGameObjectsOnly]
        [SerializeField]
        private RepairParticleMoveDirection repairParticleMoveDirection;
#if UNITY_EDITOR
#pragma warning disable
        [Title("Debug", TitleAlignment = TitleAlignments.Centered),
         InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowInInspector]
        private readonly Title title;
#pragma warning enable
#endif
        [Tooltip("Index this Robot Type has in the \"robotTypePrefabs\"-List in the \"RobotSkinConfig\"-Script")]
        [SerializeField]
        [ReadOnly]
        private short typeIndex = -1;

        [Tooltip("Index this Robot Type has in the RobotPool")] [SerializeField] [ReadOnly]
        private short poolIndex = -1;

        [Tooltip("All Robot parts inside this Robot"), InfoBox("Must have the same order as the child Objects!")]
        [ElementName("Part", "0")]
        [ChildGameObjectsOnly, ListDrawerSettings(DraggableItems = false)]
        [SerializeField]
        private SpriteRenderer[] parts;

        #endregion

        #region Privates

        private readonly LayerMask layerMask = default;
        private bool accelerate;

        private Vector2 moveDirection;

        // Animator
        private readonly int robotFalling = Animator.StringToHash("Robot_Falling");

        private readonly int robotImpact = Animator.StringToHash("Robot_Impact");

        // Parts
        private RobotExplosion[] explosibleParts;
        private List<Part> partsToDisable = new List<Part>();
        private readonly List<Part> disabledParts = new List<Part>();

        private bool partDisabled;

        // Random chance
        private const ushort MIN_CHANCE = 1;
        private const ushort MAX_CHANCE = 100;
        private byte amount;
        private float chance;
        private ushort randomNumber;
        private byte index;

        #endregion

        #region Properties

        /// <summary>
        /// Is this Robot currently destroyed or not
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Type of the robot
        /// </summary>
        public RobotType Type => type;

        /// <summary>
        /// Index this Robot Type has in the "robotTypePrefabs"-List in the "RobotSkinConfig"-Script
        /// </summary>
        public short TypeIndex
        {
            get => typeIndex;
            set => typeIndex = value;
        }

        /// <summary>
        /// Index this Robot Type has in the RobotPool
        /// </summary>
        public short PoolIndex
        {
            get => poolIndex;
            set => poolIndex = value;
        }

        /// <summary>
        /// BoxCollider2D component on the Robot
        /// </summary>
        public BoxCollider2D RobotCollider => robotCollider;

        /// <summary>
        /// All Robot parts inside this Robot
        /// </summary>
        public SpriteRenderer[] Parts => parts;

        /// <summary>
        /// Array of all RobotParts <br/>
        /// "True" = missing | "false" = not missing
        /// </summary>
        public bool[] MissingParts { get; private set; }

        /// <summary>
        /// Array of all currently active RepairArms on this Robot
        /// </summary>
        public List<RepairArm> ActiveRepairArms { get; private set; } = new List<RepairArm>();

        /// <summary>
        /// Number of Parts that are currently missing on this Robot
        /// </summary>
        public byte DisabledPartsCount { get; private set; }
        /// <summary>
        /// All Parts that are currently disabled on this Robot
        /// </summary>
        public List<Part> DisabledParts => disabledParts;
        
        #endregion

        #region --- [EVENTS] ---

        /// <summary>
        /// Callback delegate for when a robot is destroyed.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="visualOnly">When true the robot is destroyed within a context that will not increment stats. e.g. reset</param>
        public delegate void RobotDestroyedCallback(RobotBehaviour instance, bool visualOnly);

        public static event RobotDestroyedCallback OnRobotDestroyed;

        
        /// <summary>
        /// Callback delegate for when a robot is repaired.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="completion"></param>
        /// <param name="visualOnly">When true the robot is repaired within a context that will not increment stats</param>
        public delegate void RobotPartAddedCallback(RobotBehaviour instance, bool completion, bool visualOnly);

        public static event RobotPartAddedCallback OnRobotPartAdded;

        #endregion

        private void Awake()
        {
            if (repairedAnimationHandler == null)
            {
                repairedAnimationHandler = GetComponentInChildren<RobotRepairedAnimationHandler>();
            }
            
            if (rigidBody2D == null)
            {
                rigidBody2D = GetComponent<Rigidbody2D>();
            }

            if (robotCollider == null)
            {
                robotCollider = GetComponent<BoxCollider2D>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (repairParticleMoveDirection == null)
            {
                repairParticleMoveDirection = GetComponent<RepairParticleMoveDirection>();
            }

            var _tmp = GetComponentsInChildren<SpriteRenderer>(false);

            if (parts.Length < _tmp.Length)
            {
                for (byte i = 0; i < _tmp.Length; i++)
                {
                    parts[i].sprite = _tmp[i].sprite;
                }
            }

            MissingParts = new bool[parts.Length];

            explosibleParts = GetComponentsInChildren<RobotExplosion>();

            Initialize();
        }

        private void OnEnable()
        {
            Destroyed = false;
            
            AddToMissingPartsList();
        }

        private void OnDisable()
        {
            OffConveyorBelt();
            EnableAllSprites();
            DisableSprites();
        }

        private void Start()
        {
            EventController.OnPartsToDisableChanged += PartToDisableChanged;
        }

        private void OnDestroy()
        {
            EventController.OnPartsToDisableChanged -= PartToDisableChanged;
        }

        private void FixedUpdate()
        {
            MoveRobot();
        }

        private void Update()
        {
            Accelerate();
        }

        private void OnTriggerExit2D(Collider2D _Other)
        {
            if (_Other == GameController.MapExit)
            {
                ReturnToPool();
            }
        }

        /// <summary>
        /// Is called once in the "Awake"-Method, to initialize needed values
        /// </summary>
        private void Initialize()
        {
            if (gameObject.layer != layerMask.LayerToInt(LayerSettings.RobotLayer))
            {
                gameObject.layer = layerMask.LayerToInt(LayerSettings.RobotLayer);
            }

            rigidBody2D.drag = GameConfig.RobotDrag;
            rigidBody2D.gravityScale = GameConfig.RobotGravity;
            robotCollider.enabled = false;
            standCollider.enabled = false;

            explosibleParts.ToList().ForEach(_Part => _Part.Initialize());
            EnableAllSprites();
            DisableSprites();
        }

        /// <summary>
        /// Is called when the ConveyorLift releases the Robot
        /// </summary>
        public void ReleaseRobot()
        {
            EventController.RobotReleased();
            AudioSystem.PlayVFX(VFX.RobotOnRelease);

            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            standCollider.enabled = true;
            transform.SetParent(null);

            parts.ForEach(_Sprite => _Sprite.sortingOrder++);
            accelerate = true;

            if (Destroyed) return;
            animator.enabled = true;
            animator.Play(robotFalling, 0, 0);
        }

        /// <summary>
        /// Gravitational Acceleration when the Robot is falling
        /// </summary>
        private void Accelerate()
        {
            if (!accelerate) return;

            rigidBody2D.drag -= GameConfig.AccelerationStep;
            if (rigidBody2D.drag <= 0) accelerate = false;
        }

        /// <summary>
        /// Is called when the Robot touches the ConveyorBelt 
        /// </summary>
        public void OnConveyorBelt()
        {
            AudioSystem.PlayVFX(VFX.RobotOnImpact);

            rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
            standCollider.enabled = false;
            robotCollider.enabled = true;
            accelerate = false;
            rigidBody2D.drag = 0;
            moveDirection = Vector2.right;

            if (Destroyed) return;
            animator.Play(robotImpact, 0, 0);
        }

        /// <summary>
        /// Is called when the Robot is no longer on the ConveyorBelt 
        /// </summary>
        private void OffConveyorBelt()
        {
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            robotCollider.enabled = false;
            moveDirection = Vector2.zero;

            parts.ForEach(_Sprite => _Sprite.sortingOrder--);
            rigidBody2D.drag = GameConfig.RobotDrag;
        }

        /// <summary>
        /// Moves the Robot
        /// </summary>
        private void MoveRobot()
        {
            rigidBody2D.velocity = moveDirection * (GameController.CurrentRobotSpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Activates all Sprites on the Robot
        /// </summary>
        private void EnableAllSprites()
        {
            for (byte i = 0; i < parts.Length; i++)
            {
                parts[i].sprite = RobotPartConfig.RobotTypePrefabs[typeIndex].RobotParts[i];
                MissingParts[i] = false;
            }

            explosibleParts.ToList().ForEach(_Part => _Part.ResetPosition());
        }

        /// <summary>
        /// Disables a new set of parts on all Robots that are currently not on screen
        /// </summary>
        private void PartToDisableChanged()
        {
            if (gameObject.activeSelf) return;
            EnableAllSprites();
            DisableSprites();
        }

        /// <summary>
        /// Disables a random number of Sprites 
        /// </summary>
        private void DisableSprites()
        {
            repairedAnimationHandler.Reset();
            do
            {
                // TODO: Call "DisableSprites()" as an Async task and calculate the next disabled Sprites before they're needed

                partDisabled = false;
                DisabledPartsCount = 0;

                // TODO: Make "partsToDisable" a Dictionary

                partsToDisable = new List<Part>(ShuffleCountdown.PartsToDisable[type].ToList());
                disabledParts.Clear();

                // TODO: Make "MinAmountToDisable"/"MaxAmountToDisable" numbers bigger, so the random range is more diverse

                amount = (byte) Random.Range(GameConfig.MinAmountToDisable, GameConfig.MaxAmountToDisable);
                if (amount == 0) break;

                // Always leaves at least 2 Sprites enabled (Rostand + random Sprite), so the player can recognize what Robot Type it is
                amount = amount > parts.Length - 2 ? (byte) (parts.Length - 2) : amount;

                for (byte i = 0; i < amount; i++)
                {
                    if (partsToDisable.Count <= 0)
                    {
                        return;
                    }

                    //Calculates the chance for each Robot Part to be disabled
                    // "MAX_CHANCE" must be cast as a float!
                    chance = (float) MAX_CHANCE / partsToDisable.Count;

                    // Which part to disable
                    randomNumber = (ushort) Random.Range(MIN_CHANCE > MAX_CHANCE ? MAX_CHANCE : MIN_CHANCE,
                        MAX_CHANCE < MIN_CHANCE ? MIN_CHANCE : MAX_CHANCE);

                    index = (byte) Mathf.CeilToInt(randomNumber / chance);

                    disabledParts.Add(partsToDisable[index - 1]);
                    parts[partsToDisable[index - 1].RobotIndex].sprite = null;
                    MissingParts[partsToDisable[index - 1].RobotIndex] = true;
                    partsToDisable.RemoveAt(index - 1);

                    DisabledPartsCount++;
                    partDisabled = true;
                }
            } while (!partDisabled);
        }

        /// <summary>
        /// Visualises the repair of a Robot Part
        /// </summary>
        /// <param name="_Part">Robot Part to repair</param>
        public void RepairRobotPartAnimation(Part _Part)
        {
            parts[_Part.RobotIndex].sprite = _Part.RobotSprite;

            repairParticleMoveDirection.SpawnParticle(_Part.RobotIndex);
        }

        /// <summary>
        /// Repairs one Part of this Robot
        /// </summary>
        /// <param name="_Part">Robot Part to repair</param>
        public void RepairRobotPart(Part _Part)
        {
            AudioSystem.PlayVFX(VFX.RobotOnRepairProgress);

            MissingParts[_Part.RobotIndex] = false;
            DisabledPartsCount--;
            RemoveFromMissingPartsList(_Part.RobotIndex);

            ScoreHandler.IncreaseScore(transform.position);
            ScoreHandler.IncreaseMultiplier();

            var completed = DisabledPartsCount == 0;

            // When all Parts on this Robot have benn repaired
            if (completed)
            {
                repairedAnimationHandler.StartAnimation();
                AudioSystem.PlayVFX(VFX.RobotOnRepairSuccess, 300);
                EventController.RobotRepaired(this);
            }

            //TODO: pass the last value (visualOnly) when a robot is repaired in the main menu. This will tell the stats class that the 
            OnRobotPartAdded?.Invoke(this, completed, false);
        }

        /// <summary>
        /// Adds all missing Parts of this Robot to the List of the currently missing Parts on the Map
        /// </summary>
        private void AddToMissingPartsList()
        {
            foreach (var _part in disabledParts)
            {
                ShuffleCountdown.AddToMissingParts(_part);
            }
        }

        /// <summary>
        /// Removes this Robots missing parts from the List of the currently missing Parts on the Map, when it's no longer active on the Map <br/>
        /// When "index" is != null, only removes one Part from the List
        /// </summary>
        /// <param name="_Index">Index of the Robot Part it has in this Robot</param>
        public void RemoveFromMissingPartsList(sbyte? _Index = null)
        {
            // Removes one Part
            if (_Index != null)
            {
                ShuffleCountdown.RemoveFromMissingParts(Type, _Index.Value);
            }
            // Removes all Parts
            else
            {
                if (DisabledPartsCount <= 0) return;
                // Can start at index 1, because the RobotStand cannot be in the List
                for (sbyte i = 1; i < parts.Length; i++)
                {
                    if (parts[i].sprite is null)
                    {
                        ShuffleCountdown.RemoveFromMissingParts(type, i);
                    }
                }
            }
        }

        /// <summary>
        /// Makes the RobotParts explode when there are still Parts missing when the Robot leaves the Map
        /// </summary>
        /// <param name="_Shake">Set to "true" to play the shake the Robot before it explodes, set to "false" to disable the shake</param>
        /// <param name="_Reset">When true the robot is destroyed within the context of a reset and will not influence stats</param>
        /// <param name="_PlaySound">Should the explosion sound be played</param>
        public void DestroyRobot(bool _Shake, bool _Reset, bool _PlaySound = true)
        {
            if (Destroyed) return;
            Destroyed = true;

            OnRobotDestroyed?.Invoke(this, _Reset);
            
            ActiveRepairArms.ForEach(_Arm => _Arm.SkipRepair());
            ActiveRepairArms.Clear();

            animator.enabled = false;

            // Explosion with shake
            if (_Shake)
            {
                ExplosionDelay((int) GameConfig.ShakeDuration * 1000, _PlaySound);
                explosibleParts.ToList().ForEach(_Part => _Part.StartExplosion(false));
            }
            // Explosion without shake
            else
            {
                ExplosionDelay(0, _PlaySound);
                explosibleParts.ToList().ForEach(_Part => _Part.StartExplosion(true));
            }

            if (GameController.GameState == GameState.Playing && !_Reset)
            {
                PlayerHealthHandler.SubtractLife();
                ScoreHandler.ResetMultiplier();
            }
        }

        /// <summary>
        /// Spawns the Explosion Particle after the Robot has stopped shaking <br/>
        /// This Method needs to be Invoked with a delay of the ShakeDuration
        /// </summary>
        /// <param name="_PlaySound">Should the explosion sound be played</param>
        private async void ExplosionDelay(int _Delay, bool _PlaySound)
        {
            await Task.Delay(_Delay);

            if (_PlaySound) AudioSystem.PlayVFX(VFX.RobotOnDestroyed);
            //PoolController.ExplosionParticlePool.GetObject(null, transform.position.WithY(robotCollider.bounds.center.y));
        }


        /// <summary>
        /// Returns the Robot back to its ObjectPool
        /// </summary>
        public void ReturnToPool()
        {
            PoolController.RobotPools[poolIndex].Pool.ReturnObject(gameObject, true);
            RobotPool.ActiveRobots--;
        }

#if UNITY_EDITOR

        #region Proiperties

        /// <summary>
        /// RigidBody2D Component (Only for Editor!)
        /// </summary>
        public Rigidbody2D EditorRigidbody2D => rigidBody2D;

        #endregion

        /// <summary>
        /// Adjusts the size of the Collider to the size of the Robot 
        /// </summary>
        [Button]
        private void SetColliderSize()
        {
            float _left = 0;
            float _right = 0;
            float _bottom = 0;
            float _top = 0;

            foreach (var _part in parts)
            {
                if (_part.bounds.min.x < _left)
                {
                    _left = _part.bounds.min.x;
                }

                if (_part.bounds.max.x > _right)
                {
                    _right = _part.bounds.max.x;
                }

                if (_part.bounds.min.y < _bottom)
                {
                    _bottom = _part.bounds.min.y;
                }

                if (_part.bounds.max.y > _top)
                {
                    _top = _part.bounds.max.y;
                }
            }

            //DebugLog.White($"Left: {_left} | Right: {_right} | Bottom: {_bottom} | Top: {_top}");

            robotCollider.offset = new Vector2((_left + _right) / 2, (_bottom + _top) / 2);

            var _sizeX = Vector2.Distance(new Vector2(_left, 0f), new Vector2(_right, 0f));
            var _sizeY = Vector2.Distance(new Vector2(0f, _bottom), new Vector2(0f, _top));

            robotCollider.size = new Vector2(_sizeX, _sizeY);
        }
#endif
    }
}