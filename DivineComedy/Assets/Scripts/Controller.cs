using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    private float horizontalInput;
    private Vector3 curPos, virgilPos;

    public float minInput, moveDist, followDist;
    public GameObject virgil;
	
	// Update is called once per frame
	void Update () {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        curPos = transform.position;
        virgilPos = virgil.transform.position;

        if (horizontalInput > minInput)
        {
            // move right
            transform.position = new Vector3(curPos.x + moveDist, curPos.y);
            curPos.x = curPos.x + moveDist;
        } else if (horizontalInput < minInput * -1)
        {
            // move left
            transform.position = new Vector3(curPos.x - moveDist, curPos.y);
            curPos.x = curPos.x - moveDist;
        }

        // make sure virgil is following dante!
        if (virgilPos.x > curPos.x + followDist)
        {
            virgil.transform.SetPositionAndRotation(new Vector3(curPos.x + followDist, virgilPos.y), Quaternion.identity);
        }
        else if (virgilPos.x < curPos.x - followDist)
        {
            virgil.transform.SetPositionAndRotation(new Vector3(curPos.x - followDist, virgilPos.y), Quaternion.identity);
        }
    }
}
 