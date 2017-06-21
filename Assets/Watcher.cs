using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour {
    private float timeToFire = 0;
    public GameObject bulletPrefab;
    public float shootCooldown = 2;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timeToFire > 0)
        {
            timeToFire -= Time.deltaTime;
        }
    }

    public void shootAt(Transform t)
    {
        if (timeToFire <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position + t.position.normalized * 2, transform.rotation);
            Bullet bulletControl = bullet.GetComponent<Bullet>();
            bulletControl.fireAt(t);

            timeToFire = shootCooldown;
        }
    }

    public void OnTriggerStay(Collider col){

        shootAt(col.gameObject.transform);
        
    }
}
