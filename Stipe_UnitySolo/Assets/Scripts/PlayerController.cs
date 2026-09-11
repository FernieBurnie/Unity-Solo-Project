using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10f;

    PlayerInput playerInput;
    Rigidbody2D rb;

    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initalizing component data
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        //Setting up new move Vector
        moveInput = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tempMovement = rb.linearVelocity;

        rb.linearVelocity = moveInput * speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue <Vector2>();
    }
}
