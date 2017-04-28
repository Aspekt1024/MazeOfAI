using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public GameObject BulletPrefab;

    private const float bulletSpeed = 15f;
    private const float rechargeTime = 1f;
    private float timeLastShot;
    
    public void Shoot(Transform target)
    {
        if (timeLastShot + rechargeTime < Time.time)
        {
            timeLastShot = Time.time;
            FireBullet(target);
        }
    }

    private void FireBullet(Transform target)
    {
        if (BulletPrefab == null) return;
        GameObject bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        bullet.transform.LookAt(target.position);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        Destroy(bullet, 3f);
    }
}
