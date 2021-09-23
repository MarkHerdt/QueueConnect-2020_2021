using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace QueueConnect.Plugins.Ganymed.Localization
{
    [Serializable]
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public class LocalizedString
    {
        public Language Language => language;

        [ShowInInspector, ReadOnly, SerializeField, HideLabel, HorizontalGroup("_")] 
        private Language language;

        [SerializeField, HorizontalGroup("_")] public bool expand = false;
        
        [ShowIf("@expand == true"), ShowInInspector, HideLabel, MultiLineProperty(5)]
        public string TextArea
        {
            get => text;
            set => text = value;
        }
        
        [ShowIf("@expand == false"), ShowInInspector, HideLabel]
        public string TextField
        {
            get => text;
            set => text = value;
        }


        public string Text => text;
        
        [SerializeField, HideLabel, HideInInspector]
        public string text = "MISSING";
        
        public LocalizedString(Language language)
        {
            this.language = language;
        }
    }
    
    [Serializable]
    [HideMonoScript]
    public class LocalizedText : ISerializationCallbackReceiver
    {
        public static bool ShowKeysOnly;
        
        [SerializeField, Required, LabelWidth(40)] public string key = null;
        
        [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true, Expanded = false, ShowPaging = false, ShowIndexLabels = false)]
        [ShowIf("@ShowKeysOnly == false")]
        [LabelText("Content")]
        [SerializeField] public LocalizedString[] localizedText;

        public LocalizedText(string key = null)
        {
            this.key = key;
            
            localizedText = new LocalizedString[LocalizationManager.LanguageCount];
            var languages = (Language[])Enum.GetValues(typeof(Language));
            for (var i = 0; i < languages.Length; i++)
            {
                localizedText[i] = new LocalizedString(languages[i]);
            }
        }
        
        public string Load(Language language)
        {
            for (int i = 0; i < localizedText.Length; i++)
            {
                if (localizedText[i].Language == language)
                    return localizedText[i].Text;
            }

            Debug.LogWarning($"Missing {language} for Key: {key} !");
            return "MISSING";
        }
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            var languages = (Language[])Enum.GetValues(typeof(Language));
            if (localizedText.Length != languages.Length)
            {
                var temp = new LocalizedString[languages.Length];
                for (var i = 0; i < temp.Length; i++)
                {
                    try
                    {
                        temp[i] = localizedText[i];
                    }
                    catch
                    {
                        temp[i] = new LocalizedString(languages[i]);
                    }
                }

                localizedText = temp;
            }
        }
    }

    [CreateAssetMenu(menuName = "Localization/LocalizationData")]
    [HideMonoScript]
    public class LocalizationData : ScriptableObject
    {
        [ShowInInspector, PropertyOrder(-3000)]
        public static bool ShowKeysOnly
        {
            get => LocalizedText.ShowKeysOnly; 
            set => LocalizedText.ShowKeysOnly = value; 
        }
        public static LocalizationData Instance => _instance != null? _instance : _instance = Load();
        private static LocalizationData _instance = null;
        
        [Space]
        [ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 12)]
        [Searchable(FuzzySearch = false, Recursive = true)]
        [LabelText("Localized")]
        [SerializeField] private LocalizedText[] localizedTextCaches = default;

        public LocalizedText[] LocalizedTextCaches => localizedTextCaches;
        
        private void OnValidate()
        {
            for (int i = 0; i < localizedTextCaches.Length; i++)
            {
                for (int j = 0; j < i - j; j++)
                {
                    if (localizedTextCaches[i].key == localizedTextCaches[j].key)
                    {
                        localizedTextCaches[i].key = "";   
                    }
                }
            }
        }
        
        

#if UNITY_EDITOR

        public void CreateKey(string key)
        {
            localizedTextCaches = localizedTextCaches.Concat(new[]{new LocalizedText(key)}).ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [UnityEditor.MenuItem("Tools/Localization")]
        public static void Select()
        {
            UnityEditor.Selection.activeObject = Instance;
        }

#endif
        
        public static LocalizationData Load()
        {
            return LoadResource<LocalizationData>();
        }
        
        
        public static T LoadResource<T>(List<string> resourcePaths = null) where T : UnityEngine.Object
        {
            if(resourcePaths == null)
                resourcePaths = new List<string> {""};

            foreach (var path in resourcePaths)
            {
                foreach (var valid in Resources.LoadAll<T>(path))
                {
                    return valid;
                }    
            }
            
            // search the upper layer of the resources folder
            foreach (var valid in Resources.LoadAll<T>(string.Empty))
            {
                return valid;
            }

            return null;
        }
    }
}
