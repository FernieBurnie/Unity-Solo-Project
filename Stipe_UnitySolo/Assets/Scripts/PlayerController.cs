using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool hazardDamage = false;
    public bool poisoned = false;
    public bool poisonDmg = false;
    public bool sprinting = false;

    public float speed = 5.0f;
    public int health = 6;
    public float maxHealth = 6;

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

    public float jumpHeight = 4f;
    public float jumpboost = 2f;
    public float jumpDetectDistance = 1.1f;
    public float hazardCooldown = 3f;
    public float poisonDuration = 4;
    public float poisonInterval = 1;
    public float damageTimer = 0f;
    public bool toggleSprint = true;
    
    public float enemyCooldown = 1f;
    public int enemyDamage = 1;

    PlayerInput playerInput;
    Rigidbody2D rb;
    public GameObject currentEquipment;

    Ray2D footJumpRay;
    Ray2D wallLJumpRay;
    Ray2D wallRJumpRay;
    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initalizing component data
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

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
        { }

        if(poisoned)
        {
            if (!poisonDmg)
                StartCoroutine("poisonEffect");
        }
            
        wallRJumpRay.origin = transform.position;
        wallLJumpRay.origin = transform.position;
        footJumpRay.origin = transform.position;
        
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

        if (damageTimer>0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
    }

    public void Jump()
    {
        if (Physics2D.Raycast(footJumpRay.origin, footJumpRay.direction, jumpDetectDistance) || (Physics2D.Raycast(wallRJumpRay.origin, wallRJumpRay.direction, jumpDetectDistance) || Physics2D.Raycast(wallLJumpRay.origin, wallLJumpRay.direction, jumpDetectDistance)))
            rb.AddForceY(jumpHeight, ForceMode2D.Impulse);
    }

    public void ActivateEquipment()
    {
        if (currentEquipment != null)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Equipment")
        {
            currentEquipment = collision.gameObject;
            collision.gameObject.SetActive(false);

            jumpHeight += jumpboost;

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
            health++;
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
