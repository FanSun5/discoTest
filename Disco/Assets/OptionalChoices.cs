using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionalChoices : MonoBehaviour
{
    DialogueManager dialogueManager;
    DialogueLine dialogueLine;
    public TextMeshProUGUI dialogueText;

    public void SetUp(DialogueManager _dialogueManager, DialogueLine _dialogueLine, string _dialogueText)
    {
        dialogueManager = _dialogueManager;
        dialogueLine = _dialogueLine;
        dialogueText.text = _dialogueText;
    }

    public void SelectOption()
    {
        if (dialogueManager == null || dialogueLine == null)
        {
            Debug.LogWarning("OptionalChoices: dialogueManager or dialogueLine is null. SetUp() may not have been called.");
            return;
        }

        dialogueManager.UpdateDialogue(dialogueLine);
    }
}
