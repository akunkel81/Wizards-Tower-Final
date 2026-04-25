using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetPlayerCoins : MonoBehaviour
{
    public static SetPlayerCoins Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI coinAmount;

    [Header("Testing")]
    public bool startFreshEachPlay = true;
    public int startingCoins = 1000;

    private int currentCoins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (startFreshEachPlay && !PlayerPrefs.HasKey("GameStarted"))
        {
            StartNewGame();
            PlayerPrefs.SetInt("GameStarted", 1);
            PlayerPrefs.Save();
        }
        else
        {
            LoadCoins();
        }

        UpdateCoinText();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindCoinText();
        LoadCoins();
        UpdateCoinText();
    }

    private void RebindCoinText()
    {
        if (coinAmount == null)
        {
            CoinTextTag found = FindFirstObjectByType<CoinTextTag>();
            if (found != null)
                coinAmount = found.GetComponent<TextMeshProUGUI>();
        }
    }

    public void StartNewGame()
    {
        currentCoins = startingCoins;
        SaveCoins();
        Debug.Log("Started fresh with coins = " + currentCoins);
    }

    public void LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt("PlayerCoins", startingCoins);
        Debug.Log("Loaded PlayerCoins = " + currentCoins);
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", currentCoins);
        PlayerPrefs.Save();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveCoins();
        UpdateCoinText();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
        {
            Debug.Log("Not enough coins.");
            return false;
        }

        currentCoins -= amount;
        SaveCoins();
        UpdateCoinText();
        return true;
    }

    private void UpdateCoinText()
    {
        if (coinAmount != null)
            coinAmount.text = currentCoins.ToString();
    }

    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    public void ClearSavedCoins()
    {
        PlayerPrefs.DeleteKey("PlayerCoins");
        PlayerPrefs.DeleteKey("GameStarted");
        PlayerPrefs.Save();
        Debug.Log("Deleted saved PlayerCoins and GameStarted flag.");
    }
}