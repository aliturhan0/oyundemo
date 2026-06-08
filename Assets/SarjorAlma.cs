using UnityEngine;

public class SarjorAlma : MonoBehaviour
{
    [Header("Ses Efekti")]
    public AudioClip mermiAlmaSesi; // Silah kurma sesini buraya atacağız

    private void OnTriggerEnter(Collider other)
    {
        // Kutuya çarpan şey "Player" ise
        if (other.CompareTag("Player"))
        {
            // Eğer sese bir dosya koyduysan, sesi o anki konumda havaya bırakıp çal!
            if (mermiAlmaSesi != null)
            {
                AudioSource.PlayClipAtPoint(mermiAlmaSesi, transform.position);
            }
            
            Debug.Log("Şarjör Alındı! Mermiler Fullendi!");
            
            // Kutuyu yok et
            Destroy(gameObject);
        }
    }
}