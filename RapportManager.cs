using UnityEngine;
using UnityEngine.SceneManagement;

public class RapportManager : MonoBehaviour
{
    public static RapportManager Instance { get; private set; }

    [Header("Rapport UI")]
    public SpriteRenderer rapportSpriteRenderer;
    public Sprite rapportLow;
    public Sprite rapportMedium;
    public Sprite rapportHigh;
    public Sprite rapportHighest;

    private const string FarmerKey = "rapportFarmer";
    private const string BakerKey = "rapportBaker";
    private const string SalesmanKey = "rapportSalesman";
    private const string SadFatherKey = "rapportSadFather";
    private const string MinerKey = "rapportMiner";
    private const string MoonGirlKey = "rapportMoonGirl";
    private const string GoldenWizardKey = "rapportGoldenWizard";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeRapportValues();

        GameObject.Find("RapportUI").GetComponent<SpriteRenderer>().enabled = false; //off by default, only used in dialog
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
        RebindRapportSprite();
        RefreshCurrentRapportSprite();
    }

    private void Start()
    {
        RebindRapportSprite();
        RefreshCurrentRapportSprite();
    }

    private void RebindRapportSprite()
    {
        if (rapportSpriteRenderer == null)
            rapportSpriteRenderer = FindFirstObjectByType<RapportManager>()?.rapportSpriteRenderer;
    }

    private void InitializeRapportValues()
    {
        InitializeFloatKey(FarmerKey, 0f);
        InitializeFloatKey(BakerKey, 0f);
        InitializeFloatKey(SalesmanKey, 0f);
        InitializeFloatKey(SadFatherKey, 0f);
        InitializeFloatKey(MinerKey, 0f);
        InitializeFloatKey(MoonGirlKey, 0f);
        InitializeFloatKey(GoldenWizardKey, 0f);

        PlayerPrefs.Save();
    }

    private void InitializeFloatKey(string key, float defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetFloat(key, defaultValue);
    }

    public void ResetAllRapportToZero()
    {
        PlayerPrefs.SetFloat(FarmerKey, 0f);
        PlayerPrefs.SetFloat(BakerKey, 0f);
        PlayerPrefs.SetFloat(SalesmanKey, 0f);
        PlayerPrefs.SetFloat(SadFatherKey, 0f);
        PlayerPrefs.SetFloat(MinerKey, 0f);
        PlayerPrefs.SetFloat(MoonGirlKey, 0f);
        PlayerPrefs.SetFloat(GoldenWizardKey, 0f);
        PlayerPrefs.Save();

        RefreshCurrentRapportSprite();
        Debug.Log("All rapport values reset to 0.");
    }

    public float GetFarmerRapport() => PlayerPrefs.GetFloat(FarmerKey, 0f);
    public float GetBakerRapport() => PlayerPrefs.GetFloat(BakerKey, 0f);
    public float GetSalesmanRapport() => PlayerPrefs.GetFloat(SalesmanKey, 0f);
    public float GetSadFatherRapport() => PlayerPrefs.GetFloat(SadFatherKey, 0f);
    public float GetMinerRapport() => PlayerPrefs.GetFloat(MinerKey, 0f);
    public float GetMoonGirlRapport() => PlayerPrefs.GetFloat(MoonGirlKey, 0f);
    public float GetGoldenWizardRapport() => PlayerPrefs.GetFloat(GoldenWizardKey, 0f);

    public void IncreaseRapportForNPC(NPCsSO npc)
    {
        ModifyRapportByNPC(npc, 1f);
    }

    public void DecreaseRapportForNPC(NPCsSO npc)
    {
        ModifyRapportByNPC(npc, -1f);
    }

    public float GetRapportForNPC(NPCsSO npc)
    {
        string key = GetRapportKeyFromNPC(npc);
        if (string.IsNullOrEmpty(key))
            return 0f;

        return PlayerPrefs.GetFloat(key, 0f);
    }

    private void ModifyRapportByNPC(NPCsSO npc, float amount)
    {
        string key = GetRapportKeyFromNPC(npc);
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("RapportManager: could not find rapport key for NPC.");
            return;
        }

        float oldValue = PlayerPrefs.GetFloat(key, 0f);
        float newValue = oldValue + amount;

        PlayerPrefs.SetFloat(key, newValue);
        PlayerPrefs.Save();

        RefreshCurrentRapportSprite();
        Debug.Log("Updated " + key + " from " + oldValue + " to " + newValue);
    }

    private string GetRapportKeyFromNPC(NPCsSO npc)
    {
        if (npc == null)
            return null;

        string npcName = npc.characterName.Trim();

        if (npcName == "Farmer")
            return FarmerKey;

        if(npcName == "TownBaker" || npcName == "Baker")
            return BakerKey;

        if (npcName == "Salesman")
            return SalesmanKey;

        if (npcName == "SadFather" || npcName == "Sad Father")
            return SadFatherKey;

        if (npcName == "Miner")
            return MinerKey;

        if (npcName == "Moon Girl" || npcName == "MoonGirl")
            return MoonGirlKey;

        if (npcName == "Golden Wizard")
            return GoldenWizardKey;

        Debug.LogWarning("RapportManager: unknown NPC name " + npcName);
        return null;
    }

    public void RefreshCurrentRapportSprite()
    {
        if (rapportSpriteRenderer == null)
            return;

        float rapportRef = GetCurrentRapportValue();

        if (rapportRef <= 0.5f)
            rapportSpriteRenderer.sprite = rapportLow;
        else if (rapportRef <= 1.0f)
            rapportSpriteRenderer.sprite = rapportMedium;
        else if (rapportRef <= 1.5f)
            rapportSpriteRenderer.sprite = rapportHigh;
        else
            rapportSpriteRenderer.sprite = rapportHighest;
    }

    public float GetCurrentRapportValue()
    {
        if (ActionManager.Instance == null)
            return 0f;

        NPCsSO currentNPC = ActionManager.Instance.GetCurrentNPC();
        return GetRapportForNPC(currentNPC);
    }
}