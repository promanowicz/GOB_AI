﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableCharacter : GoalOrientedCharacter{

    public Transform debugDst;

    public float playerSpeed=3;
    private Vector3 goToPoint;
    private Rigidbody mRigidbody;
    private bool hasGoToPos = false;
    private bool isCrawling = false;

    public void startCrowling(){
        if (isCrawling) return;
        transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y-0.5f,transform.localScale.z);
        transform.position = new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z);
        isCrawling = true;
    }

    public void stopCrawling(){
        if (!isCrawling) return;
        transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y+0.5f,transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        isCrawling = false;
    }

    public void Start()
    {
        base.Start();
        goToPoint = transform.position;
        mRigidbody = GetComponent<Rigidbody>();
        if(debugDst!=null)
        goToPosition(debugDst);
    }

    public void goToPosition(Transform t){
        hasGoToPos = true;
        goToPoint = new Vector3(t.position.x,transform.position.y,t.position.z);
    }

    public void FixedUpdate()
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
                Vector3 velocity = (goToPoint - transform.position).normalized * (isCrawling?playerSpeed/2:playerSpeed);
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


    public void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);
    }


}
