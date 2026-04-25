using UnityEngine;
using cherrydev;

public class NPCTradeFunctions : MonoBehaviour
{
    [Header("References")]
    public DialogBehaviour dialogBehaviour;
    public NPCInventoryRuntime npcInventoryRuntime;
    public InventoryManager inventoryManager;
    public SetPlayerCoins playerCoins;
    public NPCsSO npcsSO;
    public PotionSO potions;
    public ActionManager actionManager;
    public CraftingManager craftingManager;
    public DialogueAnswerController answerController;
    public RapportManager rapportManager;

    private void Awake()
    {
        RebindReferences();
    }

    private void OnEnable()
    {
        RebindReferences();
    }

    private void RebindReferences()
    {
        dialogBehaviour = FindFirstObjectByType<DialogBehaviour>();

        npcInventoryRuntime = FindFirstObjectByType<NPCInventoryRuntime>();

        inventoryManager = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : FindFirstObjectByType<InventoryManager>();

        playerCoins = SetPlayerCoins.Instance != null
            ? SetPlayerCoins.Instance
            : FindFirstObjectByType<SetPlayerCoins>();

        actionManager = ActionManager.Instance;
        rapportManager = RapportManager.Instance;

        craftingManager = FindFirstObjectByType<CraftingManager>();
        answerController = FindFirstObjectByType<DialogueAnswerController>();
    }

    public void BindFunctions()
    {
        RebindReferences();

        if (dialogBehaviour == null)
        {
            Debug.LogError("NPCTradeFunctions: DialogBehaviour missing.");
            return;
        }

        dialogBehaviour.BindExternalFunction("BuyOffer0", BuyOffer0);
        dialogBehaviour.BindExternalFunction("BuyOffer1", BuyOffer1);
        dialogBehaviour.BindExternalFunction("BuyOffer2", BuyOffer2);
        dialogBehaviour.BindExternalFunction("BuyOffer3", BuyOffer3);

        dialogBehaviour.BindExternalFunction("SellPotionRequest0", SellPotionRequest0);
        dialogBehaviour.BindExternalFunction("SellPotionRequest1", SellPotionRequest1);

        dialogBehaviour.BindExternalFunction("CheckAffordSellPotionRequest0", CheckAffordSellPotionRequest0);
        dialogBehaviour.BindExternalFunction("CheckAffordSellPotionRequest1", CheckAffordSellPotionRequest1);

        dialogBehaviour.BindExternalFunction("EnableSellRestrictions", EnableSellRestrictions);
        dialogBehaviour.BindExternalFunction("ResetSellRestrictions", ResetSellRestrictions);

        dialogBehaviour.BindExternalFunction("IncreaseCurrentNPCRapport", IncreaseCurrentNPCRapport);
        dialogBehaviour.BindExternalFunction("DecreaseCurrentNPCRapport", DecreaseCurrentNPCRapport);
        dialogBehaviour.BindExternalFunction("SetCurrentRapportVariable", SetCurrentRapportVariable);
        dialogBehaviour.BindExternalFunction("ShowRapportValue", ShowRapportValue);

        Debug.Log("Bound buy, sell, and affordability functions.");
    }

    public void BuyOffer0() { BuyFullOffer(0); }
    public void BuyOffer1() { BuyFullOffer(1); }
    public void BuyOffer2() { BuyFullOffer(2); }
    public void BuyOffer3() { BuyFullOffer(3); }

    public void SellPotionRequest0()
    {
        RebindReferences();

        if (!HasRequestedPotionForTrade(0))
        {
            Debug.Log("Blocked SellPotionRequest0: missing required potion.");
            return;
        }

        if (!CanNPCAffordRequestedPotion(0))
        {
            Debug.Log("Blocked SellPotionRequest0: NPC cannot afford this potion.");
            return;
        }

        SellRequestedPotion(0);
    }

    public void SellPotionRequest1()
    {
        RebindReferences();

        if (!HasRequestedPotionForTrade(1))
        {
            Debug.Log("Blocked SellPotionRequest1: missing required potion.");
            return;
        }

        if (!CanNPCAffordRequestedPotion(1))
        {
            Debug.Log("Blocked SellPotionRequest1: NPC cannot afford this potion.");
            return;
        }

        SellRequestedPotion(1);
    }

