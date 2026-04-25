using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlots;
    public Dictionary<IngredientSO, int> ingredientCounts = new Dictionary<IngredientSO, int>();
    public Dictionary<PotionSO, int> potionCounts = new Dictionary<PotionSO, int>();
    private CraftingManager craft;
    public static bool anyMenuOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        craft = FindFirstObjectByType<CraftingManager>();
        RebindSceneUI();

        if (itemSlots != null && itemSlots.Length > 0)
        {
            RefreshAllSlots();
        }
    }
    void Update()
    {
        if (InventoryMenu == null)
            return;

        menuActivated = InventoryMenu.activeSelf;

        if (MenuManager.anyMenuOpen)
        return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (menuActivated)
            {
                Time.timeScale = 1;
                InventoryMenu.SetActive(false);
                menuActivated = false;
            }
            else
            {
                Time.timeScale = 0;
                InventoryMenu.SetActive(true);
                menuActivated = true;
            }
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindSceneUI();
        RefreshAllSlots();
    }
    private void RebindSceneUI()
    {
        InventoryUIRoot uiRoot = FindFirstObjectByType<InventoryUIRoot>();

        if (uiRoot == null)
        {
            Debug.LogWarning("InventoryManager: No InventoryUIRoot found in scene.");
            InventoryMenu = null;
            itemSlots = null;
            return;
        }

        InventoryMenu = uiRoot.inventoryMenu;
        itemSlots = uiRoot.itemSlots;

        Debug.Log("InventoryManager: Rebound inventory UI for new scene.");
    }
    public void RefreshAllSlots()
    {
        if (itemSlots == null || itemSlots.Length == 0)
            return;

        ClearAllSlots();

        foreach (var pair in ingredientCounts)
        {
            IngredientSO ingredient = pair.Key;
            int quantity = pair.Value;

            if (ingredient == null || quantity <= 0)
                continue;

            PlaceIngredientIntoSlots(ingredient, quantity);
        }

        foreach (var pair in potionCounts)
        {
            PotionSO potion = pair.Key;
            int quantity = pair.Value;

            if (potion == null || quantity <= 0)
                continue;

            PlacePotionIntoSlots(potion, quantity);
        }
    }

    public void ClearInventory()
    {
        ingredientCounts.Clear();
        potionCounts.Clear();

        ClearAllSlots();

        ResetInventoryProgress();

        PlayerPrefs.Save();

        Debug.Log("Inventory cleared.");
    }
        public int AddPotion(PotionSO potion, int quantity)
    {
        if (potion == null || quantity <= 0) return 0;

        int remaining = quantity;

        // 1) Fill existing stacks
        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (slot.isFull && slot.isPotion && slot.potionData == potion)
            {
                remaining = slot.AddPotion(potion, remaining);
                if (remaining <= 0)
                {
                    AddPotionToTotals(potion, quantity);
                    return 0;
                }
            }
        }

        // 2) Empty slots
        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (!slot.isFull || slot.quantity == 0)
            {
                remaining = slot.AddPotion(potion, remaining);
                if (remaining <= 0)
                {
                    AddPotionToTotals(potion, quantity);
                    return 0;
                }
            }
        }
        int successfullyAdded = quantity - remaining;
        if (successfullyAdded > 0)
        {
            AddPotionToTotals(potion, successfullyAdded);
        }
        return remaining;
        
    }
    public int GetPotionCount(PotionSO potion)
    {
        if (potion == null) return 0;

        if (potionCounts.TryGetValue(potion, out int count))
            return count;

        return 0;
    }
    private void AddPotionToTotals(PotionSO potion, int amountAdded)
    {
        if (potion == null || amountAdded <= 0)
            return;

        if (!potionCounts.ContainsKey(potion))
            potionCounts[potion] = 0;

        potionCounts[potion] += amountAdded;
    }

    public bool TryRemovePotion(PotionSO potion, int amount)
    {
        if (potion == null || amount <= 0) return false;

        int have = GetPotionCount(potion);
        if (have < amount) return false;

        int remainingToRemove = amount;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (!slot.isFull || !slot.isPotion || slot.potionData != potion) continue;

            if (remainingToRemove <= 0) break;

            int removeHere = Mathf.Min(slot.quantity, remainingToRemove);
            slot.quantity -= removeHere;
            remainingToRemove -= removeHere;

            if (slot.quantity <= 0)
            {
                slot.quantity = 0;
                slot.isFull = false;
                slot.isPotion = false;
                slot.potionData = null;
                slot.displayName = "";
                slot.itemSprite = null;
                slot.itemDescription = "";

                slot.ClearUI();
            }
            else
            {
                slot.UpdateUI();
            }
        }
        if (potionCounts.ContainsKey(potion))
        {
            potionCounts[potion] -= amount;
            if (potionCounts[potion] < 0)
                potionCounts[potion] = 0;
        }

        return true;
    }
        public int AddIngredient(
            IngredientSO ingredient,
            int quantity,
            string displayName,
            string itemType,
            int itemPrice,
            int maxPerCharacter,
            float rarity,
            string itemDescription)
        {
            if (ingredient == null || quantity <= 0) return 0;

            Debug.Log(
                "itemName " + displayName +
                " quantity " + quantity +
                " itemType " + itemType +
                " itemPrice " + itemPrice +
                " maxPerCharacter " + maxPerCharacter +
                " rarity " + rarity);

            int remaining = quantity;

            // 1) Fill existing stacks first (same ingredient)
            for (int i = 0; i < itemSlots.Length; i++)
            {
                ItemSlot slot = itemSlots[i];
                if (slot == null) continue;

                if (slot.isFull && slot.ingredientData == ingredient)
                {
                    remaining = slot.AddIngredient(
                        ingredient,
                        remaining,
                        displayName,
                        itemType,
                        itemPrice,
                        maxPerCharacter,
                        rarity,
                        itemDescription
                    );

                    if (remaining <= 0)
                    {
                        AddToTotals(ingredient, quantity);
                        return 0;
                    }
                }
            }

            // 2) Use empty slots for leftovers
            for (int i = 0; i < itemSlots.Length; i++)
            {
                ItemSlot slot = itemSlots[i];
                if (slot == null) continue;

                if (!slot.isFull || slot.quantity == 0)
                {
                    remaining = slot.AddIngredient(
                        ingredient,
                        remaining,
                        displayName,
                        itemType,
                        itemPrice,
                        maxPerCharacter,
                        rarity,
                        itemDescription
                    );

                    if (remaining <= 0)
                    {
                        AddToTotals(ingredient, quantity);
                        return 0;
                    }
                }
            }

            // 3) Could not fit everything
            int successfullyAdded = quantity - remaining;
            if (successfullyAdded > 0)
            {
                AddToTotals(ingredient, successfullyAdded);
            }

            return remaining;
        }

        private void AddToTotals(IngredientSO ingredient, int amountAdded)
        {
            if (amountAdded <= 0) return;

            if (!ingredientCounts.ContainsKey(ingredient))
            {
                ingredientCounts[ingredient] = 0;
            }

            ingredientCounts[ingredient] += amountAdded;
        }

        public void DeselectAllSlots()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].selectedShader.SetActive(false);
                itemSlots[i].thisItemSelected = false;
        
            }
        }

    public int GetIngredientCount(IngredientSO ingredient)
    {
        if (ingredient == null) return 0;

        if (ingredientCounts.TryGetValue(ingredient, out int count))
            return count;

        return 0;
    }

    public bool TryRemoveIngredient(IngredientSO ingredient, int amount)
    {
    if (ingredient == null || amount <= 0) return false;

    int have = GetIngredientCount(ingredient);
    if (have < amount) return false;

    int remainingToRemove = amount;

    for (int i = 0; i < itemSlots.Length; i++)
    {
        ItemSlot slot = itemSlots[i];
        if (slot == null) continue;

        if (!slot.isFull || slot.ingredientData != ingredient) continue;

        if (remainingToRemove <= 0) break;

        int removeHere = Mathf.Min(slot.quantity, remainingToRemove);
        slot.quantity -= removeHere;
        remainingToRemove -= removeHere;

        // Update slot UI and empty state
        if (slot.quantity <= 0)
        {
            slot.quantity = 0;
            slot.isFull = false;
            slot.ingredientData = null;
            slot.displayName = "";
            slot.itemSprite = null;
            slot.itemDescription = "";

            slot.ClearUI(); // You will add this method if you do not have it.
        }
        else
        {
            slot.UpdateUI(); // You will add this method if you do not have it.
        }
    }
    if (ingredientCounts.ContainsKey(ingredient))
    {
        ingredientCounts[ingredient] -= amount;
        if (ingredientCounts[ingredient] < 0) ingredientCounts[ingredient] = 0;
    }

    return true;
    }
    public void ResetInventoryProgress()
    {
        PlayerPrefs.DeleteKey("StarterItemsGiven");
        PlayerPrefs.Save();
    }

    private void ClearAllSlots()
    {
        if (itemSlots == null)
            return;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null)
                continue;

            slot.quantity = 0;
            slot.isFull = false;
            slot.isPotion = false;
            slot.ingredientData = null;
            slot.potionData = null;
            slot.displayName = "";
            slot.itemSprite = null;
            slot.itemDescription = "";

            slot.ClearUI();
        }
    }
    private void PlaceIngredientIntoSlots(IngredientSO ingredient, int quantity)
    {
        int remaining = quantity;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (slot.isFull && slot.ingredientData == ingredient)
            {
                remaining = slot.AddIngredient(
                    ingredient,
                    remaining,
                    ingredient.displayName,
                    ingredient.itemType,
                    ingredient.itemPrice,
                    ingredient.maxPerCharacter,
                    ingredient.rarity,
                    ingredient.itemDescription
                );

                if (remaining <= 0)
                    return;
            }
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (!slot.isFull || slot.quantity == 0)
            {
                remaining = slot.AddIngredient(
                    ingredient,
                    remaining,
                    ingredient.displayName,
                    ingredient.itemType,
                    ingredient.itemPrice,
                    ingredient.maxPerCharacter,
                    ingredient.rarity,
                    ingredient.itemDescription
                );

                if (remaining <= 0)
                    return;
            }
        }
    }
    private void PlacePotionIntoSlots(PotionSO potion, int quantity)
    {
        int remaining = quantity;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (slot.isFull && slot.isPotion && slot.potionData == potion)
            {
                remaining = slot.AddPotion(potion, remaining);
                if (remaining <= 0)
                    return;
            }
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            ItemSlot slot = itemSlots[i];
            if (slot == null) continue;

            if (!slot.isFull || slot.quantity == 0)
            {
                remaining = slot.AddPotion(potion, remaining);
                if (remaining <= 0)
                    return;
            }
        }
    }

    public void DisableInventory()
    {
        InventoryMenu.SetActive(false);
    }

    }

