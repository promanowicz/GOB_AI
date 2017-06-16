using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{

    public Transform debugTransform;

    public float timeToLiveSeconds = 4;
    public float speed = 3;
    public bool isFired = false;

    private Rigidbody mRigidbody;

    public void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        if(debugTransform!=null)
        fireAt(debugTransform);
    }

    public void fireAt(Transform t){
        transform.LookAt(t);
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.velocity = (t.position - transform.position).normalized * speed;
        isFired = true;
    }

    public	void Update () {
	    if (isFired){
	        timeToLiveSeconds -= Time.deltaTime;
	        if (timeToLiveSeconds < 0){
	            Destroy(gameObject);
	        }
	    }
	}

    private void OnCollisionEnter(Collision col){
        Destroy(gameObject);
    }
}
