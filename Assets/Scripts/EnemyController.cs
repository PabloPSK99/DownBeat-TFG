using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    [Header("Stats")]
    public int maxHealth;
    private int health;

    [Header("UI")]

    public Image healthBar;
    public Image damageBar;
    public float healthBarTime;
    public float damageBarTime;
    public float damageBarDelay;

    private void Awake()
    {
        health = maxHealth;
    }

    public void GetAttack(float damage)
    {
        GetDamage(damage);
    }

    private void GetDamage(float damage)
    {
        health = Mathf.Max(health-Mathf.RoundToInt(damage), 0);
        StartCoroutine(UpdateHealthBar());
    }

    IEnumerator UpdateHealthBar()
    {
        float scale = (float)health / maxHealth;
        iTween.ScaleTo(healthBar.gameObject, new Vector3(scale, 1, 1), healthBarTime);
        yield return new WaitForSeconds(damageBarDelay);
        iTween.ScaleTo(damageBar.gameObject, iTween.Hash(
            "scale", new Vector3(scale, 1, 1),
            "time", damageBarTime,
            "easetype", iTween.EaseType.linear
            )
        );
        yield return null;
    }
}
