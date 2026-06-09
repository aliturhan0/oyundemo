using UnityEngine;
using System.Collections;

/// <summary>
/// AK-12 viewmodel'i için basit silah kontrolü (legacy Animation component'i ile çalışır).
/// Otomatik ateş (basılı tut), R -> Reload, boştayken Idle.
/// Aim kaynağını (kamera) otomatik bulur — AK-12 kameranın altında olduğu için.
/// </summary>
[RequireComponent(typeof(Animation))]
public class AK12Silah : MonoBehaviour
{
    [Header("Ayarlar")]
    public float menzil = 100f;

    [Tooltip("Raycast'in çıkacağı nokta. Boşsa parent'taki kamera otomatik bulunur.")]
    public Transform nisanKaynagi;

    [Header("Ateş Modu")]
    [Tooltip("Açık = basılı tutunca sürekli ateş (otomatik). Kapalı = her tık 1 atış.")]
    public bool otomatik = true;
    [Tooltip("İki atış arası süre (saniye). 0.1 ≈ 600 RPM.")]
    public float atesHizi = 0.1f;

    [Tooltip("Reload süresi (saniye). 0 = animasyonun kendi uzunluğu. >0 = bu süreye sığdırılır (animasyon hızlanır).")]
    public float reloadSuresi = 0f;

    [Header("Animasyon Klip İsimleri (Animation listesindekiyle birebir aynı olmalı)")]
    public string idleKlip = "Idle";
    public string atesKlip = "Shoot";
    public string reloadKlip = "Reload";

    [Header("Efektler (opsiyonel)")]
    public ParticleSystem namluAtesi;
    public AudioSource silahSesi;

    private Animation _anim;
    private float _sonrakiAtesZamani;
    private bool _reloadEdiliyor;

    void Start()
    {
        _anim = GetComponent<Animation>();

        // KRİTİK: glTFast tüm klipleri "Loop" import ediyor.
        // Idle döngülü kalmalı; ateş/reload TEK SEFER oynayıp bitmeli.
        if (_anim != null)
        {
            if (_anim[idleKlip]   != null) _anim[idleKlip].wrapMode   = WrapMode.Loop;
            if (_anim[atesKlip]   != null) _anim[atesKlip].wrapMode   = WrapMode.Once;
            if (_anim[reloadKlip] != null) _anim[reloadKlip].wrapMode = WrapMode.Once;
        }

        // Aim kaynağı atanmamışsa: parent zincirindeki kamerayı bul (AK-12 kameranın child'ı).
        if (nisanKaynagi == null)
        {
            Camera cam = GetComponentInParent<Camera>();
            nisanKaynagi = (cam != null) ? cam.transform : transform;
        }

        if (_anim != null) _anim.Play(idleKlip);
    }

    void Update()
    {
        if (_anim == null) return;

        // Reload (ateş ederken bile basılınca araya girer)
        if (Input.GetKeyDown(KeyCode.R) && !_reloadEdiliyor)
        {
            StartCoroutine(Reload());
            return;
        }
        if (_reloadEdiliyor) return; // reload sırasında ateş yok

        // Ateş girdisi: otomatik = basılı tut, yarı-otomatik = tek tık
        bool atesGirdisi = otomatik ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (atesGirdisi && Time.time >= _sonrakiAtesZamani)
        {
            _sonrakiAtesZamani = Time.time + atesHizi;
            Ates();
        }
        else if (!Input.GetMouseButton(0) && !_anim.IsPlaying(atesKlip) && !_anim.IsPlaying(reloadKlip))
        {
            // Ateş etmiyoruz ve ateş/reload klibi oynamıyorsa Idle'a dön
            if (!_anim.IsPlaying(idleKlip)) _anim.CrossFade(idleKlip, 0.1f);
        }
    }

    void Ates()
    {
        // Ateş klibini baştan oynat (otomatikte her atışta resetlenir)
        _anim.Stop(atesKlip);
        _anim.Play(atesKlip);

        if (namluAtesi != null) { namluAtesi.Stop(); namluAtesi.Play(); }
        if (silahSesi != null) silahSesi.Play();

        AtesRaycast();
    }

    void AtesRaycast()
    {
        if (Physics.Raycast(nisanKaynagi.position, nisanKaynagi.forward, out RaycastHit hit, menzil))
        {
            Debug.Log("Vuruldu: " + hit.transform.name);
            // İleride zombi hasarı buraya eklenecek:
            // hit.transform.GetComponentInParent<ZombiSagligi>()?.HasarAl(25);
        }
    }

    IEnumerator Reload()
    {
        _reloadEdiliyor = true;

        // İstenen süreye göre animasyon hızını ayarla.
        float klipUzunluk = (_anim[reloadKlip] != null) ? _anim[reloadKlip].length : 2f;
        float hedefSure = (reloadSuresi > 0f) ? reloadSuresi : klipUzunluk;
        if (_anim[reloadKlip] != null && hedefSure > 0f)
            _anim[reloadKlip].speed = klipUzunluk / hedefSure; // kısa süre = hızlı oynat

        _anim.Play(reloadKlip);
        yield return new WaitForSeconds(hedefSure);

        _reloadEdiliyor = false;
        _anim.CrossFade(idleKlip, 0.1f);
    }
}
