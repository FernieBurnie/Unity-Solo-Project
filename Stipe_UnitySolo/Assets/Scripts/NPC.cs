using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
//When using the prefabs, reassign everything

public class NPC : MonoBehaviour
{
    public static bool hasSpokenToNPC2 = false;

    public GameObject dialoguePanel;
    public GameObject triggerCanvas;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    public int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;

    public GameObject Choice1;
    public GameObject Choice2;
    public string[] choice1Dialogue;
    public string[] choice2Dialogue;
    public int choiceTrigger = 1;

    public string[] unlockedDialogue;

    private bool usingBranch = false;
    private int dialogueStage = 0;

    void Start()
    {
        HideChoices();
    }

    void Update()
    {
        if (playerIsClose && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
            else if (dialogueText.text == dialogue[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogue[index];
            }
        }
    }
    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        usingBranch = false;
        dialoguePanel.SetActive(false);
        HideChoices();
    }

    IEnumerator Typing()
    {
        contButton.SetActive(false);
        HideChoices();

        string currentLine = "";

        if (hasSpokenToNPC2 && unlockedDialogue.Length > 0)
        {
            currentLine = unlockedDialogue[index];
        }
        else if (!usingBranch)
        {
            currentLine = dialogue[index];
        }
        else 
        {
            currentLine = choice1Dialogue[index];
        }

        foreach (char letter in currentLine.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        if (!usingBranch && !hasSpokenToNPC2 && index == choiceTrigger)
        {
            ShowChoices();
        }
        else 
        {
            contButton.SetActive(true);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        int maxIndex = usingBranch ? choice1Dialogue.Length - 1 : dialogue.Length - 1;

        if (index < maxIndex)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            if (dialogueStage < 2)
            {
                dialogueStage++;
            }
            zeroText();
        }
    }

    void ShowChoices()
    {
        if (Choice1 != null) Choice1.SetActive(true);
        if (Choice2 != null) Choice2.SetActive(true);
    }

    void HideChoices()
    {
        if (Choice1 != null) Choice1.SetActive(false);
        if (Choice2 != null) Choice2.SetActive(false);
    }

    public void PickChoice1()
    {
        HideChoices();
        usingBranch = true;
        index = 0;
        dialogueText.text = "";
        StartCoroutine(Typing());
    }

    public void PickChoice2()
    {
        HideChoices();
        usingBranch = true;
        index = 0;
        dialogueText.text = "";
        StartCoroutine(Typing());
    }

    public void StartConversation()
    {
        HideChoices();
        index = 0;
        dialogueText.text = "";

        if (hasSpokenToNPC2)
        {
            dialogueStage = 1;
        }

        if (dialogueStage == 0)
        {
            usingBranch = false;
            StartCoroutine(Typing());
        }
        else if (dialogueStage == 1)
        {
            usingBranch = true;
            StartCoroutine(Typing());
        }
        else if (dialogueStage == 2)
        {
            usingBranch = true;
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            if (triggerCanvas != null)
            {
                triggerCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
            if (triggerCanvas != null)
            {
                triggerCanvas.SetActive(false);
            }
        }
    }
}
