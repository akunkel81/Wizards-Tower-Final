using UnityEngine;
using TMPro;
using System.Text;

public class EndOfYearReport : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RapportManager rapportManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI reportText;

    private void Awake()
    {
        if (rapportManager == null)
            rapportManager = FindFirstObjectByType<RapportManager>();
    }

    private void Start()
    {
        if (rapportManager == null)
        {
            Debug.LogError("EndOfYearReport: RapportManager not found.");
            return;
        }

        Debug.Log("Farmer Rapport: " + rapportManager.GetFarmerRapport());
        Debug.Log("Baker Rapport: " + rapportManager.GetBakerRapport());
        Debug.Log("Salesman Rapport: " + rapportManager.GetSalesmanRapport());
        Debug.Log("Sad Father Rapport: " + rapportManager.GetSadFatherRapport());
        Debug.Log("Miner Rapport: " + rapportManager.GetMinerRapport());
        Debug.Log("Moon Girl Rapport: " + rapportManager.GetMoonGirlRapport());

        ShowReport();
    }

    public void ShowReport()
    {
        int totalCoins = PlayerPrefs.GetInt("PlayerCoins", 0);

        if (coinsText != null)
            coinsText.text = "Total Player Coins: $" + totalCoins;

        if (reportText == null)
            return;

        if (rapportManager == null)
        {
            reportText.text = "RapportManager not found.";
            Debug.LogError("EndOfYearReport: RapportManager not found in scene.");
            return;
        }

        StringBuilder sb = new StringBuilder();

        AddCharacterReport(sb, "Farmer", rapportManager.GetFarmerRapport());
        AddCharacterReport(sb, "Baker", rapportManager.GetBakerRapport());
        AddCharacterReport(sb, "Salesman", rapportManager.GetSalesmanRapport());
        AddCharacterReport(sb, "Sad Father", rapportManager.GetSadFatherRapport());
        AddCharacterReport(sb, "Miner", rapportManager.GetMinerRapport());
        AddCharacterReport(sb, "Moon Girl", rapportManager.GetMoonGirlRapport());

        reportText.text = sb.ToString();
    }

    private void AddCharacterReport(StringBuilder sb, string characterName, float rapportValue)
    {
        Debug.Log("EndOfYearReport reading " + characterName + " rapport = " + rapportValue);

        string status = GetRapportStatus(rapportValue);
        sb.AppendLine(characterName + ": " + status + " (" + rapportValue.ToString("0.0") + ")");
    }

    private string GetRapportStatus(float rapportValue)
    {
        if (rapportValue < 0f)
            return "Poor";

        if (rapportValue <= 0.5f)
            return "Low";

        if (rapportValue <= 1.5f)
            return "Medium";

        if (rapportValue <= 3.0f)
            return "High";

        return "Excellent";
    }
}