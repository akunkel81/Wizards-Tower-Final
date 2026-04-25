using cherrydev;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    [Header("Highlight Parameters")]
    private Color startColor;
    private Color monochromeCast;
    public Color highlightColor = Color.yellow;
    public static bool canHighlight = true;

    [Header("GUI Parameters")]
    public string toolTipName = ""; // tooltip ingame
    public static bool canToolTip = true;


    private void Start()
    {
        startColor = GetComponent<Renderer>().material.color;
        monochromeCast = startColor / 3;
    }
    private void OnMouseOver()
    {
        if (canHighlight)
        {
            GetComponent<Renderer>().material.color = monochromeCast;
            GetComponent<Renderer>().material.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = startColor;
    }

    public static void DisableHighlight()
    {
        canHighlight = false;
    }

    public static void EnableHighlight()
    {
        canHighlight = true;
    }
    public static void DisableTooltip()
    {
        canToolTip = false;
    }

    public static void EnableTooltip()
    {
        canToolTip = true;
    }

}
