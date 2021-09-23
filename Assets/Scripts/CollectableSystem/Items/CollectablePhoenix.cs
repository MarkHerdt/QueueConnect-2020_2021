using System;
using System.Collections;
using QueueConnect.Environment;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectablePhoenix : CollectableItem
    {
        private Coroutine coroutine;
        private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(.1f);
        
        protected override void Awake()
        {
            base.Awake();
            EventController.OnGameEnded += (x) => { if(coroutine != null) StopCoroutine(coroutine); };
        }
        
        protected override bool Use()
        {
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            AudioSystem.PlayVFX(VFX.OnItemPhoenixCollected);
            coroutine = StartCoroutine(UsePhoenix());
            return true;
        }

        private IEnumerator UsePhoenix()
        {
            while (PlayerHealthHandler.AddLife(false))
            {
                AudioSystem.PlayVFX(VFX.OnItemPhoenixEffect);
                yield return waitForSeconds;
            }
            while (PlayerHealthHandler.AddArmor(false))
            {
                AudioSystem.PlayVFX(VFX.OnItemPhoenixEffect);
                yield return waitForSeconds;
            }
        }

        protected override bool CheckItemSpawnCondition()
        {
            return PlayerHealthHandler.CurrentHealth < 3 || PlayerHealthHandler.CurrentArmor < 3;
        }
    }
}