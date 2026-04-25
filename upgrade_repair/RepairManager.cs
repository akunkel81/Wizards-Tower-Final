using UnityEngine;

public class RepairManager : MonoBehaviour
{
    public CraftingManager craftingManager;
    public SetPlayerCoins playerCoins;

    public GameObject repairMenu;

    public int halfRepairCost = 25;
    public int fullRepairCost = 50;
    public bool inputLocked = false;
    public static bool anyMenuOpen = false;

    private void Awake()
    {
        RebindReferences();
    }

    private void RebindReferences()
    {
        if (craftingManager == null)
            craftingManager = FindFirstObjectByType<CraftingManager>();

        if (playerCoins == null)
            playerCoins = SetPlayerCoins.Instance;

        if (playerCoins == null)
            playerCoins = FindFirstObjectByType<SetPlayerCoins>();
    }

    private void OnMouseOver()
    {
        if (repairMenu != null && repairMenu.activeSelf)
            return;

        if (MenuManager.anyMenuOpen)
        {
            SetInputLocked(true);
            return;
        }

        if (inputLocked)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (repairMenu == null)
            {
                Debug.LogError("RepairMenu not assigned.");
                return;
            }

            repairMenu.SetActive(true);
            MenuManager.anyMenuOpen = true;
            Debug.Log("anyMenuOpen set to true.");
            Time.timeScale = 0f;

            Debug.Log("Repair menu opened.");
        }
    }
    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;
    }

    public void ForceCloseMenu()
    {
        if (repairMenu != null)
            repairMenu.SetActive(false);

        MenuManager.anyMenuOpen = false;
        Time.timeScale = 1f;
    }

    public bool TryHalfRepair()
    {
        RebindReferences();

        if (craftingManager == null)
            Debug.LogError("RepairManager: craftingManager is null.");

        if (playerCoins == null)
            Debug.LogError("RepairManager: playerCoins is null.");

        if (craftingManager == null || playerCoins == null)
            return false;

        if (craftingManager.currentDurability >= craftingManager.maxDurability)
        {
            Debug.Log("Cauldron already fully repaired.");
            return false;
        }

        bool spent = playerCoins.SpendCoins(halfRepairCost);
        if (!spent)
        {
            Debug.Log("Not enough coins for half repair.");
            return false;
        }

        craftingManager.RepairHalf();
        return true;
    }

    public bool TryFullRepair()
    {
        RebindReferences();

        if (craftingManager == null)
            Debug.LogError("RepairManager: craftingManager is null.");

        if (playerCoins == null)
            Debug.LogError("RepairManager: playerCoins is null.");

        if (craftingManager == null || playerCoins == null)
            return false;

        if (craftingManager.currentDurability >= craftingManager.maxDurability)
        {
            Debug.Log("Cauldron already fully repaired.");
            return false;
        }

        bool spent = playerCoins.SpendCoins(fullRepairCost);
        if (!spent)
        {
            Debug.Log("Not enough coins for full repair.");
            return false;
        }

        craftingManager.RepairFull();
        return true;
    }
}