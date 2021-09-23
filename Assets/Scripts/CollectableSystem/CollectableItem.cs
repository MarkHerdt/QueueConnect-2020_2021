using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.Ganymed.Localization;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem;
using SoundSystem.Core;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
// ReSharper disable Unity.InefficientPropertyAccess

namespace QueueConnect.CollectableSystem
{
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public abstract class CollectableItem : MonoBehaviour, ICooldown, ILocalizationCallback
    {

        #region --- [INSPECTOR] ---

        #pragma warning disable 649
        [SerializeField] private TMP_Text itemNameDisplay;
        #pragma warning restore 649    
        [SerializeField] private ItemID itemID = ItemID.None;
        
        [SerializeField] private bool useCooldown = false;
        [ShowIf("@useCooldown == true")] [SerializeField] [MinValue(0f)]
        private float cooldown = 30f;

        
        [SerializeField] private bool useInitialCooldown = false;
        [ShowIf("@useInitialCooldown == true")] [MinValue(0f)]
        [SerializeField] private float initialCooldown = 30f;
        
        [Space]
        [SerializeField] 
        private uint spawnValue = 0;

#if UNITY_EDITOR
        [SerializeField] [ReadOnly]
        private float spawnPercentage;
#endif

        [Space]
        [FormerlySerializedAs("icon")]
        [FoldoutGroup("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [FoldoutGroup("Components")]
        [SerializeField] private Transform bottom = null;
        [FoldoutGroup("Components")]
        [SerializeField] private Image targetImage = null;
        
        [Space]
        [Required]
        [SerializeField] private Animation pulsingAnimation = null;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [PROPERTIES] ---
        
        public ItemID ItemID => itemID;

        public bool UseCooldown => useCooldown;

        public uint SpawnValue => spawnValue;

        public bool UseInitialCooldown => useInitialCooldown;

        public float InitialCooldown => initialCooldown;

        public bool Interactable { get; private set; }

        public bool CanSpawn
        {
            get => canSpawn && CheckItemSpawnCondition();
            private set => canSpawn = value;
        }
        
        public bool DespawnRequested { get; private set; }
        
        private bool EnableMovement { get; set; }
        
        
        public State ItemState { get; private set; } 
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [EVENTS] ---

        public static event Action<CollectableItem> OnItemCollectedEvent; 

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [FIIELDS] ---

        public enum State
        {
            Idle = 0,
            Falling = 1,
            Grounded = 2,
            Cooldown = 3,
        }

        private static int FallingKey
        {
            get
            {
                switch (@animationIndex)
                {
                    case 0:
                        animationIndex++;
                        return Falling1;
                    case 1:
                        animationIndex++;
                        return Falling2;
                    case 2:
                        animationIndex = 0;
                        return Falling3;
                }

                throw new Exception("Index out of Range");
            }
        }
        
        private static int animationIndex = 0;
        
        private static readonly int Falling1 = Animator.StringToHash("Falling_1");
        private static readonly int Falling2 = Animator.StringToHash("Falling_2");
        private static readonly int Falling3 = Animator.StringToHash("Falling_3");
        private static readonly int PickupKey = Animator.StringToHash("Pickup");
        private static readonly int PickupFailKey = Animator.StringToHash("PickupFail");
        private static readonly int DespawnKey = Animator.StringToHash("Despawn");
        private static readonly int EmptyKey = Animator.StringToHash("Empty");

        private bool isPlaying = false;
        private bool canSpawn = true;
        private Transform self = null;
        private Rigidbody selfRigidbody = null;
        private Vector3 cachedVelocity;
        
        private readonly Vector3 rightVector = Vector3.right;
        private readonly Vector3 downVector = Vector3.down;
        
        private volatile float yPosition;
        private volatile float xPosition;
        
        private float fallSpeed = 1;
        private CancellationTokenSource cts = new CancellationTokenSource();
        
        private Animator animationController = null;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            animationController = GetComponent<Animator>();
            selfRigidbody = GetComponent<Rigidbody>();
            self = transform;
            
            LocalizationManager.AddCallbackListener(this);
            
            EventController.OnGameEnded -= EventControllerOnOnGameEnded;
            EventController.OnGameEnded += EventControllerOnOnGameEnded;
            
            GameController.OnGameStateChanged += delegate(GameState state)
            {
                isPlaying = state == GameState.Playing;
                if (!isPlaying)
                {
                    cachedVelocity = selfRigidbody.velocity;
                    selfRigidbody.velocity = Vector3.zero;
                }
                else
                {
                    selfRigidbody.velocity = cachedVelocity;
                }
            };
            
            GetComponentInChildren<Button>().onClick.AddListener(Collect);
            gameObject.SetActive(false);
        }

        private void EventControllerOnOnGameEnded(bool wasAborted)
        {
            CooldownHandler.CancelCooldown(this);
            animationController.SetTrigger(DespawnKey);
        }

        //--------------------------------------------------------------------------------------------------------------

        #region --- [ABSTRACT] ---
        
        protected virtual bool CheckItemSpawnCondition() => true;
        
        /// <summary>
        /// Try activate the items individual effect. Return false if the effect cannot be used.
        /// </summary>
        /// <returns></returns>
        protected abstract bool Use();

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [LOGIC] ---

        /// <summary>
        /// This method is called when the player "collects" the item.
        /// </summary>
        private void Collect()
        {
            if(!Interactable || !isPlaying) return;
            Interactable = false;
            
            if (Use())
            {
                cts.Cancel();
                AudioSystem.PlayVFX(VFX.OnItemCollected);
                animationController.SetTrigger(PickupKey);
                spriteRenderer.enabled = true;
                targetImage.gameObject.SetActive(false);
                StopMovement();
                OnItemCollectedEvent?.Invoke(this);
            }
            else
            {
                SetItemIntractable(false);
                if(DespawnRequested) return;
                AudioSystem.PlayVFX(VFX.OnItemNotCollectableSound);
                animationController.SetTrigger(PickupFailKey);
                Interactable = true;
            }
        }
        
        public void Spawn(ItemSpawn spawn)
        {
            #region --- [RESET VALUES] ---

            CanSpawn = false;
            Interactable = true;
            DespawnRequested = false;
            SetItemIntractable(true);
            gameObject.SetActive(true);
            StopMovement();

            cts = new CancellationTokenSource();
            
            self.localScale = Vector3.one;
            self.position = spawn.StartPosition;
            CacheVerticalPosition();
            spriteRenderer.transform.localScale = Vector3.one;
            spriteRenderer.transform.localPosition = Vector3.zero;
            spriteRenderer.enabled = true;
            animationController.enabled = true;
            targetImage.gameObject.SetActive(true);

            #endregion
            
            StartMovement(State.Falling);
            
            animationController.SetTrigger(FallingKey);
            AwaitConveyorEntered();
        }

                
        private void Despawn()
        {
            
            try
            {
                animationController.enabled = false;
                self.localScale = Vector3.one;
                spriteRenderer.transform.localScale = Vector3.one;
                spriteRenderer.transform.localPosition = Vector3.zero;

                cts.Cancel();
                Interactable = false;
                gameObject.SetActive(false);

                if (UseCooldown)
                {
                    CooldownHandler.StartCooldown(this);
                }
            
                CanSpawn = true;
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                return;
            }
        }
        
        
        private async void AwaitConveyorEntered()
        {
            try
            {
                if (!await TaskHelper.AwaitConditionSuccess(() => yPosition > ItemConveyor.Level, cts.Token)) return;
            }
            catch
            {
                return;
            }
            animationController.SetTrigger(EmptyKey);
            ItemState = State.Grounded;

            var rawPos = self.position;
            var finePos = new Vector3(rawPos.x, ItemConveyor.Level + Vector3.Distance(rawPos, bottom.position), rawPos.z);
            self.transform.position = finePos;
            AwaitScreenExit();
        }

        private async void AwaitScreenExit()
        {
            try
            {
                if (!await TaskHelper.AwaitConditionSuccess(() => xPosition < ItemDespawn.Threshold, cts.Token)) return;
            }
            catch
            {
                return;
            }

            Despawn();
        }

        private void Update()
        {
            CacheVerticalPosition();
        }

        private void CacheVerticalPosition()
        {
            var position = bottom.position;
            yPosition = position.y;
            xPosition = position.x;
        }


        private void FixedUpdate() => MoveItem();
        
        private void MoveItem()
        {
            if (!isPlaying || !EnableMovement) return;
            switch (ItemState)
            {
                case State.Falling:
                    MovementFalling();
                    break;
                
                case State.Grounded:
                    MovementConveyor();
                    break;
            }
        }
        
        private void MovementFalling()
        {
            selfRigidbody.velocity = downVector * (GameController.GetCurrentSpeed() * fallSpeed);
            fallSpeed += (Time.fixedDeltaTime * 2);
        }

        private void MovementConveyor()
        {
            selfRigidbody.velocity = rightVector * GameController.GetCurrentSpeed();
        }
        

        protected void SetItemIntractable(bool active)
        {
            targetImage.gameObject.SetActive(active);
            spriteRenderer.color = active? Color.white : Color.grey;
        }
        
        private void StopMovement()
        {
            EnableMovement = false;
            selfRigidbody.velocity = Vector3.zero;
            fallSpeed = 1f;
        }

        private void StartMovement(State state)
        {
            EnableMovement = true;
            ItemState = state;
        }
        

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [COOLDOWN INTERFACE] ---

        public event Action onCooldownEnd;
        public event Action onCooldownStart;
        public event Action onCooldownCanceled;

        public string Name => itemID.ToString();
        
        public virtual float Duration => cooldown;


        public void OnCooldownChanged(float oldValue, float newValue)
        {
        }

        public virtual void OnCooldownEnd()
        {
            onCooldownEnd?.Invoke();
        }

        public virtual void OnCooldownStart()
        {
            onCooldownStart?.Invoke();
        }

        public virtual void OnCooldownCancel()
        {
            onCooldownCanceled?.Invoke();
        }
       
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [EDITOR] ---

#if UNITY_EDITOR
        
        private static readonly List<CollectableItem> items = new List<CollectableItem>();


        protected CollectableItem()
        {
            AddToList();
        }

        private async void AddToList()
        {
            await Task.Delay(100).BreakStack();
            if(!this || gameObject == null) return;
            if(PrefabUtility.IsPartOfPrefabAsset(gameObject))
                items.Add(this);
        }
        

        private void OnDestroy()
        {
            if(!items.Contains(this)) return;
            items.Remove(this);
        }

        private void OnValidate()
        {
            var spawnValueSum = items.Aggregate<CollectableItem, uint>(0, (current, available)=> current + available.SpawnValue);

            foreach (var item in items)
            {
                item.spawnPercentage = (float)item.spawnValue / (float)spawnValueSum * (float)100;
            }
        }
        
#endif
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        public void PlayPulsingAnimation()
        {
            pulsingAnimation.Stop();
            pulsingAnimation.Play();
        }

        public virtual void OnLanguageLoaded(Language language)
        {
            itemNameDisplay.text = LocalizationManager.GetText($"m_{itemID.ToString().ToLower()}");
        }
    }
}