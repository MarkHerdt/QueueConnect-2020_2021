using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace QueueConnect.Config
{
    /// <summary>
    /// Class to set all Prefabs for the Particles
    /// </summary>
    [HideMonoScript, ExecuteAlways]
    [CreateAssetMenu(menuName = "PoolPrefabs", fileName = "PoolPrefabs_SOB")]
    public class PoolPrefabs : ScriptableObject
    {
        #region Inspector Fields
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Prefabs", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title1;
                #pragma warning restore 649
            #endif
            #pragma warning disable 649
            [Tooltip("How many Prefabs to instantiate for the ObjectPool")]
            [BoxGroup("RepairArm")]
            [SerializeField][ReadOnly] private byte repairArmAmount;
            [Tooltip("GameObject that is spawned when a Robot Part has been repaired")]
            [AssetsOnly, BoxGroup("RepairArm")]
            [SerializeField] private GameObject repairArmPrefab;
            [Tooltip("How many Prefabs to instantiate for the ObjectPool")]
            [BoxGroup("RepairParticle")]
            [SerializeField][ReadOnly] private byte repairParticleAmount;
            [Tooltip("GameObject that is spawned when a Robot Part has been repaired")]
            [AssetsOnly, BoxGroup("RepairParticle")]
            [SerializeField] private GameObject repairParticlePrefab;
            [Tooltip("How many Prefabs to instantiate for the ObjectPool")]
            [BoxGroup("Explosion")]
            [SerializeField][ReadOnly] private byte explosionParticleAmount;
            [Tooltip("GameObject that is spawned when a Robot explodes")]
            [AssetsOnly, BoxGroup("Explosion")]
            [SerializeField] private GameObject robotExplosionPrefab;
            #pragma warning restore 649
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("FilePaths", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title2;
                #pragma warning restore 649
            #endif
            [Tooltip("FilePath to the \"ParticlePrefabs\"-ScriptableObject (Is set inside the Constructor)"), LabelText("ParticlePrefabs FilePath")]
            [FilePath(ParentFolder = "Assets/Resources", Extensions = "asset", RequireExistingPath = true)]
            [ShowInInspector][ReadOnly] private static readonly string PARTICLE_PREFABS_FILEPATH;
        #endregion

        #region Privates
            private static PoolPrefabs instance;
        #endregion

        #region Properties
            /// <summary>
            /// How many Prefabs to instantiate for the ObjectPool
            /// </summary>
            public static byte RepairArmAmount => instance.repairArmAmount;
            /// <summary>
            /// GameObject that is spawned when a Robot Part has been repaired
            /// </summary>
            public static GameObject RepairArmPrefab => instance.repairArmPrefab;
            /// <summary>
            /// How many Prefabs to instantiate for the ObjectPool
            /// </summary>
            public static byte RepairParticleAmount => instance.repairParticleAmount;
            /// <summary>
            /// GameObject that is spawned when a Robot Part has been repaired
            /// </summary>
            public static GameObject RepairParticlePrefab => instance.repairParticlePrefab;
            /// <summary>
            /// How many Prefabs to instantiate for the ObjectPool
            /// </summary>
            public static byte ExplosionParticleAmount => instance.explosionParticleAmount;
            /// <summary>
            /// GameObject that is spawned when a Robot explodes
            /// </summary>
            public static GameObject RobotExplosionPrefab => instance.robotExplosionPrefab;
            #endregion

        static PoolPrefabs()
        {
            PARTICLE_PREFABS_FILEPATH = "Config/PoolPrefabs_SOB.asset";
        }
        
        /// <summary>
        /// Initializes this ScriptableObject
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            instance = Singleton.Persistent(instance, PARTICLE_PREFABS_FILEPATH.Substring(0, PARTICLE_PREFABS_FILEPATH.IndexOf('.')), true);
        }

        private void OnEnable()
        {
            ObjectPool.OnAdditionalObjectNeeded += AdditionalObjectNeeded;
        }

        private void OnDisable()
        {
            ObjectPool.OnAdditionalObjectNeeded -= AdditionalObjectNeeded;
        }

        /// <summary>
        /// Increments the amount the ObjectPools instantiate at start, when an ObjectPool needed to instantiate an additional Object during runtime
        /// </summary>
        /// <param name="_Pool">The ObjectPool that needed to instantiate a new Object</param>
        private void AdditionalObjectNeeded(ObjectPool _Pool)
        {
            if (_Pool == PoolController.RepairArmPool)
            {
                repairArmAmount++;
            }
            else if (_Pool == PoolController.RepairParticlePool)
            {
                repairParticleAmount++;
            }
            else if (_Pool == PoolController.ExplosionParticlePool)
            {
                explosionParticleAmount++;
            }
        }

        #if UNITY_EDITOR
            private void OnValidate()
            {
                if (!EditorApplication.isPlaying)
                {
                    Initialize();
                }
            }
        #endif
    }
}