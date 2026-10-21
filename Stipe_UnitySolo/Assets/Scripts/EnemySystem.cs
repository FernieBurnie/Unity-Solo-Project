using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    SpriteRenderer sr;
    Animator anim;

    public bool isFollowing = false;

    public int health = 3;
    public int maxHealth = 3;
    public float speed = 3.5f;
    public float detectionDistance = 2;
    public float stoppingDistance = 1;

    public PlayerController player;
    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector2.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionDistance;

        anim.SetBool("isWalking", isFollowing);
        anim.speed = speed / 3.5f;

        if (isFollowing)
        {
            if (player.transform.position.x < transform.position.x)
            {
                sr.flipX = true;
                rb.linearVelocityX = -speed;
            }
            else if (player.transform.position.x > transform.position.x)
            {
                sr.flipX = false;
                rb.linearVelocityX = speed;
            }
            else if (Mathf.Abs(player.transform.position.x - transform.position.x) <= stoppingDistance)
            {
                rb.linearVelocityX = 0;
            }
        }
        else
        {
            rb.linearVelocityX = 0;
        }
    }
}