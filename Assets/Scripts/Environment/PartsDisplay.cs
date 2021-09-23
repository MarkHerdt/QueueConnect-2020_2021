using QueueConnect.GameSystem;
using System.Collections.Generic;
using UnityEngine;
using QueueConnect.Config;
using Sirenix.OdinInspector;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Displays the RobotParts to repair the Robots
    /// </summary>
    [HideMonoScript]
    public class PartsDisplay : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("All Buttons and their corresponding Sprites"), InfoBox("Must have the same order as the child Objects!")]
            [ListDrawerSettings(Expanded = true, DraggableItems = false), ChildGameObjectsOnly]
            [SerializeField] private RobotPartButton[] robotPartButtons = new RobotPartButton[8];
        #endregion

        #region Privates
            private static PartsDisplay instance;
        #endregion
        
        #region Properites
            /// <summary>
            /// All Buttons and their corresponding Sprites
            /// </summary>
            public static RobotPartButton[] RobotPartButtons => instance.robotPartButtons;
        #endregion
        
        private void Awake()
        {
            robotPartButtons = gameObject.GetComponentsInChildren<RobotPartButton>();

            var _camera = FindObjectOfType<Camera>();

            foreach (var _button in robotPartButtons)
            {
                var _tmp = _button.GetComponent<Canvas>();
                
                if (_tmp.worldCamera == null)
                {
                    _tmp.worldCamera = _camera;
                }
            }

            instance = Singleton.Persistent(this);
        }

        private void Start()
        {
            EventController.OnShuffleParts += ShuffleParts;
        }

        private void OnDestroy()
        {
            EventController.OnShuffleParts -= ShuffleParts;
        }

        /// <summary>
        /// Shuffles the Parts inside the Displays
        /// </summary>
        /// <param name="_PartsList">New List of Robot Parts</param>
        private void ShuffleParts(List<Part> _PartsList)
        {
            for (byte i = 0; i < robotPartButtons.Length; i++)
            {
                robotPartButtons[i].Part = new Part(default, null, null, -1);
                robotPartButtons[i].Sprite = null;

                // For when there are less Parts than the Button count
                if (i >= _PartsList.Count) continue;
                
                    robotPartButtons[i].Part = _PartsList[i];

                    robotPartButtons[i].Sprite = robotPartButtons[i].Part.DisplaySprite;
            }
        }
    }
}