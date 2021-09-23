using System;
using Ganymed.UISystem;
using QueueConnect.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace QueueConnect.UISystem.Prefabs
{
    public class UpperRightCornerButtonBehaviour : MonoBehaviour
    {
        public static event Action OnButtonPressed;
    
        [SerializeField] private Button button = null;
        [SerializeField] private SpriteRenderer spriteRenderer = null;

        [SerializeField] private Sprite quitTexture = null;
        [SerializeField] private Sprite backTexture = null;
        [SerializeField] private Sprite pauseTexture = null;
        [SerializeField] private Sprite playTexture = null;
    
        private void Awake()
        {
            button.onClick.AddListener(ButtonPressed);
            GameController.OnGamePausePlay += OnGamePausePlay;
            MenuManager.onMenuOpened += (menu) => SetTexture(GameController.IsPaused ? playTexture : menu is MenuMain? quitTexture : backTexture);
            MenuManager.onAllMenusClosed += () =>
                SetTexture(GameController.GameState == GameState.Playing ? pauseTexture : quitTexture);
        }

        private void OnGamePausePlay(bool isPlaying)
        {
            SetTexture(isPlaying ? playTexture : pauseTexture);
        }

        private void ButtonPressed()
        {
            OnButtonPressed?.Invoke();
            MenuManager.InvokeOnBackPressed();
        }

        private void SetTexture(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    
    
    }
}
