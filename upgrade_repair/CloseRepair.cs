using UnityEngine;

public class CloseRepairMenuButton : MonoBehaviour
{
    public GameObject repairMenu;
    public RepairManager repairManager;

    public void CloseMenu()
    {
        Debug.Log("CloseMenu clicked");

        if (repairMenu == null)
        {
            Debug.LogError("upgradeMenu is not assigned.");
            return;
        }

        repairMenu.SetActive(false);
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