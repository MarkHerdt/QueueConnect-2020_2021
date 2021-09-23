using System;
using System.Linq;
using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Attaches the missing Parts to the Robots
    /// </summary>
    [HideMonoScript]
    public class RepairArm : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("Sprite of the RobotPart on the RepairArm")]
            [SerializeField] private SpriteRenderer partSprite;
        #endregion
        
        #region Privates
            private RobotBehaviour robot;
            private Part part;
            private Vector2? targetPosition;
            private bool robotRepaired;
        #endregion

        private void Awake()
        {
            if (partSprite == null)
            {
                partSprite = GetComponentsInChildren<SpriteRenderer>().First(_Child => _Child.gameObject.Tag(Tags.RepairArm));
            }
        }

        private void Update()
        {
            Move();
        }

        /// <summary>
        /// Moves the Arm towards the Robot Parts that needs to be repaired
        /// </summary>
        private void Move()
        {
            if (targetPosition == null) return;
            
                var _position = transform.position;
                _position = Vector2.MoveTowards(_position, targetPosition.Value.WithX(_position.x), GameConfig.RepairArmSpeed * Time.deltaTime);
                transform.position = _position;

                if (Math.Abs(transform.position.y - targetPosition.Value.y) < .1f)
                {
                    if (!robotRepaired)
                    {
                        robotRepaired = true;
                        
                        robot.RepairRobotPartAnimation(part);
                        partSprite.sprite = null;
                    
                        transform.SetParent(null);
                        robot.ActiveRepairArms.Remove(this);
                        targetPosition = PoolPrefabs.RepairArmPrefab.transform.position.WithX(transform.position.x);   
                    }
                    else
                    {
                        robotRepaired = false;
                        
                        PoolController.RepairArmPool.ReturnObject(gameObject, true);
                    }
                }
        }

        /// <summary>
        /// Repairs the passed Robot Part
        /// </summary>
        /// <param name="_Robot">Robot to repair the Part on</param>
        /// <param name="_Part">Part to repair</param>
        /// <param name="_TargetPosition">Position of the Robot Part on the Robot</param>
        public void RepairPart(RobotBehaviour _Robot, Part _Part, Vector2 _TargetPosition)
        {
            robot = _Robot;
            part = _Part;
            targetPosition = _TargetPosition;

            partSprite.sprite = _Part.RobotSprite;
            
            _Robot.RepairRobotPart(_Part);
            _Robot.ActiveRepairArms.Add(this);
        }

        /// <summary>
        /// Cancels the repair animation
        /// </summary>
        public void SkipRepair()
        {
            transform.SetParent(null);
            targetPosition = (PoolPrefabs.RepairArmPrefab.transform.position + part.RobotSprite.bounds.extents).WithX(transform.position.x);
            robotRepaired = true;
        }
    }   
}