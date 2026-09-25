using UnityEngine;

public class AdvancedEnemy : MonoBehaviour
{

    //delete EVERYTHING on monday, restart
    public float speed = 4.7f;
    public int health = 5;
    public int maxHealth = 5;

    private Vector2 back;
    private Vector2 forth;
    private float phase = 20f;
    private float phaseDirection = 12f;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        transform.position = Vector2.Lerp(back, forth, phase);
        phase += Time.deltaTime * speed * phaseDirection;
        if (phase >= 1 || phase <= 0) phaseDirection *= -1; 
    }
}
