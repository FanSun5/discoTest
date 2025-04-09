using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;

public class DialogueManager : MonoBehaviour
{
    //ref to our SO
    public DialogueLine currentLine;
    //container for dialogue lines
    public Transform dialogueParent;
    public GameObject dialoguePrefab;
    //prefab for butoon choices
    public GameObject choiceButtonPrefab;
    //container for our button choices
    public Transform choiceParent;
    public Button continueButton;
    public GameObject ImageA;
    public GameObject ImageB;

    public void StartingDialogue()
    {
        UpdateDialogue(currentLine);
    }

    public void UpdateDialogue(DialogueLine dialogueLine)
    {
        currentLine = dialogueLine;

        if (!string.IsNullOrEmpty(currentLine.speakerName))
        {
            int affinity = NPCStatus.Instance.GetAffinity(currentLine.speakerName);

            if (affinity <= 0)
            {
                ShowAffinityZeroWarning(currentLine.speakerName);
                return;
            }
        }

        StartCoroutine(DisplayDialogue(currentLine));
        continueButton.enabled = true;
    }

    //好感度=0時顯示的拒絕對話文字
    void ShowAffinityZeroWarning(string speakerName)
    {
        GameObject warningBubble = Instantiate(dialoguePrefab, dialogueParent);
        TextMeshProUGUI grabTextWarning = warningBubble.GetComponent<TextMeshProUGUI>();
        grabTextWarning.text = $"<b>{speakerName}:</b> I don't want to talk to you.";

        continueButton.gameObject.SetActive(false);
        foreach (Transform _child in choiceParent) Destroy(_child.gameObject);
    }

    /*public void ShowDialogue(DialogueLine _dialogueLine)
    {

        //if we have a next line NOT EMPTY
        if(_dialogueLine.nextLine != null)
        {
            //then turn on the button
            continueButton.gameObject.SetActive(true);
        }

    }*/

    IEnumerator DisplayDialogue(DialogueLine line)
    {
        //if (line == null) return; //if we have nothing set up just exit func here

        foreach (string _dialogueLine in currentLine.dialogueLinesList)
        {

            //make a new copy of button
            GameObject textBubble = Instantiate(dialoguePrefab, dialogueParent);
            //grab getcomponenetinchilder
            TextMeshProUGUI grabText = textBubble.GetComponent<TextMeshProUGUI>();
            //set the text to whatever string we are currently looping
            grabText.text = _dialogueLine;

            if (!string.IsNullOrEmpty(line.speakerName))
            {
                grabText.text = $"<b>{line.speakerName}:</b>{_dialogueLine}";

                if (line.speakerName == "X")
                {
                    ImageA.SetActive(true);
                }
                else
                {
                    ImageA.SetActive(false);
                }
            }
            yield return new WaitForSeconds(1f);

        }
        //ensure continue button is below all chat
        continueButton.transform.SetAsLastSibling();

        //clear old choice buttons so they dont stack
        foreach (Transform _child in choiceParent) Destroy(_child.gameObject);
        //hide the continue button by default
        continueButton.gameObject.SetActive(false);
        //button choices appear after the latest chat line
        choiceParent.transform.SetAsLastSibling();

        if (line.choices != null && line.choices.Length > 0)
        {
            foreach (DialogueChoice choice in line.choices)
            {
                //create a button
                GameObject newButtonChoice = Instantiate(choiceButtonPrefab, choiceParent);
                //come back to when we have options
                TextMeshProUGUI buttonText = newButtonChoice.GetComponentInChildren<TextMeshProUGUI>();

                bool meetsRequirement = true;

                //if req stat field has something, not empty
                if (!string.IsNullOrEmpty(choice.requiredStat))
                {
                    //checks player stats and returns the current value stored in var
                    //int playerStat = GetPlayerStatValue(choice.requiredStat);
                    //checks if it is greater than or equal to required value
                    //if it is set it to true

                    int playerStat = PlayerStats.Instance.GetStat(choice.requiredStat);
                    meetsRequirement = playerStat >= choice.requiredValue;
                }

                //update button text
                buttonText.text = choice.choiceText;
                if (!meetsRequirement)
                {
                    buttonText.text += "<color=red>" + choice.requiredStat +
         ": " + choice.requiredValue + "</color>";
                }

                //grab the button componenet of the choice button
                Button buttonComp = newButtonChoice.GetComponent<Button>();

                buttonComp.onClick.AddListener(() =>
                {
                    if (meetsRequirement)
                    {
                        if (!string.IsNullOrEmpty(choice.rewardStat))
                        {
                            PlayerStats.Instance.IncreaseStat(choice.rewardStat, choice.rewardAmt);
                        }

                        newButtonChoice.GetComponent<OptionalChoices>().SetUp(this, choice.nextLine, choice.choiceText);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine.speakerName))
                        {
                            NPCStatus.Instance.DecreaseAffinity(currentLine.speakerName);
                        }

                        Debug.Log("Chose a locked option. Affinity -1.");
                    }
                });

                buttonComp.interactable = true;

                if (meetsRequirement)
                {
                    newButtonChoice.GetComponent<OptionalChoices>().SetUp(this, choice.nextLine, choice.choiceText);
                }
            }

        }
        else if (line.nextLine != null)
        {
            //if there are no choices but there is a next line show continue button
            continueButton.gameObject.SetActive(true);
            //clear everything out that was set to happen
            //bc we are using the same button for different lines we dont want them to stack over each other
            continueButton.onClick.RemoveAllListeners();
            //when btton is clicked run this code
            continueButton.onClick.AddListener(() =>
            {
                //continue to the next line
                UpdateDialogue(line.nextLine);
                continueButton.gameObject.SetActive(false);
            });
        }
    }

    /*int GetPlayerStatValue(string statName)
    {
        switch (statName.ToLower())
        {
            case "charisma": return PlayerStats.Instance.charisma;
            case "logic": return PlayerStats.Instance.logic;
            default: return 0;
        }
    }*/
}
