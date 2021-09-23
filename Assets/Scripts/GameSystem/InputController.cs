using QueueConnect.Environment;
using Rewired;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QueueConnect.GameSystem
{

    
    /// <summary>
    /// All Actions in Rewired
    /// </summary>
    public enum Actions
    {
        DPad_Left,
        DPad_Up,
        DPad_Right,
        DPad_Down,
        Action_Right,
        Action_Up,
        Action_Left,
        Action_Down
    }

    /// <summary>
    /// Manages Controller Input
    /// </summary>
    [HideMonoScript]
    public class InputController : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("All Buttons from the \"PartsDisplays\""), InfoBox("Buttons must have the same order as in the the Scene!")]
            [SceneObjectsOnly, ListDrawerSettings(Expanded = true)]
            [SerializeField] private EventTriggerButton[] robotPartButtons = new EventTriggerButton[8];
        #endregion

        #region Privates
            private static Player player;
        #endregion
        
        private void Awake()
        {
            var _isNull = false;

            for (byte i = 0; i < robotPartButtons.Length; i++)
            {
                if (robotPartButtons[i].Button != null && robotPartButtons[i].EventTrigger != null) continue;
                    _isNull = true;
                    break;
            }

            if (robotPartButtons.Length != 8 || _isNull)
            {
                var _partsDisplay = FindObjectOfType<PartsDisplay>().gameObject;

                var _robotPartButtons = _partsDisplay.GetComponentsInChildren<RobotPartButton>();

                for (byte i = 0; i < _robotPartButtons.Length; i++)
                {
                    this.robotPartButtons[i] = new EventTriggerButton(    _robotPartButtons[i].gameObject.GetComponent<Button>(),
                                                                      _robotPartButtons[i].gameObject.GetComponent<EventTrigger>());
                }
            }

            player = ReInput.players.GetPlayer(0);
            
            player.AddInputEventDelegate(DPad_Left_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.DPad_Left.ToString());
            player.AddInputEventDelegate(DPad_Left_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.DPad_Left.ToString());
            player.AddInputEventDelegate(DPad_Up_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.DPad_Up.ToString());
            player.AddInputEventDelegate(DPad_Up_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.DPad_Up.ToString());
            player.AddInputEventDelegate(DPad_Right_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.DPad_Right.ToString());
            player.AddInputEventDelegate(DPad_Right_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.DPad_Right.ToString());
            player.AddInputEventDelegate(DPad_Down_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.DPad_Down.ToString());
            player.AddInputEventDelegate(DPad_Down_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.DPad_Down.ToString());
            player.AddInputEventDelegate(Action_Right_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.Action_Right.ToString());
            player.AddInputEventDelegate(Action_Right_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.Action_Right.ToString());
            player.AddInputEventDelegate(Action_Up_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.Action_Up.ToString());
            player.AddInputEventDelegate(Action_Up_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.Action_Up.ToString());
            player.AddInputEventDelegate(Action_Left_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.Action_Left.ToString());
            player.AddInputEventDelegate(Action_Left_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.Action_Left.ToString());
            player.AddInputEventDelegate(Action_Down_Pressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, Actions.Action_Down.ToString());
            player.AddInputEventDelegate(Action_Down_Released, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, Actions.Action_Down.ToString());
        }

        /// <summary>
        /// When the Left DPad Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Left_Pressed(InputActionEventData _Data)
        {        
            robotPartButtons[0].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[0].Button.enabled)
            {
                robotPartButtons[0].Button.animator.SetBool(robotPartButtons[0].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Left DPad Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Left_Released(InputActionEventData _Data)
        {
            robotPartButtons[0].Button.animator.SetBool(robotPartButtons[0].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Top DPad Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Up_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[1].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[1].Button.enabled)
            {
                robotPartButtons[1].Button.animator.SetBool(robotPartButtons[1].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Top DPad Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Up_Released(InputActionEventData _Data)
        {
            robotPartButtons[1].Button.animator.SetBool(robotPartButtons[1].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Right DPad Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Right_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[2].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[2].Button.enabled)
            {
                robotPartButtons[2].Button.animator.SetBool(robotPartButtons[2].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Right DPad Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Right_Released(InputActionEventData _Data)
        {
            robotPartButtons[2].Button.animator.SetBool(robotPartButtons[2].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Bottom DPad Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Down_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[3].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[3].Button.enabled)
            {
                robotPartButtons[3].Button.animator.SetBool(robotPartButtons[3].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Bottom DPad Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void DPad_Down_Released(InputActionEventData _Data)
        {
            robotPartButtons[3].Button.animator.SetBool(robotPartButtons[3].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Right Action Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Right_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[4].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[4].Button.enabled)
            {
                robotPartButtons[4].Button.animator.SetBool(robotPartButtons[4].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Right Action Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Right_Released(InputActionEventData _Data)
        {
            robotPartButtons[4].Button.animator.SetBool(robotPartButtons[4].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Top Action Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Up_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[5].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[5].Button.enabled)
            {
                robotPartButtons[5].Button.animator.SetBool(robotPartButtons[5].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Top Action Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Up_Released(InputActionEventData _Data)
        {
            robotPartButtons[5].Button.animator.SetBool(robotPartButtons[5].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Left Action Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Left_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[6].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[6].Button.enabled)
            {
                robotPartButtons[6].Button.animator.SetBool(robotPartButtons[6].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Left Action Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Left_Released(InputActionEventData _Data)
        {
            robotPartButtons[6].Button.animator.SetBool(robotPartButtons[6].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// When the Bottom Action Button is pressed
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Down_Pressed(InputActionEventData _Data)
        {
            robotPartButtons[7].EventTrigger.triggers[0].callback.Invoke(null);
            if (robotPartButtons[7].Button.enabled)
            {
                robotPartButtons[7].Button.animator.SetBool(robotPartButtons[7].Button.animationTriggers.pressedTrigger, true);
            }
        }

        /// <summary>
        /// When the Bottom Action Button is released
        /// </summary>
        /// <param name="_Data"></param>
        private void Action_Down_Released(InputActionEventData _Data)
        {
            robotPartButtons[7].Button.animator.SetBool(robotPartButtons[7].Button.animationTriggers.normalTrigger, true);
        }

        /// <summary>
        /// Reference to the "Button"/"Event Trigger"-Component of a "RobotPartButton"
        /// </summary>
        [Serializable]
        private struct EventTriggerButton
        {
            #region Inspector Fields
                [Tooltip("Button Component of a \"RobotPartButton\"")]
                [SerializeField] private Button button;
                [Tooltip("Event Trigger Component of a \"RobotPartButton\"")]
                [SerializeField] private EventTrigger eventTrigger;
            #endregion

            #region Properties
                /// <summary>
                /// Button Component of a "RobotPartButton"
                /// </summary>
                public Button Button => button;
                /// <summary>
                /// Event Trigger Component of a "RobotPartButton"
                /// </summary>
                public EventTrigger EventTrigger => eventTrigger;
            #endregion

            /// <param name="_Button">Button Component of a "RobotPartButton"</param>
            /// <param name="_EventTrigger">Event Trigger Component of a "RobotPartButton"</param>
            public EventTriggerButton(Button _Button, EventTrigger _EventTrigger)
            {
                this.button = _Button;
                this.eventTrigger = _EventTrigger;
            }
        }
    }
}