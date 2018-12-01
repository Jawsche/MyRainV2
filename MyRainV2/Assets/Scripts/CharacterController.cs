using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    Rigidbody2D c_RB;

    public float a_moveSpeed;
    public float a_jumpPower;

    bool i_grounded = false;

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


        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        CheckGrounded();

        c_RB.velocity = new Vector2(h * a_moveSpeed, c_RB.velocity.y);

        //c_RB.AddForce(new Vector2(h * a_moveSpeed, c_RB.velocity.y));

        if (Input.GetButtonDown("Jump") && i_grounded)
        {
            c_RB.velocity = new Vector2(c_RB.velocity.x, a_jumpPower);
        }
        if (Input.GetButtonUp("Jump") && c_RB.velocity.y > 0f)
        {
            c_RB.velocity = new Vector2(c_RB.velocity.x, 0f);
        }
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
