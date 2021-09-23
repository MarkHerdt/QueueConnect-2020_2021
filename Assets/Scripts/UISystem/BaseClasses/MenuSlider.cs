using Ganymed.UISystem;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.UISystem.BaseClasses
{
    [RequireComponent(typeof(Animator))]
    public abstract class MenuSlider<T> : Menu<T> where T : MenuSlider<T>
    {
        private Animator animator = null;
        private readonly int close = Animator.StringToHash("Close");
        private readonly int open = Animator.StringToHash("Open");

        public static void Open()
        {
            Initialize();
        }

        public static void Close()
        {
            Terminate();
        }


        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }


        public void OnMenuClosedCallback()
        {
            Terminate();
        }
    
        public void OnMenuOpenedCallback()
        {
        }
    
        public override void OpenInstance()
        {
            base.OpenInstance();
            animator.ResetTrigger(close);
            animator.SetTrigger(open);
            AudioSystem.PlayVFX(VFX.UIMenuOpenSlider);
        }

        public override void CloseInstance()
        {
            animator.ResetTrigger(open);
            animator.SetTrigger(close);
            AudioSystem.PlayVFX(VFX.UIMenuCloseSlider);
        }
    }
}