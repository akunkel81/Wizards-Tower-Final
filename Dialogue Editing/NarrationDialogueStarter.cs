using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using cherrydev;

public class NarrationDialogueStarter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph narrationGraph;

    [Header("Scene Flow")]
    [SerializeField] private bool loadNextSceneOnFinish = true;
    [SerializeField] private string nextSceneName = "";

    private void Awake()
    {
        if (dialogBehaviour == null)
            dialogBehaviour = FindFirstObjectByType<DialogBehaviour>();
    }

    private void Start()
    {
        if (dialogBehaviour == null)
        {
            Debug.LogError("NarrationDialogueStarter: DialogBehaviour is not assigned.");
            return;
        }

        if (narrationGraph == null)
        {
            Debug.LogError("NarrationDialogueStarter: narrationGraph is not assigned.");
            return;
        }

        StartCoroutine(StartNarrationNextFrame());
    }

    private IEnumerator StartNarrationNextFrame()
    {
        yield return null;
        dialogBehaviour.StartDialog(narrationGraph);
    }

    public void OnDialogueFinished()
    {
        Debug.Log("Narration finished.");

        if (!loadNextSceneOnFinish)
            return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("NarrationDialogueStarter: nextSceneName is empty.");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}