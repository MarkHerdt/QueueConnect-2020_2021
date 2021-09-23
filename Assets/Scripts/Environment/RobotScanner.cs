using System;
using QueueConnect.Config;
using QueueConnect.GameSystem;
using QueueConnect.Robot;
using System.Collections;
using System.Collections.Generic;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using QueueConnect.Plugins.SoundSystem;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SoundSystem.Core;
using Random = UnityEngine.Random;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Targets the currently repairable Robot
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    [HideMonoScript]
    public class RobotScanner : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("BoxCollider2D Component on this GameObject")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private BoxCollider2D boxCollider2D;
            [Tooltip("Scanner GameObject in children")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private GameObject scanner;
            [Tooltip("SpriteMask Component in children")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private SpriteMask spriteMask;
            [Tooltip("RigidBody2D Component")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private Rigidbody2D scannerRigidBody2D;
            [Tooltip("BoxCollider2D Component on the Scanner GameObject")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private BoxCollider2D scannerCollider2D;
            [Tooltip("Left Scanner Element in children")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private SpriteRenderer leftScannerElement;
            [Tooltip("Right Scanner Element in children")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private SpriteRenderer rightScannerElement;
            [Tooltip("SpriteRenderer of the \"LightBeam\" GameObject")]
            [ChildGameObjectsOnly][BoxGroup("References", ShowLabel = false)]
            [SerializeField] private SpriteRenderer scannerBeamSpriteRenderer;
            [Tooltip("Normal color of the LightBeam")]
            [BoxGroup("Color", ShowLabel = false)]
            [SerializeField] private Color32 normalColor = new Color32(255, 0, 90, 110);
            [Tooltip("Color of the LightBeam, when a wrong part was selected")]
            [BoxGroup("Color", ShowLabel = false)]
            [SerializeField] private Color32 errorColor = new Color32(255, 0, 0, 110);
        #endregion
        
        #region Privates
            private static RobotScanner instance;
            // Robots
            private readonly Queue<RobotBehaviour> robots = new Queue<RobotBehaviour>();
            private LayerMask robotLayer;
            private bool robotTargeted;
            private bool eventFired;
            // Scanner
            private float scannerSpeed;
            private float dividedBy = 1;
            private bool resetCoroutine;
            private float randomMoveDelay;
            private bool randomMovement;
            private Vector2 randomPosition = Vector2.zero;
            private float delay;
            private float repairDuration;
            private float repairDelay;
            private static float previousSpawnDelay;
        #endregion

        #region Properties
            /// <summary>
            /// BoxCollider2D Component
            /// </summary>
            public static BoxCollider2D BoxCollider2D => instance.boxCollider2D;
            /// <summary>
            /// All damaged Robots on the ConveyorBelt
            /// </summary>
            public static Queue<RobotBehaviour> Robots => instance.robots;
            /// <summary>
            /// If a Robot is currently targeted by the RobotScanner
            /// </summary>
            public static bool RobotTargeted => instance.robotTargeted;
            /// <summary>
            /// Is "true" when the Coroutine in "wrongRobotPart" is currently running
            /// </summary>
            public IEnumerator ChangeLightBeamColorCoroutine { get; private set; }
            /// <summary>
            /// If "true", the scanner repairs the Robots automatically as long as this bool stays "true"
            /// </summary>
            public static bool AutoRepairRobots { get; private set; } = true;
        #endregion
        
        private void Awake()
        {
            var _children = GetComponentsInChildren<Transform>();
            
            if (boxCollider2D == null)
            {
                boxCollider2D = GetComponent<BoxCollider2D>();
            }
            if (scanner == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.Scanner)) continue;
                    
                        scanner = _child.gameObject;
                        break;
                }
            }
            if (spriteMask == null)
            {
                spriteMask = GetComponentInChildren<SpriteMask>();
            }
            if (scannerRigidBody2D == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.Scanner)) continue;
                    
                        scannerRigidBody2D = _child.GetComponent<Rigidbody2D>();
                        break;
                }
            }
            if (scannerCollider2D == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.Scanner)) continue;
                    
                        scannerCollider2D = _child.GetComponent<BoxCollider2D>();
                        break;
                }
            }
            if (leftScannerElement == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.LeftScannerElement)) continue;
                    
                        leftScannerElement = _child.GetComponent<SpriteRenderer>();
                        break;
                }
            }
            if (rightScannerElement == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.RightScannerElement)) continue;
                    
                        rightScannerElement = _child.GetComponent<SpriteRenderer>();
                        break;
                }   
            }
            if (scannerBeamSpriteRenderer == null)
            {
                foreach (var _child in _children)
                {
                    if (!_child.gameObject.Tag(Tags.ScannerBeam)) continue;
                    
                        scannerBeamSpriteRenderer = _child.GetComponent<SpriteRenderer>();
                        break;
                }
            }

            instance = Singleton.Persistent(this);
            robotLayer = robotLayer.LayerToLayerMask(LayerSettings.RobotLayer);
        }

        private void FixedUpdate()
        {
            Move();
        }
        
        private void Update()
        {
            TargetRobot();

#if UNITY_EDITOR
            // Test for AutoRepair
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartAutoRepair(5);
            }
