using System;
using System.Collections;
using System.Text;
using QueueConnect.Plugins.Ganymed.Localization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QueueConnect.UISystem
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextDistortAnimation : MonoBehaviour, ILocalizationCallback
    {
        [MinMaxSlider(0,60)]
        [SerializeField] public Vector2 cooldownRange = new Vector2(15, 30);
        [SerializeField] private bool autoPlayOnStart = true;
        [SerializeField] private bool autoAnimate = true;

        private static readonly string[] _stringPool = 
        {
            "01001010111",
            "10111010011",
            "10101010011",
            "10101010101",
            "01110000010",
            "10010111010",
            "01000001001",
            "11101001000",
            "01101110101",
            "11101010110",
            "00010111110",
        };
        
        private static readonly WaitForSeconds _waitSecond = new WaitForSeconds(1);
        private static readonly WaitForSeconds _waitHalfSecond = new WaitForSeconds(.5f);
        private static readonly WaitForSeconds _wait = new WaitForSeconds(.04f);
        
        private Vector3 _defaultScale;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private Coroutine _coroutine;
        private string _textCache;
        private TMP_Text _tmpText;
        private float _cooldown = 1f;
        
        private void Awake()
        {
            var body = transform;
            _defaultScale = body.localScale;
            _defaultPosition = body.position;
            _defaultRotation = body.rotation;
            _tmpText = GetComponentInChildren<TMP_Text>();
            _cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            LocalizationManager.AddCallbackListener(this);
        }
        
        private void Start()
        {
            if(autoPlayOnStart) PlayAnimation();
            if (!autoAnimate)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown <= 0)
            {
                _cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
                PlayAnimation();
            }
        }

        public void PlayAnimation()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            if(string.IsNullOrWhiteSpace(_tmpText.text)) return;
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(CO_AnimationBinary());
            }
        }
        
        private IEnumerator CO_AnimationBinary()
        {
            _textCache = _tmpText.text;
            for (var i = 0; i < 15; i++)
            {
                _tmpText.text = _stringPool[Random.Range(0, _stringPool.Length)];
                yield return _wait;
            }
            _tmpText.text = _textCache;
            _coroutine = null;
        }



        #region --- [RESET] ---

        private void OnDestroy()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _tmpText.text = _textCache;
            }
        }
        
        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _tmpText.text = _textCache;
            }
        }

        #endregion

        public void OnLanguageLoaded(Language language)
        {
            try
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
