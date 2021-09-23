using System;
using System.Collections.Generic;
using Ganymed.Utils.ExtensionMethods;
using Ganymed.Utils.Singleton;
using UnityEngine;

namespace Ganymed.UISystem
{
    [AddComponentMenu("UISystem/Menu Manger")]
    public class MenuManager : MonoSingleton<MenuManager>
    {
        
        #region --- [INSPECTOR] ---

        [Header("Settings")]
        [Tooltip("When enabled the Start Menu will be opened on start")]
        [SerializeField] private bool openStartMenu = true;
        [Tooltip("When enabled the Pause Menu will be opened if escape is pressed and no other menu is open")]
        [SerializeField] private bool openPauseMenu = true;
        [Tooltip("The sorting order offset that is applied to each opened menu")]
        [SerializeField] private int sortingOrderOffset = 100;
        [SerializeField] private int startSortingOrder = 200;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region --- [MENUS] ---
        
        [Header("Menu Prefabs")]
        [Tooltip("Menu that will be opened on start. Must be enabled!")]
        [SerializeField] private Menu startMenu = null;
        [Tooltip("Menu that will be opened if escape is pressed and no other menu is open. Must be enabled!")]
        [SerializeField] private Menu pauseMenu = null;

        [Space]
        [Tooltip("When enabled Menu prefabs will be loaded automatically from resources folders.")]
        [SerializeField] private bool loadFromResources = true;
        [Tooltip("When enabled only folders in subPaths will be searched for menus.")]
        [SerializeField] private bool useSubPathsOnly = true;
        [Tooltip("When enabled Menu prefabs will be loaded automatically from resources folders.")]
        [SerializeField] private string[] subPaths = new []{"Menus"};
        [Tooltip("List containing prefabs of registered Menus.")]
        [SerializeField] private List<Menu> registeredMenus = new List<Menu>();
        
        #endregion

        #region --- [PROPERTIES] ---

        public int OpenMenuCount => menuStack.Count;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [FIELDS] ---

        private static readonly Stack<Menu> menuStack = new Stack<Menu>();

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [EVENTS] ---

        public static event Action<Menu> onMenuOpened;
        public static event Action<Menu> onMenuClosed;
        public static event Action onAllMenusClosed;
        public static event Action onAnyMenuOpened;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [STATIC ACCESS] ---

        public static void OpenMenu<T>(T menu) where T : Menu
        {
            Instance.OpenMenu(menu);
        }
        
        public static void CloseMenu<T>(T menu) where T : Menu
        {
            Instance.CloseMenu(menu);
        }
        
        public static void CloseAllMenus() => Instance.CloseAllMenus();
        public static void CloseAllMenus<T>(params T[] exclude) where T : Menu
            => Instance.CloseAllMenus();

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [OPEN] ---

        private void OpenMenu(Menu menu)
        {
            if(!menu.IsEnabled) return;
            menu.OpenInstance();
            menu.GraphicRaycaster.enabled = true;
            
            // De-activate top menu
            if (menuStack.Count > 0)
            {
                foreach (var obj in menuStack)
                {
                    obj.GraphicRaycaster.enabled = false;
                }
                
                if (menu.CloseMenusUnderneath)
                    foreach (var obj in menuStack)
                    {
                        obj.CloseInstance();

                        if (obj.CloseMenusUnderneath)
                            break;
                    }
                
                // This line sets the sorting order of the opened menu and its child canvases.
                menu.SortingOrder = menuStack.Peek().SortingOrder + sortingOrderOffset; 
            }
            else
            {
                menu.SortingOrder = startSortingOrder;
            }

            if (menu.PauseGame)
            {
                Time.timeScale = 0;
            }

            menu.IsOpen = true;
            
            if(enableLogs) Debug.Log($"Opening: {menu.name}");
            
            
            onMenuOpened?.Invoke(menu);
            onAnyMenuOpened?.Invoke();
            menuStack.Push(menu);
            OnMenuOpened(menu);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INIT] ---

        protected virtual void Start()
        {
            if(openStartMenu) OpenStartMenu();
        }
        
        protected virtual void OnEnable()
        {
            registeredMenus.AddIfNotInList(startMenu);
            registeredMenus.AddIfNotInList(pauseMenu);
            if (loadFromResources) LoadMenusFromResources();
        }

