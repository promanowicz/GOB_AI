using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour {
    private float timeToFire = 0;
    public GameObject bulletPrefab;
    public float shootCooldown = 2;
    public Transform destination;
    private Rigidbody mRigidbody;
    private Vector3 goToPoint;
    private bool hasGoToPos = false;
    public int secondDelayToMove=10;
    public int playerSpeed = 5;
    // Use this for initialization
    void Start () {
        mRigidbody = GetComponent<Rigidbody>();
        goToPoint = destination.position;
        goToPoint.y = transform.position.y;
    }

    public void goToPosition(Transform t)
    {
        goToPosition(new Vector3(t.position.x, transform.position.y, t.position.z));
    }

    public void goToPosition(Vector3 t)
    {
        t.y = transform.position.y;
        hasGoToPos = true;
        goToPoint = t;
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
                Vector3 velocity = (goToPoint - transform.position).normalized *
                                   playerSpeed;
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

    // Update is called once per frame
    private float timeLive = 0f;
    void Update () {
        if (timeToFire > 0)
        {
            timeToFire -= Time.deltaTime;
        }
        timeLive += Time.deltaTime;
        if (timeLive > secondDelayToMove && !isPositionReached()){
            goToPosition(destination);
        }
    }

    public void shootAt(Transform t)
    {
        if (timeToFire <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position + t.position.normalized * -2, transform.rotation);
            Bullet bulletControl = bullet.GetComponent<Bullet>();
            bulletControl.fireAt(t);

            timeToFire = shootCooldown;
        }
    }

    public void OnTriggerStay(Collider col){
        transform.LookAt(col.gameObject.transform);
        goToPoint = transform.position;
        shootAt(col.gameObject.transform);
        
    }
}
