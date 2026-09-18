using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float verticalBoundary = 20f;

    public InputAction moveAction;
    
    private Vector2 moveInput;

    private Rigidbody playerRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // if (transform.position.x > verticalBoundary)
        // {
        //     transform.position = new Vector3(verticalBoundary, transform.position.y, transform.position.z);
        //     playerRb.linearVelocity = new Vector3(0, playerRb.linearVelocity.y, playerRb.linearVelocity.z);
        // }
        // if (transform.position.x < -verticalBoundary)
        // {
        //     transform.position = new Vector3(-verticalBoundary, transform.position.y, transform.position.z);
        //     playerRb.linearVelocity = new Vector3(0, playerRb.linearVelocity.y, playerRb.linearVelocity.z);
        // }
    }

    void FixedUpdate()
    {
        movePlayer();
    }

    // Moves Player based on arrow key input
    void movePlayer()
    {
        float horizontalInput = moveInput.x;
        // float verticalInput = moveInput.y;

        // Vector2 moveDirection = new Vector3(horizontalInput, 0f, 0f);
        // playerRb.AddForce(moveDirection * speed);

        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
        }
    }

}
