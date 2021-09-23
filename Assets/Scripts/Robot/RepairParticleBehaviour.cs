using QueueConnect.Development;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.Robot
{
    /// <summary>
    /// Class for the Particle that is spawned when a RobotPart has been repaired
    /// </summary>
    [ExecuteInEditMode]
    [HideMonoScript]
    public class RepairParticleBehaviour : MonoBehaviour
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                [Tooltip("How fast the Particle is played")]
                #pragma warning disable 649
                [SerializeField] private float simulationSpeed = 2.5f;
                #pragma warning restore 649
            #endif
            [Tooltip("All \"ParticleSystems\" in this GameObject (Also includes children)")]
            [ChildGameObjectsOnly, ListDrawerSettings(Expanded = true)]
            [SerializeField][ReadOnly] private ParticleSystem[] particleSystems;
        #endregion

        #region Privates
            private float duration;
            private Vector2 origin;
            private Vector2 target;
        #endregion

        private void Awake()
        {
            if (particleSystems.Length <= 0)
            {
                particleSystems = GetComponentsInChildren<ParticleSystem>();
            }
            else
            {
                for (byte i = 0; i < particleSystems.Length; i++)
                {
                    if (particleSystems[i] != null) continue;
                        particleSystems = GetComponentsInChildren<ParticleSystem>();
                        break;
                }
            }

            // Get the total duration of all ParticleSystems in this GameObject
            foreach (var _particleSystem in particleSystems)
            {
                var _mainModule = _particleSystem.main;
                duration += _mainModule.duration / _mainModule.simulationSpeed;
            }
        }

        private void Update()
        {
            Move();
        }

        /// <summary>
        /// The positions the Particle moves along
        /// </summary>
        /// <param name="_Origin">Spawn position of the Particle</param>
        /// <param name="_Target">Targeted position of the Particle</param>
        public void SetPositions(Vector2 _Origin, Vector2 _Target)
        {
            this.origin = _Origin;
            this.target = _Target;

            transform.localPosition = _Origin;
        }

        /// <summary>
        /// Moves the Particle from its "Origin"-position to "Target"-position
        /// </summary>
        private void Move()
        {
            gameObject.transform.localPosition = Vector2.MoveTowards(transform.localPosition, target, (Vector2.Distance(origin, target) / duration) * Time.deltaTime);
        }

        #if UNITY_EDITOR
            /// <summary>
            /// Gets a reference to all "ParticleSystems" in this GameObject
            /// </summary>
            [Button]
            private void GetParticleSystems()
            {
                particleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            [Button]
            private void SetSimulationSpeed()
            {
                foreach (var _system in particleSystems)
                {
                    var _main = _system.main;
                    _main.simulationSpeed = simulationSpeed;
                }

                if (particleSystems.Length <= 0)
                {
                    DebugLog.Red("No \"ParticleSystem\" found inside the Array");
                }
            }
        #endif
    }
}