using UnityEngine;
using TMPro;

public class PriceStatusDisplay : MonoBehaviour
{
    public TextMeshProUGUI priceTextbox;
    public string samplePotionName = "Healing Potion";

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (priceTextbox == null) return;

        var gm = GameManager.Instance;
        if (gm == null || gm.itemDatabase == null || gm.itemDatabase.potions == null || gm.itemDatabase.potions.Count == 0)
        {
            priceTextbox.text = "Item database not assigned or empty.";
            return;
        }

        float percentChange = (gm.potionSellMultiplier - 1f) * 100f;
        string sign = percentChange >= 0f ? "+" : "";
        percentChange = Mathf.Round(percentChange);

        var db = gm.itemDatabase;

        PotionSO samplePotion = null;
        foreach (var p in db.potions)
        {
            if (p != null && p.displayName == samplePotionName)
            {
                samplePotion = p;
                break;
            }
        }
        if (samplePotion == null) samplePotion = db.potions[0];

        int basePrice = samplePotion.sellPrice;
        int modifiedPrice = gm.GetEffectivePotionSellPrice(samplePotion);

        priceTextbox.text =
            "You have currently changed your prices by <color=red>" + sign + percentChange + "%</color>\n\n" +
            "Press any of the <color=blue>'Change Prices'</color> buttons to adjust your prices for this year.\n" +
            "<color=blue>Here is an example item from your inventory:</color>\n" +
            samplePotion.displayName + "\n" +
            "Base Price = <color=red>" + basePrice + "</color>\n" +
            "Modified Price = <color=red>" + modifiedPrice + "</color>";
    }
}