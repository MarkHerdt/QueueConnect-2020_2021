using Ganymed.Utils.Helper;
using UnityEngine;

namespace Ganymed.UISystem
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        
        #region --- [STATIC OPEN / CLOSE] ---

        protected virtual void OnOpen()
        {
        }
        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// Method will create and open an instance of the menu.
        /// </summary>
        public static void Initialize()
        {
            if (Instance == null)
                MenuManager.Instance.CreateMenuInstance<T>();
            
            Instance.OnOpen();
            MenuManager.OpenMenu(Instance);
        }

        /// <summary>
        /// Method will close the instance of the menu if present.
        /// </summary>
        public static void Terminate()
        {
            if (Instance == null)
            {
                MenuManager.CloseMenu(Instance);
                Debug.LogWarning("Trying to close menu {0} but Instance is null");
                return;
            }
            Instance.OnClose();
            MenuManager.CloseMenu(Instance);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------


        #region --- [ENABLE / DISABLE] ---

        public static void DisableMenu()
        {
            Instance.IsEnabled = false;
        }

        public static void EnableMenu()
        {
            Instance.IsEnabled = true;
        }

        #endregion

        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [SINGLETON] ---

        public static T Instance => instanceCache ? instanceCache : instanceCache = FindObjectOfType<T>();

        private static T instanceCache;

        public static bool TryGetInstance(out T instance)
        {
            if (instanceCache != null)
            {
                instance = instanceCache;
            }
            else
            {
                instanceCache = FindObjectOfType<T>();
                instance = instanceCache;
            }

            var all = FindObjectsOfType<T>();

            foreach (var type in all)
                if (type != instanceCache)
                    DestroyImmediate(type.gameObject);

            return instanceCache != null;

            #endregion
        }

        //--------------------------------------------------------------------------------------------------------------

        #region --- [INITIALIZATION] ---

        protected virtual void Awake()
        {
            if (this == null) return;
            instanceCache = (T) this;

            if (Application.isPlaying)
                Instance.gameObject.DontDestroyOnLoad();


            var otherObjectsOfType = FindObjectsOfType<T>();
            if (otherObjectsOfType.Length <= 1) return;

            foreach (var obj in otherObjectsOfType)
            {
                if (obj == this) continue;

                Debug.LogWarning($"Singleton: Multiple instances of the same type {GetType()} detected! " +
                                 $"Destroyed other GameObject {obj.gameObject} instance!");

                DestroyImmediate(obj.gameObject);
            }
        }

        #endregion

        #region --- [OVERRIDES] ---

        /// <summary>
        /// This method is called on an active menu if the escape key is pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            Terminate();
        }

        /// <summary>
        /// Close the menu without passing any values.
        /// </summary>
        public override void CloseDirty()
        {
            Terminate();
        }

        /// <summary>
        /// Open the menu without passing any values.
        /// </summary>
        public override void OpenDirty()
        {
            Initialize();
        }

        #endregion

        #region --- [DESTROY] ---

        protected virtual void OnDestroy()
        {
            instanceCache = null;
        }

        #endregion
    }
}