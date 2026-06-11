using UnityEngine;

public class UyduTakip : MonoBehaviour
{
    [Header("Takip Edilecek Obje")]
    public Transform hedefVaril;
    
    [Header("Radar Yüksekliği")]
    public float radarYuksekligi = 480f;

    void LateUpdate()
    {
        if (hedefVaril != null)
        {
            // Yüksekliği 480'de sabitle, sağa sola gidişlerde varili takip et!
            transform.position = new Vector3(hedefVaril.position.x, radarYuksekligi, hedefVaril.position.z);
            
            // Fizik hatalarına karşı ikonu hep gökyüzüne bakar şekilde kilitle
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}