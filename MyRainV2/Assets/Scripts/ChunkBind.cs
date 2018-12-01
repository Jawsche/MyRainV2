using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ChunkBind : MonoBehaviour {

    public GameObject otherChunk;

    


    public Vector2 idealPosHead = Vector2.up;
    [Range(0f, 5f)]
    public float returnDist = 1f;
    public float moveSpeed = 0.5f;
    private float moveSpeedInit;
    public float moveSpeedAir = 0.5f;
    private float moveSpeedAirInit;
    public float returnStrength = 1f;
    [Range(0f, 1f)]
    public float LerpTVal = 1f;
    [Range (0f,1f)]
    public float returnStrengthRatioA = 0.7f;
    [Range(0f, 1f)]
    public float returnStrengthRatioB;
    [Range(0f, 1f)]
    public float QuadraticDragConstantA = 0f;
    [Range(0f, 1f)]
    public float QuadraticDragConstantB = 0f;
    public float gravityScale = 1f;
    public float bobAmount = 4f;

    public Rigidbody2D rigidBodyA;
    public Rigidbody2D rigidBodyB;
    

    private bool p_grounded = false;
    private bool p_onWall = false;
    private bool p_standing = true;
    private float p_facingDir = 1.0f;
    private bool stickDownLast = false;
    private float currentTime = 0f;
    private float currentTimeJumps = 100f;
    private float prevFrameDirection;



    public float a_jumpStrangth = 20.0f;
    [Range(1, 10)]
    public float fallMultiplier = 2.5f;
    [Range(1, 10)]
    public float lowJumpMultiplier = 2f;
    [Range(1, 10)]
    public float jumpCooldown = 1f;


    // Use this for initialization
    void Start () {
        rigidBodyA = GetComponent<Rigidbody2D>();
        rigidBodyB = otherChunk.GetComponent<Rigidbody2D>();
        moveSpeedInit = moveSpeed;
        moveSpeedAirInit = moveSpeedAir;
        prevFrameDirection = p_facingDir;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //returnStrengthRatioB = 1f - returnStrengthRatioA;
        rigidBodyA.gravityScale = gravityScale;
        rigidBodyB.gravityScale = gravityScale;
        //rigidBodyA.drag = QuadraticDragConstant;
        //rigidBodyB.drag = QuadraticDragConstant;

        Vector2 dirVecA;
        float distA = Vector2.Distance(otherChunk.transform.position, transform.position);
        if (distA > returnDist)
        {
            dirVecA = (otherChunk.transform.position - transform.position).normalized;
        }
        else
        {
            dirVecA = idealPosHead;
        }
        //distance variable capping
        if (distA <= 1f)
            distA = 1f;
        // Old lerp to point method /////////////////////////
        //rigidBodyA.velocity += Vector2.Lerp(Vector2.zero, dirVecA * distA * (returnStrength * returnStrengthRatioA), lerpTVal);
        //rigidBodyB.velocity += Vector2.Lerp(Vector2.zero, -dirVecA * distA * (returnStrength * returnStrengthRatioB), lerpTVal);
        ////////////////////////////////////////////////////

        Vector2 returnVec;
        returnVec = Vector2.Lerp(Vector2.zero, dirVecA * (distA * distA), LerpTVal);
        //Debug.Log(returnVec);

        ////Some BS Im trying
        //Vector2 desVelTop = rigidBodyA.position - (returnDist - distA) * dirVecA * returnStrengthRatioA;
        //Vector2 desVelBot = rigidBodyB.position + (returnDist - distA) * dirVecA * returnStrengthRatioB;
        //rigidBodyA.MovePosition(desVelTop);
        //rigidBodyB.MovePosition(desVelBot);

        //binding
        rigidBodyA.AddForce(returnVec * (returnStrength * returnStrengthRatioA));
        rigidBodyB.AddForce(-returnVec * (returnStrength * returnStrengthRatioB));

        // Quadratic Drag
        Vector2 dragforceA = -QuadraticDragConstantA * rigidBodyA.velocity.normalized * rigidBodyA.velocity.sqrMagnitude;
        Vector2 dragforceB = -QuadraticDragConstantB * rigidBodyB.velocity.normalized * rigidBodyB.velocity.sqrMagnitude;
        rigidBodyA.AddForce(dragforceA);
        rigidBodyB.AddForce(dragforceB);

        PlayerInput();


    }

    

    private void PlayerInput()
    {
        //Debugging mouse snap
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            Vector3 pos = CrossPlatformInputManager.mousePosition;
            Vector3 mousePt = Camera.main.ScreenToWorldPoint(pos);
            Vector3 temp = mousePt;
            temp.z = 0;
            transform.position = temp;
        }


        //Movement
        CheckGrounded();
        CheckSides();
        float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
        if (h < 0.0f)
        {
            if(!p_onWall)
                p_facingDir = -1.0f;
            else p_facingDir = 1.0f;
        }
        if (h > 0.0f)
        {
            if (!p_onWall)
                p_facingDir = 1.0f;
            else p_facingDir = -1.0f;
        }
        
        
        if (h != 0)
        {
            if (!p_grounded)
                moveSpeed = moveSpeedAir;
            else moveSpeed = moveSpeedInit;

            rigidBodyA.velocity += new Vector2(moveSpeed * h , 0f);
            rigidBodyB.velocity += new Vector2(moveSpeed * h , 0f);


            //GetAxis as buttonDown for bobbing
            if (!stickDownLast)
                {
                    currentTime = 0f;
                }

                stickDownLast = true;
            
            //Bobbing
            if (p_standing)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= 0.15f)
                {
                    //Debug.Log("BANG");
                    rigidBodyB.AddForce(Vector2.up * bobAmount, ForceMode2D.Impulse);
                    currentTime = 0f;
                }

            }
        }else stickDownLast = false;

        //////Vertical Movement///////////////////////////////////////
        //if (v != 0)
        //{
        //    rigidBodyA.velocity += new Vector2(0f, moveSpeed * v * 20f);
        //    rigidBodyB.velocity += new Vector2(0f, moveSpeed * v * 20f);
        //}
        //////////////////////////////////////////////////////////


        //Jump
        currentTimeJumps += Time.deltaTime;
        if (CrossPlatformInputManager.GetButtonDown("Jump") && ((p_grounded && p_standing) || p_onWall) && currentTimeJumps > jumpCooldown)
        {
            //wall jump
            if (p_onWall && !p_grounded)
            {
                rigidBodyB.velocity += Vector2.right * p_facingDir * a_jumpStrangth * 1.5f;
                rigidBodyA.velocity += Vector2.right * p_facingDir * a_jumpStrangth * 1.5f;
            }
            rigidBodyA.velocity += Vector2.up * a_jumpStrangth;
            rigidBodyB.velocity += Vector2.up * a_jumpStrangth;

            currentTimeJumps = 0f;
        }
        //if (rigidBodyA.velocity.y < 0 && !p_grounded)// maybe not use p_grounded here
        //{
        //    rigidBodyA.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        //    rigidBodyB.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        //}
        //else if (rigidBodyA.velocity.y > 0.0f && !CrossPlatformInputManager.GetButton("Jump"))
        //{
        //    rigidBodyA.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        //    rigidBodyB.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        //}

        //crouching
        if (CrossPlatformInputManager.GetAxis("Vertical") < 0f && p_standing && p_grounded)
        {
            p_standing = false;
            idealPosHead = new Vector2(p_facingDir, 0f);
            moveSpeed *= 0.4f;
        }
        //stand back up
        else if (CrossPlatformInputManager.GetAxis("Vertical") > 0f && p_standing == false && p_grounded)
        {
            p_standing = true;
            idealPosHead = new Vector2(0f, 1f);
            moveSpeed = moveSpeedInit;
        }
        //switching directions while crouched
        if (!p_standing)
        {
            idealPosHead = new Vector2(p_facingDir, 0f);

            if(prevFrameDirection != p_facingDir)
            {
                rigidBodyA.AddForce(Vector2.up * 20f, ForceMode2D.Impulse);
            }
            
        }
        prevFrameDirection = p_facingDir;
    }

    private void CheckGrounded()
    {
        //Set Grounded variable
        int layerMask = ~(1 << 8);
        RaycastHit2D hit = Physics2D.Raycast(otherChunk.transform.position, -Vector2.up, 0.8f, layerMask);
        if (hit.collider != null)
        {
            p_grounded = true;
            Debug.DrawRay(otherChunk.transform.position, Vector3.down * 0.8f, Color.red);
        }
        else
        {
            p_grounded = false;
        }
        
        
    }
    private void CheckSides()
    {
        int layerMask = ~(1 << 8);
        RaycastHit2D hitLeft = Physics2D.Raycast(otherChunk.transform.position, -Vector2.right, 0.8f, layerMask);
        if (hitLeft.collider != null)
        {
            p_onWall = true;
            Debug.DrawRay(otherChunk.transform.position, Vector3.left * 0.8f, Color.red);
            currentTimeJumps = 100f;
        }
        
       

        RaycastHit2D hitRight = Physics2D.Raycast(otherChunk.transform.position, Vector2.right, 0.8f, layerMask);
        if (hitRight.collider != null)
        {
            p_onWall = true;
            Debug.DrawRay(otherChunk.transform.position, Vector3.right * 0.8f, Color.red);
            currentTimeJumps = 100f;
        }
        else if (hitRight.collider == null && hitLeft.collider == null)
            p_onWall = false;

    }

    private void OnDrawGizmos()
    {
        float distA = Vector2.Distance(otherChunk.transform.position, transform.position);
        Debug.DrawLine(transform.position, otherChunk.transform.position, Color.Lerp(Color.blue, Color.red, distA * Time.deltaTime));
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.DrawSphere(otherChunk.transform.position, 0.05f);
    }
}