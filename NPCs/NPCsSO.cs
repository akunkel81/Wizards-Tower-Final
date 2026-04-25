using UnityEngine;
using cherrydev;

[System.Serializable]
public class NPCIngredientOffer
{
    public IngredientSO ingredient;
    public int baseAmount = 1;
}

[System.Serializable]
public class NPCPotionRequest
{
    public PotionSO potion;
    public int requestAmount = 1;
}

[CreateAssetMenu(fileName = "NPCsSO", menuName = "Scriptable Objects/NPCsSO")]
public class NPCsSO : ScriptableObject
{
    public Sprite headSprite;
    public Sprite overworldSprite;
    public string characterName;
    public int characterCoinCount;

    public TrackedVariable[] trackedVariables;

    public NPCIngredientOffer[] ingredientOffers;
    public NPCPotionRequest[] potionRequests;

    public DialogNodeGraph yearOneGraph;
    public DialogNodeGraph nthYearGraph;

    public void SaveTrackedVariables()
    {
        foreach (TrackedVariable variable in trackedVariables)
        {
            variable.SaveToPrefs();
        }
    }

    public void LoadTrackedVariables()
    {
        foreach (TrackedVariable variable in trackedVariables)
        {
            variable.LoadFromPrefs();
        }
    }

}