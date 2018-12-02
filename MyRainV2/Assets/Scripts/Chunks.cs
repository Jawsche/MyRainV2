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

    [Range(0f, 1f)]
    public float ratioA = 0.7f;
    [Range(0f, 1f)]
    public float ratioB = 0.2f;
    [Range(0f, 1f)]
    public float dampA = 0.3f;
    [Range(0f, 1f)]
    public float dampB = 1f;

    public Vector2 headOffset = new Vector2(0f, 1f);
    Vector2 headOfssetInit;
    public Vector2 idealPosA;
    public float trgDistA = 0f;
    public Vector2 idealPosB;
    public float trgDistB = 0f;

    public float bobAmplitude = 0.2f;
    public float bobPeriod = 0.035f;



    // Use this for initialization
    void Start() {
        thePlayer = Player.gameObject.GetComponent<CharacterController>();
        headOfssetInit = headOffset;
    }

    // Update is called once per frame
    void Update() {
        DataCollect();

        Bind(A, idealPosA, trgDistA, ratioA, dampA);
        Bind(B, idealPosB, trgDistB, ratioB, dampB);
    }

    void DataCollect()
    {
        //Saved for later; for side flipping
        //trgDistA = 1f - headOffset.normalized.magnitude;

        float distBtwn = Vector2.Distance(A.position, B.position);
        Vector2 dirVecBtwn = (B.position - A.position).normalized;
        idealPosA = new Vector2(B.position.x + headOffset.x, B.position.y + headOffset.y);
        idealPosB = Player.position;

        States();
    }

    void States()
    {
        if (thePlayer.i_prone) //if prone
            headOffset = new Vector2(thePlayer.i_facingDir, 0f);
        else if (!thePlayer.i_prone && thePlayer.i_isRunning)//standing & running
        {
            //leaning
            headOffset = new Vector2(0.8f * thePlayer.i_facingDir, headOffset.y);

            //Bobbing
            float theta = Time.timeSinceLevelLoad / bobPeriod;
            float distance = bobAmplitude * Mathf.Sin(theta);
            idealPosB = idealPosB + Vector2.up * distance;
        }
        else headOffset = headOfssetInit;
    }


    void Bind(Rigidbody2D rb, Vector2 target, float trgDist, float strength, float dampVal)
    {

        Vector2 idealPos = target;
        float dist = Vector2.Distance(idealPos, rb.position);

        Vector2 dirVec;
        if (dist > 0f)
            dirVec = (idealPos - rb.position).normalized;
        else
            dirVec = Vector2.up;

        Vector2 newPos = rb.position - (trgDist - dist) * dirVec * strength;

        if (thePlayer.i_prone == false)
        {
            Vector2 trgPos = Vector2.Lerp(rb.position, newPos, dampVal);
        }

        else
        {
            //prettier stand when changing directions WIP, setting y = (‑(x^​2))+​1///////////
            //float binomial = newPos.x - rb.position.x;
            //newPos.y = ((binomial * binomial) +  1f);

            Vector2 trgPos = Vector2.Lerp(rb.position, newPos, dampVal);
        }


        rb.MovePosition(newPos);
    }
}
