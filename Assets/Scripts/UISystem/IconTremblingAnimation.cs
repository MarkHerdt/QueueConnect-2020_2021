using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace QueueConnect.UISystem
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class IconTremblingAnimation : MonoBehaviour
    {
        [MinMaxSlider(1f, 30f)]
        [SerializeField] private Vector2 cooldownRange = new Vector2(5f, 15f);

        [MinMaxSlider(0f, 15f)]
        [SerializeField] private Vector2 initialCooldownRange = new Vector2(0f, 5f);
        
        private Coroutine coroutine;
        private float cooldown = 5f;
        private static readonly WaitForSeconds Wait = new WaitForSeconds(.05f);
        private RectTransform rectTransform;

        private Vector2 baseMin;
        private Vector2 baseMax;

        private void Awake()
        {
            cooldown = Random.Range(initialCooldownRange.x, initialCooldownRange.y);
            rectTransform = transform as RectTransform 
                            ?? throw new Exception($"{nameof(RectTransform)} Required!");

            baseMin = rectTransform.offsetMin;
            baseMax = rectTransform.offsetMax;

            if (GetComponentInParent<Button>() is Button button)
            {
                button.onClick.AddListener(PlayAnimation);
            }
        }

        private void Update()
        {
            if ((cooldown -= Time.deltaTime) <= 0 && coroutine == null)
            {
                PlayAnimation();
            }
        }


        public void PlayAnimation()
        {
            cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
            coroutine = StartCoroutine(CO_Animation());
        }
        
        private IEnumerator CO_Animation()
        {
            for (var i = 0; i < Random.Range(2,13); i++)
            {
                var randomValueX = Random.Range(0, 3);
                var randomValueY = Random.Range(0, 3);
                rectTransform.offsetMax = new Vector2(baseMax.x + randomValueX, baseMax.y + randomValueY * -1f);
                rectTransform.offsetMin = new Vector2(baseMin.x + randomValueX, baseMin.y + randomValueY * -1f);
                yield return Wait;
            }
            rectTransform.offsetMax = baseMax;
            rectTransform.offsetMin = baseMin;
            Reset();
        }

        private void OnDisable()
        {
            Reset();
        }

        private void Reset()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                rectTransform.offsetMax = baseMax;
                rectTransform.offsetMin = baseMin;
            }
        }
    }
}