#endif
        }
        
        private void Start()
        {
            EventController.OnRobotRepaired += RobotRepaired;
            GameController.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
            EventController.OnRobotRepaired -= RobotRepaired;
            GameController.OnGameStateChanged -= GameStateChanged;
        }

        private void OnTriggerEnter2D(Collider2D _Collision)
        {
            EnqueueRobot(_Collision);
        }

        private void GameStateChanged(GameState _GameState)
        {
            if (_GameState == GameState.Playing && repairDuration <= 0)
            {
                AutoRepairRobots = false;
            }
            else if (_GameState == GameState.Menu && !GameController.IsPaused)
            {
                AutoRepairRobots = true;
            }
        }
        
        /// <summary>
        /// Enqueues a Robot when it enters the Headlights range
        /// </summary>
        /// <param name="_Collider">Collider of Robot</param>
        private void EnqueueRobot(Component _Collider)
        {
            var _robot = _Collider.gameObject.GetComponent<RobotBehaviour>();

            if (_robot != null && !_robot.Destroyed && _robot.DisabledPartsCount > 0)
            {
                AudioSystem.PlayVFX(VFX.ScannerRailMovement);
                
                robots.Enqueue(_robot);
            }
        }

        /// <summary>
        /// Dequeues the first Robot in the Queue and removes its missing Parts from the List of missing Parts
        /// </summary>
        /// <param name="_Robot">Robot to Dequeue</param>
        public static void DequeueRobot(RobotBehaviour _Robot)
        {
            if (instance.robots.Count <= 0) return;

                instance.scannerSpeed = 0;
                instance.robots.SaveDequeue();
                _Robot.RemoveFromMissingPartsList();
                
                EventController.RobotDequeued();
        }

        /// <summary>
        /// Destroys all Robots that are currently on the Map
        /// </summary>
        /// <param name="_EnableSpawn">If the Robots should spawn again after all currently active Robots have been destroyed</param>
        public static void DestroyAllRobots(bool _EnableSpawn)
        {
            GameConfig.SetRobotSpawn(false);
            
            // Dequeues all Robots
            while ((object)instance.robots.SafePeek() != null)
            {
                instance.robots.SafePeek().DestroyRobot(true, true, false);
                DequeueRobot(instance.robots.SafePeek());
            }

            // Destroys the remaining Robots that weren't enqueued
            foreach (var _robotPool in PoolController.RobotPools)
            {
                foreach (var _robot in _robotPool.Pool.ObjectsInUse)
                {
                    ((RobotBehaviour)_robot.Component)?.DestroyRobot(true, true, false);
                }
            }
            
            AudioSystem.PlayVFX(VFX.RobotOnDestroyed, (int)GameConfig.ShakeDuration * 1000);
            
            GameConfig.SetRobotSpawn(_EnableSpawn);
        }

        /// <summary>
        /// Dequeues a Robot from the Queue when it has been fully repaired
        /// </summary>
        /// <param name="_Robot">The Robot that has been repaired</param>
        private void RobotRepaired(RobotBehaviour _Robot)
        {
            if (_Robot == robots.SafePeek())
            {
                DequeueRobot(_Robot);
            }
        }

        /// <summary>
        /// Moves the scanner
        /// </summary>
        private void Move()
        {
            // When there are Robots on the Map
            if (robots.Count > 0)
            {
                dividedBy = 1;
                randomMoveDelay = 0;
                FollowRobot(robots.SafePeek().transform.position);
                AdjustScannerWidth();
            }
            // No Robots on the Map
            else
            {
                if (randomMoveDelay >= 1)
                {
                    RandomMovement();
                    return;
                }
            }
            randomMoveDelay += Time.deltaTime;
        }
        
        /// <summary>
        /// Follows the Robots position that is currently targeted by the Scanner 
        /// </summary>
        /// <param name="_RobotPosition">Position of the Robot</param>
        /// <param name="_ForceSmoothMovement">Always smoothens the movement</param>
        private void FollowRobot(Vector3 _RobotPosition, bool _ForceSmoothMovement = false)
        {
            var _position = (Vector3)scannerRigidBody2D.position;
            var _direction = _RobotPosition - _position;
            
            // Smooth movement
            if (_RobotPosition.x < _position.x || _ForceSmoothMovement)
            {
                // Acceleration
                if (scannerSpeed < GameConfig.ScannerSpeed)
                {
                    scannerSpeed = MathFx.Hermite(scannerSpeed, GameConfig.ScannerSpeed / dividedBy, 1f);
                }
                
                scannerRigidBody2D.MovePosition(_position.WithY(_position.y) + _direction * (scannerSpeed * Time.fixedDeltaTime));   
            }
            // Instant movement
            else
            {
                scannerRigidBody2D.MovePosition(_RobotPosition.WithY(_position.y) + _direction.normalized * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Adjusts the Scanners width to match the targeted Robot width
        /// </summary>
        private void AdjustScannerWidth()
        {
            if (!(Math.Abs(robots.SafePeek().RobotCollider.bounds.size.x - spriteMask.bounds.size.x) > .1f)) return;
            
                // ScannerElements
                var _leftPosition = leftScannerElement.transform.localPosition;
                leftScannerElement.transform.localPosition = _leftPosition.WithX(Mathf.Lerp(_leftPosition.x, -((robots.SafePeek().RobotCollider.bounds.size.x / 2) + 5), .5f));
                var _rightPosition = rightScannerElement.transform.localPosition;
                rightScannerElement.transform.localPosition = _rightPosition.WithX(Mathf.Lerp(_rightPosition.x, (robots.SafePeek().RobotCollider.bounds.size.x / 2) + 5, .5f));
                
                var _colliderSize = Vector2.Distance(leftScannerElement.bounds.min.WithY(0), rightScannerElement.bounds.max.WithY(0));
                var _maskSize = Vector2.Distance(leftScannerElement.transform.position.WithY(0), rightScannerElement.transform.position.WithY(0));
                
                // Collider
                scannerCollider2D.size = new Vector2(_colliderSize, scannerCollider2D.size.y);
                
                // Mask
                var _maskWidth = spriteMask.transform.localScale;
                _maskWidth = _maskWidth.WithX(Mathf.Lerp(_maskWidth.x, _maskSize, 1f));
                spriteMask.transform.localScale = _maskWidth;
        }
        
        /// <summary>
        /// Checks if a Robot is currently targeted by the LightBeam
        /// </summary>
        private void TargetRobot()
        {
            // TODO: Use Event instead of Update
            
            var _maskBounds = spriteMask.bounds;
            robotTargeted = Physics2D.BoxCast(_maskBounds.center.WithY(leftScannerElement.bounds.min.y), new Vector2(_maskBounds.size.x, _maskBounds.size.y), 0f, Vector2.down, _maskBounds.size.y, robotLayer);

            if (AutoRepairRobots && robotTargeted)
            {
                AutoRepair();
            }
            
            // "robots.Count > 0" prevents the enabling of the RobotPartButtons when there is no Robot with missing Parts on the Map
            if (robots.Count > 0 && robotTargeted)
            {
                // Only fires the event once, when a new Robot is targeted
                if (eventFired) return;
                    
                    AudioSystem.PlayVFX(VFX.ScannerRailSelectedTarget);
                    AudioSystem.PlayVFX(VFX.ScannerRailActive);
                    
                    EventController.RobotTargeted();
                    eventFired = true;
            }
            else
            {
                // Only fires the event once, when the Headlight leaves a Robot
                if (!eventFired) return;
                    
                    AudioSystem.StopVFX(VFX.ScannerRailActive);
                    
                    EventController.NoRobotTargeted();
                    eventFired = false;
            }
        }
        
        /// <summary>
        /// Moves the Scanner randomly over the Map
        /// </summary>
        private void RandomMovement()
        {
            if (!randomMovement)
            {
                randomMovement = true;
                scannerSpeed = 0;
                dividedBy = 2;
        
                // Target position
                var _bounds = boxCollider2D.bounds;
                var _offset = scannerCollider2D.bounds.size.x;
                var _randomXPosition = Random.Range(_bounds.min.x + _offset, _bounds.max.x - _offset);
                
                randomPosition = new Vector2(_randomXPosition, scanner.transform.position.y);
                
                AudioSystem.PlayVFX(VFX.ScannerRailMovement);
                
                Invoke(nameof(RandomDelay), delay);
            }
        
            FollowRobot(randomPosition, true);
        }

        /// <summary>
        /// Chooses a random delay before the Scanner moves again
        /// </summary>
        private void RandomDelay()
        {
            randomMovement = false;
            delay = Random.Range(GameConfig.RandomMovementDelay.x, GameConfig.RandomMovementDelay.y);
        }

        /// <summary>
        /// Returns the Robot, the Headlight is currently targeting
        /// </summary>
        /// <returns>Returns the RobotBehaviour of the Robot</returns>
        public RobotBehaviour GetTargetedRobot()
        {
            return robots.SafePeek();
        }

        /// <summary>
        /// STarts the Coroutine to change the LightBeam color
        /// </summary>
        public void StartCoroutine()
        {
            ChangeLightBeamColorCoroutine = ChangeLightBeamColor();
            StartCoroutine(ChangeLightBeamColorCoroutine);
        }

        /// <summary>
        /// Resets the Coroutine to change the LightBeam color
        /// </summary>
        public void ResetCoroutine()
        {
            resetCoroutine = true;
        }

        /// <summary>
        /// Changes the Color of the LightBeam when a wrong RobotPart is selected
        /// </summary>
        private static IEnumerator ChangeLightBeamColor()
        {
            do
            {
                instance.resetCoroutine = false;

                // Change color to error
                while (instance.scannerBeamSpriteRenderer.color != instance.errorColor && !instance.resetCoroutine)
                {
                    var _color = instance.scannerBeamSpriteRenderer.color;
                    
                    instance.scannerBeamSpriteRenderer.color = MathFx.Damp(_color, instance.errorColor, Time.deltaTime, GameConfig.FadeToError);
                    
                    yield return null;
                }
                // Change color to normal
                while (instance.scannerBeamSpriteRenderer.color != instance.normalColor && !instance.resetCoroutine)
                {
                    var _color = instance.scannerBeamSpriteRenderer.color;

                    instance.scannerBeamSpriteRenderer.color = MathFx.Damp(_color, instance.normalColor, Time.deltaTime, GameConfig.FadeToNormal);
                    
                    yield return null;
                }

            } while (instance.resetCoroutine);

            instance.ChangeLightBeamColorCoroutine = null;
        }

        /// <summary>
        /// Starts auto repairing the Robots for the set duration
        /// </summary>
        /// <param name="_Duration">Time in seconds, for how long the auto repair should last</param>
        public static void StartAutoRepair(float _Duration)
        {
            PartsDisplay.RobotPartButtons.ForEach(_Button => _Button.DisableButton());
            if (!AutoRepairRobots) previousSpawnDelay = RobotSpawner.CurrentSpawnDelay;
            AutoRepairRobots = true;
            instance.repairDuration += _Duration;
        }
        
        /// <summary>
        /// Automatically repairs the targeted Robot
        /// </summary>
        private void AutoRepair()
        {
            if (GameController.GameState == GameState.Menu && GameController.IsPaused) return;

                // Timer
                if (repairDuration > 0 && GameController.GameState == GameState.Playing && !GameController.IsPaused)
                {
                    // SpawnRate during auto repair 
                    if (repairDuration <= 3)
                        RobotSpawner.CurrentSpawnDelay = previousSpawnDelay;
                    else
                        RobotSpawner.CurrentSpawnDelay = RobotSpawner.CurrentSpawnDelay > 2 ? 2 : RobotSpawner.CurrentSpawnDelay;
                    
                    repairDuration -= Time.deltaTime;

                    if (repairDuration <= 0)
                    {
                        repairDuration = 0;
                        AutoRepairRobots = false;
                        PartsDisplay.RobotPartButtons.ForEach(_Button => _Button.EnableButton());
                        return;
                    }
                }
            
                var _robot = GetTargetedRobot();
                
                if (!(_robot is null) && (scanner.transform.position.x <= _robot.transform.position.x || Math.Abs(scanner.transform.position.x - _robot.transform.position.x) < 1f))
                {
                    repairDelay += Time.deltaTime;
                    if (repairDelay > .3125f)
                    {
                        for (byte i = 0; i < _robot.DisabledParts.Count;)
                        {
                            AudioSystem.PlayVFX(VFX.UIPartButtonPressedSuccess);

                            ((RepairArm) PoolController.RepairArmPool.GetObject(_NewParent: _robot.transform,
                                _NewPosition: _robot.Parts[_robot.DisabledParts[i].RobotIndex].transform.position.WithY(PoolPrefabs.RepairArmPrefab.transform.position.y + _robot.DisabledParts[i].RobotSprite.bounds.extents.y),
                                _GlobalPosition: true).Component).RepairPart(_Robot: _robot,
                                _Part: _robot.DisabledParts[i],
                                _TargetPosition: _robot.Parts[_robot.DisabledParts[i].RobotIndex].transform.position);

                            AudioSystem.PlayVFX(VFX.RepairArmSpawn);

                            ShuffleCountdown.AddToCountdown();

                            repairDelay = 0;
                            _robot.DisabledParts.RemoveAt(i);
                            break;
                        }
                    }
                }  
        }
        
        #if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                Gizmos.color = new Color(0, 1, 0, .0625f);
                
                var _bounds = spriteMask.bounds;
                Gizmos.DrawCube(new Vector2(_bounds.center.x, _bounds.center.y), new Vector2(_bounds.size.x, _bounds.size.y));
            }
        #endif
    }
}