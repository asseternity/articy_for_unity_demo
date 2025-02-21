using Articy.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float jumpForce = 15f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private ArticyObject availableDialogue;
    private DialogueManager dialogueManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        PlayerInteraction();
        PlayerMovement();
    }

    void PlayerMovement()
    {
        // Remove movement while in dialogue
        if (dialogueManager.DialogueActive)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // Simple scene restart for testing purposes
    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // All interactions and key inputs player can use
    void PlayerInteraction()
    {
        // Key option to start dialogue when near NPC
        if (Input.GetKeyDown(KeyCode.Space) && availableDialogue)
        {
            dialogueManager.StartDialogue(availableDialogue);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !availableDialogue)
        {
            // Jumping
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Key option to abort dialogue
        if (dialogueManager.DialogueActive && Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueManager.CloseDialogueBox();
        }

        // Key option to reset entire scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision is from below (ground)
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    void OnTriggerEnter2D(Collider2D aOther)
    {
        var articyReferenceComp = aOther.GetComponent<ArticyReference>();
        if (articyReferenceComp)
        {
            availableDialogue = articyReferenceComp.reference.GetObject();
        }
    }

    void OnTriggerExit2D(Collider2D aOther)
    {
        if (aOther.GetComponent<ArticyReference>() != null)
        {
            availableDialogue = null;
        }
    }
}
