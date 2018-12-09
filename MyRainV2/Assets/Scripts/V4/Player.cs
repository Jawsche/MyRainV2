using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    public Rigidbody2D c_RB;
    public Rigidbody2D bottom_RB;

    public float a_moveSpeedA;
    public float a_moveSpeedB;
    public float a_jumpPower;

    public bool i_grounded = false;
    public bool i_prone = false;
    public float i_facingDir = 1.0f;
    public bool i_isRunning = false;

    [Range(0f, 1f)]
    public float damping = 0.2f;



    // Update is called once per frame
    void Update()
    {
        //Debugging mouse snap
        if (Input.GetButton("Fire2"))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 mousePt = Camera.main.ScreenToWorldPoint(pos);
            Vector3 temp = mousePt;
            temp.z = 0;
            c_RB.position = temp;
        }

        PlayerInput();

        // going prone
        if (!i_prone && i_grounded && Input.GetAxisRaw("Vertical") <= -0.6f)
        {
            i_prone = true;
            a_moveSpeedA /= 2f;
            a_moveSpeedB /= 2f;
        }
        if (i_prone && i_grounded && Input.GetAxisRaw("Vertical") >= 0.6f)
        {
            i_prone = false;
            a_moveSpeedA *= 2f;
            a_moveSpeedB *= 2f;
        }


    }

    void PlayerInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0f && i_grounded)
            i_isRunning = true;
        else i_isRunning = false;


        CheckGrounded();
        MovePlayer(h);

        if (Input.GetButtonDown("Jump") && i_grounded)
        {
            Jump(c_RB.velocity.x, a_jumpPower);
            Jump(bottom_RB.velocity.x, a_jumpPower);
        }
        if (Input.GetButtonUp("Jump") && c_RB.velocity.y > 0f)
        {
            KillYVel();
        }
    }

    void MovePlayer(float horizontalInput)
    {
        if (horizontalInput > 0f)
            i_facingDir = 1f;
        else if (horizontalInput < 0f)
            i_facingDir = -1f;

        c_RB.AddForce(new Vector2(horizontalInput * a_moveSpeedA, c_RB.velocity.y));
        c_RB.velocity *= Mathf.Pow(1f - damping, Time.deltaTime * 10f);

        bottom_RB.AddForce(new Vector2(horizontalInput * a_moveSpeedB, bottom_RB.velocity.y));
        bottom_RB.velocity *= Mathf.Pow(1f - damping, Time.deltaTime * 10f);
    }

    void Jump(float jumpX, float jumpY)
    {
        c_RB.velocity = new Vector2(jumpX, jumpY);
        bottom_RB.velocity = new Vector2(jumpX, jumpY);
    }

    void KillYVel()
    {
        c_RB.velocity = new Vector2(c_RB.velocity.x, 0f);
        bottom_RB.velocity = new Vector2(bottom_RB.velocity.x, 0f);
    }

    private void CheckGrounded()
    {
        //Set Grounded variable
        int layerMask = ~(1 << 8);
        RaycastHit2D hit = Physics2D.Raycast(bottom_RB.position, -Vector2.up, 0.6f, layerMask);
        if (hit.collider != null)
        {
            i_grounded = true;
            Debug.DrawRay(bottom_RB.position, Vector3.down * 0.6f, Color.red);
        }
        else
        {
            i_grounded = false;
        }


    }
}
