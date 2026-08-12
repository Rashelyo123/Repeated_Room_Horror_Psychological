using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    public Color hoverColor = Color.red;
    private Color normalColor;
    public GameObject arrowObject;
    public float hoverScale = 1.1f;
    private Vector3 normalScale;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        normalColor = buttonImage.color;
        normalScale = transform.localScale;

        if (arrowObject != null)
            arrowObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
        transform.localScale = normalScale * hoverScale;

        if (arrowObject != null)
            arrowObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = normalColor;
        transform.localScale = normalScale;

        if (arrowObject != null)
            arrowObject.SetActive(false);
    }
}