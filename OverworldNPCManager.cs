using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldNPCManager : MonoBehaviour
{
    public static OverworldNPCManager Instance { get; private set; }

    [Header("Overworld NPC")]
    public SpriteRenderer overworldSpriteRenderer;
    public Sprite farmer;
    public Sprite baker;
    public Sprite salesman;
    public Sprite sadFather;
    public Sprite miner;
    public Sprite moonGirl;

    private const string FarmerKey = "OverworldFarmer";
    private const string BakerKey = "OverworldBaker";
    private const string SalesmanKey = "OverworldSalesman";
    private const string SadFatherKey = "OverworldSadFather";
    private const string MinerKey = "OverworldMiner";
    private const string MoonGirlKey = "OverworldMoonGirl";
    private const string GoldenWizardKey = "OverworldGoldenWizard";

    public string npcName;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //InitializeOverworldValues();

        GameObject.Find("OverworldUI").GetComponent<SpriteRenderer>().enabled = false; //off by default, only used in dialog
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
        RebindNPCSprite();
        RefreshCurrentOverworldSprite();
    }

    private void Start()
    {
        RebindNPCSprite();
        RefreshCurrentOverworldSprite();
    }

    private void RebindNPCSprite()
    {
        if (overworldSpriteRenderer == null)
        {
            GameObject uiObj = GameObject.Find("OverworldUI");
            if (uiObj != null)
                overworldSpriteRenderer = uiObj.GetComponent<SpriteRenderer>();
        }
    }

    //private void InitializeOverworldValues()
    //{
    //    InitializeFloatKey(FarmerKey, 0f);
    //    InitializeFloatKey(BakerKey, 0f);
    //    InitializeFloatKey(SalesmanKey, 0f);
    //    InitializeFloatKey(SadFatherKey, 0f);
    //    InitializeFloatKey(MinerKey, 0f);
    //    InitializeFloatKey(MoonGirlKey, 0f);
    //    InitializeFloatKey(GoldenWizardKey, 0f);

    //    PlayerPrefs.Save();
    //}

    private void InitializeFloatKey(string key, float defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetFloat(key, defaultValue);
    }

    //public void ResetAllOverworldToZero()
    //{
    //    PlayerPrefs.SetFloat(FarmerKey, 0f);
    //    PlayerPrefs.SetFloat(BakerKey, 0f);
    //    PlayerPrefs.SetFloat(SalesmanKey, 0f);
    //    PlayerPrefs.SetFloat(SadFatherKey, 0f);
    //    PlayerPrefs.SetFloat(MinerKey, 0f);
    //    PlayerPrefs.SetFloat(MoonGirlKey, 0f);
    //    PlayerPrefs.SetFloat(GoldenWizardKey, 0f);
    //    PlayerPrefs.Save();

    //    RefreshCurrentOverworldSprite();
    //    Debug.Log("All Overworld values reset to 0.");
    //}

    //public float GetFarmerOverworld() => PlayerPrefs.GetFloat(FarmerKey, 0f);
    //public float GetBakerOverworld() => PlayerPrefs.GetFloat(BakerKey, 0f);
    //public float GetSalesmanOverworld() => PlayerPrefs.GetFloat(SalesmanKey, 0f);
    //public float GetSadFatherOverworld() => PlayerPrefs.GetFloat(SadFatherKey, 0f);
    //public float GetMinerOverworld() => PlayerPrefs.GetFloat(MinerKey, 0f);
    //public float GetMoonGirlOverworld() => PlayerPrefs.GetFloat(MoonGirlKey, 0f);
    //public float GetGoldenWizardOverworld() => PlayerPrefs.GetFloat(GoldenWizardKey, 0f);


    private string GetNPCName(NPCsSO npc)
    {
        if (npc == null)
            return null;

        npcName = npc.characterName.Trim();

        return npcName;

        //if (npcName == "Farmer")
        //    return FarmerKey;

        //if (npcName == "TownBaker")
        //    return BakerKey;

        //if (npcName == "Salesman")
        //    return SalesmanKey;

        //if (npcName == "SadFather" || npcName == "Sad Father")
        //    return SadFatherKey;

        //if (npcName == "Miner")
        //    return MinerKey;

        //if (npcName == "Moon Girl" || npcName == "MoonGirl")
        //    return MoonGirlKey;

        //if (npcName == "Golden Wizard")
        //    return GoldenWizardKey;

        //Debug.LogWarning("OverworldManager: unknown NPC name " + npcName);
        //return null;
    }

        public void RefreshCurrentOverworldSprite()
    {
        if (overworldSpriteRenderer == null)
            return;

        if (ActionManager.Instance == null)
            return;

        NPCsSO currentNPC = ActionManager.Instance.GetCurrentNPC();
        if (currentNPC == null)
        {
            Debug.LogWarning("OverworldNPCManager: current NPC is null.");
            return;
        }

        npcName = currentNPC.characterName.Trim();

        if (npcName == "Farmer")
            overworldSpriteRenderer.sprite = farmer;
        else if (npcName == "TownBaker" || npcName == "Baker")
            overworldSpriteRenderer.sprite = baker;
        else if (npcName == "Salesman")
            overworldSpriteRenderer.sprite = salesman;
        else if (npcName == "SadFather" || npcName == "Sad Father")
            overworldSpriteRenderer.sprite = sadFather;
        else if (npcName == "Miner")
            overworldSpriteRenderer.sprite = miner;
        else if (npcName == "Moon Girl" || npcName == "MoonGirl")
            overworldSpriteRenderer.sprite = moonGirl;
        else
            Debug.LogWarning("OverworldNPCManager: unknown NPC name " + npcName);
    }

    public void ShowCurrentNPCSprite()
    {
        RebindNPCSprite();
        RefreshCurrentOverworldSprite();

        if (overworldSpriteRenderer != null)
            overworldSpriteRenderer.enabled = true;
    }

    public void HideNPCSprite()
    {
        if (overworldSpriteRenderer != null)
            overworldSpriteRenderer.enabled = false;
    }

}