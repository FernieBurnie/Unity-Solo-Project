using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class AdvancedEnemy : MonoBehaviour
{
    private bool movingRight = true;

    public float speed = 4.5f;
    public float health = 1f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animation anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animation>();
    }
    private void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            sr.flipX = true;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            sr.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            movingRight = !movingRight;
        }
    }
}
