using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunks : MonoBehaviour {

    public Transform Player;

    public Vector2 trgA;
    public Vector2 trgB;

    public Rigidbody2D A;
    public Rigidbody2D B;

    public float trgDistBtwn = 1f;

    public float ratioA = 0.7f;
    public float ratioB = 0.2f;

    public Vector2 idealPosA;
    public Vector2 idealPosB;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        idealPosA = new Vector2(Player.position.x, Player.position.y + 1f);
        idealPosB = Player.position;
        float distBtwn = Vector2.Distance(A.position, B.position);
        Vector2 dirVecBtwn = (B.position - A.position).normalized;;


        //A
        float distA = Vector2.Distance(idealPosA, A.position);

        Vector2 dirVecA;
        if (distA > 0)
            dirVecA = (idealPosA - A.position).normalized;
        else if (distBtwn > trgDistBtwn)
            dirVecA = dirVecBtwn;
        else
            dirVecA = Vector2.zero;

        A.MovePosition(A.position - (0 - distA) * dirVecA * ratioA);

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
