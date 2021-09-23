using Ganymed.UISystem;
using QueueConnect.GameSystem;
using QueueConnect.Plugins.SoundSystem;
using SoundSystem.Core;
using UnityEngine.Profiling;

namespace QueueConnect.UISystem
{
    public class MenuManagerQueueConnect : MenuManager
    {
        
        protected override void Awake()
        {
            base.Awake();
            EventController.OnGameEnded += endedByPlayer =>
            {
                if(!endedByPlayer) MenuPostGame.Open();
            };
        }
    
        protected override void OnMenuOpened(Menu menu)
        {
            AudioSystem.PlayVFX(VFX.UIMenuButtonPressedSuccess);
            GameController.SetGameState(GameState.Menu);
            EventController.MenuOpened();
        }

        protected override void OnAllMenusClosed()
        {
            EventController.AllMenusClosed();
        }
    }
}
