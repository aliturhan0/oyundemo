using UnityEngine;

public class GerilimYonetimi : MonoBehaviour
{
    public AudioSource arkaPlanSesi;
    public AudioSource gerilimSesi;

    public float baslamaMesafesi = 20f;
    public float maxGerilimMesafesi = 5f;

    void Update()
    {
        GameObject[] zombiler = GameObject.FindGameObjectsWithTag("Zombie");
        float enYakinMesafe = Mathf.Infinity;

        foreach (GameObject zombi in zombiler)
        {
            float mesafe = Vector3.Distance(transform.position, zombi.transform.position);
            if (mesafe < enYakinMesafe)
            {
                enYakinMesafe = mesafe;
            }
        }

        if (enYakinMesafe <= baslamaMesafesi)
        {
            float gerilimOrani = 1f - ((enYakinMesafe - maxGerilimMesafesi) / (baslamaMesafesi - maxGerilimMesafesi));
            gerilimOrani = Mathf.Clamp01(gerilimOrani);

            if(gerilimSesi != null) 
                gerilimSesi.volume = gerilimOrani;
            
            if(arkaPlanSesi != null) 
                arkaPlanSesi.volume = 0.5f - (gerilimOrani * 0.3f); 
        }
        else
        {
            if(gerilimSesi != null) 
                gerilimSesi.volume = Mathf.Lerp(gerilimSesi.volume, 0f, Time.deltaTime);
            
            if(arkaPlanSesi != null) 
                arkaPlanSesi.volume = Mathf.Lerp(arkaPlanSesi.volume, 0.5f, Time.deltaTime);
        }
    }
}