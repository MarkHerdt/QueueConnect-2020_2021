using Ganymed.UISystem;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.UISystem
{
    public class MenuOpenCloseAnimation<T> : Menu<T> where T : MenuOpenCloseAnimation<T>
    {
        protected Animator animator = null;
        private readonly int close = Animator.StringToHash("Close");
        private readonly int open = Animator.StringToHash("Open");

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }
    
        public static void Open()
        {
            Initialize();
        }

        public static void Close()
        {
            Terminate();
        }
    

        public virtual void OnMenuClosedCallback()
        {
            Close();
        }
    
        public virtual void OnMenuOpenedCallback()
        {
        }
    
        public override void OpenInstance()
        {
            base.OpenInstance();
            animator.SetTrigger(open);
            AudioSystem.PlayVFX(VFX.UIMenuOpenDigital);
        }

        public override void CloseInstance()
        {
            animator.SetTrigger(close);
        }
    }
}