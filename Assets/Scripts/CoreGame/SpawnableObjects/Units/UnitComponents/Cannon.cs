using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public GameObject BulletPrefab;
    public float bulletSpeed = 15f;
    public float rechargeTime = 1f;

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
        bullet.GetComponent<Bullet>().SetOwner(GetUnit(transform));
        bullet.transform.LookAt(target.position);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        Destroy(bullet, 3f);
    }

    private Unit GetUnit(Transform tf)
    {
        Unit unit = tf.GetComponent<Unit>();
        if (unit != null)
        {
            return unit;
        }

        unit = tf.GetComponentInParent<Unit>();
        if (unit != null)
        {
            return unit;
        }

        unit = tf.GetComponentInParent<Transform>().GetComponentInParent<Unit>();
        if (unit != null)
        {
            return unit;
        }
        return null;
    }
}
