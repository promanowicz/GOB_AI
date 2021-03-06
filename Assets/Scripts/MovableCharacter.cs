﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovableCharacter : GoalOrientedCharacter{
    public Transform debugDst;
    public Transform patrolAPoint;
    public Transform patrolBPoint;

    public float playerSpeed = 3;
    private Rigidbody mRigidbody;
    private Vector3 goToPoint;
    private bool hasGoToPos = false;
    protected bool isCrawling = false;
    private PatrolAction patrolAction;
    private CrowlAction crowlAction;
    private WalkAction walkAction;
    private GetAmmoAction ammoAction = null;
    public int currentMagAmmo = 2;

    public void startCrowling(){
        if (isCrawling) return;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 0.5f, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        isCrawling = true;
        RemoveAction(crowlAction);
        RemoveAction(patrolAction);
        AddAction(walkAction);
    }

    public void stopCrawling(){
        if (!isCrawling) return;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 0.5f, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        isCrawling = false;
        RemoveAction(walkAction);
        AddAction(crowlAction);
        AddAction(patrolAction);
    }

    public void Start(){
        goToPoint = transform.position;
        mRigidbody = GetComponent<Rigidbody>();
        if (debugDst != null)
            goToPosition(debugDst);
        patrolAction = new PatrolAction(this, this);
        crowlAction = new CrowlAction(this, this);
        walkAction = new WalkAction(this, this);

        AddAction(patrolAction);
        //    AddAction(crowlAction);
        base.Start();
    }

    public void goToPosition(Transform t){
        goToPosition(new Vector3(t.position.x, transform.position.y, t.position.z));
    }

    public void goToPosition(Vector3 t){
        t.y = transform.position.y;
        hasGoToPos = true;
        goToPoint = t;
        RemoveAction(patrolAction);
    }

    public void FixedUpdate(){
        if (hasGoToPos && isPositionReached() && !isCrawling){
            AddAction(patrolAction);
            patrolAction.onObjDstReached();
        }

        if (isPositionReached()){
            hasGoToPos = false;
            mRigidbody.velocity = Vector3.zero;
        }
        else{
            if (hasGoToPos){
                transform.LookAt(goToPoint);
                Vector3 velocity = (goToPoint - transform.position).normalized *
                                   (isCrawling ? playerSpeed / 2 : playerSpeed);
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
        return dstX == currX && dstZ == currY || !hasGoToPos;
    }

    public void OnCollisionEnter(Collision collision){
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag.Equals("AiBullet")){
            decreaseHP(5);
            if (!isCrawling){
                decreaseHP(10);
            }
        }
        GameObject collisioningObj = collision.gameObject;
        if (collisioningObj.tag == "ammo")
        {
            currentMagAmmo += 10;
            Destroy(collisioningObj);
            RemoveAction(ammoAction);
            AddAction(patrolAction);
            OnActionEnd(ammoAction);
            ammoAction = null;
            
        }
    }

    private Transform ammoPosition = null;

    public void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
        if (col.gameObject.tag.Equals(enemyTag) && currentlyRunningAction == patrolAction){
            hasGoToPos = false;
            goToPosition((transform.position + col.gameObject.transform.position) / 2);
            patrolAction.onObjDstReached();
            RemoveAction(patrolAction);
        }

        if (col.gameObject.tag.Equals("ammo")){
            ammoPosition = col.gameObject.transform;
            ammoAction = new GetAmmoAction(this, this);
            AddAction(ammoAction);
        }
    }

    public void OnTriggerExit(Collider col){
        base.OnTriggerExit(col);
        if (col.gameObject.tag.Equals(enemyTag) && currentlyRunningAction == patrolAction){
            AddAction(patrolAction);
        }
    }

    private class PatrolAction : Action{
        private MovableCharacter parent;
        private Transform destination;

        public PatrolAction(ActionCallback listener, MovableCharacter parent) : base(listener){
            this.parent = parent;
            destination = parent.patrolAPoint;
        }

        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.TAKE_A_REST_NAME:
                    return 0.4f;
                case Goals.FIND_ENEMY_NAME:
                    return -0.8f;
                default:
                    return 0f;
            }
        }

        public override void performAction(GameObject go, bool isLoggable){
            if (isLoggable)
                Log("");
            if (destination == parent.patrolAPoint)
                destination = parent.patrolBPoint;
            else destination = parent.patrolAPoint;
            parent.decreaseStamina(10);
            parent.goToPosition(destination);
        }

        public void onObjDstReached(){
            callback.OnActionEnd(this);
        }
    }

    private class GetAmmoAction : Action{
        private MovableCharacter parent;
        private Transform destination;

        public GetAmmoAction(ActionCallback listener, MovableCharacter parent) : base(listener){
            this.parent = parent;
            destination = parent.patrolAPoint;
        }

        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.TAKE_A_REST_NAME:
                    return 0.4f;
                case Goals.KILL_ENEMY_NAME:
                    return -1f;
                default:
                    return 0f;
            }
        }

        public override void performAction(GameObject go, bool isLoggable){
            if (isLoggable)
                Log("");

            if (parent.ammoPosition != null){
                parent.decreaseStamina(10);
                parent.goToPosition(parent.ammoPosition);
                parent.RemoveAction(this);
            }
        }

        public void onObjDstReached(){
            callback.OnActionEnd(this);
        }
    }

    private class CrowlAction : Action{
        private MovableCharacter parent;

        public CrowlAction(ActionCallback listener, MovableCharacter parent) : base(listener){
            this.parent = parent;
        }

        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.SURVIVE_NAME:
                    return 0f;
                    break;
                default:
                    return 0f;
                    break;
            }
        }

        public override void performAction(GameObject go, bool isLoggable){
            if (isLoggable) Log("");
            parent.startCrowling();
            callback.OnActionEnd(this);
        }
    }

    private class WalkAction : Action{
        private MovableCharacter parent;

        public WalkAction(ActionCallback listener, MovableCharacter parent) : base(listener){
            this.parent = parent;
        }

        public override float getGoalChange(Goal g){
            switch (g.name){
                case Goals.TAKE_A_REST_NAME:
                    return 0f;
                    break;
                default:
                    return 0f;
                    break;
            }
        }

        public override void performAction(GameObject go, bool isLoggable){
            if (isLoggable)
                Log("");
            parent.stopCrawling();
            callback.OnActionEnd(this);
        }
    }
}