    public void CheckAffordSellPotionRequest0()
    {
        RebindReferences();
        SetNpcCanAffordVariable(0);
    }

    public void CheckAffordSellPotionRequest1()
    {
        RebindReferences();
        SetNpcCanAffordVariable(1);
    }

    private void SetNpcCanAffordVariable(int requestIndex)
    {
        if (dialogBehaviour == null)
        {
            Debug.LogError("NPCTradeFunctions: DialogBehaviour missing.");
            return;
        }

        bool canAfford = CanNPCAffordRequestedPotion(requestIndex);
        dialogBehaviour.SetVariableValue("canAfford", canAfford);

        Debug.Log("canAfford set to " + canAfford + " for request " + requestIndex);
    }

    public void EnableSellRestrictions()
    {
        RebindReferences();

        Debug.Log("EnableSellRestrictions CALLED");

        if (answerController != null)
            answerController.RefreshButtons();
        else
            Debug.LogWarning("answerController is null");
    }

    public void ResetSellRestrictions()
    {
        RebindReferences();

        if (answerController != null)
            answerController.ResetButtons();
    }
    public void IncreaseCurrentNPCRapport()
    {
        RebindReferences();

        if (rapportManager == null || actionManager == null)
        {
            Debug.LogError("NPCTradeFunctions: missing rapportManager or actionManager.");
            return;
        }

        NPCsSO currentNPC = actionManager.GetCurrentNPC();
        rapportManager.IncreaseRapportForNPC(currentNPC);
    }

    public void DecreaseCurrentNPCRapport()
    {
        RebindReferences();

        if (rapportManager == null || actionManager == null)
        {
            Debug.LogError("NPCTradeFunctions: missing rapportManager or actionManager.");
            return;
        }

        NPCsSO currentNPC = actionManager.GetCurrentNPC();
        rapportManager.DecreaseRapportForNPC(currentNPC);
    }

        public void SetCurrentRapportVariable()
    {
        RebindReferences();

        if (dialogBehaviour == null)
        {
            Debug.LogError("NPCTradeFunctions: DialogBehaviour missing.");
            return;
        }

        if (rapportManager == null)
        {
            Debug.LogError("NPCTradeFunctions: rapportManager missing.");
            return;
        }

        float currentRapport = rapportManager.GetCurrentRapportValue();
        dialogBehaviour.SetVariableValue("currentRapport", currentRapport);

        Debug.Log("Set dialogue variable currentRapport to " + currentRapport);
    }
    public void ShowRapportValue()
    {
        RebindReferences();

        if (dialogBehaviour == null || RapportManager.Instance == null)
        {
            Debug.LogError("ShowRapportValue: missing references.");
            return;
        }

        float rapport = RapportManager.Instance.GetCurrentRapportValue();
        dialogBehaviour.SetVariableValue("currentRapportStatus", rapport.ToString("0.0"));

        Debug.Log("ShowRapportValue -> " + rapport.ToString("0.0"));
    }
    private void BuyFullOffer(int offerIndex)
    {
        RebindReferences();

        if (!TryGetOffer(offerIndex, out RuntimeNPCOffer offer))
            return;

        IngredientSO ingredient = offer.ingredient;
        int amountToBuy = offer.currentAmount;

        if (amountToBuy <= 0)
        {
            Debug.Log("NPC has no stock left for " + ingredient.displayName);
            return;
        }

        int pricePerItem = GetCurrentPrice(ingredient);
        int totalPrice = pricePerItem * amountToBuy;

        Debug.Log("Trying to buy " + ingredient.displayName + " x" + amountToBuy + " for " + totalPrice);

        if (!playerCoins.SpendCoins(totalPrice))
        {
            Debug.Log("Player cannot afford " + ingredient.displayName);
            return;
        }

        bool removedFromNPC = npcInventoryRuntime.TryTakeFromNPC(ingredient, amountToBuy);
        if (!removedFromNPC)
        {
            Debug.Log("Failed to remove NPC stock.");
            playerCoins.AddCoins(totalPrice);
            return;
        }

        npcInventoryRuntime.AddNPCCoins(totalPrice);

        int leftover = inventoryManager.AddIngredient(
            ingredient,
            amountToBuy,
            ingredient.displayName,
            ingredient.itemType,
            ingredient.itemPrice,
            ingredient.maxPerCharacter,
            ingredient.rarity,
            ingredient.itemDescription
        );

        if (leftover > 0)
        {
            Debug.Log("Inventory full. Leftover amount: " + leftover);
        }

        Debug.Log("Bought " + ingredient.displayName + " x" + amountToBuy);
    }

