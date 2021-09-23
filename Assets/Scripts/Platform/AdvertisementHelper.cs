using System;
using Ganymed.UISystem;
using Ganymed.UISystem.Templates;
using Ganymed.Utils.Singleton;
using QueueConnect.Environment;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.StatSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.Advertisements;

namespace QueueConnect.Platform
{
    public class AdvertisementHelper : MonoSingleton<AdvertisementHelper>
#if ENABLE_ADS
        ,IUnityAdsListener, ILocalizationCallback
#endif
    {
#if ENABLE_ADS && (UNITY_ANDROID || EDITOR_ANDROID)
        public const string GAME_ID = "4075189"; // Android ID
#elif ENABLE_ADS && (UNITY_IOS || EDITOR_IOS)
    public const string GAME_ID = "4075188"; // Apple ID
#endif

        public static event Action<ShowResult> UnityAdsDidFinish;
        
        public const string VIDEO = "Skippable";
        public const string REWARDED_VIDEO = "rewardedVideo";
        
        private static bool isPlayingRewardedAdd;
        public static bool HasBeenRevived { get; private set; }
        
        
        
        protected override void Awake()
        {
            base.Awake();
            Advertisement.Initialize(GAME_ID);
            Advertisement.AddListener(this);
            LocalizationManager.AddCallbackListener(this);
            EventController.OnGameStarted += delegate { HasBeenRevived = false; };
        }

        
        private static string yes = "MISSING";
        private static string no = "MISSING";
        private static string offer = "MISSING";
        
        public static void OfferPlayerReward()
        {
            
            
            var config = new ConfirmCancelConfiguration(offer, yes, no);
            AudioSystem.PlayVFX(VFX.OnPlayerRevive);
            MenuReward.Open(config, () =>
            {
                isPlayingRewardedAdd = true;
                Instance.ShowRewardedVideoAd();
            }, () =>
            {
                isPlayingRewardedAdd = false;
                Instance.ShowVideoAd();
            } );   
        }
        


    
        //------------------------------------------------------------------------------------------------------------------

        #region --- [ADS] ---

        /// <summary>
        /// Display a default advertisement
        /// </summary>
        public void ShowVideoAd()
        {
            Advertisement.Show(VIDEO);
        }
    
        /// <summary>
        /// Display a rewarded advertisement
        /// </summary>
        private void ShowRewardedVideoAd()
        {
            Advertisement.Show(REWARDED_VIDEO);
        }

        

        
        #endregion
    
        //------------------------------------------------------------------------------------------------------------------

        #region --- [INTERFACE IMPLEMENTATION] ---

        public void OnUnityAdsReady(string placementId)
        {
        
        }

        public void OnUnityAdsDidError(string message)
        {
        }

        public void OnUnityAdsDidStart(string placementId)
        {
        
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (isPlayingRewardedAdd && showResult == ShowResult.Finished)
            {
                PlayerHealthHandler.RevivePlayer();
                isPlayingRewardedAdd = false;
                HasBeenRevived = true;
                AudioSystem.PlayVFX(VFX.OnPlayerRevive);
            }
            else
            {
                GameController.ResumeGame();
                EventController.GameEnded(false);
            }
            
            UnityAdsDidFinish?.Invoke(showResult);
        }

        #endregion

        public void OnLanguageLoaded(Language language)
        {
            yes = "Okay";
            no = LocalizationManager.GetText("m_no");
            offer = LocalizationManager.GetText("m_offer");
        }
    }
}
