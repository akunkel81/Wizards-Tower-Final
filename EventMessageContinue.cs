using UnityEngine;

public class EventMessageContinueButton : MonoBehaviour
{
    public void ContinueToCurrentYear()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("EventMessageContinueButton: GameManager.Instance is null.");
            return;
        }

        GameManager.Instance.LoadCurrentYearScene();
    }

    public void ContinueToNextYearEvent()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("EventMessageContinueButton: GameManager.Instance is null.");
            return;
        }

        GameManager.Instance.StartNextYear();
    }
}