using System;
using System.Collections;
using System.Text;
using Ganymed.Utils.Singleton;
using JetBrains.Annotations;
using QueueConnect.CollectableSystem;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.UISystem;
using SoundSystem.Core;
using TMPro;
using UnityEngine;

namespace QueueConnect.Tutorial
{
    [RequireComponent(typeof(Animator))]
    public class TooltipMonitor : MonoSingleton<TooltipMonitor>
    {
        [SerializeField] private TMP_Text textElement = default;
        
        private Animator animator = null;
        private bool isDisplayActive = false;
        private bool hideRequested = false;
        private TextDistortAnimation textAnimation = null;
        
        private static readonly int Show = Animator.StringToHash("show");

        private string textCache;

        /// <summary>
        /// Is fired when the WaveCountdown has reached zero
        /// </summary>
        public static event Action OnWaveCountdownFinished;
        
        protected override void Awake()
        {
            base.Awake();
            textAnimation = textElement.GetComponent<TextDistortAnimation>();
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            EventController.OnGameEnded += aborted => StartCoroutine(HideTutorialTextRoutine(1));
        }

        public static void ShowText(string text, bool playAnimationAndAudio = true) => Instance.ShowTutorialTextInternal(text, playAnimationAndAudio);
        
        public static void ShowText(string text, int timerMs)
        {
            Instance.ShowTutorialTextInternal(text);
            Instance.StartCoroutine(Instance.HideTutorialTextRoutine(timerMs));
        }

        public static void HideDisplay(int delay) => Instance.StartCoroutine(Instance.HideTutorialTextRoutine(delay));


        private void ShowTutorialTextInternal(string text, bool playAnimationAndAudio = true)
        {
            textCache = text;
            hideRequested = false;
            StopAllCoroutines();
            if (isDisplayActive)
            {
                if (playAnimationAndAudio)
                {
                    UpdateTextAnimation(text);
                }
                else
                {
                    UpdateText(text);
                }
            }
            else
            {
                AudioSystem.PlayVFX(VFX.OnTutorialDisplaySpawn);
                animator.SetBool(Show, true);
            }

            isDisplayActive = true;
        }

        private IEnumerator HideTutorialTextRoutine(int delay = 3000)
        {
            hideRequested = true;
            yield return new WaitForSeconds(delay / 1000f);
            
            if (hideRequested)
            {
                AudioSystem.PlayVFX(VFX.OnTutorialDisplayDeSpawn);
                animator.SetBool(Show, false);
                isDisplayActive = false;
            }
        }

        [UsedImplicitly]
        public void OnAnimationCompletedCallback()
        {
            if(!isDisplayActive){ return;}
            UpdateTextAnimation(textCache);
        }

        private void UpdateTextAnimation(string text)
        {
            AudioSystem.PlayVFX(VFX.OnTutorialDisplayUpdate);
            textElement.text = text;
            isDisplayActive = true;
            textAnimation.PlayAnimation();
        }
        
        private void UpdateText(string text)
        {
            textElement.text = text;
            isDisplayActive = true;
        }
        
        /// <summary>
        /// Starts a wave countdown display on the in game monitor.
        /// </summary>
        /// <param name="countdownDuration"></param>
        /// <param name="waveIndex"></param>
        public static void StartCountdownToWave(float countdownDuration, int waveIndex)
        {
            ItemSpawner.Instance.StartCoroutine(WaveCountdown(countdownDuration, waveIndex));
        }

        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        
        private static readonly WaitForSeconds _wait02 = new WaitForSeconds(.1f);
        private static readonly WaitForSeconds _wait08 = new WaitForSeconds(.9f);
        
        private static IEnumerator WaveCountdown(float duration, int waveIndex)
        {
            _stringBuilder.Clear();
            ShowText(string.Empty);
            var left = (int)duration + 2;

            for (var i = left - 1; i >= 0; i--)
            {
                if(i < left -2)
                    AudioSystem.PlayVFX(VFX.OnCountdownTick);
                
                yield return _wait02;

                if (i < left - 1)
                {
                    _stringBuilder.Append("Next Wave [");
                    _stringBuilder.Append(waveIndex);
                    _stringBuilder.Append("]\n<size=42>");
                    _stringBuilder.Append(i.ToString("00"));
                    _stringBuilder.Append("</size>");
                    ShowText(_stringBuilder.ToString(), false);
                }
                
                yield return _wait08;
                
                while (GameController.IsPaused)
                {
                    yield return null;
                }
            }
            HideDisplay(0);
            AudioSystem.PlayVFX(VFX.OnWaveStarted);
            //OnWaveCountdownFinished?.Invoke();
        }
    }
}
