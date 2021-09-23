using System;
using Ganymed.UISystem;
using QueueConnect.Platform;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem;
using SoundSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    public class MenuSettings : Menu<MenuSettings>, ILocalizationCallback
    {
        #region --- [INSPECTOR] ---

        [SerializeField] private Button backButton = null;
        [SerializeField] private Button aboutButton = null;

        #endregion
        
        #region --- [FIELDS] ---

        private Animator animator = null;
        private readonly int close = Animator.StringToHash("Close");
        private readonly int open = Animator.StringToHash("Open");

        [Space] 
        [SerializeField] private Button increaseMasterButton = default;
        [SerializeField] private Button decreaseMasterButton = default;
        [SerializeField] private Button increaseVFXButton = default;
        [SerializeField] private Button decreaseVFXButton = default;
        [SerializeField] private Button increaseMusicButton = default;
        [SerializeField] private Button decreaseMusicButton = default;
        [SerializeField] private Button increaseAmbienceButton = default;
        [SerializeField] private Button decreaseAmbienceButton = default;

        [Title("Vibrations")]
        [SerializeField] private Button vibrationsButton = default;
        [SerializeField] private TMP_Text vibrationsButtonTitle = default;
        

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [BASE INIT] ---
        
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            backButton.onClick.AddListener(OnBackButtonPressed);
            aboutButton.onClick.AddListener(OnAboutButtonPressed);
            vibrationsButton.onClick.AddListener(OnVibrationsButton);
            
            increaseMasterButton.onClick.AddListener(() => sliderMaster.value += 1f);
            decreaseMasterButton.onClick.AddListener(() => sliderMaster.value -= 1f);
            increaseAmbienceButton.onClick.AddListener(() => sliderAmbience.value += 1f);
            decreaseAmbienceButton.onClick.AddListener(() => sliderAmbience.value -= 1f);
            increaseMusicButton.onClick.AddListener(() => sliderMusic.value += 1f);
            decreaseMusicButton.onClick.AddListener(() => sliderMusic.value -= 1f);
            increaseVFXButton.onClick.AddListener(() => sliderVFX.value += 1f);
            decreaseVFXButton.onClick.AddListener(() => sliderVFX.value -= 1f);
            
            LocalizationManager.AddCallbackListener(this);
        }

        private void Start()
        {
            InitAudioSettings();
            InitLanguageSettings();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LocalizationManager.RemoveCallbackListener(this);
        }

        #endregion
    
        //--------------------------------------------------------------------------------------------------------------

        #region --- [UI] ---

        private void OnVibrationsButton()
        {
            VibrationHandler.EnableVibrations = !VibrationHandler.EnableVibrations;
            vibrationsButtonTitle.text = VibrationHandler.EnableVibrations ? enabledString : disabledString;
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        private void OnBackButtonPressed()
        {
            MenuManager.InvokeOnBackPressed();
        }

        private void OnAboutButtonPressed()
        {
            MenuAbout.Open();
        }
        
        public static void Open()
        {
            Initialize();
        }
    
        
        public static void Close()
        {
            Terminate();
        }
        
        
        public void OnMenuClosedCallback()
        {
            Close();
            gameObject.SetActive(false);
        }


        public void OnMenuOpenedCallback()
        {
            gameObject.SetActive(true);
        }
    
        public override void OpenInstance()
        {
            base.OpenInstance();
            animator.ResetTrigger(close);
            animator.SetTrigger(open);
            AudioSystem.PlayVFX(VFX.UIMenuOpenDigital);
        }

        public override void CloseInstance()
        {
            animator.ResetTrigger(open);
            animator.SetTrigger(close);
            AudioSystem.PlayVFX(VFX.UIMenuCloseDigital);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [AUDIOSETTINGS] ---

        [Header("Audio Settings")]
        [SerializeField] private Button buttonMuteAudio = null;
        [SerializeField] private TMP_Text tmpAudioState = null;
        
        [SerializeField] private Slider sliderMaster = null;
        [SerializeField] private Slider sliderVFX = null;
        [SerializeField] private Slider sliderAmbience = null;
        [SerializeField] private Slider sliderMusic = null;

        [SerializeField] private TMP_Text masterVolumePercentageDisplay = null;
        [SerializeField] private TMP_Text vfxVolumePercentageDisplay = null;
        [SerializeField] private TMP_Text ambienceVolumePercentageDisplay = null;
        [SerializeField] private TMP_Text musicVolumePercentageDisplay = null;

        [SerializeField] private Color colorAudioDisabled = Color.gray;
        [SerializeField] private Color colorAudioEnabled = Color.magenta;
        [SerializeField] private Image[] sliderImages = null;

        private const float SAFE_OFFSET = 0.00001f;
        private const float LOG_BASE = 2f;
        private const float LOG_MULTIPLIER = 20f;
        private const float TEN = 10f;
        

        #region --- [VOLUME SLIDER] ---

        public void SetMasterVolume(float volume)
        {
            var normalized = Mathf.Max(volume / TEN, SAFE_OFFSET);
            var percentage = volume * TEN;
            
            AudioSystem.VolumeMaster = Mathf.Log(normalized, LOG_BASE) * LOG_MULTIPLIER;
            masterVolumePercentageDisplay.SetText($"{percentage:000}%");
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        public void SetVFXVolume(float volume)
        {
            var normalized = Mathf.Max(volume / TEN, SAFE_OFFSET);
            var percentage = volume * TEN;
            
            AudioSystem.VolumeVFX = Mathf.Log(normalized, LOG_BASE) * LOG_MULTIPLIER;
            vfxVolumePercentageDisplay.SetText($"{percentage:000}%");
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        public void SetAmbienceVolume(float volume)
        {
            var normalized = Mathf.Max(volume / TEN, SAFE_OFFSET);
            var percentage = volume * TEN;
            
            AudioSystem.VolumeAmbience = Mathf.Log(normalized, LOG_BASE) * LOG_MULTIPLIER;
            ambienceVolumePercentageDisplay.SetText($"{percentage:000}%");
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        public void SetMusicVolume(float volume)
        {
            var normalized = Mathf.Max(volume / TEN, SAFE_OFFSET);
            var percentage = volume * TEN;
            
            AudioSystem.VolumeMusic = Mathf.Log(normalized, LOG_BASE) * LOG_MULTIPLIER;
            musicVolumePercentageDisplay.SetText($"{percentage:000}%");
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        #endregion
        
        private void InitAudioSettings()
        {
            buttonMuteAudio.onClick.AddListener(AudioSystem.ToggleAudioOnOff);
            AudioSystem.OnAudioMuted += AudioSystemOnMuted;
            AudioSystem.OnAudioUnmuted += AudioSystemOnUnMuted;

            if (AudioSystem.IsMuted)
            {
                AudioSystemOnMuted();
            }
            else
            {
                AudioSystemOnUnMuted();
            }


            var master = Mathf.Pow(LOG_BASE, AudioSystem.VolumeMaster / LOG_MULTIPLIER);
            masterVolumePercentageDisplay.text = $"{master * 100:000}%";
            sliderMaster.value = master * TEN;
            
            var vfx = Mathf.Pow(LOG_BASE, AudioSystem.VolumeVFX / LOG_MULTIPLIER);
            vfxVolumePercentageDisplay.text = $"{vfx * 100:000}%";
            sliderVFX.value = vfx * TEN;
            
            var music = Mathf.Pow(LOG_BASE, AudioSystem.VolumeMusic / LOG_MULTIPLIER);
            musicVolumePercentageDisplay.text = $"{music * 100:000}%";
            sliderMusic.value = music * TEN;
            
            var ambience = Mathf.Pow(LOG_BASE, AudioSystem.VolumeAmbience / LOG_MULTIPLIER);
            ambienceVolumePercentageDisplay.text = $"{ambience * 100:000}%";
            sliderAmbience.value = ambience * TEN;
            
            
            sliderMaster.onValueChanged.AddListener(SetMasterVolume);
            sliderAmbience.onValueChanged.AddListener(SetAmbienceVolume);
            sliderVFX.onValueChanged.AddListener(SetVFXVolume);
            sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        }
        
        private void AudioSystemOnMuted()
        {
            tmpAudioState.text = disabledString;
            foreach (var image in sliderImages)
            {
                image.color = colorAudioDisabled;
            }
        }

        private void AudioSystemOnUnMuted()
        {
            tmpAudioState.text = enabledString;
            foreach (var image in sliderImages)
            {
                image.color = colorAudioEnabled;
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [LANGUAGE SETTINGS] ---

        [Header("Language Settings")]
        [SerializeField] private Button buttonPrevLanguage = null;
        [SerializeField] private Button buttonNextLanguage = null;
        [SerializeField] private TMP_Text selectedLanguageText = null;

        private int languageIndex = default;
        
        private void InitLanguageSettings()
        {
            languageIndex = (int)LocalizationManager.Language;
            selectedLanguageText.text = LocalizationManager.GetLocalizedLanguageString();
            
            buttonNextLanguage.onClick.AddListener(NextLanguage);
            buttonPrevLanguage.onClick.AddListener(PrevLanguage);
        }

        private void NextLanguage()
        {
            languageIndex++;
            if (!LocalizationManager.IsLanguageDefined(languageIndex))
            {
                languageIndex = 0;
            }
            LocalizationManager.Language = (Language) languageIndex;
            selectedLanguageText.text = LocalizationManager.GetLocalizedLanguageString();
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }
        
        private void PrevLanguage()
        {
            languageIndex--;
            if (!LocalizationManager.IsLanguageDefined(languageIndex))
            {
                languageIndex = LocalizationManager.LanguageCount - 1;
            }

            LocalizationManager.Language = (Language) languageIndex;
            selectedLanguageText.text = LocalizationManager.GetLocalizedLanguageString();
            
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
        }

        #endregion

        private static string enabledString = "Enabled";
        private static string disabledString = "Disabled";
        
        public void OnLanguageLoaded(Language language)
        {
            enabledString = LocalizationManager.GetText("m_enabled");
            disabledString = LocalizationManager.GetText("m_disabled");
            vibrationsButtonTitle.text = VibrationHandler.EnableVibrations ? enabledString : disabledString;
            tmpAudioState.text = AudioSystem.IsMuted ? disabledString : enabledString;
        }
    }
}