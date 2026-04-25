using UnityEngine;
using TMPro;

public class RecipeLineUI : MonoBehaviour
{
    public TMP_Text requirementText;
    public TMP_Text haveText;

    private CraftingManager craftingManager;

    private void Awake()
    {
        craftingManager = FindFirstObjectByType<CraftingManager>();
    }

    public void Set(string ingredientName, int need, int have)
    {
        Debug.Log("RecipeLineUI.Set called: " + ingredientName + " " + have + "/" + need);

        int adjustedNeed = need;

        if (craftingManager != null)
        {
            adjustedNeed = Mathf.Max(1, need - craftingManager.recipeReduction);
        }

        if (requirementText != null)
            requirementText.text = ingredientName + " x" + adjustedNeed;

        if (haveText != null)
            haveText.text = have + "/" + adjustedNeed;

        gameObject.SetActive(true);
    }

    public void SetEmpty()
    {
        if (requirementText != null) requirementText.text = "";
        if (haveText != null) haveText.text = "";
        gameObject.SetActive(false);
    }
}