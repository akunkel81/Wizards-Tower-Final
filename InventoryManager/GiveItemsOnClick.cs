using UnityEngine;

public class GiveItemsOnClick : MonoBehaviour
{
    [System.Serializable]
    public class ItemEntry
    {
        public IngredientSO ingredient;
        public PotionSO potion;
        public int quantity = 1;
    }

    [SerializeField] private ItemEntry[] itemsToGive;

    [Header("Chest Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    [Header("Optional")]
    [SerializeField] private bool oneTimeUse = false;

    private bool used = false;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure chest starts closed
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (oneTimeUse && used)
            return;

        GiveItems();

        // Switch to open sprite
        if (spriteRenderer != null && openedSprite != null)
            spriteRenderer.sprite = openedSprite;

        used = true;
    }

    private void GiveItems()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("GiveItemsOnClick: InventoryManager not found.");
            return;
        }

        InventoryManager inventory = InventoryManager.Instance;

        for (int i = 0; i < itemsToGive.Length; i++)
        {
            ItemEntry entry = itemsToGive[i];

            if (entry == null || entry.quantity <= 0)
                continue;

            if (entry.ingredient != null)
            {
                inventory.AddIngredient(
                    entry.ingredient,
                    entry.quantity,
                    entry.ingredient.displayName,
                    entry.ingredient.itemType,
                    entry.ingredient.itemPrice,
                    entry.ingredient.maxPerCharacter,
                    entry.ingredient.rarity,
                    entry.ingredient.itemDescription
                );
            }
            else if (entry.potion != null)
            {
                inventory.AddPotion(entry.potion, entry.quantity);
            }
        }

        Debug.Log("Items granted from click.");
    }
}