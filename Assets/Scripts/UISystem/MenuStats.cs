using System.Collections.Generic;
using Ganymed.UISystem;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using QueueConnect.StatSystem;
using SoundSystem;
using SoundSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    [RequireComponent(typeof(Animator))]
    public class MenuStats : MenuOpenCloseAnimation<MenuStats>
    {
        #region --- [INPECTOR] ---

        [SerializeField] private Button backButton = null;

        #endregion
        
        #region --- [BASE FIELDS] ---

        private readonly int close = Animator.StringToHash("Close");
        private readonly int open = Animator.StringToHash("Open");
        private static bool _dirty = true;
    
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [BASE INIT] ---
        
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            backButton.onClick.AddListener(OnBackButtonPressed);
            EventController.OnGameEnded += aborted => _dirty = true;
        }

        #endregion
    
        //--------------------------------------------------------------------------------------------------------------

        #region --- [UI] ---

        private void OnBackButtonPressed()
        {
            MenuManager.InvokeOnBackPressed();
        }
        
        public override void OnMenuClosedCallback()
        {
            Close();
            gameObject.SetActive(false);
        }


        public override void OnMenuOpenedCallback()
        {
            gameObject.SetActive(true);
        }
    
        public override void OpenInstance()
        {
            base.OpenInstance();
            animator.ResetTrigger(close);
            animator.SetTrigger(open);
            if (_dirty)
            {
                LoadStatDisplay();
                _dirty = false;
            }
        }

        public override void CloseInstance()
        {
            animator.ResetTrigger(open);
            animator.SetTrigger(close);
            AudioSystem.PlayVFX(VFX.UIMenuCloseDigital);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [STAT FIELDS] ---
        
        [Header("Stats")]
        [SerializeField] private Transform contentHolder = null;
        [SerializeField] private StatUIElement statUIElementPrefab = null;
        [SerializeField] private TMP_Text statUIElementTitlePrefab = null;

        private readonly Stack<GameObject> loadedStatObjects = new Stack<GameObject>();

        #endregion

        #region --- [STATS DISPLAY] ---

        private void LoadStatDisplay()
        {
            ClearStatDisplay();
            Debug.Log("Loading Stats");
            CreateTitleElement("title_bests");
            CreateStatElements(PlayerStats.GetMaximumsStats());
            CreateTitleElement("title_totals");
            CreateStatElements(PlayerStats.GetTotalStats());
            CreateTitleElement("title_lastgame");
            CreateStatElements(PlayerStats.GetCurrentStats());
        }

        private void CreateTitleElement(string title)
        {
            var titleObject = Instantiate(statUIElementTitlePrefab, contentHolder);
            LocalizedTextComponent.CreateInstance(titleObject.gameObject, title);
            loadedStatObjects.Push(titleObject.gameObject);
        }

        private void CreateStatElements(IEnumerable<Stat> stats)
        {
            foreach (var stat in stats)
            {
                loadedStatObjects.Push(StatUIElement.Create(statUIElementPrefab, contentHolder, stat).gameObject);
            }
        }

        private void ClearStatDisplay()
        {
            foreach (var obj in loadedStatObjects)
            {
                Destroy(obj);
            }
            loadedStatObjects.Clear();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
    }
}