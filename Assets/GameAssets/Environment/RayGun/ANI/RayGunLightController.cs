using QueueConnect.Environment;
using UnityEngine;

public class RayGunLightController : MonoBehaviour
{
    [SerializeField] private Animation lightAnimation = null;
    
    private void OnEnable()
    {
        RayGun.OnRayGunFire -= OnRayGunFire;
        RayGun.OnRayGunFire += OnRayGunFire;
    }

    private void OnDisable()
    {
        RayGun.OnRayGunFire -= OnRayGunFire;
    }

    private void OnRayGunFire()
    {
        lightAnimation.Play();
    }
}
