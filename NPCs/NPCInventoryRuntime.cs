using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeNPCOffer
{
    public IngredientSO ingredient;
    public int currentAmount;
}

public class NPCInventoryRuntime : MonoBehaviour
{
    public NPCsSO npcData;

    public int currentCharacterCoins;
    public List<RuntimeNPCOffer> currentOffers = new List<RuntimeNPCOffer>();

    public void BuildRuntimeInventory()
    {
        currentOffers.Clear();

        if (npcData == null)
        {
            Debug.LogWarning("NPCInventoryRuntime: npcData missing.");
            return;
        }

        currentCharacterCoins = npcData.characterCoinCount;

        if (npcData.ingredientOffers == null)
            return;

        for (int i = 0; i < npcData.ingredientOffers.Length; i++)
        {
            NPCIngredientOffer offer = npcData.ingredientOffers[i];

            if (offer == null || offer.ingredient == null)
                continue;

            int adjustedAmount = GetAdjustedAmount(offer);

            RuntimeNPCOffer runtimeOffer = new RuntimeNPCOffer
            {
                ingredient = offer.ingredient,
                currentAmount = adjustedAmount
            };

            currentOffers.Add(runtimeOffer);
        }
    }

    private int GetAdjustedAmount(NPCIngredientOffer offer)
    {
        if (offer == null || offer.ingredient == null)
            return 0;

        int baseAmount = offer.baseAmount;

        if (GameManager.Instance == null)
            return baseAmount;

        float availabilityMultiplier =
            GameManager.Instance.GetAvailabilityMultiplierForType(offer.ingredient.itemType);

        int adjusted = Mathf.RoundToInt(baseAmount * availabilityMultiplier);

        return Mathf.Max(0, adjusted);
    }

    public int GetCurrentAmount(IngredientSO ingredient)
    {
        if (ingredient == null)
            return 0;

        for (int i = 0; i < currentOffers.Count; i++)
        {
            if (currentOffers[i].ingredient == ingredient)
                return currentOffers[i].currentAmount;
        }

        return 0;
    }

    public bool SpendNPCCoins(int amount)
    {
        if (amount <= 0) return true;
        if (currentCharacterCoins < amount) return false;

        currentCharacterCoins -= amount;
        return true;
    }

    public void AddNPCCoins(int amount)
    {
        if (amount <= 0) return;
        currentCharacterCoins += amount;
    }
    public string GetOffersAsText()
    {
        if (currentOffers.Count == 0)
            return "No items available.";

        string result = "";

        for (int i = 0; i < currentOffers.Count; i++)
        {
            RuntimeNPCOffer offer = currentOffers[i];

            if (offer == null || offer.ingredient == null)
                continue;

            result += offer.ingredient.displayName + " x" + offer.currentAmount;

            if (i < currentOffers.Count - 1)
                result += "\n";
        }

        return result;
    }
    public bool TryTakeFromNPC(IngredientSO ingredient, int amount)
    {
        if (ingredient == null || amount <= 0)
            return false;

        for (int i = 0; i < currentOffers.Count; i++)
        {
            RuntimeNPCOffer offer = currentOffers[i];

            if (offer == null || offer.ingredient != ingredient)
                continue;

            if (offer.currentAmount < amount)
                return false;

            offer.currentAmount -= amount;
            return true;
        }

        return false;
    }

    public void AddToNPCStock(IngredientSO ingredient, int amount)
    {
        if (ingredient == null || amount <= 0)
            return;

        for (int i = 0; i < currentOffers.Count; i++)
        {
            RuntimeNPCOffer offer = currentOffers[i];

            if (offer == null || offer.ingredient != ingredient)
                continue;

            offer.currentAmount += amount;
            return;
        }

        RuntimeNPCOffer newOffer = new RuntimeNPCOffer
        {
            ingredient = ingredient,
            currentAmount = amount
        };

        currentOffers.Add(newOffer);
    }
}