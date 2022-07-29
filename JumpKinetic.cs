using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpKinetic : MonoBehaviour
{
    public float maxSpeed = 7;
    public float jumpTakeoffSpeed = 7;
    private bool jumping = false;
    private bool stopjumping;
    public float gravityModifier = 1f;
    public Vector2 velocity;
    public float speedModifier = 1f;
    public float jumpModifier = 1.5f;
    public float jumpDeceleration = 0.5f;
    public float minGroundNormalY = .65f;
    public bool IsGrounded { get; private set; }
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    public Collider2D collider2d;
    private Vector2 playerSize, boxSize;
    private bool grounded = false;
    //[Range(0, 10)] public float jumpV = 5f;
    public LayerMask mask; //ע����unity��������mask
    public float boxHeight;




    /// <summary>
    /// used in a kinetic player object
    /// ��ǰȥ��debug.log�Ĵ�ӡ������
    /// ע��Ҫ����kinetic��object�ϣ�����������޹صĲ��ֿ�����������������
    /// </summary>


    /// <summary>
    /// Bounce the object's vertical velocity.
    /// </summary>
    /// <param name="value"></param>
    public void Bounce(float value)
    {
        velocity.y = value;
    }

    /// <summary>
    /// Bounce the objects velocity in a direction.
    /// </summary>
    /// <param name="dir"></param>
    public void Bounce(Vector2 dir)
    {
        velocity.y = dir.y;
        velocity.x = dir.x;
    }

    /// <summary>
    /// Teleport to some position.
    /// </summary>
    /// <param name="position"></param>
    public void Teleport(Vector3 position)
    {
        body.position = position;
        velocity *= 0;
        body.velocity *= 0;
    }

    protected virtual void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = true;
    }

    protected virtual void OnDisable()
    {
        body.isKinematic = false;
    }

    protected virtual void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        playerSize = GetComponent<SpriteRenderer>().bounds.size;
        Debug.Log(playerSize);
        boxSize = new Vector2(playerSize.x * 0.8f, boxHeight);
    }

    protected virtual void Update()
    {
        targetVelocity = Vector2.zero;
        Detect_onground();
        //Debug.Log("haoa" + grounded + "mask = " + mask);  �˴�������취��Ӷ�����
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jumping = true;
            
        }else if (Input.GetButtonUp("Jump"))
        {
            stopjumping = true;
        }
        
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
        if (jumping)
        {
            velocity.y = jumpTakeoffSpeed * jumpModifier;
            jumping = false;
            grounded = false;
            Debug.Log("asdadasd");
        }else if (stopjumping)
        {
            stopjumping = false;
            

            if (velocity.y > 0)
            {
                velocity.y = velocity.y * jumpDeceleration;
                Debug.Log("wdwdwd");
            }
        }
    }

    private void Detect_onground()
    {
        Vector2 boxcenter = (Vector2)transform.position + (Vector2.down * playerSize.y * 0.5f);
        if (Physics2D.OverlapBox(boxcenter, boxSize, 0, mask) != null)
        {
            grounded = true;
            Debug.Log(grounded + "qaq");
        }
        else
        {
            grounded = false;
            Debug.Log(grounded + "tat");
        }
    }
    private void OnDrawGizmos()
    {
        if (grounded)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Vector2 boxcenter = (Vector2)transform.position + (Vector2.down * playerSize.y * 0.5f);
        Gizmos.DrawWireCube(boxcenter, boxSize);
    }

    protected virtual void FixedUpdate()
    {
        //if already falling, fall faster than the jump speed, otherwise use normal gravity.
        if (velocity.y < 0)
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        else
            velocity += Physics2D.gravity * Time.deltaTime;

        velocity.x = targetVelocity.x;

        IsGrounded = false;

        var deltaPosition = velocity * Time.deltaTime;

        var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        var move = moveAlongGround * deltaPosition.x;

        PerformMovement(move, false);

        move = Vector2.up * deltaPosition.y;

        PerformMovement(move, true);

    }

    void PerformMovement(Vector2 move, bool yMovement)
    {
        var distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            //check if we hit anything in current direction of travel
            var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            for (var i = 0; i < count; i++)
            {
                var currentNormal = hitBuffer[i].normal;

                //is this surface flat enough to land on?
                if (currentNormal.y > minGroundNormalY)
                {
                    IsGrounded = true;
                    // if moving up, change the groundNormal to new surface normal.
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                if (IsGrounded)
                {
                    //how much of our velocity aligns with surface normal?
                    var projection = Vector2.Dot(velocity, currentNormal);
                    if (projection < 0)
                    {
                        //slower velocity if moving against the normal (up a hill).
                        velocity = velocity - projection * currentNormal;
                    }
                }
                else
                {
                    //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                    velocity.x *= 0;
                    velocity.y = Mathf.Min(velocity.y, 0);
                }
                //remove shellDistance from actual move distance.
                var modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        body.position = body.position + move.normalized * distance;
    }

}

