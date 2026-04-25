using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static System.Action<Event> OnYearEventChosen;

    public string yearOneSceneName = "YearOne";
    public string endYearOneSceneName = "EndYearOneReport";

    [Header("Data Assets")]
    public ItemDatabaseSO itemDatabase;

    [Header("Economy")]
    public float potionSellMultiplier = 1f;        // Player button affects this
    public float ingredientPriceMultiplier = 1f;   // Events affect this

    public float cropAvailabilityMultiplier = 1f;
    public float mineralAvailabilityMultiplier = 1f;
    public float rareAvailabilityMultiplier = 1f;
    public float miscAvailabilityMultiplier = 1f;

    [Header("References")]
    public EventsLoader eventsLoader;
    public EventPicker eventPicker;

    [Header("State")]
    public Event currentYearEvent;

    [Header("Year State")]
    public int currentYear = 1;
    //public int yearTypeIntCast = 1; //0 bad year, 1 good year
    [Header("Year Names")]
    public string yearTwoSceneName = "YearTwo";
    public string yearThreeSceneName = "YearThree";
    public string yearFourSceneName = "YearFour";
    public string yearFiveSceneName = "YearFive";
    public string yearSixSceneName = "YearSix";
    public string yearSevenSceneName = "YearSeven";

    public string eventMessageSceneName = "Event_Message_1";
    public string endYearTwoSceneName = "EndYearTwoReport";
    public string endYearThreeSceneName = "EndYearThreeReport";
    public string endYearFourSceneName = "EndYearFourReport";
    public string endYearFiveSceneName = "EndYearFiveReport";
    public string endYearSixSceneName = "EndYearSixReport";
    public string endYearSevenSceneName = "EndYearSevenReport";
    private bool _initialized;

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
        Initialize();
    }

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        if (eventsLoader == null) eventsLoader = EventsLoader.Instance;
        if (eventPicker == null) eventPicker = FindFirstObjectByType<EventPicker>();

        if (eventsLoader == null || eventPicker == null)
        {
            Debug.LogError("GameManager: Missing EventsLoader or EventPicker.");
            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogError("GameManager: itemDatabase not assigned.  Assign ItemDatabase.asset in the Inspector.");
            return;
        }

        eventsLoader.LoadEventsData();

        if (eventsLoader.eventData == null || eventsLoader.eventData.events == null || eventsLoader.eventData.events.Length == 0)
        {
            Debug.LogError("GameManager: Event data did not load or is empty.");
            return;
        }

        eventPicker.eventsLoader = eventsLoader;
        currentYearEvent = eventPicker.PickRandomEvent();

        if (currentYearEvent != null)
        {
            ApplyYearEventEconomy(currentYearEvent);
            OnYearEventChosen?.Invoke(currentYearEvent);
        }
    }

    public void ApplyYearEventEconomy(Event e)
    {
        if (e == null) return;

        // Event changes ingredient prices only
    ingredientPriceMultiplier *= e.sellPriceChange;

    // Event changes availability by item type
    if (e.itemAffectedType != null)
    {
        foreach (var t in e.itemAffectedType)
        {
            ApplyAvailabilityToType(t, e.itemTypeAvail);
        }
    }
    }

    private void ApplyAvailabilityToType(string itemType, float multiplier)
    {
        if (string.IsNullOrWhiteSpace(itemType)) return;

        if (itemType == "Crop") cropAvailabilityMultiplier *= multiplier;
        else if (itemType == "Mineral") mineralAvailabilityMultiplier *= multiplier;
        else if (itemType == "Rare") rareAvailabilityMultiplier *= multiplier;
        else if (itemType == "Misc") miscAvailabilityMultiplier *= multiplier;
    }

    public int GetEffectivePotionSellPrice(PotionSO potion)
    {
        if (potion == null) return 0;
        return Mathf.RoundToInt(potion.sellPrice * potionSellMultiplier);
    }

    public int GetEffectiveIngredientPrice(IngredientSO ingredient)
        {
            if (ingredient == null) return 0;
            return Mathf.RoundToInt(ingredient.itemPrice * ingredientPriceMultiplier);
        }
        public float GetAvailabilityMultiplierForType(string itemType)
    {
        if (string.IsNullOrWhiteSpace(itemType)) return 1f;

        if (itemType == "Crop") return cropAvailabilityMultiplier;
        if (itemType == "Mineral") return mineralAvailabilityMultiplier;
        if (itemType == "Rare") return rareAvailabilityMultiplier;
        if (itemType == "Misc") return miscAvailabilityMultiplier;

        return 1f;
    }

    public float GetEffectiveIngredientWeight(IngredientSO ingredient)
    {
        if (ingredient == null) return 0f;
        return ingredient.rarity * GetAvailabilityMultiplierForType(ingredient.itemType);
    }

    public bool IsGoodYear()
    {
        if (currentYearEvent == null)
        {
            Debug.LogWarning("No currentYearEvent set.");
            return false;
        }
        PlayerPrefs.SetInt("yearType", 1);
        return currentYearEvent.sellPriceChange < 1.1f;
    }

    public bool IsBadYear()
    {
        if (currentYearEvent == null)
            return true;
        PlayerPrefs.SetInt("yearType", 0);
        return currentYearEvent.sellPriceChange >= 1.1f;
    }
    public bool IsYearOne()
    {
        return currentYear == 1;
    }

    public void StartNextYear()
    {
        currentYear++;

        ClearCurrentYearEvent();
        ResetYearEventEconomy();
        PickAndApplyNewYearEvent();

        if (!string.IsNullOrEmpty(eventMessageSceneName))
        {
            SceneManager.LoadScene(eventMessageSceneName);
        }
        else
        {
            Debug.LogError("EventMessage scene name not set.");
        }
    }

    public void ClearCurrentYearEvent()
    {
        currentYearEvent = null;
    }

    private void ResetYearEventEconomy()
    {
        ingredientPriceMultiplier = 1f;

        cropAvailabilityMultiplier = 1f;
        mineralAvailabilityMultiplier = 1f;
        rareAvailabilityMultiplier = 1f;
        miscAvailabilityMultiplier = 1f;
    }

    private void PickAndApplyNewYearEvent()
    {
        if (eventsLoader == null)
            eventsLoader = EventsLoader.Instance;

        if (eventPicker == null)
            eventPicker = FindFirstObjectByType<EventPicker>();

        if (eventsLoader == null || eventPicker == null)
        {
            Debug.LogError("GameManager: Missing EventsLoader or EventPicker.");
            return;
        }

        if (eventsLoader.eventData == null || eventsLoader.eventData.events == null || eventsLoader.eventData.events.Length == 0)
        {
            eventsLoader.LoadEventsData();
        }

        if (eventsLoader.eventData == null || eventsLoader.eventData.events == null || eventsLoader.eventData.events.Length == 0)
        {
            Debug.LogError("GameManager: Event data did not load or is empty.");
            return;
        }

        eventPicker.eventsLoader = eventsLoader;
        currentYearEvent = eventPicker.PickRandomEvent();

        if (currentYearEvent != null)
        {
            ApplyYearEventEconomy(currentYearEvent);
            OnYearEventChosen?.Invoke(currentYearEvent);
        }
    }
    public void LoadCurrentYearScene()
    {
        if (ActionManager.Instance != null)
        {
            ActionManager.Instance.StartNewYear();
        }

        string sceneToLoad = "";

        switch (currentYear)
        {
            case 1:
                sceneToLoad = yearOneSceneName;
                break;
            case 2:
                sceneToLoad = yearTwoSceneName;
                break;
            case 3:
                sceneToLoad = yearThreeSceneName;
                break;
            case 4:
                sceneToLoad = yearFourSceneName;
                break;
            case 5:
                sceneToLoad = yearFiveSceneName;
                break;
            case 6:
                sceneToLoad = yearSixSceneName;
                break;
            case 7:
                sceneToLoad = yearSevenSceneName;
                break;
            default:
                Debug.LogWarning("No gameplay scene configured for year " + currentYear);
                return;
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
        else
            Debug.LogError("Year scene name is empty for year " + currentYear);
    }
    public void EndCurrentYear()
    {
        string reportScene = "";

        switch (currentYear)
        {
            case 1:
                reportScene = endYearOneSceneName;
                break;
            case 2:
                reportScene = endYearTwoSceneName;
                break;
            case 3:
                reportScene = endYearThreeSceneName;
                break;
            case 4:
                reportScene = endYearFourSceneName;
                break;
            case 5:
                reportScene = endYearFiveSceneName;
                break;
            case 6:
                reportScene = endYearSixSceneName;
                break;
            case 7:
                reportScene = endYearSevenSceneName;
                break;
            default:
                Debug.LogWarning("No end-of-year report scene configured for year " + currentYear);
                return;
        }

        if (!string.IsNullOrEmpty(reportScene))
            SceneManager.LoadScene(reportScene);
        else
            Debug.LogError("End-of-year report scene name not set for year " + currentYear);
    }
}