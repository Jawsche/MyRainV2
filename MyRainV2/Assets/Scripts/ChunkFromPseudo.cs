using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkFromPseudo : MonoBehaviour {

    public Transform AT;
    public Transform BT;

    public Vector2 A;
    public Vector2 B;
    public Vector2 sA;
    public Vector2 sB;

    private void Start()
    {
        A = AT.position;
        B = BT.position;
    }

       
    void Update() {
     A.x = A.x + sA.x;

     A.y = A.y + sA.y;

     B.x = B.x + sB.x;

     B.y = B.y + sB.y;


     sA.x = sA.x * 0.98f;

     sA.y = (sA.y * 0.98f) + 1.2f;

     sB.x = sB.x * 0.98f;

     sB.y = (sB.y * 0.98f) + 1.2f;


     float diag = Diag(A, B);

     Vector2 rtrn = MoveToPoint(A, B);

     float dirX = rtrn.x;

     float dirY = rtrn.y;

     float getToDiag = 17f;


     A.x = A.x - (getToDiag - diag) * dirX * 0.5f;

     sA.x = sA.x - (getToDiag - diag) * dirX * 0.5f;

     A.y = A.y - (getToDiag - diag) * dirY * 0.5f;

     sA.y = sA.y - (getToDiag - diag) * dirY * 0.5f;

     B.x = B.x + (getToDiag - diag) * dirX * 0.5f;

     sB.x = sB.x + (getToDiag - diag) * dirX * 0.5f;

     B.y = B.y + (getToDiag - diag) * dirY * 0.5f;

     sB.y = sB.y + (getToDiag - diag) * dirY * 0.5f;

     AT.position = A;
     BT.position = B;
    }

    float Diag(Vector2 point1, Vector2 point2)
    {
        float rectHeight = Mathf.Abs(point1.y - point2.y);

        float rectWidth = Mathf.Abs(point1.x - point2.x);

        float diagonal = Mathf.Sqrt((rectHeight * rectHeight) + (rectWidth * rectWidth));
        return diagonal;
    }

    Vector2 MoveToPoint(Vector2 point1, Vector2 point2)
    {
        point2.x = point2.x - point1.x;

        point2.y = point2.y - point1.y;

        Vector2 dirVec;

        float diag = Diag(Vector2.zero, point2);
        if (diag > 0)
        {
            dirVec.x = point2.x / diag;

            dirVec.y = point2.y / diag;
        }
        else
        {
            dirVec.x = 0;

            dirVec.y = 1;
        }
        Debug.Log(dirVec);
        return (dirVec);
    }
    }