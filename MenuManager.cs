using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static bool anyMenuOpen = false;

    public bool isOpen = false;

    public bool TryOpen(GameObject menu)
    {
        if (anyMenuOpen)
        {
            Debug.Log("Cannot open menu, another is already open.");
            return false;
        }

        if (menu == null)
            return false;

        menu.SetActive(true);

        isOpen = true;
        anyMenuOpen = true;

        Time.timeScale = 0f;

        return true;
    }

    public void Close(GameObject menu)
    {
        if (menu == null)
            return;

        menu.SetActive(false);

        isOpen = false;
        anyMenuOpen = false;

        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        isOpen = false;
        anyMenuOpen = false;
        Time.timeScale = 1f;
    }
}