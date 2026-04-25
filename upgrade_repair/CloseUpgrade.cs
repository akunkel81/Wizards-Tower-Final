using UnityEngine;

public class CloseUpgradeMenuButton : MonoBehaviour
{
    public GameObject upgradeMenu;

    public void CloseMenu()
    {
        Debug.Log("CloseMenu clicked");

        if (upgradeMenu == null)
        {
            Debug.LogError("upgradeMenu is not assigned.");
            return;
        }

        upgradeMenu.SetActive(false);
        Time.timeScale = 1f;
                MenuManager.anyMenuOpen = false;
                CraftingManager crafting = FindFirstObjectByType<CraftingManager>();
        if (crafting != null)
            crafting.SetInputLocked(false);

        RepairManager repair = FindFirstObjectByType<RepairManager>();
        if (repair != null)
            repair.SetInputLocked(false);

        UpgradeManager upgrade = FindFirstObjectByType<UpgradeManager>();
        if (upgrade != null)
            upgrade.SetInputLocked(false);
    }
}