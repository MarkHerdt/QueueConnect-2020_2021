using System;
using System.Collections;
using System.Globalization;
using QueueConnect.GameSystem;
using TMPro;
using UnityEngine;

public class UpdateMultiplier : MonoBehaviour
{
    #region Inspector Fields
    
    [SerializeField] private TextMeshProUGUI textMesh = null;
    #pragma warning disable 109
    [SerializeField] private new Animation animation = null;
    #pragma warning restore 109

    private ulong cachedValue = default;
    private Coroutine Countdown = null;
    
    #endregion

    private void Awake()
    {
        EventController.OnGameEnded += delegate
        {
            if(Countdown != null)
                StopCoroutine(Countdown);
        };
    }

    public bool EnableMultiplierReset { get; set; } = false;
    
    public void UpdateMultiplierText(ulong value)
    {
        if(Countdown != null)
            StopCoroutine(Countdown);
        
        cachedValue = value;
        textMesh.text = value.ToString();
        animation.Stop();
        animation.Play();
    }
    
    /// <summary>
    /// Reset the score to 0 (visually) over the set duration.
    /// </summary>
    public void ResetMultiplier(float duration = 1f)
    {
        animation.Stop();
        if(!EnableMultiplierReset) return;
        if(Countdown != null)
            StopCoroutine(Countdown);

        Countdown = StartCoroutine(CountdownRoutine(duration));
    }

    private IEnumerator CountdownRoutine(float duration = 1f)
    {
        duration = Mathf.Max(duration, .1f);
        var decrement = cachedValue / duration;
        while (cachedValue > 0)
        {
            cachedValue -= (ulong)Mathf.Max(1,(int)(decrement * Time.deltaTime));
            textMesh.text = Mathf.Max(0,cachedValue).ToString(CultureInfo.InvariantCulture);
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
        textMesh.text = "Connect";
    }
}
