using System;
using System.Collections.Generic;
using UnityEngine;

namespace QueueConnect.Plugins.Ganymed.Localization
{
    /// <summary>
    /// Enum contains every language supported by this game.
    /// Languages must have the same name as their corresponding <see cref="SystemLanguage"/> counterpart. 
    /// </summary>
    public enum Language
    {
        English = 0,
        German = 1,
        French = 2,
        Spanish = 3,
    }

    /// <summary>
    /// Implement this interface and add register the object by calling <see cref="LocalizationManager.AddCallbackListener"/>
    /// to receive a callback when the <see cref="LocalizationManager.Language"/> has updated.
    /// </summary>
    public interface ILocalizationCallback
    {
        void OnLanguageLoaded(Language language);
    }
    
    public static class LocalizationManager
    {
        private static readonly List<ILocalizationCallback> LocalizationCallbacks = new List<ILocalizationCallback>();
        public static void AddCallbackListener(ILocalizationCallback listener)
        {
            LocalizationCallbacks.Add(listener);
            listener.OnLanguageLoaded(Language);
        }

        public static void RemoveCallbackListener(ILocalizationCallback listener) => LocalizationCallbacks.Remove(listener);
        public delegate void LanguageChangedCallback(Language newLanguage);
        public static int LanguageCount => Enum.GetNames(typeof(Language)).Length;
        
        public static Language Language
        {
            get => _language;
            set
            {
                if (!Enum.IsDefined(typeof(Language), value)) return;
                _language = value;
                PlayerPrefs.SetInt(nameof(Language), (int)value);
                PlayerPrefs.Save();

                for (int i = 0; i < LocalizationCallbacks.Count; i++)
                {
                    LocalizationCallbacks[i].OnLanguageLoaded(value);
                }
            }
        }
        
        //------------

        
        private static Language _language = Language.English;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            //Check if custom _language is defined
            var loadedLanguage =
                (Language) PlayerPrefs.GetInt(nameof(Language), ParseToLanguage(Application.systemLanguage));
            _language = IsLanguageDefined(loadedLanguage) ? loadedLanguage : Language.English;
            
            var localizationData = LocalizationData.Load() ?? throw new Exception($"Could Not Load {nameof(LocalizationData)}");

            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                var tempDictionary = new Dictionary<string, string>();
                foreach (var localizedText in localizationData.LocalizedTextCaches)
                {
                    tempDictionary.Add(localizedText.key, localizedText.Load(lang));
                }
                LocalizedTextCache.Add(lang, tempDictionary);
            }

            Language = _language;
        }
        
        private static int ParseToLanguage(SystemLanguage systemLanguage)
        {
            foreach (Language supportedLanguage in Enum.GetValues(typeof(Language)))
            {
                if(supportedLanguage.ToString().Equals(systemLanguage.ToString(), StringComparison.OrdinalIgnoreCase))
                    return (int)supportedLanguage;
            }

            return (int)Language.English;
        }
        
        
        public static bool IsLanguageDefined(Language value)
        {
            return Enum.IsDefined(typeof(Language), value);
        }
        
        public static bool IsLanguageDefined(int value)
        {
            return Enum.IsDefined(typeof(Language), value);
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INIT] ---

        private static readonly Dictionary<Language, Dictionary<string, string>> LocalizedTextCache =
            new Dictionary<Language, Dictionary<string, string>>();

        #endregion
        
        #region --- [GET TEXT BY LANGUAGE] ---

        public static string GetText(string key)
        {
            if(string.IsNullOrWhiteSpace(key)) return string.Empty;
            try
            {
                return LocalizedTextCache[Language][key];
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{key} was not present in the dictionary! {e.Message}");
                return "MISSING";
            }
        }
        
        public static string GetText(string key, Language language)
        {
            if(string.IsNullOrWhiteSpace(key)) return string.Empty;
            return LocalizedTextCache[language][key];
        }

        #endregion

        public static string GetLocalizedLanguageString()
        {
            switch (Language)
            {
                case Language.English:
                    return "English";
                case Language.German:
                    return "Deutsch";
                case Language.French:
                    return "Français";
                case Language.Spanish:
                    return "Español";
                default:
                    Debug.LogError($"{Language} has not a native definition!");
                    return Language.ToString();
            }
           
        }  
    }
}