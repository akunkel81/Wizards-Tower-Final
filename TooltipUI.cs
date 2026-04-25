using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private RectTransform backgroundRectTransform;

    public static TooltipUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate(); // Force text update to get correct size

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 padding = new Vector2(10f, 6f);
        backgroundRectTransform.sizeDelta = textSize + padding;
    }

    private void Update()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        // Keep the tooltip within screen bounds
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;

        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
    }

    public void ShowTooltip(string tooltipText)
    {
        gameObject.SetActive(true);
        SetText(tooltipText);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }


}
