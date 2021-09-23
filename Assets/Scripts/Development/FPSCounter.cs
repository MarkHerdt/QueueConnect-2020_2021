using System.Linq;
using QueueConnect.ExtensionMethods;
using QueueConnect.GameSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace QueueConnect.Development
{
    /// <summary>
    /// Measures average frames per second
    /// </summary>
    [HideMonoScript]
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FPSCounter : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("Background GameObject")]
            [SceneObjectsOnly]
            [SerializeField] private SpriteRenderer background;
            [Tooltip("RectTransform Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private RectTransform rectTransform;
            [Tooltip("TextMeshProUI Component")]
            [ChildGameObjectsOnly]
            [SerializeField] private TextMeshProUGUI textMeshProUI;
            [Tooltip("Time in seconds between the measurements")]
            [PropertyRange("min", "max")]
            [SerializeField] private float measurePeriod = .5f;
            #if UNITY_EDITOR
                [HorizontalGroup("MinMax", LabelWidth = 30)]
                [BoxGroup("MinMax/Left", ShowLabel = false)]
                #pragma warning disable
                [SerializeField] private float min = .1f;
                [BoxGroup("MinMax/Right", ShowLabel = false)]
                [SerializeField] private float max = 1.0f;
                #pragma warning enable
            #endif
        #endregion

        #region Privates
            private bool updatePosition;
            private int counter;
            private float nextMeasurement;
            private int currentFPS;
        #endregion
        
        private void Awake()
        {
            if (background == null)
            {
                background = FindObjectsOfType<Collider2D>().First(_Collider => _Collider.gameObject.Tag(Tags.MapExit)).GetComponent<SpriteRenderer>();
            }
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (textMeshProUI == null)
            {
                textMeshProUI = GetComponent<TextMeshProUGUI>();
            }
            if (transform.parent.GetComponent<Canvas>().worldCamera == null)
            {
                transform.parent.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
            }
        }
        
        private void OnEnable()
        {
            CheckScreenSize.OnScreenSizeChanged += AllowUpdate;
        }

        private void OnDisable()
        {
            CheckScreenSize.OnScreenSizeChanged -= AllowUpdate;
        }
        
        private void Start()
        {
            nextMeasurement = Time.realtimeSinceStartup + measurePeriod;
        }
        
        private void Update()
        {
            UpdatePosition();
            
            Counter();
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

            var _bounds = background.bounds;
            rectTransform.position = new Vector2(_bounds.min.x, _bounds.max.y);
        }
        
        /// <summary>
        /// Updates the FPS
        /// </summary>
        private void Counter()
        {
            counter++;

            if (!(Time.realtimeSinceStartup > nextMeasurement)) return;
            
                currentFPS = (int)(counter / measurePeriod);
                counter = 0;
                nextMeasurement += measurePeriod;
                textMeshProUI.text = $"FPS: {currentFPS.ToString()}";
        }
    }
}