using System.Collections;
using System.Globalization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace QueueConnect.Environment
{
    /// <summary>
    /// Displays the new Score value on the ScoreDisplay
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI), typeof(Animation))]
    [HideMonoScript]
    public class UpdateScore : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("\"TextMeshProUGUI\"-Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private TextMeshProUGUI textMeshProUI;
            [Tooltip("Animator Component")]
            [ChildGameObjectsOnly]
            #pragma warning disable 109
            [SerializeField] private new Animation animation;
            #pragma warning restore 109
        #endregion

        #region Fileds
        
        private ulong cachedValue = default;
        private Coroutine Countdown = null;

        #endregion
        
        private void Awake()
        {
            if (textMeshProUI == null)
            {
                textMeshProUI = GetComponent<TextMeshProUGUI>();
            }
            if (animation == null)
            {
                animation = GetComponent<Animation>();
            }
        }

        /// <summary>
        /// Is called by the Animation when it has finished playing
        /// </summary>
        private void PlayAnimation()
        {
            
        }
        
        /// <summary>
        /// Updates the Text of the "TextMeshProUGUI"-Component to the passed string
        /// </summary>
        public void UpdateText(ulong value)
        {
            if(Countdown != null)
                StopCoroutine(Countdown);
            
            cachedValue = (value * 100);
            textMeshProUI.text = cachedValue.ToString();
            animation.Stop();
            animation.Play();
        }

        /// <summary>
        /// Reset the score to 0 (visually) over the set duration.
        /// </summary>
        /// <param name="duration"></param>
        public void ResetScore(float duration = 1f)
        {
            if(Countdown != null)
                StopCoroutine(Countdown);

            Countdown = StartCoroutine(CountdownRoutine(duration));
        }

        private IEnumerator CountdownRoutine(float duration = 1f)
        {
            duration = Mathf.Max(duration, .1f);
            var decrement = cachedValue;
            while (cachedValue > 0)
            {
                cachedValue -= (ulong)Mathf.Max(1,(int)(decrement * Time.deltaTime));
                textMeshProUI.text = Mathf.Max(0,cachedValue).ToString(CultureInfo.InvariantCulture);
                yield return null;
            }

            Countdown = null;
        }
        
        private void OnApplicationQuit()
        {
            if(Countdown != null)
                StopCoroutine(Countdown);
        }

        public void ResetToDefault()
        {
            cachedValue = default;
            textMeshProUI.text = "Queue";
        }
    }
}