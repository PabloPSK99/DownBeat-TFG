using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public GameObject shockWave;
    public GameObject dust;
    public GameObject flash;
    public GameObject bullet;
    public GameObject plusBullet;
    public GameObject bulletExplosion;
    public GameObject plusBulletExplosion;
    public GameObject techCharge;
    public GameObject attackCharge;
    public Transform hand;
    public Transform enemy;

    public void Punch()
    {
        Destroy(Instantiate(shockWave, hand), 2);
        Destroy(Instantiate(bulletExplosion, (hand.position + enemy.position) / 2, Quaternion.identity), 2);
    }

    public void Shoot()
    {
        Destroy(Instantiate(shockWave, hand), 2);
        Destroy(Instantiate(flash, hand), 1);
    }

    public void Bullet(bool plus)
    {
        StartCoroutine(ShootBullet(plus));
    }

    public void ChargeAttack()
    {
        Destroy(Instantiate(attackCharge, hand), 2);
    }

    public void ChargeTech()
    {

    }

    IEnumerator ShootBullet(bool plus)
    {
        GameObject b = Instantiate(plus ? plusBullet : bullet, hand.position, bullet.transform.rotation);
        yield return null;
        b.transform.position = enemy.position;
        Destroy(Instantiate(plus ? plusBulletExplosion : bulletExplosion, enemy.position, Quaternion.identity), 1);
        Destroy(b);
    }
}
