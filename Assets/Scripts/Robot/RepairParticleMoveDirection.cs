using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QueueConnect.Robot
{
    /// <summary>
    /// Sets the position, the Particle that is spawned when a RobotParts has been repaired, moves along
    /// </summary>
    [RequireComponent(typeof(RobotBehaviour))]
    [ExecuteInEditMode, HideMonoScript]
    public class RepairParticleMoveDirection : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("RobotBehaviour-Script of this Robot")]
            [ChildGameObjectsOnly]
            [SerializeField] private RobotBehaviour robotBehaviour;
            [Tooltip("Positions the Particle moves along for each RobotPart")]
            [ListDrawerSettings(DraggableItems = false)]
            #pragma warning disable 649
            [SerializeField] private ParticleDirection[] particleDirection;
            #pragma warning restore 649
        #endregion

        private void Awake()
        {
            if (robotBehaviour == null)
            {
                robotBehaviour = GetComponent<RobotBehaviour>();
            }
        }

        /// <summary>
        /// Instantiates the Particle
        /// </summary>
        public void SpawnParticle(sbyte _Index)
        {
            // Index of "particleDirection" must be subtracted by 1, because the incoming index also contains the RobotStand
            var _particle = PoolController.RepairParticlePool.GetObject(gameObject.transform, particleDirection[_Index - 1].from);
            ((RepairParticleBehaviour) _particle.Component).SetPositions(particleDirection[_Index - 1].from, particleDirection[_Index - 1].to);
        }

        #if UNITY_EDITOR
            #region Inspector Fields
                [Tooltip("Sprite to visualize the \"From\"-positions")]
                [AssetsOnly]
                #pragma warning disable 649
                [SerializeField] private GameObject startSprite;
                [Tooltip("Sprite to visualize the \"To\"-positions")]
                [AssetsOnly]
                [SerializeField] private GameObject endSprite;
                #pragma warning restore 649
                [Tooltip("Needs to be \"true\" if you want to make a new \"ParticleDirection\"-Array with the \"GetRepairableParts\"-Method")]
                [SerializeField] private bool allowNewParticleDirectionArray;
                [Tooltip("Index of Robot Part")]
                [SerializeField] private sbyte index;
            #endregion

            #region Privates
                private readonly List<GameObject> testSprites = new List<GameObject>();
                private const sbyte TMP_INDEX = -1;
            #endregion

            private void OnValidate()
            {
                if (index == TMP_INDEX) return;
                    if (index > particleDirection.Length - 1)
                    {
                        index = (sbyte)(particleDirection.Length - 1);
                    }
            }

            /// <summary>
            /// Instantiates the Particle
            /// </summary>
            [Button]
            private void SpawnParticle()
            {
                // Index of "robotBehaviour.Parts" needs to be added by 1 because it also contains the RobotStand
                var _particle = Instantiate(PoolPrefabs.RepairParticlePrefab, particleDirection[index].from, robotBehaviour.Parts[index + 1].transform.localRotation, gameObject.transform);
                _particle.GetComponent<RepairParticleBehaviour>().SetPositions(particleDirection[index].from, particleDirection[index].to);
                
                var _destroyParticle = _particle.GetComponent<DestroyParticle>();
                _destroyParticle.ObjectPooling = false;
                _destroyParticle.Deactivate = false;
                _destroyParticle.ForceDestroy = true;
                
                _particle.SetActive(true);
            }

            /// <summary>
            /// Spawns 2 Sprites in this Robot
            /// </summary>
            [Button]
            private void SpawnTestSprite()
            {
                testSprites.ToList().ForEach(DestroyImmediate);
                testSprites.Clear();

                var _localPosition = transform.localPosition;
                testSprites.Add(Instantiate(startSprite, _localPosition, Quaternion.identity, gameObject.transform));
                testSprites.Add(Instantiate(endSprite, _localPosition, Quaternion.identity, gameObject.transform));
            }
            
            /// <summary>
            /// Deletes the 2 Sprites to visualize the ParticleDirection
            /// </summary>
            [Button]
            private void DeleteTestSprite()
            {
                testSprites.ToList().ForEach(DestroyImmediate);
                testSprites.Clear();
            }
        
            /// <summary>
            /// Gets all Parts of this Robot that are possible to be repaired
            /// </summary>
            [Button]
            private void GetRepairableParts()
            {
                if (allowNewParticleDirectionArray)
                {
                    particleDirection = new ParticleDirection[robotBehaviour.Parts.Length - 1];

                    // Starts at 1, so the RobotStand won't be added
                    for (byte i = 1; i < robotBehaviour.Parts.Length; i++)
                    {
                        particleDirection[i - 1].part = robotBehaviour.Parts[i].gameObject;
                    }

                    allowNewParticleDirectionArray = false;
                }
                else
                {
                    DebugLog.White_Red_White("\"AllowNewParticleDirectionArray\" needs to be set to \"true\" \n", "WARNING: ", "All positions inside the Array have to be set again if you call this Method!");
                }
            }

            /// <summary>
            /// Sets "From/To"-positions of the "particleDirection"-entry to the coordinates of the "Start/End"-Sprites
            /// </summary>
            [Button]
            private void SetPositions()
            {
                if (testSprites.Count <= 0 || testSprites[0] == null || testSprites[1] == null) return;
                    particleDirection[index].from = new Vector2(testSprites[0].transform.localPosition.x, testSprites[0].transform.localPosition.y);
                    particleDirection[index].to = new Vector2(testSprites[1].transform.localPosition.x, testSprites[1].transform.localPosition.y);
            }

            private void OnDrawGizmos()
            {
                if (testSprites.Count <= 0 || testSprites[0] == null || testSprites[1] == null) return;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(testSprites[0].transform.position, testSprites[1].transform.position);
            }
        #endif

        /// <summary>
        /// Positions, the Particle moves along
        /// </summary>
        [Serializable]
        private struct ParticleDirection
        {
            #region Inspector Fields
                #pragma warning disable 649
                [Tooltip("The RobotPart (Not necessarily needed, just to show what Part the coordinates belong to)")]
                [SerializeField][ReadOnly] public GameObject part;
                [Tooltip("Spawn position of the Particle")]
                [SerializeField] public Vector2 from;
                [Tooltip("Targeted position of the Particle")]
                [SerializeField] public Vector2 to;
                #pragma warning restore 649
            #endregion
        }
    }
}