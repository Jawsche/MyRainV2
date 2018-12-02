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



    // Use this for initialization
    void Start () {
        thePlayer = Player.gameObject.GetComponent<CharacterController>();
        headOfssetInit = headOffset;
	}
	
	// Update is called once per frame
	void Update () {

        idealPosA = new Vector2(B.position.x + headOffset.x, B.position.y + headOffset.y);
        idealPosB = Player.position;
        float distBtwn = Vector2.Distance(A.position, B.position);
        Vector2 dirVecBtwn = (B.position - A.position).normalized;;

        if (thePlayer.i_prone)
        {
            headOffset = new Vector2(thePlayer.i_facingDir, 0f);
        }
        else headOffset = headOfssetInit;

        //A
        float distA = Vector2.Distance(idealPosA, A.position);
        trgDistA = 1f - headOffset.normalized.magnitude;

        Vector2 dirVecA;
        if (distA > 0f)
            dirVecA = (idealPosA - A.position).normalized;
        else
            dirVecA = Vector2.up;

        A.MovePosition(A.position - (trgDistA - distA) * dirVecA * ratioA);

        //B
        float distB = Vector2.Distance(idealPosB, B.position);

        Vector2 dirVecB;
        if (distB > 0)
            dirVecB = (idealPosB - B.position).normalized;
        else if (distBtwn > trgDistBtwn)
            dirVecB = -dirVecBtwn;
        else
            dirVecB = Vector2.zero;

        B.MovePosition(B.position + (distB) * dirVecB * ratioB);


    }

}
