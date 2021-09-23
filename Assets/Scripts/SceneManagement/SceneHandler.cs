using System.Collections;
using System.Threading.Tasks;
using QueueConnect.Plugins.SoundSystem;
using Sirenix.OdinInspector;
using SoundSystem;
using SoundSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable UseFormatSpecifierInInterpolation

namespace QueueConnect.SceneManagement
{
    public class SceneHandler : MonoBehaviour
    {
        [Title("Loading")]
        [SerializeField] private TMP_Text progressText = default;
        [SerializeField] private Image progressBar = default;

        private void Awake() => LoadMainSceneAsync();

        private async void LoadMainSceneAsync()
        {
            await Task.Delay(500);
            
            // Load scene
            var asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            while (!asyncOperation.isDone)
            {
                var progress = (asyncOperation.progress / 2f);
                SetProgress(progress);
                await Task.Delay(25);
            }
            
            progressBar.fillAmount = asyncOperation.progress;
            
            // Await scene initialization
            while (!InitializationHelper.IsDone)
            {
                var progress = .5f + InitializationHelper.Progress;
                SetProgress(progress);
                await Task.Delay(25);
            }

            // Unload loading scene.
            SceneManager.UnloadSceneAsync(0);
        }

        private void SetProgress(float progress)
        {
            progressText.text = $"Loading: {(progress * 100f).ToString("00")}%";
            progressBar.fillAmount = progress;
        }
        
    }
}
