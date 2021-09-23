using System.Runtime.CompilerServices;
using QueueConnect.Environment;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.Tutorial;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.CollectableSystem
{
    public class CollectableMotivation : CollectableItem
    {
        [SerializeField] private string[] textKeys = default;
        private string[] _motivationalText = default;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetRandomText() 
            => _motivationalText[Random.Range(0, _motivationalText.Length)] ?? string.Empty;
        
        protected override bool Use()
        {
            TooltipMonitor.ShowText(GetRandomText(), 5000);
            AudioSystem.PlayVFX(VFX.OnItemCollected);
            return true;
        }

        protected override bool CheckItemSpawnCondition()
        {
            return !RobotScanner.AutoRepairRobots && WaveController.WaveDurationLeft > 5f;
        }

        public override void OnLanguageLoaded(Language language)
        {
            base.OnLanguageLoaded(language);
            _motivationalText = new string[textKeys.Length];
            for (var i = 0; i < textKeys.Length; i++)
            {
                _motivationalText[i] = LocalizationManager.GetText(textKeys[i]);
            }
        }
    }
}