using QueueConnect.Config;
using QueueConnect.GameSystem;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Environment
{
    public class ConveyorBelt : MonoBehaviour
     {
         #region Privates
            private Animator[] animators;
         #endregion
         
         private void Awake()
         {
             animators = GetComponentsInChildren<Animator>();
         }
         
         private void Start()
         {
             GameController.OnConveyorSpeedChanged += SetAnimationSpeed;
             SetAnimationSpeed(GameController.CurrentRobotSpeed);
         }

         private void OnCollisionEnter2D(Collision2D _Other)
         {
             _Other.transform.GetComponent<RobotBehaviour>()?.OnConveyorBelt();
         }

         /// <summary>
         /// Matches the AnimationSpeed of the ConveyorBelt to the speed the Robots move with
         /// </summary>
         private void SetAnimationSpeed(float speed)
         {
             foreach (var _animator in animators)
             {
                 _animator.speed = speed / 600;
             }
         }
     }   
}
