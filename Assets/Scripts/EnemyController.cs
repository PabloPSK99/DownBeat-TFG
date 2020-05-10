using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Rhythm rhythm;
    public PlayerController player;
    public Node currentNode;

    [Header("Stats")]
    public int maxHealth;
    private int health;
    public int damage;
    private float damageToBlock;
    public float maxBlockableRatio;
    public bool offbeat;

    [Header("Actions")]
    public Action currentAction;
    public float successChance;
    public float successChanceThreshold;
    public float randomness;
    public float cutFixRatio;

    [Header("UI")]

    public Image healthBar;
    public Image damageBar;
    public float healthBarTime;
    public float damageBarTime;
    public float damageBarDelay;
    public UIController UIController;

    private void Awake()
    {
        health = maxHealth;
        currentAction = Action.None;
    }

    private void Update()
    {
        if (!offbeat)
        {
            Phase1();
        }
        else
        {
            rhythm.ScheduleFunction(3, "RecoverBeat", this);
        }
    }

    public void Phase1()
    {
        if(currentAction == Action.None)
        {

            if (player.currentAction == Action.Attack || player.currentAction == Action.Block || player.currentAction == Action.Twirl || player.currentAction == Action.Flash || player.currentAction == Action.Reload)
            {
                if(rhythm.normalized < 0.02f || rhythm.normalized > 0.98f)
                {
                    Counter();
                }
            }
            else if (rhythm.normalized < 0.02f)
            {
                currentAction = Action.Attack;
                float random = Random.value * randomness;
                RandomAttack(2f + random - randomness / 2, new int[2] { 0, 1 });
                rhythm.ScheduleFunction(2.1f, "GoIdle", this);
            }
        }
    }

    public void Phase2()
    {
        if (currentAction == Action.None && rhythm.normalized < 0.05f)
        {
            currentAction = Action.Attack;
            float random = Random.value * randomness;
            //RandomAttack(2f + random - randomness / 2);
            rhythm.ScheduleFunction(3, "GoIdle", this);
        }


        if ( currentAction == Action.None && rhythm.normalized > 0.95f && player.currentNode.forward == null)
        {
            currentAction = Action.Tech;
            float random = Random.value * randomness;
            RandomTech(2f + random - randomness / 2);
            rhythm.ScheduleFunction(3, "GoIdle", this);
        }
    }

    public void GoIdle()
    {
        currentAction = Action.None;
        UpdateCurrentNode();
    }

    private void UpdateCurrentNode()
    {
        Node targetNode = player.currentNode.GetFirstNodeOnThisAxis();
        //Contar nodos hacia la izquierda 
        int nodes = 0;
        Node aux = currentNode;
        while (aux != targetNode)
        {
            aux = aux.left;
            nodes++;
        }
        float distance;
        if (nodes <= 3)
        {
            distance = nodes / 6f;
        }
        else
        {
            distance = -(6 - nodes) / 6f;
        }
        StartCoroutine(RotateByEased(distance, 0.2f + Mathf.Abs(distance), iTween.EaseType.easeInOutQuad));
        currentNode = targetNode;
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


    public void OffBeat()
    {
        UIController.PopUpText("OFFBEAT!");
        currentAction = Action.None;
        offbeat = true;
        Node node = currentNode;
        for (int circle = 0; circle <3; circle++)
        {
            for (int i = 0; i < 6; i++)
            {
                node.Cancel();
                node = node.left;
            }
            node = node.back;
        }
    }

    public void RecoverBeat()
    {
        offbeat = false;
    }

    public void Counter()
    {
        if (player.currentAction == Action.Attack)
        {
            float random = Random.value * randomness;
            if (rhythm.IsDownBeat())
            {
                currentAction = Action.Block;
                rhythm.ScheduleFunction(2f + random, "Block", this);            }
            else
            {
                currentAction = Action.Block;
                rhythm.ScheduleFunction(1f + random, "Block", this);
            }
        }
        else if (player.currentAction == Action.Twirl || player.currentAction == Action.Flash || player.currentAction == Action.Reload)
        {
            float random = Random.value * randomness / 2;
            if (rhythm.IsDownBeat())
            {
                currentAction = Action.Cut;
                rhythm.ScheduleFunction(random * cutFixRatio, "Cut", this);
                rhythm.ScheduleFunction(1, "GoIdle", this);
            }
        }
        else if (player.currentAction == Action.Block)
        {
            float random = Random.value * randomness / 2;
            if (!rhythm.IsDownBeat())
            {
                currentAction = Action.Cut;
                rhythm.ScheduleFunction(random * cutFixRatio, "Cut", this);
                rhythm.ScheduleFunction(1, "GoIdle", this);
            }
        }
    }

    public void Cut()
    {
        UpdateCurrentNode();
        if (player.currentAction == Action.Block || player.currentAction == Action.Twirl || player.currentAction == Action.Flash || player.currentAction == Action.Reload)
        {
            CheckSuccess();
            if (successChance == 100f) //Crítico
            {
                player.OffBeat();
            }
            else
            {
                OffBeat();
            }
        }
        else
        {
            OffBeat();
        }
    }

    public void RandomAttack(float time, int[] indexes)
    {
        int index = indexes[Mathf.FloorToInt(Random.value * (indexes.Length - 0.01f))];
        UpdateCurrentNode();
        switch (index)
        {
            case 0:
                Stab(time);
                break;
            case 1:
                Spear(time);
                break;
            case 2:
                Swipe(time);
                break;
        }
    }

    public void RandomTech(float time)
    {
        FireSwipe(time);
        /*
        int index = Mathf.FloorToInt(Random.value * 2.99f);
        switch (index)
        {
            case 0:
                Spear(time);
                break;
            case 1:
                Swipe(time);
                break;
            case 2:
                Stab(time);
                break;
        }
        */
    }

    #region Attacks

    public void Stab(float time)
    {
        player.currentNode.ChargeHere(damage, false, time);
    }


    public void Spear(float time)
    {
        Node node = currentNode;
        for (int i = 0; i < 3; i++)
        {
            node.ChargeHere(damage, false, time);
            node = node.back;
        }
    }

    public void Swipe(float time)
    {
        Node node = player.currentNode;
        for (int i = 0; i < 6; i++)
        {
            node.ChargeHere(damage, false, time);
            node = node.left;
        }
    }

    #endregion

    public void FireSwipe(float time)
    {
        Node node = currentNode;
        for (int i = 0; i < 6; i++)
        {
            node.ChargeHere(damage, true, time);
            node = node.left;
        }
    }

    private void Block()
    {
        if (rhythm.IsDownBeat())
        {
            CheckSuccess();
            StartCoroutine(WaitForDamage(0.5f));
        }
        else
        {
            currentAction = Action.None;
        }
    }

    IEnumerator WaitForDamage(float time)
    {
        float elapsed = 0;
        while (elapsed <= time)
        {
            if (damageToBlock != 0)
            {
                if (successChance == 100f) //Crítico
                {
                    UIController.PopUpNumber(damageToBlock, NumberType.Block, true);
                    player.OffBeat();
                }
                else
                {
                    UIController.PopUpNumber(damageToBlock * (maxBlockableRatio * successChance / 100), NumberType.Block, false);
                    UIController.PopUpNumber(damageToBlock * (1 - maxBlockableRatio * successChance / 100), NumberType.Attack, false);
                    GetDamage(damageToBlock * (1 - maxBlockableRatio * successChance / 100));
                }
                damageToBlock = 0;
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentAction = Action.None;
    }

    public void GetAttack(float damage)
    {
        if (currentAction == Action.Block)
        {
            damageToBlock = damage;
        }
        else
        {
            if(damage > player.damage)
            {
                UIController.PopUpNumber(damage, NumberType.Attack, true);
            }
            else
            {
                UIController.PopUpNumber(damage, NumberType.Attack, false);
            }
            GetDamage(damage);
            if (currentAction == Action.Tech)
            {
                OffBeat();
            }
        }
    }

    public void GetTech(float damage)
    {
        UIController.PopUpNumber(damage, NumberType.Tech, true);
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
            successChance = Mathf.Min(100, (0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold));
            print((0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold) + "%");
        }
        else                   //Si es contratiempo
        {
            successChance = Mathf.Min(100, (rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold));
            print((rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold) + "%");
        }
        if (successChance < 50)
        {
            float rng = Random.value * 100;
            return successChance >= rng;
        }
        return true;
    }


}
