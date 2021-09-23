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
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    [RequireComponent(typeof(Animator))]
    public class MenuPostGame : Menu<MenuPostGame>
    {
        [SerializeField] private Button continueButton = default;
        [SerializeField] private Button showStatsButton = default;
        
        private Animator animator = null;
        private readonly int close = Animator.StringToHash("Close");
        private readonly int open = Animator.StringToHash("Open");
    
        //------------------------------------------------------------------------------------------------------------------
    
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            continueButton.onClick.AddListener(ButtonContinue);
            showStatsButton.onClick.AddListener(ButtonShowAllStats);
        }

        //------------------------------------------------------------------------------------------------------------------

        #region --- [BUTTONS] ---
    
        public void ButtonContinue()
        {
            MenuManager.CloseAllMenus();
            GameController.ExitPlayMode(true);
            MenuMain.Open();
        }

        public void ButtonShowAllStats()
        {
            MenuStats.Open();
        }

        #endregion
    
        //------------------------------------------------------------------------------------------------------------------

        public override void OnBackPressed()
        {
        
        }

        #region --- [OPEN / CLOSE] ---
    
        public static void Open()
        {
            Initialize();
            Instance.LoadGameStats();
            AudioSystem.PlayMusic(Music.MainMenu);
        }

        public static void Close()
        {
            Terminate();
        }
    
        public override void OpenInstance()
        {
            base.OpenInstance();
            animator.SetTrigger(open);
        }

        public override void CloseInstance()
        {
            animator.SetTrigger(close);
        }
    
        public void OnMenuClosedCallback()
        {
            Close();
        }
    
        public void OnMenuOpenedCallback()
        {
            scrollRect.ScrollToTop();
        }
    
        #endregion
        
        //------------------------------------------------------------------------------------------------------------------

        #region --- [PLAYER STATS] ---
        
        [Header("Stats")]
        [SerializeField] private Transform contentHolder = null;
        [SerializeField] private StatUIElement statUIElementPrefab = null;
        [SerializeField] private TMP_Text statUIElementTitlePrefab = null;
        [SerializeField] private ScrollRect scrollRect = null;

        private readonly Stack<GameObject> loadedStatObjects = new Stack<GameObject>();
        

        private void LoadGameStats()
        {
            ClearDisplay();
            CreateTitleElement("SCORE");
            CreateStatElements(PlayerStats.GetCurrentStats());
        }
        
        private void CreateTitleElement(string title)
        {
            var titleObject = Instantiate(statUIElementTitlePrefab, contentHolder);
            titleObject.text = LocalizationManager.GetText("m_score");
            loadedStatObjects.Push(titleObject.gameObject);
        }

        private void CreateStatElements(IEnumerable<Stat> stats)
        {
            foreach (var stat in stats)
            {
                loadedStatObjects.Push(StatUIElement.Create(statUIElementPrefab, contentHolder, stat).gameObject);
            }
        }

        private void ClearDisplay()
        {
            foreach (var obj in loadedStatObjects)
            {
                Destroy(obj);
            }
            loadedStatObjects.Clear();
        }
        

        #endregion
    }
    
    public static class ScrollRectExtensions
    {
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}