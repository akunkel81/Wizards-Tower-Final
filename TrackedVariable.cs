using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class TrackedVariable
{
    public string npcName;
    public string key;
    public float value;

    public void SaveToPrefs()
    {
        PlayerPrefs.SetFloat("V_" + npcName + "_" + key, value);
    }

    public void LoadFromPrefs()
    {
        string fullKey = "V_" + npcName + "_" + key;
        if (PlayerPrefs.HasKey(fullKey))
        {
            value = PlayerPrefs.GetFloat(fullKey);
        }
    }
}

