using QueueConnect.Plugins.SoundSystem;
using SoundSystem;
using SoundSystem.Core;
using UnityEngine;

namespace QueueConnect.UISystem.Other
{
    public class MuteButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private Color colorAudioDisabled = Color.gray;
        [SerializeField] private Color colorAudioEnabled = Color.cyan;
        [SerializeField] private SpriteRenderer icon = null;
        [SerializeField] private Sprite iconEnabled = null;
        [SerializeField] private Sprite iconDisabled = null;
    
        private void Awake()
        {
            icon.color = AudioSystem.IsMuted ? colorAudioDisabled : colorAudioEnabled;
            icon.sprite = AudioSystem.IsMuted ? iconDisabled : iconEnabled;
            AudioSystem.OnAudioMuted += OnAudioDisabled;
            AudioSystem.OnAudioUnmuted += OnAudioEnabledIcon;
        }

        public void OnMuteButtonPressed()
        {
            AudioSystem.ToggleAudioOnOff();
        }

        private void OnAudioDisabled()
        {
            icon.color = colorAudioDisabled;
            icon.sprite = iconDisabled;
        }
        
        private void OnAudioEnabledIcon()
        {
            icon.color = colorAudioEnabled;
            icon.sprite = iconEnabled;
            AudioSystem.PlayVFX(VFX.OnAudioUnmute);
            AudioSystem.PlayVFX(VFX.UIPartButtonPressedSuccess);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            icon.color = AudioSystem.IsMuted ? colorAudioDisabled : colorAudioEnabled;
        }

#endif
    }
}
