using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Bu objedeki VE tüm çocuklarındaki renderer'ların gölge dökmesini kapatır.
/// Silah/kol viewmodel'ine tak → fenerin yaptığı dev silah gölgesi sorunu biter.
/// (Silah görünmeye devam eder, sadece GÖLGE dökmez.)
/// </summary>
public class GolgeKapat : MonoBehaviour
{
    void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            r.shadowCastingMode = ShadowCastingMode.Off;
    }
}
