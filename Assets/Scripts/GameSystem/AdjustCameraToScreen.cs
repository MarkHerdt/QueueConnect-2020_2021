using System.Globalization;
using System.Linq;
using QueueConnect.Attributes;
using QueueConnect.Config;
using QueueConnect.Development;
using QueueConnect.ExtensionMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QueueConnect.GameSystem
{
    /// <summary>
    /// Adjusts the CameraScale to the Screen
    /// </summary>
    [ExecuteInEditMode, HideMonoScript]
    [RequireComponent(typeof(Camera))]
    public class AdjustCameraToScreen : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("\"Map\" GameObject in the Scene")]
            [SceneObjectsOnly]
            [SerializeField] private GameObject environment;
            [Tooltip("SpriteRenderer of the \"Background\" GameObject in the Scene")]
            [SceneObjectsOnly]
            [SerializeField] private SpriteRenderer background;
            [Tooltip("Camera Component")]
            [ChildGameObjectsOnly]
            #pragma warning disable 109
            [SerializeField] private new Camera camera;
            #pragma warning restore 109
            [Tooltip("SpriteRenderer of the Border"), InfoBox("Must have the same order as in the Scene! (Left, Right, Bottom, Top)")][ElementNames("Left", "Right", "Bottom", "Top")]
            [SceneObjectsOnly, ListDrawerSettings(DraggableItems = false)]
            [SerializeField] private GameObject[] border = new GameObject[4];
            #if UNITY_EDITOR
                #pragma warning disable 649
                [Title("Debug", TitleAlignment = TitleAlignments.Centered), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)][ShowInInspector] private readonly Title title;
                #pragma warning restore 649
            #endif
            [Tooltip("Turns DebugLogs on/off for this class")]
            #pragma warning disable 649
            [SerializeField] private bool debugLogs;
            #pragma warning restore 649
            [Tooltip("Width of the Screen")]
            [BoxGroup("Screen")]
            [SerializeField][ReadOnly] private float screenWidth;
            [Tooltip("Height of the Screen")]
            [BoxGroup("Screen")]
            [SerializeField][ReadOnly] private float screenHeight;
            [Tooltip("Width of the SafeArea")]
            [BoxGroup("SafeArea")]
            [SerializeField][ReadOnly] private float safeWidth;
            [Tooltip("Height of the SafeArea")]
            [BoxGroup("SafeArea")]
            [SerializeField][ReadOnly] private float safeHeight;
        #endregion

        #region Privates
            #pragma warning disable 649
            private static AdjustCameraToScreen instance;
            #pragma warning restore 649
        #endregion

        #region Properties
            public static GameObject[] Border => instance.border;
        #endregion
        
        private void Awake()
        {
            if (Application.isPlaying)
            {
                var _gameObjects = FindObjectsOfType<GameObject>();

                if (environment == null)
                {
                    foreach (var _gameObject in _gameObjects)
                    {
                        if (!_gameObject.Tag(Tags.Map)) continue;
                        
                            environment = _gameObject;
                            break;
                    }
                }
                if (background == null)
                {
                    foreach (var _gameObject in _gameObjects)
                    {
                        if (!_gameObject.Tag(Tags.Background)) continue;
                        
                            background = _gameObject.GetComponent<SpriteRenderer>();
                            break;
                    }
                }
                if (border.Length != 4)
                {
                    border = new GameObject[4];

                    foreach (var _gameObject in _gameObjects)
                    {
                        if (!_gameObject.Tag(Tags.Border)) continue;
                        
                            var _children = _gameObject.GetComponentsInChildren<SpriteRenderer>();

                            for (byte i = 0; i < _children.Length; i++)
                            {
                                border[i] = _children[i].gameObject;
                            }

                            break;
                    }
                }
                else
                {
                    var _children = (from _gameObject in _gameObjects where _gameObject.Tag(Tags.Border) select _gameObject.GetComponentsInChildren<SpriteRenderer>()).FirstOrDefault();
                    
                    for (byte i = 0; i < border.Length; i++)
                    {
                        if (border[i] == null)
                        {
                            border[i] = _children?[i].gameObject;
                        }
                    }
                }
                if (camera == null)
                {
                    camera = GetComponent<Camera>();
                }
            }
            
            AdjustCameraSize();
        }
        
        private void Start()
        {
            CheckScreenSize.OnScreenSizeChanged += AdjustCameraSize;
        }

        private void OnDestroy()
        {
            CheckScreenSize.OnScreenSizeChanged -= AdjustCameraSize;
        }
        
        /// <summary>
        /// Passes the values to the GameConfig
        /// </summary>
        /// <param name="_MapWidth">Width of the Map</param>
        /// <param name="_MapHeight">Height of the Map</param>
        /// <param name="_MapCenter">Center point of the Map</param>
        private static void PassValuesToConfig(float _MapWidth, float _MapHeight, Vector3 _MapCenter)
        {
            GameConfig.MapWidth = _MapWidth;
            GameConfig.MapHeight = _MapHeight;
            GameConfig.MapCenter = _MapCenter;
        }
        
        /// <summary>
        /// Resets the ProjectionMatrix and the OrthographicSize
        /// </summary>
        /// <param name="_Size">Size the Orthographic Camera should have</param>
        [Button]
        private void ResetCameraValues(byte _Size)
        {
            if (camera == null) return;
            
                camera.ResetProjectionMatrix();
                camera.orthographicSize = _Size;
        }
        
        /// <summary>
        /// Calculates the values to adjust the camera size to the devices screen
        /// </summary>
        [Button]
        private void AdjustCameraSize()
        {
            if (environment == null || background == null || camera == null) return;

                ResetCameraValues(1);

                // Screen sizes in world units
                var _screenWidth = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(0f, .5f)), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                var _screenHeight = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(.5f, 0f)), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));
                var _safeWidth = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, 0f)), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, 0f)));
                var _safeHeight = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMin)), camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMax)));
                var _backgroundBounds = background.bounds;
                var _backgroundWidth = Vector2.Distance(new Vector2(_backgroundBounds.min.x, 0f), new Vector2(_backgroundBounds.max.x, 0f));
                var _backgroundHeight = Vector2.Distance(new Vector2(0f, _backgroundBounds.min.y), new Vector2(0f, _backgroundBounds.max.y));
                
                // Aspect Ratios
                var _screenAspect = _screenWidth / _screenHeight;
                var _safeAspect = _safeWidth / _safeHeight;
                var _backgroundAspect = _backgroundWidth / _backgroundHeight;

                // Mobile
                if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    if (_safeAspect <= 1.6f)
                    {
                        camera.orthographicSize = _backgroundWidth * _screenHeight / _screenWidth * .5f;
                    }
                    else
                    {
                        var _safeBackgroundAspect = _safeHeight / _backgroundHeight;
                        var _deltaHeight = _screenHeight - _safeHeight;
                        var _offset = _deltaHeight / _safeBackgroundAspect;

                        camera.orthographicSize = (_backgroundHeight / 2) + (_offset / 2);
                    }   
                    
                    #if UNITY_EDITOR
                        showSafeArea = true;
                        distanceLeft = Vector2.Distance(new Vector2(_backgroundBounds.min.x, _backgroundBounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, Screen.safeArea.center.y)));
                        distanceRight = Vector2.Distance(new Vector2(_backgroundBounds.max.x, _backgroundBounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, Screen.safeArea.center.y)));
                        distanceBottom = Vector2.Distance(new Vector2(_backgroundBounds.center.x, _backgroundBounds.min.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMin)));
                        distanceTop = Vector2.Distance(new Vector2(_backgroundBounds.center.x, _backgroundBounds.max.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMax)));
                    #endif
                }
                // Other
                else
                {
                    if (_screenAspect < 1.666667f)
                    {
                        camera.orthographicSize = _backgroundWidth * _screenHeight / _screenWidth * .5f;
                    }
                    else
                    {
                        camera.orthographicSize = _backgroundHeight / 2;
                    }
                    
                    #if UNITY_EDITOR
                        showSafeArea = false;
                        distanceLeft = Vector2.Distance(new Vector2(_backgroundBounds.min.x, _backgroundBounds.center.y), camera.ViewportToWorldPoint(new Vector2(0f, .5f)));
                        distanceRight = Vector2.Distance(new Vector2(_backgroundBounds.max.x, _backgroundBounds.center.y), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                        distanceBottom = Vector2.Distance(new Vector2(_backgroundBounds.center.x, _backgroundBounds.min.y), camera.ViewportToWorldPoint(new Vector2(.5f, 0f)));
                        distanceTop = Vector2.Distance(new Vector2(_backgroundBounds.center.x, _backgroundBounds.max.y), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));
                    #endif
                }

                CenterMap();

                var _backgroundPosition = background.transform.position;
                var _screenCenter = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, _backgroundPosition.z));
                var _backGroundBounds = background.bounds;
                var _backgroundCenter = new Vector3(_backGroundBounds.center.x, _backGroundBounds.center.y, _backgroundPosition.z); 
                PassValuesToConfig(_backgroundWidth, _backgroundHeight, _backgroundCenter);
                
                SetBorderSize(_backgroundCenter, _backgroundWidth, _backgroundHeight, _screenCenter);
                
                // Displays the values (for debugging)
                #if UNITY_EDITOR
                    if (debugLogs)
                    {
                        var _debugWidth = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(0f, .5f)), camera.ViewportToScreenPoint(new Vector2(1f, .5f)));
                        var _debugHeight = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(.5f, 0f)), camera.ViewportToScreenPoint(new Vector2(.5f, 1f)));
                
                        DebugLog.White("-----------------------------------------");
                        DebugLog.White_Red_White_Red("ScreenWidth: ", $"{_debugWidth.ToString(CultureInfo.InvariantCulture)}", " | ScreenHeight: ", $"{_debugHeight.ToString(CultureInfo.InvariantCulture)}");
                        DebugLog.White_Green_White_Green("SafeWidth: ", $"{(showSafeArea ? Screen.safeArea.width : 0).ToString(CultureInfo.InvariantCulture)}", " | SafeHeight: ", $"{(showSafeArea ? Screen.safeArea.height : 0).ToString(CultureInfo.InvariantCulture)}");
                        DebugLog.White_Red_White_Green_White_Blue("Aspect: ", $"{_screenAspect.ToString(CultureInfo.InvariantCulture)}", " | SafeAspect: ", $"{(showSafeArea ? _safeAspect : 0).ToString(CultureInfo.InvariantCulture)}", " | Background Aspect: ", $"{_backgroundAspect.ToString(CultureInfo.InvariantCulture)}");
                    }
                    
                    
                    screenWidth = _screenWidth;
                    screenHeight = _screenHeight;
                    safeWidth = _safeWidth;
                    safeHeight = _safeHeight;
                    backgroundWidth = _backgroundWidth;
                    backgroundHeight = _backgroundHeight;
                #endif
        }
        
        /// <summary>
        /// Centers the Map in front of the Camera
        /// </summary>
        [Button]
        private void CenterMap()
        {
            if (camera == null || environment == null) return;
            
                camera.transform.position = Vector3.zero;
                
                if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    environment.transform.position = camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.center.x, Screen.safeArea.center.y, 1f));   
                }
                else
                {
                    var _cameraPosition = camera.transform.position;
                    environment.transform.position = _cameraPosition.WithZ(_cameraPosition.z + 1);
                }
        }
        
        /// <summary>
        /// Resizes the Border to fill the empty space
        /// </summary>
        /// <param name="_BackgroundCenter">Center point of the Background</param>
        /// <param name="_BackgroundWidth">Width of the Background Sprite in world units</param>
        /// <param name="_BackgroundHeight">Height of the Background Sprite in world units</param>
        /// <param name="_ScreenCenter">Center point of the Screen</param>
        private void SetBorderSize(Vector3 _BackgroundCenter, float _BackgroundWidth, float _BackgroundHeight, Vector3 _ScreenCenter)
        {
            if (background == null || border.Length <= 0 || border[0] == null || border[1] == null || border[2] == null || border[3] == null || camera == null) return;
            
                var _ppu = background.sprite.pixelsPerUnit;
            
                border[0].transform.position = new Vector2(_BackgroundCenter.x - (_BackgroundWidth / 2), _BackgroundCenter.y);
                var _left = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(0f, .5f)), new Vector2(border[0].transform.position.x, border[0].transform.position.y)) * _ppu;
                border[0].transform.localScale = new Vector2(_left, border[0].transform.localScale.y);

                border[1].transform.position = new Vector2(_BackgroundCenter.x + (_BackgroundWidth / 2), _BackgroundCenter.y);
                var _right = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(1f, .5f)), new Vector2(border[1].transform.position.x, border[1].transform.position.y)) * _ppu;
                border[1].transform.localScale = new Vector2(_right, border[1].transform.localScale.y);
                
                border[2].transform.position = new Vector2(_ScreenCenter.x, _BackgroundCenter.y - (_BackgroundHeight / 2));
                var _bottom = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(.5f, 0)), new Vector2(border[2].transform.position.x, border[2].transform.position.y)) * _ppu;
                border[2].transform.localScale = new Vector2((_BackgroundWidth * _ppu) + _left + _right, _bottom);

                border[3].transform.position = new Vector2(_ScreenCenter.x, _BackgroundCenter.y + (_BackgroundHeight / 2));
                var _top = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(.5f, 1)), new Vector2(border[3].transform.position.x, border[3].transform.position.y)) * _ppu;
                border[3].transform.localScale = new Vector2((_BackgroundWidth * _ppu) + _left + _right, _top);
        }
        
        #if UNITY_EDITOR
            #region Inspector Fields
                [Tooltip("Width of the Background")]
                [BoxGroup("Background")]
                [SerializeField][ReadOnly] private float backgroundWidth;
                [Tooltip("Height of the Background")]
                [BoxGroup("Background")]
                [SerializeField][ReadOnly] private float backgroundHeight;
                [Tooltip("Distance from the left border of the Background to the left border of the SafeArea/ScreenBorder (Depends on if there is a SafeArea or not)")]
                [BoxGroup("Distance")]
                [SerializeField] private float distanceLeft;
                [Tooltip("Distance from the right border of the Background to the right border of the SafeArea/ScreenBorder (Depends on if there is a SafeArea or not)")]
                [BoxGroup("Distance")]
                [SerializeField] private float distanceRight;
                [Tooltip("Distance from the bottom border of the Background to the bottom border of the SafeArea/ScreenBorder (Depends on if there is a SafeArea or not)")]
                [BoxGroup("Distance")]
                [SerializeField] private float distanceBottom;
                [Tooltip("Distance from the top border of the Background to the top border of the SafeArea/ScreenBorder (Depends on if there is a SafeArea or not)")]
                [BoxGroup("Distance")]
                [SerializeField] private float distanceTop;
            #endregion

            #region Privates
                private bool showSafeArea = true;
                private float aspect;
            #endregion

            /// <summary>
            /// Calculates and displays the values without passing them to the Camera
            /// </summary>
            [Button]
            private void CalculateValues()
            {
                if (background == null || camera == null) return;

                    // Screen sizes in world units
                    var _screenWidth = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(0f, .5f)), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                    var _screenHeight = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(.5f, 0f)), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));
                    var _safeWidth = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, 0f)), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, 0f)));
                    var _safeHeight = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMin)), camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMax)));
                    var _bounds = background.bounds;
                    var _backgroundWidth = Vector2.Distance(new Vector2(_bounds.min.x, 0f), new Vector2(_bounds.max.x, 0f));
                    var _backgroundHeight = Vector2.Distance(new Vector2(0f, _bounds.min.y), new Vector2(0f, _bounds.max.y));
                    
                    // Aspect Ratios
                    var _screenAspect = _screenWidth / _screenHeight;
                    var _safeAspect = _safeWidth / _safeHeight;
                    var _backgroundAspect = _backgroundWidth / _backgroundHeight;
                    
                    if (SystemInfo.deviceType == DeviceType.Handheld)
                    {
                        showSafeArea = true;
                        distanceLeft = Vector2.Distance(new Vector2(_bounds.min.x, _bounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, Screen.safeArea.center.y)));
                        distanceRight = Vector2.Distance(new Vector2(_bounds.max.x, _bounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, Screen.safeArea.center.y)));
                        distanceBottom = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.min.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMin)));
                        distanceTop = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.max.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMax)));

                        aspect = _screenAspect;
                    }
                    else
                    {
                        showSafeArea = false;
                        distanceLeft = Vector2.Distance(new Vector2(_bounds.min.x, _bounds.center.y), camera.ViewportToWorldPoint(new Vector2(0f, .5f)));
                        distanceRight = Vector2.Distance(new Vector2(_bounds.max.x, _bounds.center.y), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                        distanceBottom = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.min.y), camera.ViewportToWorldPoint(new Vector2(.5f, 0f)));
                        distanceTop = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.max.y), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));

                        aspect = _screenAspect;
                    }
                    
                    if (debugLogs)
                    {
                        var _debugWidth = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(0f, .5f)), camera.ViewportToScreenPoint(new Vector2(1f, .5f)));
                        var _debugHeight = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(.5f, 0f)), camera.ViewportToScreenPoint(new Vector2(.5f, 1f)));
                        
                        DebugLog.White("-----------------------------------------");
                        DebugLog.White_Red_White_Red("ScreenWidth: ", $"{_debugWidth.ToString(CultureInfo.InvariantCulture)}", " | ScreenHeight: ", $"{_debugHeight.ToString(CultureInfo.InvariantCulture)}");
                        DebugLog.White_Green_White_Green("SafeWidth: ", $"{(showSafeArea ? Screen.safeArea.width : 0).ToString(CultureInfo.InvariantCulture)}", " | SafeHeight: ", $"{(showSafeArea ? Screen.safeArea.height : 0).ToString(CultureInfo.InvariantCulture)}");
                        DebugLog.White_Red_White_Green_White_Blue("Aspect: ", $"{_screenAspect.ToString(CultureInfo.InvariantCulture)}", " | SafeAspect: ", $"{(showSafeArea ? _safeAspect : 0).ToString(CultureInfo.InvariantCulture)}", " | Background Aspect: ", $"{_backgroundAspect.ToString(CultureInfo.InvariantCulture)}");
                    }
                        
                    screenWidth = _screenWidth;
                    screenHeight = _screenHeight;
                    safeWidth = _safeWidth;
                    safeHeight = _safeHeight;
                    backgroundWidth = _backgroundWidth;
                    backgroundHeight = _backgroundHeight;
            }

            /// <summary>
            /// Adjusts the ProjectionMatrix of the Camera to fit the Screen (Deprecated)
            /// </summary>
            private void AdjustCameraSize_Deprecated()
            {
                if (environment == null || background == null || camera == null) return;
                    
                    ResetCameraValues(100);
            
                    // Screen sizes in world units
                    var _screenWidth = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(0f, .5f)), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                    var _screenHeight = Vector2.Distance(camera.ViewportToWorldPoint(new Vector2(.5f, 0f)), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));
                    var _safeWidth = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, 0f)), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, 0f)));
                    var _safeHeight = Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMin)), camera.ScreenToWorldPoint(new Vector2(0f, Screen.safeArea.yMax)));
                    var _bounds = background.bounds;
                    var _backgroundWidth = Vector2.Distance(new Vector2(_bounds.min.x, 0f), new Vector2(_bounds.max.x, 0f));
                    var _backgroundHeight = Vector2.Distance(new Vector2(0f, _bounds.min.y), new Vector2(0f, _bounds.max.y));
                    
                    // Aspect Ratios
                    float _aspect;
                    var _screenAspect = _screenWidth / _screenHeight;
                    var _safeAspect = _safeWidth / _safeHeight;
                    var _backgroundAspect = _backgroundWidth / _backgroundHeight;
            
                    // Values to calculate with
                    var _width = 0f;
                    var _height = 0f;
                    var _deltaWidth = 0f;
                    var _deltaHeight = 0f;
                    var _offset = 0f;

                    // Mobile
                    if (SystemInfo.deviceType == DeviceType.Handheld)
                    {
                        _aspect = _screenAspect;
                    
                        _width = _safeWidth;
                        _height = _safeHeight;
                    
                        #if UNITY_EDITOR
                            showSafeArea = true;
                            distanceLeft = Vector2.Distance(new Vector2(_bounds.min.x, _bounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMin, Screen.safeArea.center.y)));
                            distanceRight = Vector2.Distance(new Vector2(_bounds.max.x, _bounds.center.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.xMax, Screen.safeArea.center.y)));
                            distanceBottom = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.min.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMin)));
                            distanceTop = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.max.y), camera.ScreenToWorldPoint(new Vector2(Screen.safeArea.center.x, Screen.safeArea.yMax)));
                        #endif
                    }
                    // Other
                    else
                    {
                        _aspect = _screenAspect;
                        
                        _width = _screenWidth;
                        _height = _screenHeight;
                        
                        #if UNITY_EDITOR
                            showSafeArea = false;
                            distanceLeft = Vector2.Distance(new Vector2(_bounds.min.x, _bounds.center.y), camera.ViewportToWorldPoint(new Vector2(0f, .5f)));
                            distanceRight = Vector2.Distance(new Vector2(_bounds.max.x, _bounds.center.y), camera.ViewportToWorldPoint(new Vector2(1f, .5f)));
                            distanceBottom = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.min.y), camera.ViewportToWorldPoint(new Vector2(.5f, 0f)));
                            distanceTop = Vector2.Distance(new Vector2(_bounds.center.x, _bounds.max.y), camera.ViewportToWorldPoint(new Vector2(.5f, 1f)));
                        #endif
                    }
                    
                    _deltaWidth = GetDeltaValue_Deprecated(_width, _backgroundWidth);
                    _deltaHeight = GetDeltaValue_Deprecated(_height, _backgroundHeight);
                    
                    // Both Screen-values are smaller
                    if (_width <= _backgroundWidth && _height <= _backgroundHeight)
                    {
                        // Adds the bigger value
                        _offset = _deltaWidth > _deltaHeight ? _deltaWidth : _deltaHeight;
                    }
                    // One Screen-value is smaller
                    else if ((_width <= _backgroundWidth && _height >= _backgroundHeight) || (_width >= _backgroundWidth && _height <= _backgroundHeight))
                    {
                        if (_width < _backgroundWidth)
                        {
                            _offset = _deltaWidth;
                        }
                        else if (_height < _backgroundHeight)
                        {
                            _offset = _deltaHeight;
                        }
                    }
                    // Both Screen-values are bigger
                    else
                    {
                        // Subtracts the smaller value
                        _offset = _deltaWidth < _deltaHeight ? -_deltaWidth : -_deltaHeight;
                    }
            
                    // Resizes the Camera
                    var _orthographicSize = camera.orthographicSize;
                    camera.projectionMatrix = Matrix4x4.Ortho(left:   -(_orthographicSize * _aspect) - _offset,
                                                              right:   (_orthographicSize * _aspect) + _offset,
                                                              bottom: -_orthographicSize - _offset,
                                                              top:     _orthographicSize + _offset,
                                                              zNear:   camera.nearClipPlane,
                                                              zFar:    camera.farClipPlane);
                        
                    CenterMap();

                    var _backgroundPosition = background.transform.position;
                    var _screenCenter = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, _backgroundPosition.z));
                    var _backGroundBounds = background.bounds;
                    var _backgroundCenter = new Vector3(_backGroundBounds.center.x, _backGroundBounds.center.y, _backgroundPosition.z); 
                    PassValuesToConfig(_backgroundWidth, _backgroundHeight, _backgroundCenter);
                
                    SetBorderSize(_backgroundCenter, _backgroundWidth, _backgroundHeight, _screenCenter);
                    
                    // Displays the values (for debugging)
                    #if UNITY_EDITOR
                        if (debugLogs)
                        {
                            var _debugWidth = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(0f, .5f)), camera.ViewportToScreenPoint(new Vector2(1f, .5f)));
                            var _debugHeight = Vector2.Distance(camera.ViewportToScreenPoint(new Vector2(.5f, 0f)), camera.ViewportToScreenPoint(new Vector2(.5f, 1f)));
                    
                            DebugLog.White("-----------------------------------------");
                            DebugLog.White_Red_White_Red("ScreenWidth: ", $"{_debugWidth.ToString(CultureInfo.InvariantCulture)}", " | ScreenHeight: ", $"{_debugHeight.ToString(CultureInfo.InvariantCulture)}");
                            DebugLog.White_Green_White_Green("SafeWidth: ", $"{(showSafeArea ? Screen.safeArea.width : 0).ToString(CultureInfo.InvariantCulture)}", " | SafeHeight: ", $"{(showSafeArea ? Screen.safeArea.height : 0).ToString(CultureInfo.InvariantCulture)}");
                            DebugLog.White_Red_White_Green_White_Blue("Aspect: ", $"{_screenAspect.ToString(CultureInfo.InvariantCulture)}", " | SafeAspect: ", $"{(showSafeArea ? _safeAspect : 0).ToString(CultureInfo.InvariantCulture)}", " | Background Aspect: ", $"{_backgroundAspect.ToString(CultureInfo.InvariantCulture)}");
                        }
                        
                        screenWidth = _screenWidth;
                        screenHeight = _screenHeight;
                        safeWidth = _safeWidth;
                        safeHeight = _safeHeight;
                        backgroundWidth = _backgroundWidth;
                        backgroundHeight = _backgroundHeight;
                    #endif
            }
            
            /// <summary>
            /// Calculates which of the passed values is bigger
            /// </summary>
            /// <param name="_Value1"></param>
            /// <param name="_Value2"></param>
            /// <returns>Returns the bigger value</returns>
            private static float GetDeltaValue_Deprecated(float _Value1, float _Value2)
            {
                if (_Value1 > _Value2)
                {
                    return (_Value1 - _Value2) / 2;
                }
            
                return (_Value2 - _Value1) / 2;
            }
            
            private void OnEnable()
            {
                instance = Singleton.Persistent(this);
                
                CheckScreenSize.OnScreenSizeChanged += AdjustCameraSize;
            }

            private void OnDisable()
            {
                CheckScreenSize.OnScreenSizeChanged -= AdjustCameraSize;
            }

            private void OnDrawGizmos()
            {
                if (background == null || camera == null || !showSafeArea) return;
                    // Draws the SafeArea
                    Gizmos.color = Color.green;
                    
                    var _backgroundPosition = background.transform.position;
                    
                    // Left
                    Gizmos.DrawLine(camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMin, _backgroundPosition.z)), 
                                    camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMax, _backgroundPosition.z)));
                    // Top
                    Gizmos.DrawLine(camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMax, _backgroundPosition.z)), 
                                    camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMax, _backgroundPosition.z)));
                    // Right
                    Gizmos.DrawLine(camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMax, _backgroundPosition.z)), 
                                    camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMin, _backgroundPosition.z)));
                    // Bottom
                    Gizmos.DrawLine(camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMin, _backgroundPosition.z)), 
                                    camera.ScreenToWorldPoint(new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMin, _backgroundPosition.z)));
            }
        #endif
    }
}