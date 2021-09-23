using Ganymed.UISystem;
using Ganymed.UISystem.Templates;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.UISystem.BaseClasses;
using SoundSystem;
using SoundSystem.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    public class MenuMain : MenuSlider<MenuMain>, ILocalizationCallback
    {
        #region --- [SETUP] ---

        [SerializeField] private Button StartButton = null;
        [SerializeField] private Button StatsButton = null;
        [SerializeField] private Button SettingsButton = null;
    
        protected override void Awake()
        {
            base.Awake();
            StartButton.onClick.AddListener(OnStartButtonPressed);
            StatsButton.onClick.AddListener(OnStatsButtonPressed);
            SettingsButton.onClick.AddListener(OnSettingsButtonPressed);
            
            LocalizationManager.AddCallbackListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LocalizationManager.RemoveCallbackListener(this);
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------------

        #region --- [ASSET / BUTTON USAGE] ---

        private void OnStartButtonPressed()
        {
            MenuGameModeSelection.Open(true);
        }

        private void OnStatsButtonPressed()
        {
            MenuStats.Open();
        }
    
        private void OnSettingsButtonPressed()
        {
            MenuSettings.Open();
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------------

        #region --- [OVERRIDES] ---

        private static string quitMessage = "Exit Game?";
        private static string yes = "yes";
        private static string no = "no";
        
        
        public override void OnBackPressed()
        {
            MenuConfirm.Open(
                new ConfirmCancelConfiguration(
                    quitMessage,
                    yes,
                    no),
                Quit,
                () => AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess));
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        #endregion

        public void OnLanguageLoaded(Language language)
        {
            quitMessage = LocalizationManager.GetText("txt_quit");
            yes         = LocalizationManager.GetText("m_yes");
            no          = LocalizationManager.GetText("m_no");
        }
    }
}
