using UnityEngine;
using UnityEditor;

/// <summary>
/// Seçili objedeki (ve TÜM çocuklarındaki) "Missing Script" component'lerini temizler.
/// Kullanım: Hierarchy'de objeyi seç -> üst menü: Tools > Eksik Scriptleri Temizle.
/// </summary>
public static class EksikScriptTemizle
{
    [MenuItem("Tools/Eksik Scriptleri Temizle (Secili + Cocuklar)")]
    static void Temizle()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("Önce Hierarchy'de bir obje seç (ör. Player kökü).");
            return;
        }

        int toplam = 0;
        foreach (var secili in Selection.gameObjects)
        {
            // Obje + tüm çocuklar (pasif olanlar dahil)
            var hepsi = secili.GetComponentsInChildren<Transform>(true);
            foreach (var t in hepsi)
            {
                int adet = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                if (adet > 0)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                    toplam += adet;
                    Debug.Log($"Temizlendi: '{t.name}' -> {adet} eksik script kaldırıldı.");
                    EditorUtility.SetDirty(t.gameObject);
                }
            }
        }

        Debug.Log(toplam > 0
            ? $"BİTTİ. Toplam {toplam} eksik script kaldırıldı. Artık prefab'ı kaydedebilirsin."
            : "Seçili objede eksik script bulunamadı.");
    }
}
