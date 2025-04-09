using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCStatus : MonoBehaviour

{
    public TextMeshProUGUI affinityTextUI;
    public static NPCStatus Instance;

    private Dictionary<string, int> npcAffinity = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetAffinity(string npcName)
    {
        if (!npcAffinity.ContainsKey(npcName)) npcAffinity[npcName] = 3;
        return npcAffinity[npcName];
    }

    public void DecreaseAffinity(string npcName)
    {
        if (!npcAffinity.ContainsKey(npcName)) npcAffinity[npcName] = 3;
        npcAffinity[npcName] = Mathf.Max(0, npcAffinity[npcName] - 1);
        Debug.Log($"[NPC {npcName}] affinity -1£¬now {npcAffinity[npcName]}");
    }

    public bool CanTalk(string npcName)
    {
        if (string.IsNullOrEmpty(npcName)) return true;

        if (!npcAffinity.ContainsKey(npcName))
        {
            npcAffinity[npcName] = 3; 
            Debug.Log($" {npcName} Affinity=3");
        }

        return npcAffinity[npcName] > 0;
    }

    private void Update()
    {
        if (affinityTextUI == null) return;

        affinityTextUI.text = "";

        foreach (var pair in npcAffinity)
        {
            string color = pair.Value == 0 ? "<color=red>" : "<color=white>";
            affinityTextUI.text += $"{pair.Key}: {color}{pair.Value}</color>\n";
        }
    }
}
