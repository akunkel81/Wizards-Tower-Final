using UnityEngine;

public class PopupText : MonoBehaviour
{
    public float visibleTime = 15f;

    void Start()
    {
        gameObject.SetActive(true);
        Invoke(nameof(HidePopup), visibleTime);
    }

    void HidePopup()
    {
        gameObject.SetActive(false);
    }
}