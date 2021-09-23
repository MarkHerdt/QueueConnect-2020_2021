using System;
using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using QueueConnect.Robot;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QueueConnect.Environment
{
    /// <summary>
    /// The selectable Robot Parts in the Displays
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasRenderer), typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Button), typeof(EventTrigger))]
    [RequireComponent(typeof(Image), typeof(Animator))]
    [HideMonoScript]
    public class RobotPartButton : MonoBehaviour
    {
        #region Inspector Fields

        [Tooltip("Button Component on this GameObject")] [ChildGameObjectsOnly] [SerializeField]
        private Button button;

        [Tooltip("EventTrigger Component on this GameObject")] [ChildGameObjectsOnly] [SerializeField]
        private EventTrigger eventTrigger;

        [Tooltip("SpriteRenderer of the Robot Part inside the Button")] [ChildGameObjectsOnly] [SerializeField]
        private SpriteRenderer robotPart;

        [Tooltip("Color of the Robot Part inside the Button, when the Button is not disabled")] [SerializeField]
        private Color32 normalColor = new Color32(128, 191, 191, 255);

        [Tooltip("Color of the Robot Part inside the Button, when the Button is disabled")] [SerializeField]
        private Color32 disabledColor = new Color32(128, 191, 191, 75);

        [Tooltip("HeadLight GameObject in the Scene")] [SceneObjectsOnly] [SerializeField]
        private RobotScanner robotScanner;

        [Tooltip("Robot Part in this Button")] [SerializeField]
        private Part part;

        #endregion

        #region Privates

        private RobotBehaviour robot;

        #endregion

        #region Properties

        /// <summary>
        /// SpriteRenderer Component of this Button
        /// </summary>
        public Sprite Sprite
        {
            set => robotPart.sprite = value;
        }

        /// <summary>
        /// Robot Part in this Button
        /// </summary>
        public Part Part
        {
            get => part;
            set => part = value;
        }

        #endregion

        #region --- [EVENTS] ---

        public static event Action OnWrongRobotPartSelected;

        #endregion

        private void Awake()
        {
            if (button == null)
            {
                button = gameObject.GetComponent<Button>();
            }

            if (eventTrigger == null)
            {
                eventTrigger = GetComponent<EventTrigger>();
            }

            if (robotPart == null)
            {
                robotPart = GetComponentInChildren<SpriteRenderer>();
            }

            if (robotScanner == null)
            {
                robotScanner = FindObjectOfType<RobotScanner>();
            }
        }

        private void Start()
        {
            DisableButton();

            EventController.OnRobotTargeted += EnableButton;
            EventController.OnNoRobotTargeted += DisableButton;
        }

        private void OnDestroy()
        {
            EventController.OnRobotTargeted -= EnableButton;
            EventController.OnNoRobotTargeted -= DisableButton;
        }

        /// <summary>
        /// Makes this Button clickable
        /// </summary>
        public void EnableButton()
        {
            if (RobotScanner.Robots.Count <= 0 || !RobotScanner.RobotTargeted || RobotScanner.AutoRepairRobots) return;

                if (GameController.GameState == GameState.Playing)
                {
                    button.enabled = true;
                    button.interactable = true;
                    eventTrigger.enabled = true;
                    robotPart.color = normalColor;
                }
        }

        /// <summary>
        /// Makes this Button unclickable
        /// </summary>
        public void DisableButton()
        {
            button.enabled = false;
            button.interactable = false;
            eventTrigger.enabled = false;
            button.animator.SetBool(button.animationTriggers.normalTrigger, true);
            robotPart.color = disabledColor;
        }

        /// <summary>
        /// Tries to repair the currently targeted Robot with the RobotPart in this Button when it's pressed
        /// </summary>
        public void OnButtonPressed()
        {
            if (!button.interactable) return;

            robot = robotScanner.GetTargetedRobot();

            // RobotPart in this Button is missing on the Robot
            if (!(robot is null) && robot.Type == part.Type && robot.MissingParts[part.RobotIndex])
            {
                AudioSystem.PlayVFX(VFX.UIPartButtonPressedSuccess);

                ((RepairArm) PoolController.RepairArmPool.GetObject(_NewParent: robot.transform,
                                                                    _NewPosition: robot.Parts[part.RobotIndex].transform.position.WithY(PoolPrefabs.RepairArmPrefab.transform.position.y + part.RobotSprite.bounds.extents.y),
                                                                    _GlobalPosition: true).Component).RepairPart(_Robot: robot,
                                                                    _Part: part,
                                                                    _TargetPosition: robot.Parts[part.RobotIndex].transform.position);

                AudioSystem.PlayVFX(VFX.RepairArmSpawn);

                //ShuffleCountdown.AddToCountdown();
            }
            // RobotPart in this Button is not missing or doesn't belong to the Robot
            else
            {
                AudioSystem.PlayVFX(VFX.UIPartButtonPressedFailure);

                if (robotScanner.ChangeLightBeamColorCoroutine != null)
                {
                    robotScanner.ResetCoroutine();
                }
                else
                {
                    robotScanner.StartCoroutine();
                }

                if (GameConfig.AllowMistakes) return;
                OnWrongRobotPartSelected?.Invoke();
                PlayerHealthHandler.SubtractLife();
                ScoreHandler.CutMultiplierInHalf();
                ShuffleCountdown.RemoveFromCountdown();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Automatically gets a reference to the "Headlight"-Script
        /// </summary>
        [Button]
        private void GetHeadlightReference()
        {
            var _headlight = FindObjectOfType<RobotScanner>();
            var _partsDisplay = GetComponentInParent<PartsDisplay>();
            var _robotPartButtons = _partsDisplay.GetComponentsInChildren<RobotPartButton>();

            foreach (var _button in _robotPartButtons)
            {
                _button.robotScanner = _headlight;
            }
        }
#endif
    }
}