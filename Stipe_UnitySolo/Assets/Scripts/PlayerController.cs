using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Vector2 originalPosition;
    
    public bool hazardDamage = false;
    public bool poisoned = false;
    public bool poisonDmg = false;

    //general 
    public float speed = 5.0f;
    public int health = 6;
    public float maxHealth = 6;
    public float jumpHeight = 4f;
    public float jumpBoost = 2f;
    public bool jumpBoostActivated = false;
    public float jumpDetectDistance = 1.1f;

    //stamina and sprinting
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float sprintBoost = 2.0f;
    public float sprintCooldown = 2;
    public float staminaRegen = 5;
    public float staminaCooldown = 2;
    public float sprintCost = .1f;
    public bool canSprint = true;
    public bool regenStamina = false;
    public bool sprintStop = false;
    public bool staminaStop = false;
    public bool sprinting = false;

    //wall jumping
    public float jumpCost = 20f;
    public float jumpCooldown = 0.4f;
    public bool canJump = true;
    public bool jumpStop = false;
    public float wallJumpForceY = 7f;
    public float wallJumpForceX = 12f;

    public float hazardCooldown = 3f;
    public float poisonDuration = 4;
    public float poisonInterval = 1;
    public float damageTimer = 0f;
    public bool toggleSprint = true;

    public float enemyCooldown = 1f;
    public int enemyDamage = 1;

    PlayerInput playerInput;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    public AudioSource speaker;
    public GameObject currentEquipment;
    public GameManager gm;
    private static PlayerController instance;


    Ray2D footJumpRay;
    Ray2D wallLJumpRay;
    Ray2D wallRJumpRay;
    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initalizing component data
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        speaker = GetComponent<AudioSource>();
        originalPosition = transform.position;

        currentEquipment = null;

        //Setting up new move Vector
        moveInput = Vector2.zero;

        footJumpRay = new Ray2D(transform.position, -transform.up);
        wallLJumpRay = new Ray2D(transform.position, -transform.right);
        wallRJumpRay = new Ray2D(transform.position, transform.right);
    }

    // Update is called once per frame
    void Update()
    {
        // Die
        if (health <= 0)
        {
            Die();
        }

        if (poisoned)
        {
            if (!poisonDmg)
                StartCoroutine("poisonEffect");
        }

        if (moveInput.x > 0)
        {
            sr.flipX = false;
            anim.SetBool("isWalking", true);
        }
        else if (moveInput.x < 0)
        {
            sr.flipX = true;
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
        footJumpRay.origin = transform.position;
        wallRJumpRay.origin = transform.position;
        wallRJumpRay.origin = transform.position;

        bool isGrounded = Physics2D.Raycast(footJumpRay.origin, footJumpRay.direction, jumpDetectDistance);

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
        }

        footJumpRay.direction = -transform.up;
        wallRJumpRay.direction = transform.right;
        wallLJumpRay.direction = -transform.right;
        Vector2 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;

        if (sprinting)
        {
            if ((moveInput.x == 1 || moveInput.x == -1) && stamina > 0)
            {
                tempMove.x *= sprintBoost;

                stamina -= sprintCost * Time.deltaTime;

                if (stamina < 0)
                    stamina = 0;

                StopCoroutine("staminaReset");
            }
            else
            {
                canSprint = false;
                sprinting = false;
            }
        }

        if (!sprinting)
        {
            if (!regenStamina && !staminaStop && stamina < maxStamina)
            {
                StartCoroutine("staminaReset");
            }
            if (!canSprint && !sprintStop)
            {
                StartCoroutine("sprintReset");
            }
            if (regenStamina)
            {
                stamina += staminaRegen * Time.deltaTime;

                if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                    regenStamina = false;
                }
            }
        }
        rb.linearVelocity = tempMove;

        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && canJump && stamina >= jumpCost)
        {
            bool grounded = Physics2D.Raycast(footJumpRay.origin, footJumpRay.direction, jumpDetectDistance);
            bool hitLeftWall = Physics2D.Raycast(wallLJumpRay.origin, wallLJumpRay.direction, jumpDetectDistance);
            bool hitRightWall = Physics2D.Raycast(wallRJumpRay.origin, wallRJumpRay.direction, jumpDetectDistance);

            if (grounded)
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                PlayJumpEffects();
            }
            else if (hitLeftWall)
            {
                WallJump(1f);
            }
            if (hitRightWall)
            {
                WallJump(-1f);
            }
        }

    }

    void WallJump(float pushDir)
    {
        rb.linearVelocity = new Vector2(pushDir * wallJumpForceX, wallJumpForceY);
        PlayJumpEffects();
    }

    void PlayJumpEffects()
    {
        speaker.Play();
        anim.SetBool("isJumping", true);

        stamina -= jumpCost;
        if (stamina < 0) stamina = 0;

        canJump = false;
        StartCoroutine(jumpReset());
    }

    public void ActivateEquipment()
    {
        if (currentEquipment != null)
        {
            if (currentEquipment.name == "Jump")
            {
                jumpHeight += jumpBoost;

                jumpBoostActivated = true;

                currentEquipment = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Equipment")
        {
            currentEquipment = collision.gameObject;
            collision.gameObject.SetActive(false);

            jumpHeight += jumpBoost;

        }

        if (collision.gameObject.CompareTag("Collectable"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.AddCollectable(1);
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (canSprint)
        {
            if (toggleSprint)
            {
                sprinting = !sprinting;
            }
            else if (!toggleSprint)
            {
                sprinting = context.ReadValueAsButton();

                if (!sprinting)
                    canSprint = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            health--;
        }

        if (collision.gameObject.tag == "Health")
        {
            if (health >= maxHealth)
            {
                return;
            }
            else
            {
                health++;
            }
        }

        if (collision.gameObject.tag == "Poison")
        {
            poisoned = true;
            StartCoroutine("poisonCooldown");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            health--;
        }

        if (collision.gameObject.tag == "LevelEnd")
        {
            GameManager.instance.LoadNextScene();
        }

        if (collision.gameObject.tag == "OutOfBounds")
        {
            Die();
        }

        if (collision.gameObject.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            GameManager.instance.collectablesCount++;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            if (!hazardDamage)
                StartCoroutine("damageCooldown");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            if (damageTimer <= 0)
            {
                health -= enemyDamage;
                damageTimer = enemyCooldown;
            }
        } 
    }

    public void OnCollisionExit2D(Collision2D collison)
    {
        if(collison.gameObject.tag == "Hazard")
        {
            if (hazardDamage)
            {
                StopCoroutine("damageCooldown");
                hazardDamage = false;
            }
        }
    }

    public void Die()
    {
        transform.position = originalPosition;
            
        if (rb!= null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    IEnumerator damageCooldown()
    {
        hazardDamage = true;

        yield return new WaitForSeconds(hazardCooldown);

        health--;
        hazardDamage = false;
    }

    IEnumerator poisonEffect()
    {
        poisonDmg = true;

        yield return new WaitForSeconds(poisonInterval);

        health--;
        poisonDmg = false;
    }

    IEnumerator poisonCooldown()
    {
        yield return new WaitForSeconds(poisonDuration);

        poisoned = false;
    }

    IEnumerator jumpReset()
    {
        jumpStop = true;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
        jumpStop = false;
    }

    IEnumerator sprintReset()
    {
        sprintStop = true;

        yield return new WaitForSeconds(sprintCooldown);

        canSprint = true;
        sprintStop = false;
    }

    IEnumerator staminaReset()
    {
        staminaStop = true;

        yield return new WaitForSeconds(staminaCooldown);

        regenStamina = true;
        staminaStop = false;
    }
}
