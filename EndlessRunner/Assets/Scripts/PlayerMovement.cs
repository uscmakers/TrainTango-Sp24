using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerSide
    {
        Left = -1,
        Center = 0,
        Right = 1
    }

    public static PlayerMovement instance;

    public GameObject gameOverUI;

    public float speed = 5;
    [SerializeField] Rigidbody rb;

    float horizontalInput;

    [SerializeField] float laneDistance = 4f; // Distance between each lane
    // Define lane positions
    float leftLane = -3f;
    float middleLane = 0f;
    float rightLane = 3f;
    float currentLaneX = 0f; // Start in the middle lane
    [SerializeField] float horizontalMultiplier = 2;

    [SerializeField] float jumpForce = 400f;
    [SerializeField] LayerMask groundMask;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void FixedUpdate()
    {
        if (GameManager.gameState != GameManager.GameState.Playing) return;

        // Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        // Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime * horizontalMultiplier;
        // rb.MovePosition(rb.position + forwardMove + horizontalMove);

        Vector3 targetPosition = new Vector3(currentLaneX, rb.position.y, rb.position.z);
        Vector3 moveVector = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(moveVector);
    }

    public void GameplayUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Lane change
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }

    public void Die()
    {
        // Restart the game
        // Invoke("Restart", 2);
        gameOverUI.SetActive(true);
        GameManager.gameState = GameManager.GameState.GameOver;
    }

    public void Restart()
    {
        GameManager.instance.Restart();
    }

    public void MoveLeft()
    {
        if (currentLaneX == middleLane)
        {
            currentLaneX = leftLane;
        }
        else if (currentLaneX == rightLane)
        {
            currentLaneX = middleLane;
        }
    }
    public void MoveRight()
    {
        if (currentLaneX == middleLane)
        {
            currentLaneX = rightLane;
        }
        else if (currentLaneX == leftLane)
        {
            currentLaneX = middleLane;
        }
    }

    public void MoveCenter()
    {
        currentLaneX = middleLane;
    }

    public void Jump()
    {
        // Check wether we are currently grounded
        float height = GetComponent<Collider>().bounds.size.y;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, (height / 2) + 0.1f, groundMask);
        // If we are, jump
        rb.AddForce(Vector3.up * jumpForce);
    }

}
