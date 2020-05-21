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
    public GameObject blockCharge;
    public GameObject blockChargeHalf;
    public GameObject flashCharge;
    public GameObject techCharge;
    public GameObject reloadSparks;
    public GameObject spearTrails;
    public float flashTrailDelay;
    public GameObject flashImpact;
    public GameObject attackCharge;
    public Transform leftHand;
    public Transform rightHand;
    public Transform rightFoot;
    public Transform chest;
    public Transform enemy;
    public Transform halberd;
    public Transform halberdPoint;
    public Transform halberdPivot;

    public void FixHalberd()
    {
        StartCoroutine(FixHalberdWhile(0.2f));
        GameObject t = Instantiate(spearTrails, halberd);
        TrailRenderer[] trs = t.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer tr in trs)
        {
            StartCoroutine(Trail(tr));
            StartCoroutine(StopEmit(tr, 0.2f));
        }
        iTween.MoveBy(t, Vector3.down * 15, 0.2f);
        iTween.RotateBy(t, Vector3.up * 1, 0.2f);
        iTween.ScaleTo(t.transform.GetChild(0).gameObject, Vector3.zero, 0.2f);
        Destroy(t, 2);
    }

    IEnumerator FixHalberdWhile(float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            halberd.LookAt(halberdPivot);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator StopEmit(TrailRenderer tr, float time)
    {
        yield return new WaitForSeconds(time);
        tr.emitting = false;
    }

    public void RevertHalberd(float time)
    {
        StartCoroutine(RevertHalberdWhile(time));
    }

    IEnumerator RevertHalberdWhile(float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            halberd.localRotation = Quaternion.Lerp(halberd.localRotation, Quaternion.identity, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        halberd.localRotation = Quaternion.identity;
    }

    public void Sweep()
    {
        Transform parent = halberdPivot.parent;
        halberdPivot.SetParent(transform);
        halberdPivot.position += Vector3.up;
        GameObject t = Instantiate(spearTrails, halberdPivot);
        TrailRenderer[] trs = t.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer tr in trs)
        {
            StartCoroutine(Trail(tr));
            StartCoroutine(StopEmit(tr, 0.3f));
        }
        iTween.RotateBy(t, Vector3.up * 3, 0.2f);
        iTween.ScaleTo(t.transform.GetChild(0).gameObject, Vector3.zero, 0.3f);
        StartCoroutine(RotateByEased(1f, 0.2f, iTween.EaseType.easeOutSine));
        StartCoroutine(NewParent(halberdPivot, parent, 0.2f));
        Destroy(t, 2);
    }

    IEnumerator NewParent(Transform target, Transform newParent, float time)
    {
        yield return new WaitForSeconds(time);
        target.parent = newParent;
    }

    IEnumerator RotateByEased(float rotation, float duration, iTween.EaseType easetype)
    {
        float lastRotation = transform.rotation.eulerAngles.y;
        iTween.RotateBy(gameObject, iTween.Hash(
            "amount", Vector3.up * rotation,
            "time", duration,
            "easetype", easetype
            )
        );
        yield return new WaitForSeconds(duration);
        transform.rotation = Quaternion.Euler(0, lastRotation + rotation * 360, 0);
    }

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
        Destroy(Instantiate(reloadSparks, transform), 5f);
        Destroy(Instantiate(techCharge, chest), 2f);
    }

    public void Block(bool downBeat)
    {
        Destroy(Instantiate(downBeat? blockCharge:blockChargeHalf, rightHand), 3f);
    }

    public void Fail()
    {
        ParticleSystem[] pSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pSystems)
        {
            Destroy(ps.gameObject);
        }
    }

    public void ChargeFlash()
    {
        GameObject t = Instantiate(flashCharge, leftHand);
        Destroy(t, 3);
        StartCoroutine(Trail(t.GetComponentInChildren<TrailRenderer>(), flashTrailDelay));
    }

    public void ChargeTwirl()
    {
        Destroy(Instantiate(techCharge, chest), 2f);
    }

    public void FlashImpact()
    {
        Destroy(Instantiate(flashImpact, enemy.position + (leftHand.position - enemy.position).normalized, flashImpact.transform.rotation), 2);
    }

    public void TwirlImpact()
    {
        Destroy(Instantiate(flashImpact, (enemy.position+rightFoot.position)/2, flashImpact.transform.rotation), 2);
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
            if (tr.widthMultiplier > 0) tr.widthMultiplier -= Time.deltaTime / fadeTime;
            else tr.widthMultiplier = 0;
            yield return null;
        }
    }
}
