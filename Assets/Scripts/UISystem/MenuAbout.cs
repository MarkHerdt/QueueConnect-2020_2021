using Ganymed.UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem
{
    public class MenuAbout : MenuOpenCloseAnimation<MenuAbout>
    {
        [SerializeField] private Button backButton = default;

        protected override void Awake()
        {
            base.Awake();
            backButton.onClick.AddListener(() => MenuManager.InvokeOnBackPressed());
        }
    }
}