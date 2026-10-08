using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject pauseMenu;
    public bool paused = false;
    
    //change name later, kept as enemy for now
    public int enemyCount = 0;

    public TextMeshProUGUI BoostActivationText;

    //delete later if it doesn't work, testing dialogue systems, or i can make a new script, see if that works instead
    TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] CanvasGroup dialogueBox;

    public static event Action OnDialogueStarted;
    public static event Action OnDialogueEnded;
    bool skipLineTriggered;

    public class DialogueTree : ScriptableObject
    {
        public DialogueSection[] sections;
    }
    [System.Serializable]
    public struct DialogueSection
    {
        [TextArea]
        public string[] dialogue;
        public bool endAfterDialogue;
        public BranchPoint branchPoint;
    }
    [System.Serializable]
    public struct BranchPoint
    {
        [TextArea]
        public string question;
        public Answer[] answers;
    }
    [System.Serializable]
    public struct Answer
    {
        public string answerLabel;
        public int nextElement;
    }
    //delete everything above if needed.

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

            //change name later, kept as enemy for now

            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
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
        if(levelID >= SceneManager.sceneCountInBuildSettings)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
    }

    public void LoadNextLevel()
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

    //delete later if it doesn't work, testing dialogue systems
    public void ShowDialogue(string dialogue, string name)
    {
        nameText.text = name + "...";
        dialogueText.text = dialogue;
        dialoguePanel.SetActive(true);
    }

    public void EndDialogue()
    {
        nameText.text = null;
        dialogueText.text = null; ;
        dialoguePanel.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void StartDialogue(string[] dialogue, int startPosition, string name)
    {
        nameText.text = name + "...";
        dialogueBox.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(RunDialogue(dialogue, startPosition));
    }

    IEnumerator RunDialogue(string[] dialogue, int startPosition)
    {
        skipLineTriggered = false;
        OnDialogueStarted?.Invoke();

        for (int i = startPosition; i < dialogue.Length; i++)
        {
            dialogueText.text = dialogue[i];
            while (skipLineTriggered == false)
            {
                yield return null;
            }
            skipLineTriggered = false;
        }

        OnDialogueEnded?.Invoke();
        dialogueBox.gameObject.SetActive(false);
    }

    public void SkipLine()
    {
        skipLineTriggered = true;
    }
}
