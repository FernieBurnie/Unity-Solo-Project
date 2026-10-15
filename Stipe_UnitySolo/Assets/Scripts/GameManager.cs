using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject pauseMenu;
    public bool paused = false;

    public int collectablesCount = 0;

    public TextMeshProUGUI BoostActivationText;

    public Image healthBar;
    public Image staminaBar;

    //delete later if it doesn't work, testing npc system
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    public int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            BoostActivationText = GameObject.Find("Boost Active").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("Health").GetComponent<Image>();
            staminaBar = GameObject.Find("Stamina").GetComponent<Image>();

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");

            pauseMenu.SetActive(false);

            collectablesCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (paused)
            {
                Time.timeScale = 0;

                pauseMenu.SetActive(true);
                ;
            }
            else
            {
                Time.timeScale = 1;

                pauseMenu.SetActive(false);
            }


            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

            staminaBar.fillAmount = (float)player.stamina / (float)player.maxStamina;

            if (player.currentEquipment)
            {
                BoostActivationText.text = "Jump Boost Activated";
            }
            else
                BoostActivationText.text = "";
        }

        /*if (Keyboard.current[InputSystem.].wasPressedThisFrame && playerIsClose)
        {
            if (dialoguePanel.gameObject.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
        else
        {
            contButton.SetActive(false);
        }*/
    }

    /*public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }
    public void NextLine()
    {


        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }*/

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        pauseMenu.SetActive(paused);
    }

    public void LoadLevel(int levelID)
    {
        if(levelID >= SceneManager.sceneCountInBuildSettings)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
    }

    public void LoadNextScene()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        LoadLevel(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
