

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ArmedCharacter : MovableCharacter{
    public Transform debugShootTarget;

    public GameObject bulletPrefab;
    public float shootCooldown = 2;
    private float timeToFire = 0;

    
    private ShootAtEnemy shootAction;
    private RunawayAction runawayAction;
    public void Start(){
        if (debugShootTarget != null){
            shootAt(debugShootTarget);
        }
        shootAction = new ShootAtEnemy(this,this);
        runawayAction = new RunawayAction(this,this);
        base.Start();
    }

    void Update()
    {
        base.Update();
        if (timeToFire > 0)
        {
            timeToFire -= Time.deltaTime;
        }
    }

    public void shootAt(Transform t){
        if (timeToFire <= 0 && currentMagAmmo > 0){
            GameObject bullet = Instantiate(bulletPrefab, transform.position+t.position.normalized*2,transform.rotation);
            Bullet bulletControl = bullet.GetComponent<Bullet>();
            bulletControl.fireAt(t);

            timeToFire = shootCooldown;
            currentMagAmmo--;
        }
        if (currentMagAmmo == 0){
           
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag.Equals("AiBullet"))
        {
            if (hp() < 90){
                AddAction(runawayAction);
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {

    }

    private Transform enemyPosition = null;
    public void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
        if (col.gameObject.tag.Equals(enemyTag)){
            enemyPosition = col.gameObject.transform;
            AddAction(shootAction);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        base.OnTriggerExit(col);
        if (col.gameObject.tag.Equals(enemyTag)){
            enemyPosition = null;
            RemoveAction(shootAction);
            RemoveAction(runawayAction);
        }
    }
    private class ShootAtEnemy : Action
    {
        private ArmedCharacter parent;

        public ShootAtEnemy(ActionCallback listener, ArmedCharacter par) : base(listener)
        {
            parent = par;
        }

        private IEnumerator coroutine;

        public override float getGoalChange(Goal g)
        {
            switch (g.name)
            {
                case Goals.KILL_ENEMY_NAME:
                    return -2;
                default:
                    return 0f;
            }
        }

        public override void performAction(GameObject go, bool isLoggable)
        {
            if(isLoggable)
            Log("");
            if (parent.enemyPosition != null){
                parent.shootAt(parent.enemyPosition);
            }
            parent.RemoveAction(this);
            parent.StartCoroutine(WaitSomeTime(1));
        }

        private IEnumerator WaitSomeTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (parent.enemyPosition != null){
                parent.AddAction(this);
            }
            callback.OnActionEnd(this);
        }
    }

    private class RunawayAction : Action{
        private ArmedCharacter parent;

        public RunawayAction(ActionCallback listener, ArmedCharacter parent) : base(listener)
        {
            this.parent = parent;
        }

        public override float getGoalChange(Goal g)
        {
            switch (g.name)
            {
                case Goals.SURVIVE_NAME:
                    return -3f;
                    break;
                default:
                    return 0f;
                    break;
            }
        }

        public override void performAction(GameObject go,bool isLoggalbe){
            if (isLoggalbe) Log("");
            parent.goToPosition(parent.transform.position-parent.enemyPosition.position);
            parent.StartCoroutine(WaitSomeTime(2));
        }
        private IEnumerator WaitSomeTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
         
            callback.OnActionEnd(this);
        }

    }
}
