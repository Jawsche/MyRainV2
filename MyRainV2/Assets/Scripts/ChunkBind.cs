using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ChunkBind : MonoBehaviour {

    public GameObject otherChunk;

    public float moveSpeed = 0.5f;
    private float moveSpeedInit;


    public Vector2 idealPosHead = Vector2.up;
    public float returnStrength = 1f;
    [Range(0f, 5f)]
    public float returnDist = 1f;
    [Range (0f,1f)]
    public float returnStrengthRatioA = 0.7f;
    [Range(0f, 1f)]
    public float returnStrengthRatioB;
    [Range(0f, 1f)]
    public float lerpTVal = 1f;
    [Range(0f, 0.99f)]
    public float getToRestDamping = 0.99f;
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
    void Awake () {
        rigidBodyA = GetComponent<Rigidbody2D>();
        rigidBodyB = otherChunk.GetComponent<Rigidbody2D>();
        moveSpeedInit = moveSpeed;
        prevFrameDirection = p_facingDir;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        returnStrengthRatioB = 1f - returnStrengthRatioA;

        Vector2 dirVec;
        float distA = Vector2.Distance(otherChunk.transform.position, transform.position);
        if (distA > returnDist)
        {
            dirVec = (otherChunk.transform.position - transform.position).normalized;
        }
        else
        {
            dirVec = idealPosHead;
        }
        //distance variable capping
        if (distA <= 1f)
            distA = 1f;

        // Old lerp-to-point method /////////////////////////
        //rigidBodyA.velocity += Vector2.Lerp(Vector2.zero, dirVec * distA * (returnStrength * returnStrengthRatioA), lerpTVal);
        //rigidBodyB.velocity += Vector2.Lerp(Vector2.zero, -dirVec * distA * (returnStrength * returnStrengthRatioB), lerpTVal);
        ////////////////////////////////////////////////////

        Vector2 returnVec;
        returnVec = dirVec * (distA * distA);
        

        //Debug.Log(returnVec);

        rigidBodyA.velocity += returnVec * (returnStrength * returnStrengthRatioA);
        rigidBodyB.velocity -= returnVec * (returnStrength * returnStrengthRatioB);

        //Exponential damping////////////////////////////////////////////////////////
        //rigidBodyA.velocity *= Mathf.Pow(1f - getToRestDamping, Time.deltaTime * 10f);
        //rigidBodyB.velocity *= Mathf.Pow(1f - getToRestDamping, Time.deltaTime * 10f);

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
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        //float v = CrossPlatformInputManager.GetAxis("Vertical");
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
            rigidBodyA.velocity += new Vector2(moveSpeed * h , 0f);
            rigidBodyB.velocity += new Vector2(moveSpeed * h , 0f);
            

            //GetAxis as buttonDown for bobbing
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (!stickDownLast)
                {
                    currentTime = 0f;
                }

                stickDownLast = true;
            }
            else
                stickDownLast = false;

            //Vertical Movement///////////////////////////////////////
            //if (v != 0)
            //{
            //    rigidBodyA.velocity += new Vector2(0f, moveSpeed * v * returnStrengthRatioA * 2f);
            //    rigidBodyB.velocity += new Vector2(0f, moveSpeed * v * returnStrengthRatioB * 2f);
            //}
            //////////////////////////////////////////////////////////

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
        }
        

        //Jump
        currentTimeJumps += Time.deltaTime;
        if (CrossPlatformInputManager.GetButtonDown("Jump") && ((p_grounded && p_standing) || p_onWall) && currentTimeJumps > jumpCooldown)
        {
            //wall jump
            if (p_onWall && !p_grounded)
            {
                rigidBodyB.velocity += Vector2.right * p_facingDir * a_jumpStrangth *1.5f;
                rigidBodyA.velocity += Vector2.right * p_facingDir * a_jumpStrangth * 1.5f;
            }
            rigidBodyA.velocity += Vector2.up * a_jumpStrangth;
            rigidBodyB.velocity += Vector2.up * a_jumpStrangth;

            currentTimeJumps = 0f;
        }
        if (rigidBodyA.velocity.y < 0 && !p_grounded)// maybe not use p_grounded here
        {
            rigidBodyA.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
            rigidBodyB.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rigidBodyA.velocity.y > 0.0f && !CrossPlatformInputManager.GetButton("Jump"))
        {
            rigidBodyA.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            rigidBodyB.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }

        //crouching
        if (CrossPlatformInputManager.GetAxisRaw("Vertical") == -1f && p_standing && p_grounded)
        {
            p_standing = false;
            idealPosHead = new Vector2(p_facingDir, 0f);
            moveSpeed *= 0.4f;
        }
        //stand back up
        else if (CrossPlatformInputManager.GetAxisRaw("Vertical") == 1f && p_standing == false && p_grounded)
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