using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The light component to toggle.")]
    public Light flashlightLight;
    
    [Tooltip("Key used to toggle the flashlight.")]
    public KeyCode toggleKey = KeyCode.F;

    [Tooltip("Optional audio source for click sound.")]
    public AudioSource clickAudioSource;

    private void Start()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponent<Light>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (flashlightLight != null)
            {
                flashlightLight.enabled = !flashlightLight.enabled;
                
                if (clickAudioSource != null)
                {
                    clickAudioSource.Play();
                }
            }
        }
    }
}
