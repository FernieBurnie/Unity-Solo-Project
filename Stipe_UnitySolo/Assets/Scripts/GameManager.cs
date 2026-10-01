using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject pauseMenu;
    public bool paused = false;

    public TextMeshProUGUI BoostActivationText;

    public Image healthBar;
    public Image staminaBar;

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
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
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
    }

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
        if(levelID >= SceneManager.sceneCount)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
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
