using UnityEngine;
using Sirenix.OdinInspector;
using QueueConnect.GameSystem;
using System.Linq;
using TMPro;
using Button = UnityEngine.UI.Button;

namespace QueueConnect.Test
{
    /// <summary>
    /// Class to test things
    /// </summary>
    [ExecuteInEditMode, HideMonoScript]
    internal class DevelopmentTest : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("Button to toggle the Debug Menu")]
            [ChildGameObjectsOnly]
            [SerializeField] private RectTransform buttonRect;
            [Tooltip("RIght RobotPart Button")]
            [SceneObjectsOnly]
            #pragma warning disable 649
            [SerializeField] private SpriteRenderer buttonRight;
            #pragma warning restore 649
            [Tooltip("DebugLog GameObject in children")]
            [ChildGameObjectsOnly]
            [SerializeField] private GameObject debugLog;
            [Tooltip("Content GameObject in children")]
            [ChildGameObjectsOnly]
            [SerializeField] private GameObject content;
            [Tooltip("RectTransform Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private RectTransform contentRectTransform;
            [Tooltip("TextMeshPro Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private TextMeshProUGUI log;
#if UNITY_EDITOR
            [SerializeField] private bool setTimeScale = false;
            [SerializeField][Range(0,5f)] private float timeScale = 1f;
#endif
        #endregion

        #region Privates
            private bool updatePosition;
            private float rectHeight;
            private float textSize;
            private bool restart;
            private float previousSpeed;
        #endregion

        #region Properties
            /// <summary>
            /// Singleton of "Test.cs"
            /// </summary>
            public static DevelopmentTest Instance { get; private set; }
        #endregion

        #region RuntimeInitializeOnLoadMethod_Order
            //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
            //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            //Awake()
            //OnEnable()
            //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
            //[RuntimeInitializeOnLoadMethod()]
            //Start()
        #endregion
        
        private void Awake()
        {
            #if !UNITY_EDITOR
                if (!UnityEngine.Debug.isDebugBuild)
                {
                    Destroy(gameObject);
                }
            #endif
            
            if (buttonRect == null)
            {
                buttonRect = GetComponentInChildren<Button>().GetComponent<RectTransform>();
            }
            if (buttonRect.GetComponent<Canvas>().worldCamera == null)
            {
                buttonRect.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
            }
            if (debugLog == null)
            {
                debugLog = GetComponentsInChildren<Canvas>().FirstOrDefault(_Canvas => _Canvas.sortingOrder == short.MaxValue - 1)?.gameObject;
            }
            if (content == null)
            {
                content = debugLog != null ? debugLog.GetComponentInChildren<TextMeshProUGUI>().transform.parent.GetComponent<GameObject>() : null;
            }
            if (contentRectTransform == null)
            {
                contentRectTransform = debugLog != null ? debugLog.GetComponentInChildren<TextMeshProUGUI>().transform.parent.GetComponent<RectTransform>() : null;
            }
            if (log == null)
            {
                log = debugLog != null ? debugLog.GetComponentInChildren<TextMeshProUGUI>() : null;
            }
            
            Instance = Singleton.Persistent(this);
            rectHeight = contentRectTransform != null ? contentRectTransform.rect.height : 0;
            
            debugLog.SetActive(true);
            debugLog.SetActive(false);
        }

        private void Start()
        {
            //log.localizedText = "";
        }

        private void OnEnable()
        {
            CheckScreenSize.OnScreenSizeChanged += AllowUpdate;
        }

        private void OnDisable()
        {
            CheckScreenSize.OnScreenSizeChanged -= AllowUpdate;
        }

        private void Update()
        {
            UpdatePosition();
#if UNITY_EDITOR
            if (setTimeScale)
                Time.timeScale = timeScale;
#endif
        }

        /// <summary>
        /// Is called whenever the ScreenSize changes
        /// </summary>
        private void AllowUpdate()
        {
            updatePosition = true;
        }
        
        /// <summary>
        /// Updates the Position of the FPS Counter
        /// </summary>
        private void UpdatePosition()
        {
            if (!updatePosition) return;
            
                updatePosition = false;

                var _bounds = buttonRight.bounds;
                buttonRect.position = new Vector2(_bounds.center.x, _bounds.center.y - _bounds.size.y - 1);
        }
        
        /// <summary>
        /// Enables/disables the "DebugLog"-GameObject
        /// </summary>
        public void DebugLogSetActive()
        {
            debugLog.SetActive(!debugLog.activeSelf);

            if (!debugLog.activeSelf) return;
            
                Canvas.ForceUpdateCanvases();
                    
                var _sizeDelta = contentRectTransform.sizeDelta;
                _sizeDelta = new Vector2(_sizeDelta.x, log.textBounds.size.y > rectHeight ? log.textBounds.size.y : rectHeight);
                contentRectTransform.sizeDelta = _sizeDelta;
        }

        /// <summary>
        /// Adds localizedText to the Log <br/>
        /// Each Method call is separated with "-------------------------------------"
        /// </summary>
        /// <param name="_Text"></param>
        public static void SetLogText(string _Text)
        {
            Instance.log.text += $"{_Text.ToUpper()}";
            Instance.log.text += "\n-------------------------------------\n";
        }
    }
}