//using UnityEngine;

//public class MenuSwap : MonoBehaviour
//{
//    public UpgradeManager upgradeManager;
//    public InventoryManager inventoryManager;
//    public CraftingManager craftingManager;


//    // methods used to make sure multiple menus can't be opened

//    public void SwapToInventory()
//    {
//        upgradeManager.ForceCloseMenu();
//        craftingManager.ForceCloseMenu();
//    }

//    public void SwapToCrafting()
//    {
//        upgradeManager.ForceCloseMenu();
//        inventoryManager.ForceCloseInventory();
//    }

//    public void SwapToUpgrade()
//    {
//        inventoryManager.ForceCloseInventory();
//        craftingManager.ForceCloseMenu();
//    }

//    public void CloseAll() // might need this we shall see 
//    {
//        upgradeManager.ForceCloseMenu();
//        craftingManager.ForceCloseMenu();
//        inventoryManager.ForceCloseInventory();
//    }
//    void Start()
//    {

//    }
//    void Update()
//    {
        
//    }
//}
