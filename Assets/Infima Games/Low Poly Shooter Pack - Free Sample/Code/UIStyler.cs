using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Herhangi bir UI Button objesine ekleyerek güzel hover/press animasyonu kazandırır.
/// Inspector'dan Normal ve Highlighted sprite'ları atayın.
/// </summary>
[RequireComponent(typeof(Button))]
public class UIStyler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite hoverSprite;

    [Header("Scale Animation")]
    public float hoverScale  = 1.06f;
    public float pressScale  = 0.94f;
    public float animSpeed   = 12f;

    [Header("Text Glow (opsiyonel)")]
    [Tooltip("Butondaki TMPro veya Text bileşeni (boş bırakabilirsiniz).")]
    public Graphic buttonText;
    public Color normalTextColor    = Color.white;
    public Color hoverTextColor     = new Color(1f, 0.85f, 0.3f); // Altın sarısı

    private Image  _image;
    private Vector3 _targetScale;
    private Vector3 _originalScale;

    private void Awake()
    {
        _image         = GetComponent<Image>();
        _originalScale = transform.localScale;
        _targetScale   = _originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.unscaledDeltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        _targetScale = _originalScale * hoverScale;
        if (hoverSprite  != null) _image.sprite = hoverSprite;
        if (buttonText   != null) buttonText.color = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        _targetScale = _originalScale;
        if (normalSprite != null) _image.sprite = normalSprite;
        if (buttonText   != null) buttonText.color = normalTextColor;
    }

    public void OnPointerDown(PointerEventData e)
    {
        _targetScale = _originalScale * pressScale;
    }

    public void OnPointerUp(PointerEventData e)
    {
        _targetScale = _originalScale * hoverScale;
    }
}
