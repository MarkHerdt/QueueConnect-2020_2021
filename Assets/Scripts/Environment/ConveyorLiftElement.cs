using QueueConnect.ExtensionMethods;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Environment
{
    [HideMonoScript]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ConveyorLiftElement : MonoBehaviour
    {
        #region Inspector Field
            [Tooltip("SpriteRenderer Component")]
            [SerializeField] private SpriteRenderer spriteRenderer;
            [Tooltip("ConveyorLift Script")]
            [SerializeField] private ConveyorLift conveyorLift;
            [Tooltip("Position the Robot is released at")]
            [SerializeField] private GameObject robotRelease;
            [Header("Debug")]
            [Tooltip("RobotBehaviour of the Robot in this Element")]
            [SerializeField][ReadOnly] private RobotBehaviour robot;
        #endregion

        #region Privates
            private float height;
            private Transform thisTransform;
            private Vector3 position;
        #endregion

        #region Properties
            /// <summary>
            /// RobotBehaviour of the Robot in this Element
            /// </summary>
            public RobotBehaviour Robot { get => robot; set => robot = value; }
        #endregion
        
        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (conveyorLift == null)
            {
                conveyorLift = GetComponentInParent<ConveyorLift>();
            }
            if (robotRelease == null)
            {
                var _children = conveyorLift.GetComponentsInChildren<Transform>();

                foreach (var _child in _children)
                {
                    if (!_child.Tag(Tags.RobotRelease)) continue;
                        robotRelease = _child.gameObject;
                        break;
                }
            }

            height = spriteRenderer.bounds.size.y;
            thisTransform = transform;
        }

        private void Update()
        {
            MoveElement();
        }

        private void OnTriggerEnter2D(Collider2D _Other)
        {
            if (_Other.gameObject != robotRelease.gameObject) return;

                if (Robot != null)
                {
                    Robot.ReleaseRobot();
                }
            
        }

        private void OnTriggerExit2D(Collider2D _Other)
        {
            if (_Other.gameObject != conveyorLift.gameObject) return;
            
                ResetPosition();
        }

        /// <summary>
        /// Moves the Element upwards
        /// </summary>
        private void MoveElement()
        {
            position = transform.localPosition;
            
            position = new Vector2(position.x, position.y + (conveyorLift.Speed * Time.deltaTime));
            thisTransform.localPosition = position;
        }

        /// <summary>
        /// Resets the position of the Element if it's out of screen
        /// </summary>
        private void ResetPosition()
        {
            // Sometimes RobotStands are stuck to the LiftElement (dirty fix)
            if (transform.childCount > 0 && robot == null)
            {
                transform.GetChild(0).GetComponent<RobotBehaviour>().ReturnToPool();
            }
            Robot = null;

            thisTransform.localPosition = thisTransform.localPosition.WithY(ConveyorLift.LiftElements[0].transform.localPosition.y - height);
            conveyorLift.UpdateList(this);
        }
    }   
}