using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Rigidbody2D body;
    private CapsuleCollider2D capsuleCollider2D;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private float jumpVelocity = 3.5f;
    [SerializeField] private float movementSpeed = 1f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private float canJump = 0f;
    [SerializeField] private float jumpDelay = 0.8f;
    [SerializeField] private float fallMultiplier = 1.5f;
    private Vector2 fallMultiplierVector;
    private bool groundCheck = true;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        fallMultiplierVector = Vector2.up * (fallMultiplier - 1);
    }

    private void Update()
    {
        groundCheck = IsGrounded();
        MovementHandler();
        AnimationHandler();
        FallGravityHandler();


    }

    private bool IsGrounded()
    {
        float extraBoxHeight = 0.05f;
        Vector3 boxCastOffset = new Vector3 (.02f, 0f, 0f);

        RaycastHit2D boxcastHit = Physics2D.BoxCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size - boxCastOffset, 0f, Vector2.down, extraBoxHeight, platformLayerMask);

        // just debug color box
        //Color boxColor;
        //if (boxcastHit.collider != null)
        //{
        //    boxColor = Color.green;
        //}
        //else
        //{
        //    boxColor = Color.red;
        //}

        //Debug.DrawRay(capsuleCollider2D.bounds.center + new Vector3(capsuleCollider2D.bounds.extents.x, 0) - (boxCastOffset/2), Vector2.down * (capsuleCollider2D.bounds.extents.y + extraHeightText), boxColor);
        //Debug.DrawRay(capsuleCollider2D.bounds.center - new Vector3(capsuleCollider2D.bounds.extents.x, 0) + (boxCastOffset/2), Vector2.down * (capsuleCollider2D.bounds.extents.y + extraHeightText), boxColor);
        //Debug.DrawRay(capsuleCollider2D.bounds.center - new Vector3(0,capsuleCollider2D.bounds.extents.y + extraHeightText), Vector2.right * (capsuleCollider2D.bounds.extents.x), boxColor);

        return boxcastHit.collider != null;
    }

    private void MovementHandler()
    {
        
        if (Input.GetKey("up") && groundCheck && Time.time > canJump)
        {
            body.velocity = Vector2.up * jumpVelocity;
            canJump = Time.time + jumpDelay;

        }
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
        else
        {
            if (groundCheck)
            {
                body.velocity = new Vector2(0, body.velocity.y);
            }
        }
    }
    private void AnimationHandler()
    {
        int animationTrigger = 0;

        if (groundCheck && body.velocity.x != 0)
        {
            animationTrigger = 1;
        }

        if (animationTrigger == 1){
            animator.SetFloat("Speed", 1);
            return;
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void FallGravityHandler()
    {
        // jump fall gravity multiplier
        if (body.velocity.y < 0)
        {
            body.velocity += fallMultiplierVector * Physics2D.gravity.y * Time.deltaTime;
        }
    }
}
