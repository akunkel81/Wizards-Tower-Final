using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingManager : MonoBehaviour
{
    [Header("Cauldron Durability")]
    public int maxDurability = 5;
    public int currentDurability = 5;
    public bool isCauldronBroken = false;

    [Header("Crafting Effects")]
    public int recipeReduction = 0;
    public float upgradeSellMultiplier = 1f;

    [Header("UI")]
    public GameObject craftingUI;

    [Header("Optional Sprites")]
    public SpriteRenderer cauldronSpriteRenderer;
    public Sprite healthySprite;
    public Sprite damagedSprite;
    public Sprite brokenSprite;

    private InventoryManager _inventory;
    private const string CurrentDurabilityKey = "CurrentDurability";
    private const string MaxDurabilityKey = "MaxDurability";
    private const string RecipeReductionKey = "RecipeReduction";
    private const string UpgradeSellMultiplierKey = "UpgradeSellMultiplier";
    private const string IsBrokenKey = "IsBroken";
    public static bool anyMenuOpen = false;

    public bool inputLocked = false;

    //public static CraftingManager DurabilityInstance { get; private set; }

    private void Awake()
    {
        _inventory = FindFirstObjectByType<InventoryManager>();
        Debug.Log("CraftingManager found inventory: " + (_inventory != null));

        LoadCraftingState();
        LoadDurability();
        SaveDurability();

        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);
        UpdateBrokenState();
        UpdateCauldronVisual();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (craftingUI == null)
        {
            Debug.LogError("Crafting UI not assigned.");
            return;
        }

        if (craftingUI.activeSelf)
            return;

        if (MenuManager.anyMenuOpen)
            return;

        if (inputLocked)
            return;

        craftingUI.SetActive(true);
        MenuManager.anyMenuOpen = true;
        Time.timeScale = 0f;

        Debug.Log("Crafting menu opened.");
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
    }

    public void ForceCloseMenu()
    {
        if (craftingUI != null)
            craftingUI.SetActive(false);

        MenuManager.anyMenuOpen = false;
        inputLocked = false;
        Time.timeScale = 1f;
    }

    public int GetOwnedCount(IngredientSO ingredient)
    {
        if (_inventory == null || ingredient == null) return 0;
        return _inventory.GetIngredientCount(ingredient);
    }

    public bool CanCraft(PotionSO potion)
    {
        if (potion == null || potion.recipe == null || _inventory == null) return false;
        if (isCauldronBroken) return false;

        foreach (var entry in potion.recipe)
        {
            if (entry == null || entry.ingredient == null) return false;

            int required = GetRequiredIngredientAmount(entry.quantity);
            int owned = _inventory.GetIngredientCount(entry.ingredient);

            if (owned < required)
                return false;
        }

        return true;
    }

    public bool TryCraft(PotionSO potion)
    {
        if (potion == null)
        {
            Debug.Log("TryCraft failed: potion is null");
            return false;
        }

        if (_inventory == null)
        {
            Debug.LogError("CraftingManager: InventoryManager missing.");
            return false;
        }

        if (potion.recipe == null || potion.recipe.Length == 0)
        {
            Debug.LogError("CraftingManager: Potion has no recipe.");
            return false;
        }

        if (isCauldronBroken)
        {
            Debug.Log("Cannot craft " + potion.displayName + ". Cauldron is broken.");
            return false;
        }

        if (!CanCraft(potion))
        {
            Debug.Log("Cannot craft " + potion.displayName + ". Missing ingredients.");
            return false;
        }

        // Remove ingredients
        foreach (var entry in potion.recipe)
        {
            if (entry == null || entry.ingredient == null) continue;

            int required = GetRequiredIngredientAmount(entry.quantity);

            bool removed = _inventory.TryRemoveIngredient(entry.ingredient, required);
            if (!removed)
            {
                Debug.LogError("CraftingManager: Failed to remove ingredient " + entry.ingredient.displayName);
                return false;
            }
        }

        // Add crafted potion
        _inventory.AddPotion(potion, 1);

        // Wear down cauldron
        ReduceDurability(1);

        Debug.Log("Crafted: " + potion.displayName);
        return true;
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (upgrade == null) return;

        switch (upgrade.upgradeType)
        {
            case UpgradeType.Fortify:
                maxDurability += upgrade.durabilityBonus;
                currentDurability += upgrade.durabilityBonus;
                break;

            case UpgradeType.FlavorBoost:
                upgradeSellMultiplier *= upgrade.sellPriceMultiplier;
                break;

            case UpgradeType.Efficient:
                recipeReduction += upgrade.ingredientReduction;
                break;

            case UpgradeType.Potency:
                upgradeSellMultiplier *= upgrade.sellPriceMultiplier;
                break;
        }

        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);

        SaveCraftingState();
        UpdateBrokenState();
        UpdateCauldronVisual();

        Debug.Log("Applied upgrade: " + upgrade.upgradeName);
    }

    public int GetCraftedPotionSellPrice(PotionSO potion)
    {
        if (potion == null) return 0;

        float yearlyMultiplier = 1f;

        if (GameManager.Instance != null)
            yearlyMultiplier = GameManager.Instance.potionSellMultiplier;

        float finalMultiplier = yearlyMultiplier * upgradeSellMultiplier;

        return Mathf.RoundToInt(potion.sellPrice * finalMultiplier);
    }

    public void ReduceDurability(int amount)
    {
        currentDurability -= amount;
        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);

        SaveCraftingState();
        UpdateBrokenState();
        UpdateCauldronVisual();

        Debug.Log("Cauldron durability: " + currentDurability + "/" + maxDurability);
    }

    public void RepairFull()
    {
        currentDurability = maxDurability;
        UpdateBrokenState();
        UpdateCauldronVisual();
        SaveDurability();
    }

    private void LoadDurability()
    {
        currentDurability = PlayerPrefs.GetInt(CurrentDurabilityKey, currentDurability);
        maxDurability = PlayerPrefs.GetInt(MaxDurabilityKey, maxDurability);
    }

    private void SaveDurability()
    {
        PlayerPrefs.SetInt(CurrentDurabilityKey, currentDurability);
        PlayerPrefs.SetInt(MaxDurabilityKey, maxDurability);
        PlayerPrefs.Save();
    }

    private int GetRequiredIngredientAmount(int baseAmount)
    {
        return Mathf.Max(1, baseAmount - recipeReduction);
    }

    private void UpdateBrokenState()
    {
        isCauldronBroken = currentDurability <= 0;
        int isCauldronBrokenInt = isCauldronBroken ? 1 : 0;
        PlayerPrefs.SetInt(IsBrokenKey, isCauldronBrokenInt);
        PlayerPrefs.Save();
    }

    private void UpdateCauldronVisual()
    {
        if (cauldronSpriteRenderer == null)
        {
            Debug.LogWarning("No cauldronSpriteRenderer assigned.");
            return;
        }

        Debug.Log("Current durability: " + currentDurability + "/" + maxDurability);

        if (isCauldronBroken)
        {
            Debug.Log("Setting BROKEN sprite");
            cauldronSpriteRenderer.sprite = brokenSprite;
            return;
        }

        float durabilityPercent = (float)currentDurability / maxDurability;
        Debug.Log("Durability percent: " + durabilityPercent);

        if (durabilityPercent <= 0.5f)
        {
            Debug.Log("Setting DAMAGED sprite");
            cauldronSpriteRenderer.sprite = damagedSprite;
        }
        else
        {
            Debug.Log("Setting HEALTHY sprite");
            cauldronSpriteRenderer.sprite = healthySprite;
        }
    }
    public void RepairHalf()
    {
        int repairAmount = Mathf.CeilToInt(maxDurability * 0.5f);

        currentDurability += repairAmount;
        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);

        SaveCraftingState();
        UpdateBrokenState();
        UpdateCauldronVisual();

        Debug.Log("Half repaired cauldron. Durability: " + currentDurability + "/" + maxDurability);
    }
    private void LoadCraftingState()
    {
        maxDurability = PlayerPrefs.GetInt(MaxDurabilityKey, maxDurability);
        currentDurability = PlayerPrefs.GetInt(CurrentDurabilityKey, currentDurability);
        recipeReduction = PlayerPrefs.GetInt(RecipeReductionKey, recipeReduction);
        upgradeSellMultiplier = PlayerPrefs.GetFloat(UpgradeSellMultiplierKey, upgradeSellMultiplier);
    }

    private void SaveCraftingState()
    {
        PlayerPrefs.SetInt(MaxDurabilityKey, maxDurability);
        PlayerPrefs.SetInt(CurrentDurabilityKey, currentDurability);
        PlayerPrefs.SetInt(RecipeReductionKey, recipeReduction);
        PlayerPrefs.SetFloat(UpgradeSellMultiplierKey, upgradeSellMultiplier);
        PlayerPrefs.Save();
    }

    public void ClearSavedCraftingState()
    {
        PlayerPrefs.DeleteKey(CurrentDurabilityKey);
        PlayerPrefs.DeleteKey(MaxDurabilityKey);
        PlayerPrefs.DeleteKey(RecipeReductionKey);
        PlayerPrefs.DeleteKey(UpgradeSellMultiplierKey);
        PlayerPrefs.Save();

        Debug.Log("Cleared saved crafting state.");
    }

    public static int GetCurrentDurabilityValue()
    {
        Debug.Log("Getting Durability Values");

        int curDur = PlayerPrefs.GetInt(CurrentDurabilityKey);
        Debug.Log("curDur value is " + curDur);
        return curDur;

    }

    public static int GetMaxDurabilityValue()
    {
        int maxDur = PlayerPrefs.GetInt(MaxDurabilityKey); // DurabilityInstance.currentDurability;
        Debug.Log("maxDur value is " + maxDur);
        return maxDur;
        
    }
    public static bool getBrokenStatus()
    {
        Debug.Log("Getting Broken Status");
        int isBroken = PlayerPrefs.GetInt(IsBrokenKey); // DurabilityInstance.isCauldronBroken;
        return isBroken != 0;
    }
}