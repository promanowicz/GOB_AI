

using UnityEngine;

class ArmedCharacter : MovableCharacter{
    public Transform debugShootTarget;

    public GameObject bulletPrefab;
    public float shootCooldown = 2;
    private float timeToFire = 0;

    public int magCapacity = 7;
    public int totalAmmo = 4;
    public int currentMagAmmo = 2;

    public void Start(){
        base.Start();
        if (debugShootTarget != null){
            shootAt(debugShootTarget);
        }
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
            if (totalAmmo == 0){
                outOfAmmo();
            }
            else{
                reloadNeeded();
            }
        }
    }

    public void reload(){
    }

    protected void reloadNeeded(){
    }

    protected void outOfAmmo(){
    }

    public void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        GameObject collisioningObj = collision.gameObject;
        if (collisioningObj.tag == "ammo"){
            totalAmmo += 10;
            if (currentMagAmmo == 0){
                reloadNeeded();
            }
            Destroy(collisioningObj);
        }
    }

    public void OnTriggerEnter(Collider col){
        base.OnTriggerEnter(col);
    }
}
