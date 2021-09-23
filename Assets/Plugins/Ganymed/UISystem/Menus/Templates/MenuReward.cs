using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ganymed.UISystem.Templates
{
    public class MenuReward : Menu<MenuReward>
    {
                
        #region --- [COMPONENTS] ---

        [SerializeField] private TMP_Text label = null;
        [SerializeField] private TMP_Text confirmButtonText = null;
        [SerializeField] private TMP_Text cancelButtonText = null;
        [SerializeField] private Button confirmButton = null;
        [SerializeField] private Button cancelButton = null;
            
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [FIELDS] ---

        private Action confirm = null;
        private Action cancel = null;

        private const string defaultLabel = "Confirm Action";
        private const string defaultConfirmLabel = "Confirm";
        private const string defaultCancelLabel = "Cancel";

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [ASSET ACCESS] ---

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(OnConfirmPressed);
            cancelButton.onClick.AddListener(OnCancelPressed);
        }

        [AssetUsage]
        private void OnConfirmPressed()
        {
            Terminate();
            confirm?.Invoke();
        }

        [AssetUsage]
        private void OnCancelPressed()
        {
            Terminate();
            cancel?.Invoke();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(11);
            OnConfirmPressed();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [OPEN / CLOSE] ---

        public static void Open(ConfirmCancelConfiguration configuration, Action confirmCallback = null, Action cancelCallback = null)
        {
            Initialize();
            
            var Menu = Instance;
            Menu.label.text = configuration.labelText ?? defaultLabel;
            Menu.confirmButtonText.text = configuration.confirmButtonLabelText  ?? defaultConfirmLabel;
            Menu.cancelButtonText.text = configuration.cancelButtonLabelText ?? defaultCancelLabel;
            Menu.confirm = confirmCallback;
            Menu.cancel = cancelCallback;
        }
        
        public static void Open(Action confirmCallback = null, Action cancelCallback = null)
        {
            Initialize();
            
            var Menu = Instance;
            Menu.label.text = defaultLabel;
            Menu.confirmButtonText.text = defaultConfirmLabel;
            Menu.cancelButtonText.text = defaultCancelLabel;
            Menu.confirm = confirmCallback;
            Menu.cancel = cancelCallback;
        }

        public static void Close()
        {
            Terminate();
        }

        #endregion
    }
}