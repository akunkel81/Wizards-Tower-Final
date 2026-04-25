using UnityEngine;
using cherrydev;
//using Math;

public class DialogChoice : MonoBehaviour
{
    public static DialogBehaviour dialogBehaviour;
    public static ActionManager actionManager;
    int dialogChoice = dialogBehaviour.GetVariableValue<int>("Dialog Choice");
    //DialogBehaviour.SetVariableValue("Dialog Choice", 0);

    public void setNPCStock(int choice)
    {
        choice = Random.Range(1, 3);
    }
}


