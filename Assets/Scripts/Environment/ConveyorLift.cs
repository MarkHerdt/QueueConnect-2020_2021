using System.Collections.Generic;
using QueueConnect.Config;
using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Environment
{
    [HideMonoScript]
    public class ConveyorLift : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("All Elements of the ConveyorLift")]
            [ListDrawerSettings(Expanded = true)]
            [ShowInInspector][ReadOnly] private List<ConveyorLiftElement> liftElements = new List<ConveyorLiftElement>();
        #endregion

        #region Privates
            private static ConveyorLift instance;
        #endregion
        
        #region Properties
            /// <summary>
            /// Speed the elements move with
            /// </summary>
            public float Speed { get; private set; }
            /// <summary>
            /// All Elements of the ConveyorLift
            /// </summary>
            public static List<ConveyorLiftElement> LiftElements => instance.liftElements;
        #endregion
        
        private void Awake()
        {
            if (liftElements.Count > 0) liftElements.Clear();
            var _children = GetComponentsInChildren<ConveyorLiftElement>();

            for (byte i = 0; i < _children.Length; i++)
            {
                liftElements.Add(_children[i]);
            }

            instance = Singleton.Persistent(this);
        }

        private void Start()
        {
            GameController.OnConveyorSpeedChanged += SetConveyorElementSpeed;
            SetConveyorElementSpeed(GameController.CurrentRobotSpeed);
        }

        /// <summary>
        /// Matches the speed of the ConveyorLift to the speed the Robots move with
        /// </summary>
        public static void SetConveyorElementSpeed(float speed)
        {
            instance.Speed = speed / 50;
        }

        /// <summary>
        /// Removes the last ListElement and inserts the passed GameObject at the beginning of the List
        /// </summary>
        /// <param name="_ConveyorElement">Element to insert into the List</param>
        public void UpdateList(ConveyorLiftElement _ConveyorElement)
        {
            liftElements.RemoveAt(liftElements.Count - 1);
            liftElements.Insert(0, _ConveyorElement);
        }
    }   
}