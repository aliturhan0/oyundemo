using UnityEngine;

/// <summary>
/// Gelişmiş kafa (kamera) bob'u: HIZA göre ölçeklenir (yürü/koş farkı hissedilir) +
/// figure-8 deseni (doğal adım ritmi: dikey, yatayın 2 katı frekansta).
/// Kameranın ÜSTÜNE takılır. Sadece localPosition'a offset ekler — dönüşe (mouse look) karışmaz.
/// </summary>
public class KameraBob : MonoBehaviour
{
    [Header("Hız Tepkisi")]
    [Tooltip("Bu hızda bob tam güçte (koşma hızı civarı).")]
    public float maxHiz = 6f;
    [Tooltip("Bu hızın altında bob yok (durma).")]
    public float hareketEsigi = 0.3f;

    [Header("Pozisyon Bob")]
    [Tooltip("Yukarı-aşağı miktar (asıl hissedilen).")]
    public float dikeyMiktar = 0.06f;
    [Tooltip("Yana miktar.")]
    public float yatayMiktar = 0.04f;
    [Tooltip("Temel adım frekansı.")]
    public float bobFrekans = 7f;

    [Header("Yumuşaklık")]
    public float yumusaklik = 12f;

    private Vector3 _baslangicPos;
    private Vector3 _offset;
    private float _bobZaman;
    private Transform _oyuncu;
    private Vector3 _sonPos;

    void Start()
    {
        _baslangicPos = transform.localPosition;
        _oyuncu = transform.root;          // oyuncu kökü — yürürken yer değiştirir
        _sonPos = _oyuncu.position;
    }

    void LateUpdate()
    {
        // --- Gerçek yatay hız ---
        Vector3 delta = _oyuncu.position - _sonPos;
        delta.y = 0f;
        float hiz = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        _sonPos = _oyuncu.position;

        // Hız oranı 0..1 (yürü ~0.5, koş ~1) — "koşu hissi" bundan gelir
        float t = Mathf.Clamp01(hiz / maxHiz);

        Vector3 hedef = Vector3.zero;
        if (hiz > hareketEsigi)
        {
            // Frekans hızla artar: koşunca adımlar sıklaşır
            _bobZaman += Time.deltaTime * bobFrekans * Mathf.Lerp(0.7f, 1.7f, t);

            // Figure-8: dikey, yatayın 2 KATI frekansta (gerçek yürüyüş ritmi)
            float yatay = Mathf.Sin(_bobZaman);
            float dikey = Mathf.Cos(_bobZaman * 2f);

            // Genlik de hızla büyür (t ile çarpılır)
            hedef = new Vector3(yatay * yatayMiktar, dikey * dikeyMiktar, 0f) * t;
        }
        else
        {
            _bobZaman = 0f;
        }

        _offset = Vector3.Lerp(_offset, hedef, Time.deltaTime * yumusaklik);
        transform.localPosition = _baslangicPos + _offset;
    }
}
