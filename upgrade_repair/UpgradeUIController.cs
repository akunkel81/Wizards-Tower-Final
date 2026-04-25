using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIController : MonoBehaviour
{
    [Header("Data")]
    public Upgrade[] upgrades;

    [Header("Left List")]
    public upgradeSlot[] slots;

    [Header("Right Detail UI")]
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradeDescriptionText;
    public TextMeshProUGUI upgradeCostText;

    [Header("Buy Button")]
    public Button buyButton;

    [Header("References")]
    public CraftingManager craftingManager;
    public SetPlayerCoins playerCoins;

    [Header("Ownership UI")]
    public TextMeshProUGUI ownershipText;

    [Header("Upgrade Image UI")]
    public Image upgradeImage;

    private Upgrade _selectedUpgrade;
    public UpgradeManager upgradeManager;

    private void Awake()
    {
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
    }

    private void OnEnable()
    {
        Debug.Log("Rebinding UpgradeUIController references");
        Debug.Log("SetPlayerCoins.Instance = " + SetPlayerCoins.Instance);

        upgradeManager = FindFirstObjectByType<UpgradeManager>();

        playerCoins = SetPlayerCoins.Instance;
        if (playerCoins == null)
            playerCoins = FindFirstObjectByType<SetPlayerCoins>();

        craftingManager = FindFirstObjectByType<CraftingManager>();

        PopulateLeftList();

        if (upgrades != null && upgrades.Length > 0)
        {
            SelectUpgrade(upgrades[0]);
        }
        else
        {
            ClearRightSide();
        }
    }

    private void PopulateLeftList()
    {
        if (upgrades == null || upgrades.Length == 0)
        {
            Debug.LogError("UpgradeUIController: upgrades array is empty.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            Upgrade u = (i < upgrades.Length) ? upgrades[i] : null;

            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(u != null);

                if (u != null)
                    slots[i].Bind(u, this);
            }
        }
    }

    public void SelectUpgrade(Upgrade upgrade)
    {
        _selectedUpgrade = upgrade;

        if (upgradeNameText != null)
            upgradeNameText.text = upgrade.upgradeName;

        if (upgradeDescriptionText != null)
            upgradeDescriptionText.text = upgrade.upgradeDescription;

        if (upgradeCostText != null)
            upgradeCostText.text = "$" + upgrade.upgradeCost;

        if (upgradeImage != null)
            upgradeImage.sprite = upgrade.upgradeImage;

        UpdateOwnershipText();
        UpdateBuyButton();
    }

    private void UpdateBuyButton()
    {
        if (buyButton == null)
            return;

        if (_selectedUpgrade == null || playerCoins == null || craftingManager == null || upgradeManager == null)
        {
            buyButton.interactable = false;

            Debug.Log(
                "UpdateBuyButton blocked | " +
                "selected=" + (_selectedUpgrade != null) +
                " playerCoins=" + (playerCoins != null) +
                " craftingManager=" + (craftingManager != null) +
                " upgradeManager=" + (upgradeManager != null)
            );

            UpdateOwnershipText();
            return;
        }

        bool alreadyBought = upgradeManager.IsPurchased(_selectedUpgrade);
        bool enoughCoins = playerCoins.GetCurrentCoins() >= _selectedUpgrade.upgradeCost;

        buyButton.interactable = !alreadyBought && enoughCoins;

        Debug.Log(
            "UpdateBuyButton | alreadyBought=" + alreadyBought +
            " enoughCoins=" + enoughCoins +
            " currentCoins=" + playerCoins.GetCurrentCoins() +
            " cost=" + _selectedUpgrade.upgradeCost
        );

        UpdateOwnershipText();
    }

    private void UpdateOwnershipText()
    {
        if (ownershipText == null)
            return;

        if (_selectedUpgrade == null || upgradeManager == null)
        {
            ownershipText.text = "";
            return;
        }

        bool owned = upgradeManager.IsPurchased(_selectedUpgrade);
        ownershipText.text = owned ? "OWNED" : "NOT OWNED";
    }

    public void OnClickBuy()
    {
        Debug.Log("OnClickBuy called");

        if (_selectedUpgrade == null)
        {
            Debug.Log("No upgrade selected.");
            return;
        }

        if (playerCoins == null || craftingManager == null || upgradeManager == null)
        {
            Debug.LogError("UpgradeUIController: missing references.");
            return;
        }

        if (upgradeManager.IsPurchased(_selectedUpgrade))
        {
            Debug.Log("Upgrade already purchased.");
            return;
        }

        bool spent = playerCoins.SpendCoins(_selectedUpgrade.upgradeCost);

        if (!spent)
        {
            Debug.Log("Not enough coins for upgrade.");
            return;
        }

        craftingManager.ApplyUpgrade(_selectedUpgrade);
        upgradeManager.MarkPurchased(_selectedUpgrade);

        Debug.Log("Bought upgrade: " + _selectedUpgrade.upgradeName);

        UpdateBuyButton();

        ActionManager actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager != null)
        {
            actionManager.UseAction(1);
        }
    }

    private void ClearRightSide()
    {
        _selectedUpgrade = null;

        if (upgradeNameText != null) upgradeNameText.text = "";
        if (upgradeDescriptionText != null) upgradeDescriptionText.text = "";
        if (upgradeCostText != null) upgradeCostText.text = "";

        if (upgradeImage != null)
            upgradeImage.sprite = null;

        if (buyButton != null)
            buyButton.interactable = false;
    }
}