using System.Collections;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.CollectableSystem;
using Sirenix.OdinInspector;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Displays the Score of the Player
    /// </summary>
    [HideMonoScript]
    public class ScoreHandler : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("\"UpdateScore\"-Script off \"Score-Amount\"-GameObject in children")]
            [ChildGameObjectsOnly]
            [SerializeField] private UpdateScore scoreBehaviour;
            [Tooltip("\"Multiplier-Amount\" GameObject in children")]
            [ChildGameObjectsOnly]
            #pragma warning  disable
            [SerializeField] private UpdateMultiplier multiplierBehaviour;
            [SerializeField] private Animation startAnimation = null;

            [SerializeField] private Color scoreShieldActiveColor = Color.cyan;
            [SerializeField] private Color scoreShieldInactiveColor = Color.grey;
            [SerializeField] private SpriteRenderer scoreShieldImage = null;

            [Header("Reset Transforms")]
            [SerializeField] private Transform scoreParentTransform;
            [SerializeField] private Transform scoreTransform;
            [SerializeField] private Transform multiplierTransform;
            [SerializeField] private Transform multiplierParentTransform;
            #pragma warning enable
        #endregion

        #region Privates
            private Vector3 initScoreParentTransformPosition = default;
            private Vector3 initMultiplierParentTransformPosition = default;
            private static ScoreHandler instance;
            private float headlightExitXPosition;
            
            private ulong score;
            private ulong currentMultiplier;

            private bool hasScoreShield = false;
        #endregion
        
        #region --- [PROPERTIES] ---
        
        public static ulong Score
        {
            get => instance.score;
            private set
            {
                OnScoreChanged?.Invoke(value);
                instance.score = value;
            }
        }

        public static bool HasScoreShield
        {
            get => instance.hasScoreShield;
            set
            {
                if (!value && instance.hasScoreShield)
                {
                    AudioSystem.PlayVFX(VFX.OnShiedDown);
                    instance.StartCoroutine(instance.SetScoreShieldColor(value));
                }
                else if (value && !instance.hasScoreShield)
                {
                    instance.StartCoroutine(instance.SetScoreShieldColor(value));
                }
                
                instance.hasScoreShield = value;
            }
        }

        private IEnumerator SetScoreShieldColor(bool active)
        {
            var colorA = active ? scoreShieldActiveColor : scoreShieldInactiveColor;
            var colorB = active ? scoreShieldInactiveColor : scoreShieldActiveColor;
            
            for (int i = 0; i < 6; i++)
            {
                scoreShieldImage.color = i.IsEven() ? colorA : colorB;
                AudioSystem.PlayVFX(VFX.ElectricSpark);
                yield return ItemSpawner.WaitForPointOne;
            }

            scoreShieldImage.color = colorA;
        }


        public static ulong Multiplier
        {
            get => instance.currentMultiplier;
            private set
            {
                OnMultiplierChanged?.Invoke(value);
                instance.currentMultiplier = value;
            }
        }

        #endregion
        
        #region --- [CSHARP EVENTS & DELEGATES] ---

        public delegate void ScoreDelegate(ulong newScore);
        public delegate void MultiplierDelegate(ulong newMultiplier);
        public static event ScoreDelegate OnScoreChanged;
        public static event MultiplierDelegate OnMultiplierChanged;

        #endregion

        private void Awake()
        {
            initScoreParentTransformPosition = scoreParentTransform.position;
            initMultiplierParentTransformPosition = multiplierParentTransform.position;
            
            if (scoreBehaviour == null)
            {
                scoreBehaviour = GetComponentInChildren<UpdateScore>();
            }

            instance = Singleton.Persistent(this);

            EventController.OnGameEnded += ResetDisplay;
            EventController.OnGameStarted += () =>
            {
                startAnimation.Play();
                multiplierBehaviour.EnableMultiplierReset = true;
                multiplierBehaviour.ResetMultiplier();
                scoreBehaviour.ResetScore();
            };
        }

        private void Start()
        {
            headlightExitXPosition = RobotScanner.BoxCollider2D.bounds.max.x;
        }

        public void ResetDisplay(bool wasAborted)
        {
            scoreBehaviour.ResetToDefault();
            multiplierBehaviour.ResetToDefault();
            scoreTransform.localScale = Vector3.zero;
            multiplierTransform.localScale = Vector3.zero;

            scoreParentTransform.position = initScoreParentTransformPosition;
            multiplierParentTransform.position = initMultiplierParentTransformPosition;
        }

        /// <summary>
        /// Calculates the distance between the Robot and the exit Point of the Headlight
        /// </summary>
        /// <param name="_RobotPosition">WorldPosition fo the Robot (transform.position, not transform.localPosition)</param>
        public static void IncreaseScore(Vector2 _RobotPosition)
        {
            if (GameController.GameState != GameState.Playing) return;
            
            var _distance = Vector2.Distance(_RobotPosition, new Vector2(instance.headlightExitXPosition, _RobotPosition.y)) / 100f;
            var _value = (ulong)(_distance < 1f ? 1 : Mathf.RoundToInt(_distance));
            
            Score += _value * (Multiplier <= 0 ? 1 : Multiplier);

            AudioSystem.PlayVFX(VFX.UIOnScoreIncreased);
            instance.scoreBehaviour.UpdateText(Score);
        }

        /// <summary>
        /// Increases the multiplier by 1
        /// </summary>
        public static void IncreaseMultiplier()
        {
            if (GameController.GameState != GameState.Playing) return;
            Multiplier++;
            instance.multiplierBehaviour.UpdateMultiplierText(Multiplier);
        }
        
        
       
        public static void CutMultiplierInHalf()
        {
            if (GameController.GameState != GameState.Playing) return;
            if (HasScoreShield)
            {
                HasScoreShield = false;
                return;
            }
            
            Multiplier /= 2;
            instance.multiplierBehaviour.UpdateMultiplierText(Multiplier);
        }
        

        /// <summary>
        /// Resets the multiplier back to 0
        /// </summary>
        public static void ResetMultiplier(bool fullReset = false)
        {
            if (GameController.GameState != GameState.Playing) return;
            if (!fullReset && HasScoreShield)
            {
                HasScoreShield = false;
                return;
            }
            
            instance.multiplierBehaviour.ResetMultiplier();
            Multiplier = 0;
        }


        /// <summary>
        /// Resets the Score to 0
        /// </summary>
        public static void ResetScore()
        {
            ResetMultiplier(true);
            Score = 0;
            instance.scoreBehaviour.ResetScore();
        }
    }
}