        private void LoadMenusFromResources()
        {
            if(enableLogs) Debug.Log("Searching Resources for menu prefabs to register!");
            if(!useSubPathsOnly) LoadMenuFromPath();
            foreach (var subPath in subPaths)
            {
                LoadMenuFromPath(subPath);
            }
        }

        private void LoadMenuFromPath(string path = "")
        {
            foreach (var obj in Resources.LoadAll(path, typeof(GameObject)))
            {
                var menu = (obj as GameObject)?.GetComponent<Menu>();
                registeredMenus.AddIfNotInList(menu);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [CLOSE] ---

        private void CloseMenu(Menu menu)
        {
            if (menuStack.Count == 0)
            {
                //Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
                return;
            }

            if (menuStack.Peek() != menu)
            {
                //Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
                return;
            }

            if (menu != null)
            {
                menu.IsOpen = false;
                if(enableLogs) Debug.Log($"Closing: {menu.name}");
            }
            
            CloseTopMenu();
        }

        
        private void CloseTopMenu()
        {
            var instance = menuStack.Pop();
            onMenuClosed?.Invoke(instance);
            OnMenuClosed(instance);

            instance.CloseInstance();

            ValidateMenuStack();
            
            foreach (var menu in menuStack)
            {
                menu.GraphicRaycaster.enabled = true;
                onMenuOpened?.Invoke(menu);
                if (menu.CloseMenusUnderneath)
                    break;
            }
            
            // Re-activate top menu
            // If a re-activated menu is an overlay we need to activate the menu under it

            if (menuStack.Count <= 0)
            {
                onAllMenusClosed?.Invoke();
                OnAllMenusClosed();
            }
            
            if (!instance.CloseMenusUnderneath) return;
            
            foreach (var menu in menuStack)
            {
                menu.OpenInstance();

                if (menu.CloseMenusUnderneath)
                    break;
            }
        }
        
        private void CloseAllMenus(params Menu[] exclude)
        {
            if(enableLogs) Debug.Log($"Closing All Menus");
            while (menuStack.Count > 0)
            {
                CloseMenu(menuStack.Peek());
            }
            onAllMenusClosed?.Invoke();
            OnAllMenusClosed();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [UPDATE (OUTSOURCE)] ---

        public static bool InvokeOnBackPressed()
        {
            if (menuStack.Count > 0)
            {
                menuStack.Peek().OnBackPressed();
                
                return true;
            }
            if (Instance.openPauseMenu)
            {
                Instance.OpenPauseMenu();
                return true;
            }
            
            return false;
        }

        protected virtual void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
            if (menuStack.Count > 0)
            {
                menuStack.Peek()?.OnBackPressed();
            }
            else if(openPauseMenu)
            {
                OpenPauseMenu();
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [MENU INSTANTIATION] ---

        public void CreateMenuInstance<T>() where T : Menu
        {
            Instantiate(GetMenuPrefab<T>());
        }


        private T GetMenuPrefab<T>() where T : Menu
        {
            foreach (var field in registeredMenus)
            {
                var prefab = field as T;
                if (prefab != null) return prefab;
            }

            throw new MissingReferenceException($"Menu of type: {typeof(T)} does not exist!");
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [VIRTUAL] ---

        /// <summary>
        /// Method is called on Start to open the start menu.
        /// Override if you want to open a start menu and pass values!
        /// </summary>
        protected virtual void OpenStartMenu()
        {
            startMenu.OpenDirty();
        }

        /// <summary>
        /// Method is called on Esc to open the pause menu.
        /// Override if you want to open a start menu and pass values!
        /// </summary>
        protected virtual void OpenPauseMenu()
        {
            pauseMenu.OpenDirty();
        }


        protected virtual void OnMenuClosed(Menu menu)
        {
        }
        
        protected virtual void OnMenuOpened(Menu menu)
        {
        }
        
        /// <summary>
        /// Method is called when every menu has been closed.
        /// This includes deactivated menus.
        /// </summary>
        protected virtual void OnAllMenusClosed()
        {
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [UTILITIES] ---

        /// <summary>
        /// Returns true if the passed menu is the currently active menu instance.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public static bool IsTopMenu(Menu menu) => menuStack.Count > 0 && menuStack.Peek() == menu;

        private void ValidateMenuStack()
        {
            while (menuStack.Count > 0 && menuStack.Peek() == null)
            {
                menuStack.Pop();
            }
        }

        #endregion
        
    }
}