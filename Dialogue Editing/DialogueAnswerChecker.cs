using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cherrydev;

public class DialogueAnswerController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonRequestLink
    {
        public int buttonIndex;
        public int requestIndex;
        public bool hideIfMissing = false;
    }

    [Header("References")]
    [SerializeField] private AnswerPanel answerPanel;
    [SerializeField] private NPCTradeFunctions npcTradeFunctions;
    [SerializeField] private GameObject dialogueRoot;

    [Header("Button Mappings")]
    [SerializeField] private List<ButtonRequestLink> buttonMappings = new();

    [Header("Options")]
    [SerializeField] private bool runContinuouslyWhileOpen = false;
    [SerializeField] private bool useDelayedRefresh = true;
    [SerializeField] private int delayedFrames = 2;

    private void Awake()
    {
        if (answerPanel == null)
            answerPanel = GetComponent<AnswerPanel>();

        if (npcTradeFunctions == null)
            npcTradeFunctions = FindFirstObjectByType<NPCTradeFunctions>();
    }

    private void LateUpdate()
    {
        if (!runContinuouslyWhileOpen)
            return;

        if (dialogueRoot != null && !dialogueRoot.activeInHierarchy)
            return;

        if (answerPanel == null || npcTradeFunctions == null)
            return;

        ApplyButtonStates();
    }

    public void RefreshButtons()
    {
        if (useDelayedRefresh)
            StartCoroutine(RefreshButtonsDelayed());
        else
            ApplyButtonStates();
    }

    private IEnumerator RefreshButtonsDelayed()
    {
        for (int i = 0; i < delayedFrames; i++)
            yield return null;

        ApplyButtonStates();
    }

    public void ApplyButtonStates()
    {
        if (answerPanel == null)
        {
            Debug.LogError("DialogueAnswerController: AnswerPanel missing.");
            return;
        }

        if (npcTradeFunctions == null)
        {
            Debug.LogError("DialogueAnswerController: NPCTradeFunctions missing.");
            return;
        }

        int buttonCount = answerPanel.GetButtonCount();
        Debug.Log("DialogueAnswerController: button count = " + buttonCount);

        for (int i = 0; i < buttonMappings.Count; i++)
        {
            ButtonRequestLink mapping = buttonMappings[i];

            if (mapping.buttonIndex < 0 || mapping.buttonIndex >= buttonCount)
            {
                Debug.LogWarning("DialogueAnswerController: Invalid button index " + mapping.buttonIndex);
                continue;
            }

            Button button = answerPanel.GetButtonByIndex(mapping.buttonIndex);
            if (button == null)
            {
                Debug.LogWarning("DialogueAnswerController: Button is null at index " + mapping.buttonIndex);
                continue;
            }

            bool hasRequiredPotion = npcTradeFunctions.HasRequestedPotionForTrade(mapping.requestIndex);

            Debug.Log("Checking button " + mapping.buttonIndex +
                    " for request " + mapping.requestIndex +
                    " hasPotion=" + hasRequiredPotion);

            if (mapping.hideIfMissing)
            {
                button.gameObject.SetActive(hasRequiredPotion);
            }
            else
            {
                button.gameObject.SetActive(true);
                button.interactable = hasRequiredPotion;
            }
        }
    }

    public void ResetButtons()
    {
        if (answerPanel == null)
            return;

        int count = answerPanel.GetButtonCount();

        for (int i = 0; i < count; i++)
        {
            Button button = answerPanel.GetButtonByIndex(i);
            if (button == null)
                continue;

            button.gameObject.SetActive(true);
            button.interactable = true;
        }
    }

    public void SetContinuousEnforcement(bool enabled)
    {
        runContinuouslyWhileOpen = enabled;
    }
}
    // void Update()
    // {
    //     NPCsSO currentNPC = actionManager.GetCurrentNPC();
    //     NPCPotionRequest request = currentNPC.potionRequests[0];
    //     int requestAmount = Mathf.Max(1, request.requestAmount);
    //     Debug.Log("Requested: " + requestAmount);
    //     PotionSO requestedPotion = request.potion;

    //     int owned = inventoryManager.GetPotionCount(requestedPotion);
    //     if (owned < requestAmount)
    //     {
    //         Debug.Log("Player does not have enough of requested potion: " + requestedPotion.displayName);
    //         return;
    //     }
    // }
