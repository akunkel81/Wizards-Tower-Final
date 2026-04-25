using UnityEngine;

public class EndReport : MonoBehaviour
{
public void EndYear()
{
    if (GameManager.Instance == null) return;

    GameManager.Instance.EndCurrentYear();
}
}
