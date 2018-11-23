using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ChunkBind : MonoBehaviour {

    public GameObject otherChunk;

    public float moveSpeed = 0.5f;
    private float moveSpeedInit;
    public float returnStrength = 2000f;
    public float returnDist = 0f;
    [Range (0f,1f)]
    public float returnStrengthRatioA = 0.7f;
    [Range(0f, 1f)]
    public float returnStrengthRatioB;

    public Rigidbody2D rigidBodyA;
    public Rigidbody2D rigidBodyB;
    
    public Vector2 idealPosA;

    private bool p_grounded = false;
    private bool p_standing = false;
    private float p_facingDir = 1.0f;


   
    public float a_jumpStrangth = 20.0f;
    [Range(1, 10)]
    public float fallMultiplier = 2.5f;
    [Range(1, 10)]
    public float lowJumpMultiplier = 2f;

    // Use this for initialization
    void Start () {
        rigidBodyA = GetComponent<Rigidbody2D>();
        rigidBodyB = otherChunk.GetComponent<Rigidbody2D>();
        moveSpeedInit = moveSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        returnStrengthRatioB = 1f - returnStrengthRatioA;
        Vector3 target = (new Vector3(otherChunk.transform.position.x + idealPosA.x, otherChunk.transform.position.y + idealPosA.y));
        float dist = Vector2.Distance(target, transform.position);
        Vector2 dirVecA = (target - transform.position).normalized;
        Vector2 dirVecB = -(target - transform.position).normalized;
        Debug.Log(dist);

        if (dist > returnDist)
        {
            rigidBodyA.velocity += Vector2.Lerp(Vector2.zero,  dirVecA * dist, Time.deltaTime * (returnStrength* returnStrengthRatioA));
            rigidBodyB.velocity += Vector2.Lerp(Vector2.zero, dirVecB * dist, Time.deltaTime * (returnStrength * returnStrengthRatioB));
        }

        PlayerInput();


    }

    

    private void PlayerInput()
    {
        //Movement
        CheckGrounded();
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        if (h < 0.0f)
            p_facingDir = -1.0f;
        if (h > 0.0f)
            p_facingDir = 1.0f;
        if (h != 0)
        {
            rigidBodyA.velocity += new Vector2(moveSpeed * h * returnStrengthRatioA, 0f);
        }
        if (v != 0)
        {
            rigidBodyA.velocity += new Vector2(0f, moveSpeed * v * returnStrengthRatioA);
        }
        //Jump
        if (CrossPlatformInputManager.GetButtonDown("Jump") && p_grounded)
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
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && p_standing == true)
        {
            p_standing = false;
            if (p_facingDir == 1.0f)
                idealPosA = new Vector2(1f, 0f);
            else if (p_facingDir == -1.0f)
                idealPosA = new Vector2(-1f, 0f);
            moveSpeed *= 0.7f;
        }
        //stand back up
        else if (CrossPlatformInputManager.GetButtonDown("Fire1") && p_standing == false)
        {
            p_standing = true;
            idealPosA = new Vector2(0f, 1f);
            moveSpeed = moveSpeedInit;
        }
    }

    private void CheckGrounded()
    {
        //Set Grounded variable
        RaycastHit2D hit = Physics2D.Raycast(otherChunk.transform.position, -Vector2.up, 0.6f);
        if (hit.collider != null)
        {
            p_grounded = true;
            Debug.DrawRay(otherChunk.transform.position, Vector3.down * 0.6f, Color.red);
            //Debug.Log(hit.collider.name);
        }
        else
        {
            p_grounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        float dist = Vector2.Distance(otherChunk.transform.position, (new Vector3(transform.position.x + idealPosA.x, transform.position.y + idealPosA.y)));

        Debug.DrawLine(transform.position, otherChunk.transform.position, Color.blue);
        Debug.DrawLine((new Vector3(otherChunk.transform.position.x + idealPosA.x, otherChunk.transform.position.y + idealPosA.y)), otherChunk.transform.position, Color.green);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere((new Vector3(otherChunk.transform.position.x + idealPosA.x, otherChunk.transform.position.y + idealPosA.y)), 0.1f);
        Gizmos.DrawSphere(otherChunk.transform.position, 0.1f);
    }
}
