using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public Rigidbody2D topChunkRB;
    public Rigidbody2D bottomChunkRB;
    public float moveSpeed = 2.0f;
    [Range(0f, 1f)]
    public float getToDiagRatioTop = 0.5f;
    [Range(0f, 1f)]
    public float getToDiagRatioBottom = 0.5f;
    [Range(0f, 1f)]
    public float fHorizontalDamping = 0.22f;
    [Range(0f, 0.99f)]
    public float getToDiagDamping = 0.99f;
    public float getToDiag = 17f;
    public float returnDist = 0f;
    public bool p_grounded = false;

    public Vector2 topChunkVel;
    public Vector2 bottomChunkVel;


    private void FixedUpdate()
    {

        // Debugging mouse snap
        if (Input.GetButton("Fire2"))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 mousePt = Camera.main.ScreenToWorldPoint(pos);
            Vector3 temp = mousePt;
            temp.z = 0;
            bottomChunkRB.position = temp;
        }

        //getToDiagRatioBottom = 1f - getToDiagRatioTop;


        //CheckGrounded();
        //if (p_grounded)
        //{
        //    topChunkVel.y = 0f;
        //    bottomChunkVel.y = 0f;
        //}

        Movement();

        //topChunkRB.position += topChunkVel;
        //bottomChunkRB.position += bottomChunkVel;
        
        float distance = Vector3.Distance(topChunkRB.position, bottomChunkRB.position);
        Vector2 dirVecTop = moveToPoint(topChunkRB.position, (bottomChunkRB.position + (Vector2.up * getToDiag)));
        Vector2 dirVecBot = moveToPoint(topChunkRB.position, bottomChunkRB.position);

        Vector2 dirVec = moveToPoint(topChunkRB.position, bottomChunkRB.position);


        ////Old pseudo Code///////////////////////////////////////
        //float dirX = dirVec.x;
        //float dirY = dirVec.y;
        //topChunkVel.x = topChunkVel.x - (getToDiag - distance) * dirX * getToDiagRatioTop;
        //topChunkVel.y = topChunkVel.y - (getToDiag - distance) * dirY * getToDiagRatioTop;
        //bottomChunkVel.x = bottomChunkVel.x + (getToDiag - distance) * dirX * getToDiagRatioBottom;
        //bottomChunkVel.y = bottomChunkVel.y + (getToDiag - distance) * dirY * getToDiagRatioBottom;
        /////////////////////////////////////////////////////////


        ///
        // Supposed formula for forces//////////////////////////
        //var wantedVel : float = 13.37; //we want 13.37 velocity
        //function Start()
        //{
        //    addVel(); //call our force adding function
        //}
        //function addVel()
        //{
        //    var calculatedForce : float = wantedVel * (50 * rigidbody.mass); //calculate force amount
        //rigidbody.AddForce(Vector3(calculatedForce, 0, 0)); //add force on X
        //Debug.Log("velocity: " + rigidbody.velocity.x); //debug to see final velocity
        //}
        //CONSOLE:
        // velocity: 13.37
        ///////////////////////////////////////////////////////
        ///

        Vector2 desVelTop = topChunkRB.velocity - (getToDiag - distance) * dirVecTop * getToDiagRatioTop;
        Vector2 desVelBot = bottomChunkRB.velocity + (getToDiag - distance) * dirVec * getToDiagRatioBottom;

        //topChunkRB.AddForce(addVel(desVelTop, topChunkRB));
        //bottomChunkRB.AddForce(addVel(desVelBot, bottomChunkRB));
        desVelTop *= Mathf.Pow(1f - getToDiagDamping, Time.deltaTime * 10f);
        desVelBot *= Mathf.Pow(1f - getToDiagDamping, Time.deltaTime * 10f);

        desVelTop.y += Physics2D.gravity.y * topChunkRB.gravityScale;
        desVelBot.y += Physics2D.gravity.y * bottomChunkRB.gravityScale;

        topChunkRB.velocity = desVelTop;
        bottomChunkRB.velocity = desVelBot;
        
    }

    Vector2 addVel(Vector2 wantedVelocity, Rigidbody2D rb)
    {
        Vector2 calculatedForce = wantedVelocity * (50f * rb.mass);
        //rb.AddForce(calculatedForce);
        return (calculatedForce);
    }

    Vector3 moveToPoint(Vector3 point1, Vector3 point2)
    {
        point2 = point2 - point1;
        float distanceBetween = Vector3.Magnitude(point2);

        Vector3 dirVecToPoint;
        if (distanceBetween > returnDist)
        {
            dirVecToPoint = point2.normalized;
        }
        else
        {
            dirVecToPoint = Vector3.up * getToDiag;
        }
        return dirVecToPoint;

    }

    void CheckGrounded()
    {
        //Set Grounded variable
        int layerMask = ~(1 << 8);
        RaycastHit2D hit = Physics2D.Raycast(bottomChunkRB.position, -Vector2.up, 0.8f, layerMask);
        if (hit.collider != null)
        {
            p_grounded = true;
            Debug.DrawRay(bottomChunkRB.position, Vector3.down * 0.8f, Color.red);
        }
        else p_grounded = false;
        
    }

    void Movement()
    {
        //Moving Left & Right
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        if (HorizontalInput != 0f)
        {
            topChunkVel.x += HorizontalInput * moveSpeed;
            topChunkVel.x *= Mathf.Pow(1f - fHorizontalDamping, Time.deltaTime * 10f);
            bottomChunkVel.x += HorizontalInput * moveSpeed;
            bottomChunkVel.x *= Mathf.Pow(1f - fHorizontalDamping, Time.deltaTime * 10f);
        }
        else
        {
            topChunkVel.x = 0f;
            bottomChunkVel.x = 0f;
        }
    }




}
