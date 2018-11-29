using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleChunk : MonoBehaviour {

    public Transform topChunk;
    public Transform bottomChunk;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        

        transform.position = (topChunk.position + bottomChunk.position) / 2f;
        Vector3 dirVec = (topChunk.position - bottomChunk.position);
        float angle = Vector3.SignedAngle(Vector3.up, dirVec, Vector3.forward);
        Vector3 temp = transform.eulerAngles;
        temp.z = angle;
        transform.eulerAngles = temp;

        Vector3 tempScale = transform.localScale;
        tempScale.y = dirVec.magnitude;
        transform.localScale = tempScale;


	}
}
