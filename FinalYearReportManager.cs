using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalYearReportManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI playerEndingText;
    [SerializeField] private TextMeshProUGUI npcEndingText;

    [Header("Optional Buttons")]
    [SerializeField] private Button repurchaseButton;
    [SerializeField] private Button leaveTownButton;
    [SerializeField] private Button secretHomeButton;

    [Header("Ending Scenes")]
    [SerializeField] private string repurchaseEndingSceneName = "Ending_RepurchaseContract";
    [SerializeField] private string leaveTownEndingSceneName = "Ending_LeaveTown";
    [SerializeField] private string secretHomeEndingSceneName = "Ending_SecretHome";

    [Header("Contract")]
    [SerializeField] private int contractCost = 600;
    [SerializeField] private float secretHomeThreshold = 10f;

    private const string FarmerKey = "rapportFarmer";
    private const string BakerKey = "rapportBaker";
    private const string SalesmanKey = "rapportSalesman";
    private const string SadFatherKey = "rapportSadFather";
    private const string MinerKey = "rapportMiner";
    private const string MoonGirlKey = "rapportMoonGirl";
    private const string GoldenWizardKey = "rapportGoldenWizard";

    private int playerCoins;

    private float farmerRapport;
    private float bakerRapport;
    private float salesmanRapport;
    private float sadFatherRapport;
    private float minerRapport;
    private float moonGirlRapport;
    private float goldenWizardRapport;

    private float totalRapport;

    private void Start()
    {
        LoadData();
        RefreshUI();
    }

    private void LoadData()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);

        farmerRapport = PlayerPrefs.GetFloat(FarmerKey, 0f);
        bakerRapport = PlayerPrefs.GetFloat(BakerKey, 0f);
        salesmanRapport = PlayerPrefs.GetFloat(SalesmanKey, 0f);
        sadFatherRapport = PlayerPrefs.GetFloat(SadFatherKey, 0f);
        minerRapport = PlayerPrefs.GetFloat(MinerKey, 0f);
        moonGirlRapport = PlayerPrefs.GetFloat(MoonGirlKey, 0f);
        goldenWizardRapport = PlayerPrefs.GetFloat(GoldenWizardKey, 0f);

        totalRapport =
            farmerRapport +
            bakerRapport +
            salesmanRapport +
            sadFatherRapport +
            minerRapport +
            moonGirlRapport +
            goldenWizardRapport;
    }

    private void RefreshUI()
    {
        ShowOptions();
        RefreshButtons();
        ShowDefaultPreview();
    }

    private void ShowOptions()
    {
        if (optionsText == null)
            return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Final Choice");
        sb.AppendLine();
        sb.AppendLine("Player Coins: $" + playerCoins);
        sb.AppendLine("Total Rapport: " + totalRapport.ToString("0.0"));
        sb.AppendLine();

        if (CanRepurchaseContract())
            sb.AppendLine("1. Repurchase contract ($" + contractCost + ")");
        else
            sb.AppendLine("1. Repurchase contract ($" + contractCost + ") [Unavailable]");

        sb.AppendLine("2. Leave the town");

        if (CanUnlockSecretHomeEnding())
            sb.AppendLine("3. Townsfolk help build you a new home");

        optionsText.text = sb.ToString();
    }

    private void RefreshButtons()
    {
        if (repurchaseButton != null)
            repurchaseButton.interactable = CanRepurchaseContract();

        if (leaveTownButton != null)
            leaveTownButton.interactable = true;

        if (secretHomeButton != null)
            secretHomeButton.gameObject.SetActive(CanUnlockSecretHomeEnding());
    }

    private void ShowDefaultPreview()
    {
        if (playerEndingText != null)
            playerEndingText.text = "Choose your final outcome.";

        if (npcEndingText != null)
            npcEndingText.text = BuildNpcEpilogues();
    }

    public bool CanRepurchaseContract()
    {
        return playerCoins >= contractCost;
    }

    public bool CanUnlockSecretHomeEnding()
    {
        return totalRapport >= secretHomeThreshold;
    }

    public void ChooseRepurchaseContract()
    {
        if (!CanRepurchaseContract())
        {
            Debug.Log("Not enough money to repurchase contract.");
            return;
        }

        if (playerEndingText != null)
        {
            playerEndingText.text =
                "You repurchase your contract for $" + contractCost + ".\n\n" +
                "Your future remains uncertain, but it is yours again.";
        }

        if (npcEndingText != null)
            npcEndingText.text = BuildNpcEpilogues();

        LoadEndingScene(repurchaseEndingSceneName);
    }

    public void ChooseLeaveTown()
    {
        if (playerEndingText != null)
        {
            playerEndingText.text =
                "You decide to leave the town.\n\n" +
                "You gather what you can carry and prepare to begin again elsewhere.";
        }

        if (npcEndingText != null)
            npcEndingText.text = BuildNpcEpilogues();

        LoadEndingScene(leaveTownEndingSceneName);
    }

    public void ChooseSecretHomeEnding()
    {
        if (!CanUnlockSecretHomeEnding())
        {
            Debug.Log("Secret ending not unlocked.");
            return;
        }

        if (playerEndingText != null)
        {
            playerEndingText.text =
                "As you prepare to leave, the townsfolk stop you.\n\n" +
                "For all the help you gave them, they come together to build you a new home.\n\n" +
                "You stay, not because of the contract, but because you finally belong.";
        }

        if (npcEndingText != null)
            npcEndingText.text = BuildNpcEpilogues();

        LoadEndingScene(secretHomeEndingSceneName);
    }

    private void LoadEndingScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("FinalYearReportManager: ending scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private string BuildNpcEpilogues()
    {
        StringBuilder sb = new StringBuilder();

        AppendFarmerEnding(sb);
        AppendMinerEnding(sb);
        AppendMoonGirlEnding(sb);
        AppendSadFatherEnding(sb);
        AppendSalesmanEnding(sb);
        AppendBakerEnding(sb);

        return sb.ToString();
    }

    private void AppendFarmerEnding(StringBuilder sb)
    {
        sb.AppendLine("Farmer");
        if (farmerRapport >= 1f)
            sb.AppendLine("Her fields and crops flourish under your help, and she is able to pursue some of her own hobbies in her free time.");
        else
            sb.AppendLine("Maintaining her field is a struggle, and she must commit all of her time and energy to preventing town starvation.");
        sb.AppendLine();
    }

    private void AppendMinerEnding(StringBuilder sb)
    {
        sb.AppendLine("Miner");
        if (minerRapport >= 1f)
            sb.AppendLine("She finds extra gold and minerals from her digs and gets to live lavishly, donating her extra spoils to the rest of the town.");
        else
            sb.AppendLine("The back pain from all of her digs overwhelms her, and she is unable to continue mining for the town.");
        sb.AppendLine();
    }

    private void AppendMoonGirlEnding(StringBuilder sb)
    {
        sb.AppendLine("Moon Girl");
        if (moonGirlRapport >= 1f)
            sb.AppendLine("Her potion gets made and her customer is very happy. It is implied she helped an old sapphic couple get together, including her customer, a former widow.");
        else
            sb.AppendLine("Her potion does not get made, and her reputation in town grows worse as she is seen as a useless alchemist who cannot properly meet requests.");
        sb.AppendLine();
    }

    private void AppendSadFatherEnding(StringBuilder sb)
    {
        sb.AppendLine("Sad Father");
        if (sadFatherRapport >= 1f)
            sb.AppendLine("His son gets over his illness and goes on to thrive.");
        else
            sb.AppendLine("His son is unable to recover and remains sickly.");
        sb.AppendLine();
    }

    private void AppendSalesmanEnding(StringBuilder sb)
    {
        sb.AppendLine("Salesman");
        if (salesmanRapport >= 1f)
            sb.AppendLine("His business takes off and everyone is buying weather stones.");
        else
            sb.AppendLine("His business fails and he has to leave town to keep pursuing entrepreneurship.");
        sb.AppendLine();
    }

    private void AppendBakerEnding(StringBuilder sb)
    {
        sb.AppendLine("Town Baker");
        if (bakerRapport >= 1f)
            sb.AppendLine("He is able to find love and woos the woman he has been pursuing.");
        else
            sb.AppendLine("He gets caught for using love potions and his crush leaves him.");
        sb.AppendLine();
    }
}