    private void SellFullOffer(int offerIndex)
    {
        RebindReferences();

        if (!TryGetOffer(offerIndex, out RuntimeNPCOffer offer))
            return;

        IngredientSO ingredient = offer.ingredient;

        int playerOwned = inventoryManager.GetIngredientCount(ingredient);
        if (playerOwned <= 0)
        {
            Debug.Log("Player does not own any " + ingredient.displayName);
            return;
        }

        int amountToSell = playerOwned;
        int pricePerItem = GetCurrentPrice(ingredient);
        int totalPrice = pricePerItem * amountToSell;

        if (!npcInventoryRuntime.SpendNPCCoins(totalPrice))
        {
            Debug.Log("NPC cannot afford " + amountToSell + " " + ingredient.displayName);
            return;
        }

        bool removedFromPlayer = inventoryManager.TryRemoveIngredient(ingredient, amountToSell);
        if (!removedFromPlayer)
        {
            Debug.Log("Failed to remove ingredient from player inventory.");
            npcInventoryRuntime.AddNPCCoins(totalPrice);
            return;
        }

        playerCoins.AddCoins(totalPrice);
        npcInventoryRuntime.AddToNPCStock(ingredient, amountToSell);

        Debug.Log("Sold " + ingredient.displayName + " x" + amountToSell);
    }

    private bool TryGetOffer(int offerIndex, out RuntimeNPCOffer offer)
    {
        offer = null;

        if (npcInventoryRuntime == null)
        {
            Debug.LogError("NPCTradeFunctions: npcInventoryRuntime missing.");
            return false;
        }

        if (inventoryManager == null)
        {
            Debug.LogError("NPCTradeFunctions: inventoryManager missing.");
            return false;
        }

        if (playerCoins == null)
        {
            Debug.LogError("NPCTradeFunctions: playerCoins missing.");
            return false;
        }

        if (npcInventoryRuntime.currentOffers == null || npcInventoryRuntime.currentOffers.Count == 0)
        {
            Debug.LogWarning("NPC has no offers.");
            return false;
        }

        if (offerIndex < 0 || offerIndex >= npcInventoryRuntime.currentOffers.Count)
        {
            Debug.LogWarning("Invalid offer index: " + offerIndex);
            return false;
        }

        offer = npcInventoryRuntime.currentOffers[offerIndex];

        if (offer == null || offer.ingredient == null)
        {
            Debug.LogWarning("Offer is null or has no ingredient.");
            return false;
        }

        return true;
    }

    private int GetCurrentPrice(IngredientSO ingredient)
    {
        if (ingredient == null) return 0;

        if (GameManager.Instance != null)
            return GameManager.Instance.GetEffectiveIngredientPrice(ingredient);

        return ingredient.itemPrice;
    }

