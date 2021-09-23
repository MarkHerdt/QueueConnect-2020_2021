using Ganymed.UISystem;
using Ganymed.UISystem.Templates;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.UISystem.BaseClasses;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    public class MenuPause : MenuSlider<MenuPause>, ILocalizationCallback
    {

        #region --- [SETUP] ---

        [SerializeField] private Button continueButton = null;
        [SerializeField] private Button settingsButton = null;
        [SerializeField] private Button mainMenuButton = null;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnContinueButtonPressed);
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
            LocalizationManager.AddCallbackListener(this);
        }

        private static string quitMessage = "Quit And Return To Main Menu?";
        private static string yes = "yes";
        private static string no = "no";

        #endregion
    
        //------------------------------------------------------------------------------------------------------------------

        #region --- [ASSET / BUTTON USAGE] ---

        private void OnMainMenuButtonPressed()
        {
            void Confirm()
            {
                Close();
                GameController.ExitPlayMode(true);
                MenuMain.Open();
                AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
            }
            
            
            
            MenuConfirm.Open(new ConfirmCancelConfiguration(
                    quitMessage,
                yes, no),
                Confirm,
                () => AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess));
        }

        private void OnSettingsButtonPressed()
        {
            MenuSettings.Open();
        }

        private void OnContinueButtonPressed()
        {
            Close();
            GameController.ResumeGame();
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------------

        #region --- [OVERRIDES] ---

        protected override void OnOpen()
        {
            GameController.PauseGame();
        }

        protected override void OnClose()
        {
            if (MenuManager.IsTopMenu(this))
            {
                GameController.ResumeGame();
            }
        }

        public override void OnBackPressed()
        {
            OnContinueButtonPressed();
        }

        #endregion

        public void OnLanguageLoaded(Language language)
        {
            quitMessage = LocalizationManager.GetText("txt_quitGame");
            yes         = LocalizationManager.GetText("m_yes");
            no          = LocalizationManager.GetText("m_no");
        }
    }
}