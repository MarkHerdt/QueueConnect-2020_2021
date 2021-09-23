using Ganymed.UISystem;
using QueueConnect.GameSystem;
using QueueConnect.UISystem.BaseClasses;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    public class MenuGameModeSelection : MenuSlider<MenuGameModeSelection>
    {
        [SerializeField] private Button playButton = null;
        [SerializeField] private Button tutorialButton = null;
        [SerializeField] private Button backButton = null;

        #region --- [STATIC ACCESS] ---

        public static void Open(bool prioritizeTutorial)
        {
            Initialize();
        }

        #endregion
    
        protected override void Awake()
        {
            base.Awake();
            playButton.onClick.AddListener(OnPlayButtonPressed);
            tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
            backButton.onClick.AddListener(OnBackButtonPressed);
        }


        private void OnPlayButtonPressed()
        {
            GameController.StartGame();
            MenuManager.CloseAllMenus();
        }
    
        private void OnTutorialButtonPressed()
        {
            GameController.StartTutorial();
            MenuManager.CloseAllMenus();
        }

        private void OnBackButtonPressed()
        {
            MenuManager.InvokeOnBackPressed();
        }
    }
}
