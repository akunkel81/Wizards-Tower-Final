using UnityEngine;
using TMPro;

public class EventsDisplay : MonoBehaviour
{
    public TextMeshProUGUI eventTextbox;

    private void OnEnable()
    {
        GameManager.OnYearEventChosen += UpdateEventText;

        if (GameManager.Instance != null && GameManager.Instance.currentYearEvent != null)
        {
            UpdateEventText(GameManager.Instance.currentYearEvent);
        }
    }

    private void OnDisable()
    {
        GameManager.OnYearEventChosen -= UpdateEventText;
    }

    private void UpdateEventText(Event eventToPrint)
    {
        if (eventTextbox == null)
        {
            Debug.LogError("EventsDisplay: eventTextbox not assigned.");
            return;
        }

        if (eventToPrint == null)
        {
            Debug.LogError("EventsDisplay: eventToPrint is null.");
            return;
        }

        float percentChange = (eventToPrint.sellPriceChange - 1f) * 100f;

        string priceText;

        if (percentChange > 0)
            priceText = "Prices have gone up by " + Mathf.RoundToInt(percentChange) + "%.";
        else if (percentChange < 0)
            priceText = "Prices have dropped by " + Mathf.Abs(Mathf.RoundToInt(percentChange)) + "%.";
        else
            priceText = "Prices have not changed.";

        string itemList = "";

        if (eventToPrint.itemAffectedType != null && eventToPrint.itemAffectedType.Length > 0)
        {
            for (int i = 0; i < eventToPrint.itemAffectedType.Length; i++)
            {
                itemList += eventToPrint.itemAffectedType[i];

                if (i < eventToPrint.itemAffectedType.Length - 1)
                    itemList += ", ";
            }
        }

        string availabilityText;

        if (eventToPrint.itemTypeAvail > 1f)
            availabilityText = "NPCs have more " + itemList + " than usual.";
        else if (eventToPrint.itemTypeAvail < 1f)
            availabilityText = "NPCs have less " + itemList + " than usual.";
        else
            availabilityText = itemList + " availability is unchanged.";

        string commandText = "In response to the event, do you want to change your prices for this year?\nThis will affect all items.";

        eventTextbox.text =
            "Event: " + eventToPrint.name + "\n\n" +
            priceText + "\n" +
            availabilityText + "\n\n" +
            commandText;
    }
}