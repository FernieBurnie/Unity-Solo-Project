using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10f;
    public float jumpDetectDistance = 1.1f;

    PlayerInput playerInput;
    Rigidbody2D rb;

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

        //Setting up new move Vector
        moveInput = Vector2.zero;

        footJumpRay = new Ray2D(transform.position, -transform.up);
        wallLJumpRay = new Ray2D(transform.position, -transform.right);
        wallRJumpRay = new Ray2D(transform.position, transform.right);
    }

    // Update is called once per frame
    void Update()
    {
        footJumpRay.origin = transform.position;
        wallRJumpRay.origin = transform.position;
        wallLJumpRay.origin = transform.position;

        footJumpRay.direction = -transform.up;
        wallRJumpRay.direction = transform.right;
        wallLJumpRay.direction = -transform.right;
        Vector2 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;

        rb.linearVelocity = tempMove;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
    }

    public void Jump()
    {
        if (Physics2D.Raycast(footJumpRay.origin, footJumpRay.direction, jumpDetectDistance) || (Physics2D.Raycast(wallRJumpRay.origin,wallRJumpRay.direction, jumpDetectDistance) || Physics2D.Raycast(wallLJumpRay.origin, wallLJumpRay.direction, jumpDetectDistance)))
            rb.AddForceY(jumpHeight, ForceMode2D.Impulse);
    }
}
