using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIController : MonoBehaviour
{
    [Header("Data")]
    public ItemDatabaseSO itemDatabase;

    [Header("Left List")]
    public CraftingSlot[] slots;

    [Header("Right Detail UI")]
    public TMP_Text potionNameText;
    public Image potionImage;
    public TMP_Text potionDescriptionText;

    [Header("Recipe Lines")]
    public RecipeLineUI[] recipeLines;

    [Header("Craft Button")]
    public Button craftButton;

    public CraftingManager craftingManager;
    private PotionSO _selectedPotion;
    [SerializeField] private GameObject craftingMenu; 

    private void OnEnable()
    {
        PopulateLeftList();

        if (itemDatabase != null && itemDatabase.potions != null && itemDatabase.potions.Count > 0)
        {
            SelectPotion(itemDatabase.potions[0]);
        }
        else
        {
            ClearRightSide();
        }
    }

    private void PopulateLeftList()
    {
        if (itemDatabase == null || itemDatabase.potions == null)
        {
            Debug.LogError("CraftingUIController: itemDatabase missing or potions empty.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            PotionSO p = (i < itemDatabase.potions.Count) ? itemDatabase.potions[i] : null;

            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(p != null);
                if (p != null) slots[i].Bind(p, this);
            }
        }
    }

    public void SelectPotion(PotionSO potion)
    {
        _selectedPotion = potion;

        if (potionNameText != null)
            potionNameText.text = potion.displayName;

        if (potionImage != null)
        {
            potionImage.sprite = potion.potionSprite;
            potionImage.enabled = (potionImage.sprite != null);
        }

        if (potionDescriptionText != null)
        {
            int sell = (GameManager.Instance != null)
                ? GameManager.Instance.GetEffectivePotionSellPrice(potion)
                : potion.sellPrice;

            potionDescriptionText.text =
                "Sell Price: " + sell + "\n" +
                potion.itemDescription;
        }

        UpdateRecipeUI(potion);
        UpdateCraftButtonInteractable(potion);
    }

    private void UpdateRecipeUI(PotionSO potion)
    {
        for (int i = 0; i < recipeLines.Length; i++)
        {
            recipeLines[i].SetEmpty();
        }

        if (potion == null || potion.recipe == null || craftingManager == null) return;

        for (int i = 0; i < potion.recipe.Length && i < recipeLines.Length; i++)
        {
            var entry = potion.recipe[i];
            if (entry == null || entry.ingredient == null) continue;

            int have = craftingManager.GetOwnedCount(entry.ingredient);
            int need = entry.quantity;

            recipeLines[i].Set(entry.ingredient.displayName, need, have);
        }
    }

    private void UpdateCraftButtonInteractable(PotionSO potion)
    {
        if (craftButton == null) return;
        craftButton.interactable = true;
    }

    private void ClearRightSide()
    {
        _selectedPotion = null;

        if (potionNameText != null) potionNameText.text = "";
        if (potionDescriptionText != null) potionDescriptionText.text = "";

        if (potionImage != null)
        {
            potionImage.sprite = null;
            potionImage.enabled = false;
        }

        for (int i = 0; i < recipeLines.Length; i++)
        {
            recipeLines[i].SetEmpty();
        }

        if (craftButton != null)
            craftButton.interactable = false;
    }

    public void OnClickCraft()
{
    Debug.Log("OnClickCraft called");

    if (_selectedPotion == null)
    {
        Debug.Log("OnClickCraft: _selectedPotion is null");
        return;
    }

    if (craftingManager == null)
    {
        Debug.Log("OnClickCraft: craftingManager is null");
        return;
    }

    Debug.Log("Trying to craft: " + _selectedPotion.displayName);

    bool crafted = craftingManager.TryCraft(_selectedPotion);

    Debug.Log("TryCraft returned: " + crafted);

    if (crafted)
    {
        UpdateRecipeUI(_selectedPotion);
        UpdateCraftButtonInteractable(_selectedPotion);

            ActionManager actionManager = FindFirstObjectByType<ActionManager>();
            if (actionManager != null)
                actionManager.UseAction(1);
    }
}

    public void CloseCraftingUI()
    {
        Debug.Log("CloseMenu clicked");

        if (craftingMenu != null)
        {
            Debug.Log("Closing object: " + craftingMenu.name);
            craftingMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("CraftingUIController: craftingMenu is not assigned.");
        }

        Time.timeScale = 1f;
        MenuManager.anyMenuOpen = false;

        CraftingManager crafting = FindFirstObjectByType<CraftingManager>();
        if (crafting != null)
            crafting.SetInputLocked(false);

        RepairManager repair = FindFirstObjectByType<RepairManager>();
        if (repair != null)
            repair.SetInputLocked(false);

        UpgradeManager upgrade = FindFirstObjectByType<UpgradeManager>();
        if (upgrade != null)
            upgrade.SetInputLocked(false);
    }
    }