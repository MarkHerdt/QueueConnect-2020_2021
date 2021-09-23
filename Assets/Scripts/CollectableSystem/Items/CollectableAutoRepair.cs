using System;
using System.Collections;
using QueueConnect.Environment;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.Tutorial;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableAutoRepair : CollectableItem
    {
        [SerializeField] private float autoRepairDuration = 7f;
        [SerializeField] private AnimationCurve timeScale = new AnimationCurve();
        
        private static readonly WaitForSeconds Wait02 = new WaitForSeconds(.1f);
        private static readonly WaitForSeconds Wait08 = new WaitForSeconds(.9f);

        private Coroutine coroutine;
        
        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemAutoRepairCollected);
            GameController.SlowTime(autoRepairDuration, timeScale);
            RobotScanner.StartAutoRepair(autoRepairDuration);
            if(coroutine != null) ItemSpawner.Instance.StopCoroutine(coroutine);
            coroutine = ItemSpawner.Instance.StartCoroutine(Countdown(autoRepairDuration));
            return true;
        }

        private static IEnumerator Countdown(float duration)
        {
            PlayerHealthHandler.SetInvincibility(true);
            TooltipMonitor.ShowText(string.Empty);
            var left = (int)duration + 2;

            for (var i = left - 1; i >= 0; i--)
            {
                if(i < left -1)
                    AudioSystem.PlayVFX(VFX.OnCountdownTick);
                
                yield return Wait02;
                PlayerHealthHandler.SetInvincibility(true);

                if(i < left -1)
                    TooltipMonitor.ShowText($"AutoRepair\n<size=42>{i:00}</size>", false);
                
                yield return Wait08;
                
                while (GameController.IsPaused)
                {
                    yield return null;
                }
            }
            TooltipMonitor.HideDisplay(0);
            AudioSystem.PlayVFX(VFX.OnAutoRepairEnd);
            PlayerHealthHandler.SetInvincibility(false);
        }

        protected override bool CheckItemSpawnCondition()
        {
            return WaveController.WaveDurationLeft > (autoRepairDuration + 3f);
        }
    }
}