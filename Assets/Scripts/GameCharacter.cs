using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour{

    public Transform debugDst;

    public float playerSpeed;
    private Vector3 goToPoint;
    private Rigidbody mRigidbody;
    public int hpPoints = 100;
    private bool hasGoToPos = false;

    void Start()
    {
        goToPoint = transform.position;
        mRigidbody = GetComponent<Rigidbody>();
        goToPosition(debugDst);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void goToPosition(Transform t){
        hasGoToPos = true;
        goToPoint = new Vector3(t.position.x,transform.position.y,t.position.z);
    }

    private void FixedUpdate()
    {
        if (isPositionReached())
        {
            hasGoToPos = false;
            mRigidbody.velocity = Vector3.zero;
        }
        else
        {
            if (hasGoToPos)
            {
                transform.LookAt(goToPoint);
                Vector3 velocity = (goToPoint - transform.position).normalized * playerSpeed;
                if (!velocity.Equals(mRigidbody.velocity))
                    mRigidbody.velocity = velocity;
            }
        }
    }

    bool isPositionReached()
    {
        int dstX = Mathf.RoundToInt(goToPoint.x);
        int dstZ = Mathf.RoundToInt(goToPoint.z);
        int currX = Mathf.RoundToInt(transform.position.x);
        int currY = Mathf.RoundToInt(transform.position.z);
        return dstX == currX && dstZ == currY;
    }


}
