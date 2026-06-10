using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dürbün / nişan alma (ADS) + tam ekran dürbün UI arayüzü.
/// Sağ tıkla (basılı tut) -> kamera zoom (FOV düşer) + tam ekran dürbün görseli açılır
/// + silah modeli gizlenir + crosshair gizlenir. Bırakınca her şey eski haline döner.
///
/// KURULUM (çok basit):
/// 1) Bu scripti SİLAH objesine (AK-12) tak.
/// 2) Assets/Sprites/DurbunOverlay.png'i seç -> Inspector'da Texture Type = "Sprite (2D and UI)" yap -> Apply.
/// 3) O sprite'ı bu scriptteki "Durbun Gorseli" alanına sürükle.
/// Canvas/Image'i ELLE KURMANA GEREK YOK — script oyun başında otomatik oluşturur.
/// </summary>
public class Durbun : MonoBehaviour
{
    [Header("Tuş")]
    [Tooltip("Sağ tık = 1. Basılı tutunca dürbün açık.")]
    public int aimMouseButton = 1;
    [Tooltip("Açık = basılı tutunca aim. Kapalı = her tıkta aç/kapa (toggle).")]
    public bool basiliTut = true;

    [Header("Zoom (FOV)")]
    [Tooltip("Aim'deyken kamera FOV. Düşük = daha çok zoom. Normal FOV oyun başında otomatik alınır.")]
    public float aimFOV = 28f;
    [Tooltip("Zoom geçiş hızı. Büyük = hızlı.")]
    public float zoomHizi = 12f;

    [Header("Dürbün UI (Scope Overlay)")]
    [Tooltip("Tam ekran dürbün görseli. Assets/Sprites/DurbunOverlay.png'i Sprite yapıp buraya sürükle.")]
    public Sprite durbunGorseli;
    [Tooltip("Aim'deyken silah modelini gizle (dürbün görüntüsünü kapatmasın).")]
    public bool aimdaSilahiGizle = true;
    [Tooltip("Aim'deyken gizlenecek crosshair/nişangâh UI objesi (opsiyonel).")]
    public GameObject gizlenecekCrosshair;

    [Header("Silah Pozisyonu (opsiyonel)")]
    [Tooltip("Silahın aim'deyken duracağı yer (silahın KARDEŞİ boş obje). Boşsa silah hareket etmez. UI gizleme açıksa gerekmez.")]
    public Transform nisanPozisyonu;
    [Tooltip("Silahın hip<->aim geçiş hızı.")]
    public float silahHareketHizi = 12f;

    private Camera _cam;
    private float _normalFOV;
    private Vector3 _hipLocalPos;
    private Quaternion _hipLocalRot;
    private bool _aimAcik;

    private GameObject _overlay;          // runtime oluşturulan dürbün görseli
    private Renderer[] _silahRenderlari;  // aim'de gizlenecek silah parçaları

    void Start()
    {
        // Kamerayı parent zincirinde bul (silah kameranın child'ı).
        _cam = GetComponentInParent<Camera>();
        if (_cam == null) _cam = Camera.main;
        if (_cam != null) _normalFOV = _cam.fieldOfView;

        // Silahın başlangıç (hip) pozisyonu.
        _hipLocalPos = transform.localPosition;
        _hipLocalRot = transform.localRotation;

        // Aim'de gizlenecek silah render'ları.
        _silahRenderlari = GetComponentsInChildren<Renderer>();

        // Dürbün UI'ını otomatik oluştur.
        if (durbunGorseli != null) OverlayOlustur();
    }

    void Update()
    {
        // Aim girdisi
        if (basiliTut)
            _aimAcik = Input.GetMouseButton(aimMouseButton);
        else if (Input.GetMouseButtonDown(aimMouseButton))
            _aimAcik = !_aimAcik;

        // 1) FOV zoom
        if (_cam != null)
        {
            float hedefFOV = _aimAcik ? aimFOV : _normalFOV;
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, hedefFOV, Time.deltaTime * zoomHizi);
        }

        // 2) Dürbün UI overlay
        if (_overlay != null && _overlay.activeSelf != _aimAcik)
            _overlay.SetActive(_aimAcik);

        // 3) Aim'de silah modelini gizle
        if (aimdaSilahiGizle && _silahRenderlari != null)
        {
            foreach (var r in _silahRenderlari)
                if (r != null && r.enabled == _aimAcik) r.enabled = !_aimAcik;
        }

        // 4) Aim'de crosshair gizle
        if (gizlenecekCrosshair != null && gizlenecekCrosshair.activeSelf == _aimAcik)
            gizlenecekCrosshair.SetActive(!_aimAcik);

        // 5) Silah pozisyonu (nisan pozisyonu atanmışsa)
        if (nisanPozisyonu != null)
        {
            Vector3 hedefPos = _aimAcik ? nisanPozisyonu.localPosition : _hipLocalPos;
            Quaternion hedefRot = _aimAcik ? nisanPozisyonu.localRotation : _hipLocalRot;
            transform.localPosition = Vector3.Lerp(transform.localPosition, hedefPos, Time.deltaTime * silahHareketHizi);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, hedefRot, Time.deltaTime * silahHareketHizi);
        }
    }

    /// <summary>Tam ekran dürbün Canvas+Image'ini runtime'da kurar (editör kurulumu gerekmez).</summary>
    void OverlayOlustur()
    {
        GameObject canvasGO = new GameObject("DurbunCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // her şeyin üstünde
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        _overlay = new GameObject("DurbunImage");
        _overlay.transform.SetParent(canvasGO.transform, false);
        Image img = _overlay.AddComponent<Image>();
        img.sprite = durbunGorseli;
        img.raycastTarget = false;          // tıklamaları engellemesin
        img.preserveAspect = false;         // tüm ekranı kaplasın

        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _overlay.SetActive(false);
    }
}
