using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Rhythm rhythm;
    public PlayerController player;

    [Header("Stats")]
    public int maxHealth;
    private int health;
    public int damage;

    [Header("Actions")]
    public Action currentAction;
    public float successChance;
    public float randomness;

    [Header("UI")]

    public Image healthBar;
    public Image damageBar;
    public float healthBarTime;
    public float damageBarTime;
    public float damageBarDelay;

    private void Awake()
    {
        health = maxHealth;
        currentAction = Action.None;
    }

    private void Update()
    {
        if(currentAction == Action.None && rhythm.normalized < 0.05f)
        {
            currentAction = Action.Attack;
            float random = Random.value * randomness;
            rhythm.ScheduleFunction(2f + random - randomness/2, "Attack", this);
            rhythm.ScheduleFunction(3, "GoIdle", this);
        }
    }

    public void GoIdle()
    {
        currentAction = Action.None;
    }

    public void OffBeat()
    {

    }

    public void Attack()
    {
        CheckSuccess();
        if (successChance == 100f) //Crítico
        {
            player.GetAttack(damage * 1.5f);
        }
        else
        {
            player.GetAttack(damage * successChance / 100);
        }


    }

    public void GetAttack(float damage)
    {
        GetDamage(damage);
    }

    public void GetTech(float damage)
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



    //------------------------------------------SUCCESS----------------------------------------------

    private bool CheckSuccess()
    {
        if (rhythm.IsDownBeat()) //Si es tiempo 
        {
            successChance = Mathf.Min(100, (0.5f - rhythm.normalized) * 200);
        }
        else                   //Si es contratiempo
        {
            successChance = Mathf.Min(100, (rhythm.normalized - 0.5f) * 200);
        }

        if (successChance < 50)
        {
            float rng = Random.value * 100;
            return successChance >= rng;
        }
        return true;
    }


}
