using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody2D body;
    public CapsuleCollider2D capsuleCollider2D;
    [SerializeField] public LayerMask platformLayerMask;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public GameObject spawnPoint;
    public GameObject checkPoint;
    public GameObject colObject;

    private Vector2 fallMultiplierVector;
    private bool groundCheck = true;
    private float coyotteCounter;
    private int checkpointCounter = 0;
    private int lifeCounter = 3;
    private bool midair = false;



    [SerializeField] private float jumpVelocity = 3.5f;
    [SerializeField] private float movementSpeed = 1.3f;
    [SerializeField] private float fallMultiplier = 1.5f;
    [SerializeField] private float coyotteTime = 0.125f;
    [SerializeField] private float worldLowerBound = -3f;
    [SerializeField] private Sprite emptyHearth;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource footStep;
    [SerializeField] private AudioSource jumpSound;

    // Initializes some values on start
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        fallMultiplierVector = Vector2.up * (fallMultiplier - 1);
        spawnPoint = GameObject.Find("SpawnPoint");
    }

    // update with every frame
    private void Update()
    {
        WorldBoundCheck();
        groundCheck = IsGrounded();
        MovementHandler();
        CanJump();
        AnimationHandler();
        FallGravityHandler();
        JumpedSound();


    }

    // checking tags of colliders on trigger
    void OnTriggerEnter2D(Collider2D col_trig)
    {
        // changing spawnpoint to checkpoint and removing checkopint object
        if (col_trig.CompareTag("Checkpoint"))
        {
            checkpointCounter++;
            checkPoint = GameObject.Find($"Checkpoint ({checkpointCounter})");
            spawnPoint.transform.position = checkPoint.transform.position;
            Destroy(checkPoint);
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        colObject = col.gameObject;
        // handling collision with enemies
        if (colObject.CompareTag("Enemy"))
        {
            // if player hits enemy from above, remove enemy
            if (colObject.transform.position.y + colObject.GetComponent<CapsuleCollider2D>().size.y < transform.position.y)
            {
               // Destroy(colObject);
            }
            // else respawn player and stop enemy movement from collision
            else
            {
                colObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, body.velocity.y);
                Respawn();
            }
        }
    }


    // checks if player is grounded by projecting boxcast below his collider
    private bool IsGrounded()
    {
        float extraBoxHeight = 0.05f;
        Vector3 boxCastOffset = new Vector3 (.04f, 0f, 0f);
        RaycastHit2D boxcastHit = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size - boxCastOffset, 0f, Vector2.down, extraBoxHeight, platformLayerMask);

        //Color boxColor;
        //if (boxcastHit.collider != null)
        //{
        //    boxColor = Color.green;
        //}
        //else
        //{
        //    boxColor = Color.red;
        //}

        //Debug.DrawRay(capsuleCollider2D.bounds.center + new Vector3(capsuleCollider2D.bounds.extents.x, 0) - (boxCastOffset / 2), Vector2.down * (capsuleCollider2D.bounds.extents.y + extraBoxHeight), boxColor);
        //Debug.DrawRay(capsuleCollider2D.bounds.center - new Vector3(capsuleCollider2D.bounds.extents.x, 0) + (boxCastOffset / 2), Vector2.down * (capsuleCollider2D.bounds.extents.y + extraBoxHeight), boxColor);
        //Debug.DrawRay(capsuleCollider2D.bounds.center - new Vector3(0, capsuleCollider2D.bounds.extents.y + extraBoxHeight), Vector2.right * (capsuleCollider2D.bounds.extents.x - (boxCastOffset.x/2)), boxColor);

        return boxcastHit.collider != null;
    }

    // check if player can jump based on grounded & coyotte time
    private void CanJump()
    {
        if(groundCheck)
        {
            coyotteCounter = coyotteTime;
        }
        else
        {
            coyotteCounter -= Time.deltaTime;
        }
    }

    // handles simple movement
    private void MovementHandler()
    {
        // jumping when key is pressed
        if (Input.GetKeyDown("up") && coyotteCounter > 0f)
        {
            body.velocity = Vector2.up * jumpVelocity;

        }
        // lowering jump velocity when jump key is released
        else if (Input.GetKeyUp("up") && body.velocity.y > 0f)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
        }
        // left & right movement + flip of sprite
        else if (Input.GetKey("right"))
        {
            body.velocity = new Vector2(movementSpeed, body.velocity.y);
            spriteRenderer.flipX = false;

        }
        else if (Input.GetKey("left"))
        {
            body.velocity = new Vector2(-movementSpeed, body.velocity.y);
            spriteRenderer.flipX = true;

        }
        // stopping all horizontal movement when no keys are pressed
        else
        {

            if (groundCheck)
            {
                body.velocity = new Vector2(0f, body.velocity.y);
            }
        }
    }
    //handles animations
    private void AnimationHandler()
    {
        int animationTrigger = 0;
        // sets trigger to running animation
        if (groundCheck && body.velocity.x != 0f && body.velocity.y > -0.1f && body.velocity.y < 0.1f)
        {
            animationTrigger = 1;
        }
        // sets trigger to idle animation
        else
        {
            animationTrigger = 0;
        }

        // plays animations based on trigger
        if (animationTrigger == 1){
            animator.SetFloat("Speed", 1f);
            return;
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void FallGravityHandler()
    {
        // jump fall gravity multiplier, to make falling faster than going up
        if (body.velocity.y < 0f)
        {
            body.velocity += fallMultiplierVector * Physics2D.gravity.y * Time.deltaTime;
        }
    }

    // respawns player when he dies
    private void WorldBoundCheck()
    {
        if(transform.position.y < worldLowerBound)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        deathSound.Play();
        lifeCounter--;
        if (lifeCounter <= 0)
        {
            SceneManager.LoadScene("DeathScene");
        }
        else
        {
            GameObject.Find($"HearthImage ({lifeCounter})").GetComponent<Image>().sprite = emptyHearth;
            transform.position = spawnPoint.transform.position;
            body.velocity = new Vector2(0f, 0f);
        }

    }
    private void Footstep()
    {
        footStep.Play();
    }

    private void JumpedSound()
    {
        if(groundCheck == false)
        {
            midair = true;
        }
        else if (midair && groundCheck)
        {
            jumpSound.Play();
            midair = false;
        }
    }
}
