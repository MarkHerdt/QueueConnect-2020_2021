using System;
using Ganymed.Utils.Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace QueueConnect.Plugins.Ganymed.Localization
{
    /// <summary>
    /// Eier
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [ScriptOrder(-10000)]
    public class LocalizedTextComponent : MonoBehaviour, ILocalizationCallback
    {
        [SerializeField] public bool isStatic = true;
        [SerializeField] public string key = "MISSING";

        private string _text;
        private TMP_Text _tmpText;
        
        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _text = LocalizationManager.GetText(key);
            _text = _text == "MISSING" ? "<color=red>MISSING</color>" : _text;
            LocalizationManager.AddCallbackListener(this);
        }
        
        public void OnLanguageLoaded(Language newLanguage)
        {
            _text = LocalizationManager.GetText(key);

            if (isStatic)
            {
                _text = _text == "MISSING" ? "<color=red>MISSING</color>" : _text;
                if (_tmpText) _tmpText.text = _text;
            }
        }

        private void OnEnable()
        {
            _tmpText.text = _text;
        }


        public static LocalizedTextComponent CreateInstance(GameObject where, string key)
        {
            var component = where.AddComponent<LocalizedTextComponent>();
            component.key = key;
            component._text = LocalizationManager.GetText(key);
            component._text = component._text == "MISSING" ? "<color=red>MISSING</color>" : component._text;
            component._tmpText.text = component._text;
            return component;
        }
    }
    
    #if UNITY_EDITOR
    
    [UnityEditor.CustomEditor(typeof(LocalizedTextComponent))]
    public class LocalizedTextComponentInspector : UnityEditor.Editor
    {
        private LocalizedTextComponent _target;
        public const string HELP_BOX = "HelpBox";
        
        private void OnEnable()
        {
            _target = (LocalizedTextComponent)target;
        }

        private bool _hasKey = false;
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(HELP_BOX);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.Width(50));
            _target.key = EditorGUILayout.TextField(_target.key);
            if (GUILayout.Button("Open Config"))
            {
                LocalizationData.Select();
            }
            if (!_hasKey)
            {
                if (GUILayout.Button("Create Key") || Event.current.keyCode == KeyCode.Return)
                {
                    LocalizationData.Instance.CreateKey(_target.key);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            _hasKey = false;
            foreach (var localizedText in LocalizationData.Instance.LocalizedTextCaches)
            {
                if (localizedText.key == _target.key)
                {
                    foreach (var localizedString in localizedText.localizedText)
                    {
                        EditorGUILayout.BeginVertical(HELP_BOX);
                        EditorGUILayout.LabelField($"Language: {localizedString.Language}");
                        localizedString.text = localizedString.expand 
                            ? EditorGUILayout.TextArea(localizedString.Text) 
                            : EditorGUILayout.TextField(localizedString.Text);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(5);
                        _hasKey = true;
                    }
                }
            }

            if (!_hasKey)
            {
                EditorGUILayout.BeginVertical(HELP_BOX);
                EditorGUILayout.LabelField("Suggested Keys:");
                foreach (var localizedText in LocalizationData.Instance.LocalizedTextCaches)
                {
                    if (localizedText.key.StartsWith(_target.key))
                    {
                        EditorGUILayout.LabelField(localizedText.key);
                        if (Event.current.keyCode == KeyCode.Tab)
                        {
                            _target.key = localizedText.key;
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("SAVE"))
            {
                UnityEditor.EditorUtility.SetDirty(_target);
            }
        }
    }
    
    #endif
}
