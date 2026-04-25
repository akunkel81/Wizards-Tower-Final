using UnityEngine;

public class PriceChange : MonoBehaviour
{
    [Header("Step Size")]
    public float stepPercent = 0.1f; // 10%

    [Header("Limits")]
    public float minMultiplier = 0.5f;
    public float maxMultiplier = 2f;

    public void ApplyIncrease()
    {
        AdjustPrice(stepPercent);
    }

    public void ApplyDecrease()
    {
        AdjustPrice(-stepPercent);
    }

    private void AdjustPrice(float percentChange)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PriceChange: GameManager missing.");
            return;
        }

        float current = GameManager.Instance.potionSellMultiplier;

        // Convert to "steps" relative to 1.0
        float offset = current - 1f;

        // Snap to nearest step to prevent drift
        offset = Mathf.Round(offset / stepPercent) * stepPercent;

        // Apply step change
        offset += percentChange;

        // Clamp range
        offset = Mathf.Clamp(offset, minMultiplier - 1f, maxMultiplier - 1f);

        // Rebuild final multiplier
        GameManager.Instance.potionSellMultiplier = 1f + offset;

        Debug.Log("New multiplier: " + GameManager.Instance.potionSellMultiplier);

        FindFirstObjectByType<PriceStatusDisplay>()?.UpdateDisplay();
    }
}