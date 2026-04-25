using cherrydev;
using UnityEngine;

public class HighlightManagerCauldron : MonoBehaviour
{
    [Header("Highlight Parameters")]
    private Color startColor;
    public Color highlightColor = Color.yellow;
    public static bool canHighlight = true;

    [Header("GUI Parameters")]
    public static string toolTipAdjective = ""; // healthy/damaged/broken
    public static string toolTipName = "Cauldron"; // tooltip ingame
    public static string tooltipDurability = ""; // % value, don't show if broken
    public static bool canToolTip = true;
    public static bool showPercent = true;

    //public static CraftingManager DurabilityInstanceRef { get; private set; }

    public static int curDurRef;
    public static int maxDurRef;
    public static float durabilityPercentRef;
    public static bool isBrokenRef;



    private void Start()
    {
        startColor = GetComponent<Renderer>().material.color;
    }

    private static void FetchCauldronData() //this is technically being ran every frame you're over the mouse which is, not great!
    {
        curDurRef = CraftingManager.GetCurrentDurabilityValue();
        maxDurRef = CraftingManager.GetMaxDurabilityValue();
        isBrokenRef = CraftingManager.getBrokenStatus();
        // CraftingManager.getBrokenStatus(isBrokenRef);


        durabilityPercentRef = (float)curDurRef / maxDurRef;

        if (isBrokenRef) // space for strings here because lazy/formatting
        {
            toolTipAdjective = "Broken ";
            showPercent = false;
            canHighlight = false; // to cue that "hey don't use this" tooltip NOT disabled
            return;
        }

        if (durabilityPercentRef <= 0.5f)
        {
            toolTipAdjective = "Damaged ";
            showPercent = true;
            return;
        }
        else
        {
            toolTipAdjective = "Healthy ";
            showPercent = true;
            return;
        }
    }

    private static void ShowPercentSymbol(float percentRef) { 
    if (showPercent)
        {
            percentRef = percentRef * 100; // so not decimal
            tooltipDurability = (percentRef.ToString()+"%");
        }
    else
        {
            tooltipDurability = "";
        }
    }

    private void OnMouseOver()
    {
        FetchCauldronData();
        ShowPercentSymbol(durabilityPercentRef);


        if (canHighlight)
        {
            GetComponent<Renderer>().material.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = startColor;
    }

    public static void DisableCauldronHighlight()
    {
        canHighlight = false;
    }

    public static void EnableCauldronHighlight()
    {
        canHighlight = true;
    }

    public static void DisableCauldronTooltip()
    {
        canToolTip = false;
    }

    public static void EnableCauldronTooltip()
    {
        canToolTip = false;
    }



}

