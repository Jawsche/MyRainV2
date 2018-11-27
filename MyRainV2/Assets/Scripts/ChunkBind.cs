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

    public Rigidbody2D rigidBodyA;
    public Rigidbody2D rigidBodyB;
    

    private bool p_grounded = false;
    private bool p_standing = true;
    private float p_facingDir = 1.0f;
    private bool stickDownLast = false;
    private float currentTime = 0f;



    public float a_jumpStrangth = 20.0f;
    [Range(1, 10)]
    public float fallMultiplier = 2.5f;
    [Range(1, 10)]
    public float lowJumpMultiplier = 2f;

    public float amplitude;
    public float period;

    // Use this for initialization
    void Start () {
        rigidBodyA = GetComponent<Rigidbody2D>();
        rigidBodyB = otherChunk.GetComponent<Rigidbody2D>();
        moveSpeedInit = moveSpeed;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        

        returnStrengthRatioB = 1f - returnStrengthRatioA;
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



        //float theta = Time.timeSinceLevelLoad / period;
        //float distance = amplitude * Mathf.Sin(theta) + 0.5f;
        ////transform.position = targetA + Vector3.up * distance;
        //Vector3 sinVal = (Vector3.up ) * distance;
        //Debug.Log(sinVal);

        if (distA <= 1f)
            distA = 1f;

        //rigidBodyA.velocity += Vector2.Lerp(Vector2.zero, dirVecA * distA * (returnStrength * returnStrengthRatioA), lerpTVal);
        //rigidBodyB.velocity += Vector2.Lerp(Vector2.zero, -dirVecA * distA * (returnStrength * returnStrengthRatioB), lerpTVal);


        Vector2 returnVec;
        returnVec = dirVecA * (distA * distA);
        

        //Debug.Log(returnVec);

        rigidBodyA.velocity += returnVec * (returnStrength * returnStrengthRatioA);
        rigidBodyB.velocity -= returnVec * (returnStrength * returnStrengthRatioB);

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
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        //float v = CrossPlatformInputManager.GetAxis("Vertical");
        if (h < 0.0f)
            p_facingDir = -1.0f;
        if (h > 0.0f)
            p_facingDir = 1.0f;
        
        
        if (h != 0)
        {
            rigidBodyA.velocity += new Vector2(moveSpeed * h , 0f);
            rigidBodyB.velocity += new Vector2(moveSpeed * h , 0f);

            //GetAxis as buttonDown
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (!stickDownLast)
                {
                    currentTime = 0.5f;
                    //Debug.Log("Grp");
                }

                stickDownLast = true;
            }
            else
                stickDownLast = false;

            
            //Bobbing
            if (p_standing)
            {
                currentTime += Time.deltaTime;
                Debug.Log(currentTime);
                float theta = currentTime / period;
                float distance = amplitude * Mathf.Sin(theta);
                Vector2 sinVal = (Vector2.up) * (distance);
                rigidBodyB.velocity += sinVal;
                //Debug.Log(sinVal);
            }
        }
        //if (v != 0)
        //{
        //    rigidBodyA.velocity += new Vector2(0f, moveSpeed * v * returnStrengthRatioA * 2f);
        //    rigidBodyB.velocity += new Vector2(0f, moveSpeed * v * returnStrengthRatioB * 2f);
        //}

        //Jump
        if (CrossPlatformInputManager.GetButtonDown("Jump") && p_grounded && p_standing)
        {
            rigidBodyA.AddForce(Vector2.up * a_jumpStrangth, ForceMode2D.Impulse);
            rigidBodyB.AddForce(Vector2.up * a_jumpStrangth, ForceMode2D.Impulse);
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
            if(p_facingDir == 1.0f)
            {
                idealPosHead = new Vector2(p_facingDir, 0f);
            }
            else if(p_facingDir == -1.0f)
            {
                idealPosHead = new Vector2(p_facingDir, 0f);
            }
        }
        
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
            //Debug.Log(hit.collider.name);
        }
        else
        {
            p_grounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(idealPosA, 0.1f);
        //Gizmos.DrawSphere(idealPosB, 0.1f);
        float distA = Vector2.Distance(otherChunk.transform.position, transform.position);
        Debug.DrawLine(transform.position, otherChunk.transform.position, Color.Lerp(Color.blue, Color.red, distA * Time.deltaTime));
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.DrawSphere(otherChunk.transform.position, 0.05f);
    }
}
