﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour{
    public Camera playerCamera;


    /*    public Text hpText;
        public Text pointsText;
        public Text TimeLeftText;*/

    public float playerSpeed;
    public string HPpointsDscr = "HP: ";

    private Vector3 goToPoint;

    private Rigidbody mRigidbody;

    public int hpPoints = 100;

    private bool hasGoToPos = false;

    void Start(){
        goToPoint = transform.position;
        mRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private bool isFiring = false;
    void Update(){
        Vector3 positon = Input.mousePosition;
        positon.z = playerCamera.transform.position.y;
        goToPoint = playerCamera.ScreenToWorldPoint(positon);
        goToPoint.y = transform.position.y;
        if (Input.GetButtonDown("Fire1")){
            transform.forward = goToPoint;
            hasGoToPos = true;
        }
        if (Input.GetButtonDown("Fire2")){
            isFiring = true;
        }
        if(Input.GetButtonUp("Fire2")){
            isFiring = false;
        }
        if (isFiring){
            transform.LookAt(goToPoint);
        }
    }

    private void FixedUpdate(){
        if (isPositionReached()){
            hasGoToPos = false;
            mRigidbody.velocity = Vector3.zero;
        }
        else{
            if (hasGoToPos){
                transform.LookAt(goToPoint);
                Vector3 velocity = (goToPoint - transform.position).normalized * playerSpeed;
                if (!velocity.Equals(mRigidbody.velocity))
                    mRigidbody.velocity = velocity;
            }
        }
    }

    bool isPositionReached(){
        int dstX = Mathf.RoundToInt(goToPoint.x);
        int dstZ = Mathf.RoundToInt(goToPoint.z);
        int currX = Mathf.RoundToInt(transform.position.x);
        int currY = Mathf.RoundToInt(transform.position.z);
        return dstX == currX && dstZ == currY;
    }

/*    private void OnCollisionEnter(Collision other){
        if (other.gameObject.tag == "Trap" || other.gameObject.tag=="Enemy"){
            Vector3 otherPos = other.gameObject.transform.position;
            otherPos -= transform.position;
            //otherPos.y = transform.position.y;
            hasGoToPos = false;
            mRigidbody.velocity = Vector3.zero;
            mRigidbody.AddForce(otherPos.normalized * -30);
            hpPoints -= 1;
        }
    }*/

}
