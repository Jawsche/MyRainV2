using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunks : MonoBehaviour {

    public Transform Player;
    CharacterController thePlayer;

    public Vector2 trgA;
    public Vector2 trgB;

    public Rigidbody2D A;
    public Rigidbody2D B;

    public float trgDistBtwn = 1f;

    [Range (0f, 1f)]
    public float ratioA = 0.7f;
    [Range(0f, 1f)]
    public float ratioB = 0.2f;

    public Vector2 headOffset = new Vector2 (0f,1f);
    Vector2 headOfssetInit;
    public Vector2 idealPosA;
    public float trgDistA = 0f;
    public Vector2 idealPosB;
    public float trgDistB = 0f;



    // Use this for initialization
    void Start () {
        thePlayer = Player.gameObject.GetComponent<CharacterController>();
        headOfssetInit = headOffset;
	}
	
	// Update is called once per frame
	void Update () {

        //Saved for later; for flipping
        //trgDistA = 1f - headOffset.normalized.magnitude;
        
        float distBtwn = Vector2.Distance(A.position, B.position);
        Vector2 dirVecBtwn = (B.position - A.position).normalized;


        if (thePlayer.i_prone)
        {
            headOffset = new Vector2(thePlayer.i_facingDir, 0f);
        }
        else headOffset = headOfssetInit;


        idealPosA = new Vector2(B.position.x + headOffset.x, B.position.y + headOffset.y);
        Bind(A, idealPosA, trgDistA, ratioA);

        idealPosB = Player.position;
        Bind(B, idealPosB, trgDistB, ratioB);
    }

    void Bind(Rigidbody2D rb, Vector2 target, float trgDist, float strength)
    {

        Vector2 idealPos = target;
        float dist = Vector2.Distance(idealPos, rb.position);

        Vector2 dirVec;
        if (dist > 0f)
            dirVec = (idealPos - rb.position).normalized;
        else
            dirVec = Vector2.up;

        rb.MovePosition(rb.position - (trgDist - dist) * dirVec * strength);
    }
}
