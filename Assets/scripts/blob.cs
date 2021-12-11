using UnityEngine;

public class blob : MonoBehaviour
{
    public Rigidbody2D body;
    public CapsuleCollider2D capsuleCollider2D;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private float worldLowerBound = -3f;
    [SerializeField] private float movementSpeed = 0.5f;
    [SerializeField] public LayerMask platformLayerMask;
    private bool turnCheck = true;
    private float distance;
    private float lastFlip;

    // initializes some variables on start
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        // removes objects if it gets below world minimal vertical bound
        if (transform.position.y < worldLowerBound)
        {
            Destroy(this.gameObject);
        }

        turnCheck = TurnAround();
        MovementHandler();

    }
    void OnCollisionEnter2D(Collision2D col)
    {
        // handling collision with other enemies
        if (col.gameObject.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);
        }
    }

    private bool TurnAround()
    {
        // creates series of rays checking collision at certain points
        Vector3 boxCastOffset = new Vector3(.075f, 0f, 0f);
        RaycastHit2D boxcastHitBottomLeft = Physics2D.Raycast(capsuleCollider2D.bounds.center + boxCastOffset, Vector2.down, 0.25f, platformLayerMask);
        RaycastHit2D boxcastHitBottomRight = Physics2D.Raycast(capsuleCollider2D.bounds.center - boxCastOffset, Vector2.down, 0.25f, platformLayerMask);
        RaycastHit2D boxcastHitLeft = Physics2D.Raycast(capsuleCollider2D.bounds.center, Vector2.left, 0.125f, platformLayerMask);
        RaycastHit2D boxcastHitRight = Physics2D.Raycast(capsuleCollider2D.bounds.center, Vector2.right, 0.125f, platformLayerMask);

        //Color boxColor;

        if (boxcastHitBottomLeft.collider == null || boxcastHitBottomRight.collider == null || boxcastHitLeft.collider != null || boxcastHitRight.collider != null)
        {
            // fixes bug that multiple hits are detected too quickly by checking distance traveled since last hit
            distance = transform.position.x - lastFlip;
            lastFlip = transform.position.x;
            if (distance > 0.1f || distance < -0.1f)
            {
                //boxColor = Color.red;
                //Debug.DrawRay(capsuleCollider2D.bounds.center + boxCastOffset, Vector2.down * 0.2f, boxColor);
                //Debug.DrawRay(capsuleCollider2D.bounds.center - boxCastOffset, Vector2.down * 0.2f, boxColor);
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            //boxColor = Color.green;
            //Debug.DrawRay(capsuleCollider2D.bounds.center + boxCastOffset, Vector2.down * 0.2f, boxColor);
            //Debug.DrawRay(capsuleCollider2D.bounds.center - boxCastOffset, Vector2.down * 0.2f, boxColor);
            return false;
        }
    }

    private void MovementHandler()
    {
        // inverts movements speed if turn is needed and flip sprite if it's needed & moving
        if (turnCheck == true)
        {
            movementSpeed *= -1;
            if (body.velocity.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (body.velocity.x < 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        // sets new speed forever while vertical speed is close to 0 and horizontal speed is close to normal move speed
        if ((body.velocity.y < 0.5f || body.velocity.y > -0.5f) && body.velocity.x < 0.6f && body.velocity.x > -0.6f)
        {
            body.velocity = new Vector2(movementSpeed, body.velocity.y);
        }

    }

}
