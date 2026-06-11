using UnityEngine;

/// <summary>
/// Ekranın sol üstünde canlı FPS + ms gösterir. Optimizasyon ölçmek için.
/// KULLANIM: Sahnede boş bir GameObject oluştur (adı "FPS"), bu scripti ona tak. Play'e bas.
/// </summary>
public class FPSSayaci : MonoBehaviour
{
    [Tooltip("Kaç saniyede bir güncellensin (yumuşak okuma).")]
    public float guncellemeAraligi = 0.5f;

    [Tooltip("Yazı boyutu.")]
    public int yaziBoyutu = 28;

    [Tooltip("Sayaç ekranın üstünden ne kadar aşağıda olsun (piksel).")]
    public float usttenBosluk = 80f;

    [Tooltip("Sayaç soldan ne kadar içeride olsun (piksel).")]
    public float soldanBosluk = 8f;

    private float _sure;
    private int _kareSayisi;
    private float _fps;
    private float _ms;
    private GUIStyle _stil;

    void Update()
    {
        _kareSayisi++;
        _sure += Time.unscaledDeltaTime;

        if (_sure >= guncellemeAraligi)
        {
            _fps = _kareSayisi / _sure;
            _ms = (_sure / _kareSayisi) * 1000f;
            _kareSayisi = 0;
            _sure = 0f;
        }
    }

    void OnGUI()
    {
        if (_stil == null)
        {
            _stil = new GUIStyle(GUI.skin.label);
            _stil.fontSize = yaziBoyutu;
            _stil.fontStyle = FontStyle.Bold;
        }

        // 60+ yeşil, 30-60 sarı, altı kırmızı.
        _stil.normal.textColor = _fps >= 60f ? Color.green : (_fps >= 30f ? Color.yellow : Color.red);

        float x = soldanBosluk;
        float y = usttenBosluk;

        // Hafif arka plan (okunsun diye)
        GUI.color = new Color(0f, 0f, 0f, 0.5f);
        GUI.DrawTexture(new Rect(x, y, 230, yaziBoyutu + 14), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.Label(new Rect(x + 6, y + 4, 400, yaziBoyutu + 10),
            string.Format("FPS: {0:0}  ({1:0.0} ms)", _fps, _ms), _stil);
    }
}
