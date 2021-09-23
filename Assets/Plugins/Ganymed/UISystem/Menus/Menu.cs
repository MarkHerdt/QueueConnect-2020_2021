using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ganymed.UISystem
{
    /// <summary>
    /// Base class for every menu build with the custom menu UISystem.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class Menu : MonoBehaviour
    {
        #region --- [INSPECTOR] ---

        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")] [SerializeField]
        private bool destroyMenuWhenClosed = true;

        [FormerlySerializedAs("disableMenusUnderneath")] [Tooltip("Disable menus that are under this one in the stack")] [SerializeField]
        private bool closeMenusUnderneath = true;

        [Tooltip("When enabled the timescale will be manipulated when opening / closing this menu")] [SerializeField]
        private bool pauseGame = false;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [PROPERTIES] ---

        /// <summary>
        /// Returns true if the Menu is currently opened.
        /// </summary>
        public bool IsOpen { get; set; } = false;

        /// <summary>
        /// Menus can not be opened if they are not enabled.
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// Get / Set the sorting order of the menus canvas. 
        /// </summary>
        public int SortingOrder
        {
            get => Canvas.sortingOrder;
            set
            {
                Canvas.sortingOrder = value;
            
                foreach (var child in ChildCanvasesSortingOrder)
                {
                    child.Key.sortingOrder = child.Value + value;
                }
            }
        }
        
        #endregion
        
        #region --- [PROPERTIES: READONLY] ---

        public bool DestroyMenuWhenClosed => destroyMenuWhenClosed;
        
        public bool CloseMenusUnderneath => closeMenusUnderneath;
        
        public bool PauseGame => pauseGame;

        #endregion
        
        #region --- [PROPERTIES: COMPONENTS] ---

        /// <summary>
        /// Get the parent canvas of this menu.
        /// </summary>
        public Canvas Canvas
        {
            get => menuCanvas ? menuCanvas : menuCanvas = GetComponent<Canvas>();
            private set => menuCanvas = value;
        }

        /// <summary>
        /// Get every child canvas of this menu.
        /// </summary>
        public Canvas[] ChildCanvases
        {
            get => childCanvases ?? (childCanvases = GetComponentsInChildren<Canvas>().Where(x => x != Canvas).ToArray());
            private set => childCanvases = value;
        }
        
        /// <summary>
        /// Get the parent GraphicRaycaster of this menu.
        /// </summary>
        public GraphicRaycaster GraphicRaycaster
        {
            get => graphicRaycaster ? graphicRaycaster : graphicRaycaster = GetComponent<GraphicRaycaster>();
            private set => graphicRaycaster = value;
        }
        
        /// <summary>
        /// Get the CanvasScaler of this menu.
        /// </summary>
        public CanvasScaler CanvasScaler
        {
            get => canvasScaler ? canvasScaler : canvasScaler = GetComponent<CanvasScaler>();
            private set => canvasScaler = value;
        }

        public Dictionary<Canvas, int> ChildCanvasesSortingOrder
        {
            get
            {
                if (childCanvasesSortingOrder == null)
                {
                    childCanvasesSortingOrder = new Dictionary<Canvas, int>();
                    foreach (var child in ChildCanvases)
                    {
                        childCanvasesSortingOrder.Add(child, child.sortingOrder);
                    }
                }

                return childCanvasesSortingOrder;
            }
            private set => childCanvasesSortingOrder = value;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [FIELDS] ---

        private Canvas menuCanvas = null;
        private Canvas[] childCanvases = null;
        private GraphicRaycaster graphicRaycaster = null;
        private CanvasScaler canvasScaler = null;
        private Dictionary<Canvas, int> childCanvasesSortingOrder;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [OPEN CLOSE] ---

        public virtual void OpenInstance()
        {
            if(!this) return;
            gameObject.SetActive(true);
        }

        public virtual void CloseInstance()
        {
            if (DestroyMenuWhenClosed)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [ABSTRACT] ---

        /// <summary>
        /// Open the menu without passing any values.
        /// </summary>
        public abstract void OpenDirty();
        
        /// <summary>
        /// Close the menu without passing any values.
        /// </summary>
        public abstract void CloseDirty();

        /// <summary>
        /// This method is called on an active menu if the escape key is pressed.
        /// </summary>
        public abstract void OnBackPressed();

        #endregion
    }
}