    private void SellRequestedPotion(int requestIndex)
    {
        RebindReferences();

        if (actionManager == null || inventoryManager == null || playerCoins == null)
        {
            Debug.LogError("Missing references for potion selling.");
            return;
        }

        NPCsSO currentNPC = actionManager.GetCurrentNPC();
        if (currentNPC == null)
        {
            Debug.LogError("No current NPC found.");
            return;
        }

        if (currentNPC.potionRequests == null || requestIndex < 0 || requestIndex >= currentNPC.potionRequests.Length)
        {
            Debug.LogWarning("Invalid potion request index: " + requestIndex);
            return;
        }

        NPCPotionRequest request = currentNPC.potionRequests[requestIndex];
        if (request == null || request.potion == null)
        {
            Debug.LogWarning("Potion request is null.");
            return;
        }

        PotionSO requestedPotion = request.potion;
        int requestAmount = Mathf.Max(1, request.requestAmount);

        int owned = inventoryManager.GetPotionCount(requestedPotion);
        if (owned < requestAmount)
        {
            Debug.Log("Player does not have enough of requested potion: " + requestedPotion.displayName);
            return;
        }

        int currentSellPrice = requestedPotion.sellPrice;

        if (craftingManager != null)
            currentSellPrice = craftingManager.GetCraftedPotionSellPrice(requestedPotion);
        else if (GameManager.Instance != null)
            currentSellPrice = GameManager.Instance.GetEffectivePotionSellPrice(requestedPotion);

        int totalSellPrice = currentSellPrice * requestAmount;

        bool removed = inventoryManager.TryRemovePotion(requestedPotion, requestAmount);
        if (!removed)
        {
            Debug.Log("Failed to remove potion from inventory.");
            return;
        }

        playerCoins.AddCoins(totalSellPrice);

        Debug.Log("Sold " + requestedPotion.displayName + " x" + requestAmount + " for " + totalSellPrice);
    }

    public bool HasRequestedPotionForTrade(int requestIndex)
    {
        RebindReferences();

        if (actionManager == null || inventoryManager == null)
        {
            Debug.LogError("NPCTradeFunctions: Missing ActionManager or InventoryManager.");
            return false;
        }

        NPCsSO currentNPC = actionManager.GetCurrentNPC();
        if (currentNPC == null)
        {
            Debug.LogWarning("NPCTradeFunctions: No current NPC found.");
            return false;
        }

        if (currentNPC.potionRequests == null || requestIndex < 0 || requestIndex >= currentNPC.potionRequests.Length)
        {
            Debug.LogWarning("NPCTradeFunctions: Invalid potion request index: " + requestIndex);
            return false;
        }

        NPCPotionRequest request = currentNPC.potionRequests[requestIndex];
        if (request == null || request.potion == null)
        {
            Debug.LogWarning("NPCTradeFunctions: Potion request is null.");
            return false;
        }

        PotionSO requestedPotion = request.potion;
        int requestAmount = Mathf.Max(1, request.requestAmount);
        int owned = inventoryManager.GetPotionCount(requestedPotion);

        return owned >= requestAmount;
    }

    public bool CanNPCAffordRequestedPotion(int requestIndex)
    {
        RebindReferences();

        if (actionManager == null || npcInventoryRuntime == null)
        {
            Debug.LogError("NPCTradeFunctions: Missing ActionManager or NPCInventoryRuntime.");
            return false;
        }

        NPCsSO currentNPC = actionManager.GetCurrentNPC();
        if (currentNPC == null)
        {
            Debug.LogWarning("NPCTradeFunctions: No current NPC found.");
            return false;
        }

        if (currentNPC.potionRequests == null || requestIndex < 0 || requestIndex >= currentNPC.potionRequests.Length)
        {
            Debug.LogWarning("NPCTradeFunctions: Invalid potion request index: " + requestIndex);
            return false;
        }

        NPCPotionRequest request = currentNPC.potionRequests[requestIndex];
        if (request == null || request.potion == null)
        {
            Debug.LogWarning("NPCTradeFunctions: Potion request is null.");
            return false;
        }

        PotionSO requestedPotion = request.potion;
        int requestAmount = Mathf.Max(1, request.requestAmount);

        int currentSellPrice = requestedPotion.sellPrice;

        if (craftingManager != null)
            currentSellPrice = craftingManager.GetCraftedPotionSellPrice(requestedPotion);
        else if (GameManager.Instance != null)
            currentSellPrice = GameManager.Instance.GetEffectivePotionSellPrice(requestedPotion);

        int totalSellPrice = currentSellPrice * requestAmount;

        return npcInventoryRuntime.currentCharacterCoins >= totalSellPrice;
    }
}