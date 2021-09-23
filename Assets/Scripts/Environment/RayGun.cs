using System;
using System.Collections.Generic;
using QueueConnect.Config;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.Environment
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    [HideMonoScript]
    public class RayGun : MonoBehaviour
    {
        #region Inspector Fields

        [Tooltip("Animator Component")] [SerializeField]
        private Animator animator;

        #endregion

        #region Privates

        private static RayGun instance;
        private LayerMask robotLayer;
        private static readonly int SHOOT = Animator.StringToHash("Shoot");

        #endregion

        #region Properties
        
        public static Queue<RobotBehaviour> Robots { get; } = new Queue<RobotBehaviour>();


        #endregion

        #region Events

        public static event Action OnRayGunFire;

        #endregion

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            instance = Singleton.Persistent(this);
            robotLayer = robotLayer.LayerToLayerMask(LayerSettings.RobotLayer);
        }

        private void Update()
        {
            var _raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, GameConfig.MapHeight / 2, robotLayer);

            if (_raycastHit2D.collider != Robots.SafePeek()?.RobotCollider)
            {
                return;
            }

            if (Robots.SafePeek()?.DisabledPartsCount > 0 && _raycastHit2D.collider.bounds.center.x >= transform.position.x)
            {
                Shoot();
                Robots.SafePeek().DestroyRobot(false, GameController.GameState != GameState.Playing);
                
                Robots?.Dequeue();
            }
            // Temporary fix
            else if(Robots.SafePeek()?.DisabledPartsCount <= 0)
            {
                Robots?.Dequeue();
            }
        }

        /// <summary>
        /// Destroys the Robot
        /// </summary>
        private void Shoot()
        {
            AudioSystem.PlayVFX(VFX.RaygunFire);
            OnRayGunFire?.Invoke();
            animator.Play(SHOOT, 0, .3f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawRay(transform.position, Vector2.down * (GameConfig.MapHeight / 2));
        }
#endif
    }
}