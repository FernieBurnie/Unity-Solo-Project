using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    GameManager gm;

    public PlayerController player;
    public AudioSource speaker;

    public GameObject pauseMenu;
    public GameObject Door;
    public GameObject doorDialogue;

    public bool paused = false;

    public int collectablesCount = 0;
    public int collectablesRequired = 5;

    public TextMeshProUGUI BoostActivationText;
    public TextMeshProUGUI counter;

    public Image healthBar;
    public Image staminaBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speaker = GetComponent<AudioSource>();
        Time.timeScale = 1;
        collectablesCount = 0;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            BoostActivationText = GameObject.Find("Boost Active").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("Health").GetComponent<Image>();
            staminaBar = GameObject.Find("Stamina").GetComponent<Image>();

            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (healthBar != null && player != null)
            {
                healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            }

            if (staminaBar != null && player != null)
            {
                staminaBar.fillAmount = (float)player.stamina / (float)player.maxStamina;
            }

            if (BoostActivationText != null && player != null)
            {
                if (player.currentEquipment)
                {
                    BoostActivationText.text = "Jump Boost Activated";
                }
                else
                    BoostActivationText.text = "";
            }

            if (counter != null)
            {
                counter.text = ":" + collectablesCount.ToString();
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;
        paused = false;

        if (scene.buildIndex > 0)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

                if (spawnPoint != null)
                {
                    player.transform.position = spawnPoint.transform.position;
                }
            }

            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }

            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>())
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            }
            if (GameObject.Find("Boost Active") != null)
            {
                BoostActivationText = GameObject.Find("Boost Active").GetComponent<TextMeshProUGUI>();
            }
            if (GameObject.Find("Health") != null)
            {
                healthBar = GameObject.Find("Health").GetComponent<Image>();
            }
            if (GameObject.Find("Stamina") != null)
            {
                staminaBar = GameObject.Find("Stamina").GetComponent<Image>();
            }
        }
    }

    public void Pause()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(paused);
        }
    }

    public void LoadLevel(int levelID)
    {
        if (levelID >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Scene ID too high: " + levelID);
        }
        else
        {
            SceneManager.LoadScene(levelID);
        }
    }

    public void LoadNextScene()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AddCollectable(int amount)
    {
        collectablesCount += amount;

        Debug.Log("Total Collectables: " + collectablesCount);

        CheckCollectables();
    }

    public void CheckCollectables()
    {
        if (collectablesCount >= collectablesRequired)
        {
            if (Door != null)
            {
                Door.SetActive(false);
            }

            doorDialogue.SetActive(true);
        }
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
