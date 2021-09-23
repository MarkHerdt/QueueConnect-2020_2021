using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector;

namespace QueueConnect.GameSystem
{
	/// <summary>
	/// Destroys/Deactivates a Particle after it's done playing
	/// </summary>
	[ExecuteInEditMode, HideMonoScript]
	[RequireComponent(typeof(ParticleSystem))]
	public class DestroyParticle : MonoBehaviour
	{
		#region Inspector Fields
			[Tooltip("Returns this Object back to its ObjectPool if \"true\"")]
			[BoxGroup("Bools", ShowLabel = false)]
			#pragma warning disable 649
			[SerializeField] private bool objectPooling;
			[Tooltip("Deactivates this Particle instead of destroying it if \"true\"")]
			[BoxGroup("Bools", ShowLabel = false)]
			[SerializeField] private bool deactivate;
			#pragma warning restore 649
			[Tooltip("All \"ParticleSystems\" in this GameObject (Also includes children)")]
			[ChildGameObjectsOnly, ListDrawerSettings(Expanded = true)]
			[SerializeField][ReadOnly] private ParticleSystem[] particleSystems;
        #endregion

        #region Privates
			private WaitForSeconds waitForSeconds;
			private float duration;
        #endregion

        #region Properties
			/// <summary>
			/// ObjectPool this Particle belongs to
			/// </summary>
			public ObjectPool ObjectPool { get; set; }
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
			foreach (var _particleSystem in particleSystems.ToList())
			{
				var _mainModule = _particleSystem.main;
				duration += _mainModule.duration / _mainModule.simulationSpeed; 
				duration += _mainModule.startDelay.constant;
			}

			waitForSeconds = new WaitForSeconds(duration);
		}

        private void OnEnable()
		{
			StartCoroutine(CheckIfAlive());
		}
	
		/// <summary>
		/// Checks if the ParticleSystem has finished and Destroys/Deactivates it
		/// </summary>
		private IEnumerator CheckIfAlive ()
		{
			do
			{
				yield return waitForSeconds;
				
				if (objectPooling)
				{
					if (ObjectPool != null)
					{
						ObjectPool.ReturnObject(gameObject, true);
						break;
					}
				}
				if (deactivate)
				{
					gameObject.SetActive(false);
				}
				else
				{
					#if UNITY_EDITOR
						if (!EditorApplication.isPlaying)
						{
							// Only destroys the Object when not in Prefab view
							if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null || ForceDestroy)
							{
								DestroyImmediate(gameObject);
							}
						}
						else
						{
							Destroy(gameObject);
						}
					#else
							Destroy(gameObject);
					#endif
				}
				
			} while ((object)particleSystems[0] != null && particleSystems[0].IsAlive(true));
		}

		#if UNITY_EDITOR
			/// <summary>
			/// Returns this Object back to its ObjectPool if "true"
			/// </summary>
			public bool ObjectPooling { set => objectPooling = value; }
			/// <summary>
			/// Deactivates this Particle instead of destroying it if "true"
			/// </summary>
			public  bool Deactivate { set => deactivate = value; }
			/// <summary>
			/// Forces this GameObject to be destroyed (Even in Prefab-mode, be careful when you use this!) (Only for Editor)
			/// </summary>
			public bool ForceDestroy { get; set; }

			/// <summary>
			/// Gets a reference to all "ParticleSystems" in this GameObject
			/// </summary>
			[Button]
			private void GetParticleSystems()
			{
				particleSystems = GetComponentsInChildren<ParticleSystem>();
			}

			private void OnValidate()
			{
				if (objectPooling)
				{
					deactivate = false;
				}
				else if (deactivate)
				{
					objectPooling = false;
				}
			}
		#endif
    }
}