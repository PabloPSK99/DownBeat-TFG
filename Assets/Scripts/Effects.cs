using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public GameObject shockWave;
    public GameObject dust;
    public GameObject flash;
    public GameObject trail;
    public GameObject wideTrail;
    public GameObject bulletExplosion;
    public GameObject plusBulletExplosion;
    public GameObject flashCharge;
    public GameObject techCharge;
    public GameObject reloadSparks;
    public float flashTrailDelay;
    public GameObject flashImpact;
    public GameObject attackCharge;
    public Transform leftHand;
    public Transform rightHand;
    public Transform chest;
    public Transform enemy;

    public void Punch()
    {
        Destroy(Instantiate(shockWave, rightHand), 2);
        Destroy(Instantiate(bulletExplosion, (rightHand.position + enemy.position) / 2, bulletExplosion.transform.rotation), 2);
    }

    public void Shoot()
    {
        Destroy(Instantiate(shockWave, rightHand), 2);
        Destroy(Instantiate(flash, rightHand), 1);
    }

    public void Bullet(bool plus)
    {
        StartCoroutine(ShootBullet(plus));
    }

    public void ChargeAttack()
    {
        Destroy(Instantiate(attackCharge, rightHand), 2);
    }

    public void Reload()
    {
        print("!");
        Destroy(Instantiate(reloadSparks, transform), 5f);
        Destroy(Instantiate(techCharge, chest), 2f);
    }

    public void ChargeFlash()
    {
        GameObject t = Instantiate(flashCharge, leftHand);
        Destroy(t, 3);
        StartCoroutine(Trail(t.GetComponentInChildren<TrailRenderer>(), flashTrailDelay));
    }

    public void FlashImpact()
    {
        Destroy(Instantiate(flashImpact, enemy.position + (leftHand.position - enemy.position).normalized, flashImpact.transform.rotation), 2);
    }

    IEnumerator ShootBullet(bool plus)
    {
        GameObject t = Instantiate(plus? wideTrail : trail, rightHand.position, Quaternion.identity);
        StartCoroutine(Trail(t.GetComponent<TrailRenderer>()));
        yield return null;
        t.transform.position = enemy.position;
        Destroy(Instantiate(plus ? plusBulletExplosion : bulletExplosion, enemy.position + (leftHand.position - enemy.position).normalized/2, bulletExplosion.transform.rotation), 1);
    }

    IEnumerator Trail(TrailRenderer tr, float delay)
    {
        tr.enabled = false;
        yield return new WaitForSeconds(delay);
        tr.enabled = true;
        float fadeTime = tr.time;
        Destroy(tr.gameObject, fadeTime);
        yield return null;
        while (tr != null)
        {
            print(tr.widthMultiplier);
            tr.widthMultiplier -= Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    IEnumerator Trail(TrailRenderer tr)
    {
        float fadeTime = tr.time;
        Destroy(tr.gameObject, fadeTime);
        yield return null;
        while (tr != null)
        {
            print(tr.widthMultiplier);
            tr.widthMultiplier -= Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
