using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    Rigidbody2D c_RB;

    public float a_moveSpeed;
    public float a_jumpPower;

    public bool i_grounded = false;
    public bool i_prone = false;
    public float i_facingDir = 1.0f;
    public bool i_isRunning = false;

    // Use this for initialization
    void Start()
    {
        c_RB = GetComponent<Rigidbody2D>();
    }

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
            a_moveSpeed /= 2f;
        }
        if (i_prone && i_grounded && Input.GetAxisRaw("Vertical") >= 0.6f)
        {
            i_prone = false;
            a_moveSpeed *= 2f;
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

        c_RB.velocity = new Vector2(horizontalInput * a_moveSpeed, c_RB.velocity.y);
    }

    void Jump(float jumpX, float jumpY)
    {
        c_RB.velocity = new Vector2(jumpX, jumpY);
    }

    void KillYVel()
    {
        c_RB.velocity = new Vector2(c_RB.velocity.x, 0f);
    }

    private void CheckGrounded()
    {
        //Set Grounded variable
        int layerMask = ~(1 << 8);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.6f, layerMask);
        if (hit.collider != null)
        {
            i_grounded = true;
            Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.red);
        }
        else
        {
            i_grounded = false;
        }


    }
}
