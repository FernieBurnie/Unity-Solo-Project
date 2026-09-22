using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI BoostActivationText;

    public Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        BoostActivationText = GameObject.Find("Boost Active").GetComponent<TextMeshProUGUI>();

        healthBar = GameObject.Find("Health").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

        if (player.currentEquipment)
        {
            BoostActivationText.text = "Jump Boost Activated";
        }
        else
            BoostActivationText.text = "";
    }
}
