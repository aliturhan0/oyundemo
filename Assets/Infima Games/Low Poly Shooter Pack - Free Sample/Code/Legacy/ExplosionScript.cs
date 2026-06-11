using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	[Header("Customizable Options")]
	//How long before the explosion prefab is destroyed
	public float despawnTime = 10.0f;
	//How long the light flash is visible
	public float lightDuration = 0.02f;
	[Header("Light")]
	public Light lightFlash;

	[Header("Audio")]
	public AudioClip[] explosionSounds;
	public AudioSource audioSource;

	private void Start () {
		//Start the coroutines
		StartCoroutine (DestroyTimer ());
		StartCoroutine (LightFlash ());

		if (audioSource != null && explosionSounds != null && explosionSounds.Length > 0)
		{
			//Get a random impact sound from the array
			audioSource.clip = explosionSounds[Random.Range(0, explosionSounds.Length)];
			// Sesi 2D (Global) yaparak mesafe sorununu coz ve sesi son ses (1.0) yap
			audioSource.spatialBlend = 0.0f;
			audioSource.volume = 1.0f;
			//Play the random explosion sound
			audioSource.Play();
		}
	}

	private IEnumerator LightFlash () {
		//Show the light
		lightFlash.GetComponent<Light>().enabled = true;
		//Wait for set amount of time
		yield return new WaitForSeconds (lightDuration);
		//Hide the light
		lightFlash.GetComponent<Light>().enabled = false;
	}

	private IEnumerator DestroyTimer () {
		//Destroy the explosion prefab after set amount of seconds
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
}