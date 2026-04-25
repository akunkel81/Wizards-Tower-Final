using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetAllData : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "StartNarration";

    public void StartNewGame()
    {
        Debug.Log("Starting new game");

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ClearInventoryForNewGameOnly();

        DestroyPersistentManagers();

        Time.timeScale = 1f;

        SceneManager.LoadScene(firstSceneName);
    }

    private void ClearInventoryForNewGameOnly()
    {
        InventoryManager inventory = InventoryManager.Instance != null
            ? InventoryManager.Instance
            : FindFirstObjectByType<InventoryManager>();

        if (inventory != null)
        {
            inventory.ClearInventory();

            inventory.ingredientCounts.Clear();
            inventory.potionCounts.Clear();

            Debug.Log("Inventory fully cleared for new game.");
        }
        else
        {
            Debug.LogWarning("No InventoryManager found to clear.");
        }
    }

    private void DestroyPersistentManagers()
    {
        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        if (ActionManager.Instance != null)
            Destroy(ActionManager.Instance.gameObject);

        if (RapportManager.Instance != null)
            Destroy(RapportManager.Instance.gameObject);

        if (SetPlayerCoins.Instance != null)
            Destroy(SetPlayerCoins.Instance.gameObject);

        if (OverworldNPCManager.Instance != null)
            Destroy(OverworldNPCManager.Instance.gameObject);
    }

    [ContextMenu("Reset All Saved Data")]
    public void ResetEverything()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("All PlayerPrefs data cleared.");
    }
}