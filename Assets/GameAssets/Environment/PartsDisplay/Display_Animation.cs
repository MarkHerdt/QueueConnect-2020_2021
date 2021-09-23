using System.Threading.Tasks;
using UnityEngine;

public sealed class Display_Animation : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] private bool randomize;
    #pragma warning restore 649
    [SerializeField] [Range(100,100000)]private int randomizationIntervalInMilliseconds = 3000;
    [SerializeField] [Range(.5f, 1f)] private float minAnimationSpeed = .9f;
    [SerializeField] [Range(1f, 2f)] private float maxAnimationSpeed = 1.2f;
    
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        randomizationIntervalInMilliseconds = Mathf.Abs(randomizationIntervalInMilliseconds);
        RandomizeAnimationSpeed();
    }

    private async void RandomizeAnimationSpeed()
    {
        while (Application.isPlaying && randomize)
        {
            animator.speed = UnityEngine.Random.Range(minAnimationSpeed, maxAnimationSpeed);
            await Task.Delay(randomizationIntervalInMilliseconds);
        }
    }
}
