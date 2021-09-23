using System.Collections.Generic;
using QueueConnect.Config;
using QueueConnect.Environment;
using QueueConnect.Robot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.GameSystem
{
    [HideMonoScript]
    public class PoolController : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("List of RobotPools for all the Robot Prefabs")]
            [ListDrawerSettings(DraggableItems = false)]
            [SerializeField] private List<RobotPool> robotPools = new List<RobotPool>();
        #endregion
        
        #region Privates
            private ObjectPool repairArmPool;
            private ObjectPool repairParticlePool;
            private ObjectPool explosionParticlePool;
        #endregion

        #region Properties
            /// <summary>
            /// Singleton of "PoolController"
            /// </summary>
            public static PoolController Instance { get; private set; }
            /// <summary>
            /// List of RobotPools for all the Robot Prefabs
            /// </summary>
            public static List<RobotPool> RobotPools => Instance.robotPools;
            /// <summary>
            /// Object Pool for "RepairParticle"
            /// </summary>
            public static ObjectPool RepairArmPool => Instance.repairArmPool;
            /// <summary>
            /// Object Pool for "RepairParticle"
            /// </summary>
            public static ObjectPool RepairParticlePool => Instance.repairParticlePool;
            /// <summary>
            /// Object Pool for "ExplosionParticle"
            /// </summary>
            public static ObjectPool ExplosionParticlePool => Instance.explosionParticlePool;
        #endregion

        private void Awake()
        {
            Instance = Singleton.Persistent(this);
        }

        private void Start()
        {
            EventController.OnParticleInstantiated += ParticleInstantiated;
        }

        private void OnDestroy()
        {
            EventController.OnParticleInstantiated -= ParticleInstantiated;
        }
        
        /// <summary>
        /// Creates all needed ObjectPools
        /// </summary>
        public void CreatePools()
        {
            CreateRobotPools();
            CreateRepairArmPool();
            CreateRepairParticlePool();
            CreateExplosionParticlePool();
        }
        
        /// <summary>
        /// Creates an ObjectPool for each Robot type
        /// </summary>
        private static void CreateRobotPools()
        {
            for (short i = 0; i < RobotPartConfig.RobotTypePrefabs.Count; i++)
            {
                // Index this Robot Type has in the Object Pool
                RobotPartConfig.RobotTypePrefabs[i].RobotScript.PoolIndex = (short)RobotPools.Count;

                RobotPools.Add(new RobotPool(_Type:             RobotPartConfig.RobotTypePrefabs[i].Type,
                                                  _SpawnChance: RobotPartConfig.RobotTypePrefabs[i].SpawnChance, 
                                                  _TypeIndex:   i,
                                                  _StartAmount: GameConfig.RobotPoolStartCount,
                                                  _Pool:        new ObjectPool(_Prefab:        RobotPartConfig.RobotTypePrefabs[i].Prefab,
                                                                               _Parent:        RobotSpawner.Instance.transform,
                                                                               _Origin:        null,
                                                                               _CallBack:      null,
                                                                               _ComponentType: RobotPartConfig.RobotTypePrefabs[i].Prefab.GetComponent<RobotBehaviour>())));
            }
        }

        /// <summary>
        /// Creates an ObjectPool for the "RepairArms"
        /// </summary>
        private void CreateRepairArmPool()
        {
            repairArmPool = new ObjectPool(_Prefab:        PoolPrefabs.RepairArmPrefab, 
                                           _Parent:        gameObject.transform,
                                           _Origin:        null,
                                           _CallBack:      null,
                                           _ComponentType: PoolPrefabs.RepairArmPrefab.GetComponent<RepairArm>());

            repairArmPool.AddObject(PoolPrefabs.RepairArmAmount);
        }
        
        /// <summary>
        /// Creates an ObjectPool for the "RepairParticles"
        /// </summary>
        private void CreateRepairParticlePool()
        {
            repairParticlePool = new ObjectPool(_Prefab:        PoolPrefabs.RepairParticlePrefab, 
                                                _Parent:        gameObject.transform, 
                                                _Origin:        null, 
                                                _CallBack:      EventController.ParticleInstantiated, 
                                                _ComponentType: PoolPrefabs.RepairParticlePrefab.GetComponent<RepairParticleBehaviour>());

            repairParticlePool.AddObject(PoolPrefabs.RepairParticleAmount);
        }

        /// <summary>
        /// Creates an ObjectPool for the "ExplosionParticles"
        /// </summary>
        private void CreateExplosionParticlePool()
        {
            explosionParticlePool = new ObjectPool(_Prefab:   PoolPrefabs.RobotExplosionPrefab, 
                                                   _Parent:   gameObject.transform, 
                                                   _Origin:   null, 
                                                   _CallBack: EventController.ParticleInstantiated);

            explosionParticlePool.AddObject(PoolPrefabs.ExplosionParticleAmount);
        }

        /// <summary>
        /// Gives the "DestroyParticle"-Script the reference to the Particles ObjectPool
        /// </summary>
        /// <param name="_Pool">ObjectPool the Particle belongs to</param>
        /// <param name="_Particle">Particle that was Instantiated</param>
        private static void ParticleInstantiated(ObjectPool _Pool, GameObject _Particle)
        {
            _Particle.GetComponent<DestroyParticle>().ObjectPool = _Pool;
        }
    }
}