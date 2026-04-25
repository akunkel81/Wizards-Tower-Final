using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradeUI;
    public GameObject upgradeMenu;

    private HashSet<string> purchasedUpgrades = new HashSet<string>();
    public bool inputLocked = false;

    private void Awake()
    {
        LoadPurchasedUpgrades();
    }

    private void OnMouseOver()
    {
        if (upgradeMenu != null && upgradeMenu.activeSelf)
            return;

        if (MenuManager.anyMenuOpen)
            return;

        if (inputLocked)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (upgradeMenu == null)
            {
                Debug.LogError("UpgradeMenu not assigned.");
                return;
            }

            upgradeMenu.SetActive(true);
            MenuManager.anyMenuOpen = true;
            Time.timeScale = 0f;

            Debug.Log("Upgrade menu opened.");
        }
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
    }

    public void ForceCloseMenu()
    {
        if (upgradeMenu != null)
            upgradeMenu.SetActive(false);

        MenuManager.anyMenuOpen = false;
        inputLocked = false;
        Time.timeScale = 1f;
    }

    public bool IsPurchased(Upgrade upgrade)
    {
        if (upgrade == null)
            return false;

        string key = GetUpgradeKey(upgrade);

        if (purchasedUpgrades.Contains(key))
            return true;

        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public bool MarkPurchased(Upgrade upgrade)
    {
        if (upgrade == null)
            return false;

        string key = GetUpgradeKey(upgrade);

        bool added = purchasedUpgrades.Add(key);

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        return added;
    }

    public void ResetPurchasedUpgrade(Upgrade upgrade)
    {
        if (upgrade == null)
            return;

        string key = GetUpgradeKey(upgrade);

        purchasedUpgrades.Remove(key);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    private string GetUpgradeKey(Upgrade upgrade)
    {
        if (!string.IsNullOrWhiteSpace(upgrade.saveKey))
            return upgrade.saveKey;

        return "upgrade_" + upgrade.upgradeName.Replace(" ", "_").ToLowerInvariant();
    }

    private void LoadPurchasedUpgrades()
    {
        purchasedUpgrades.Clear();
    }
}