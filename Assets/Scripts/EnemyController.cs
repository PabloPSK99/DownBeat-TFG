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
    public Transform halberdPivot;
    public Transform halberd;
    public Effects effects;

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
    private Animator anim;
    public int maxThunders;
    public float healing;
    public int retaliate;
    private int phase;

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
        retaliate = 5;
        phase = 1;
        currentAction = Action.None;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!offbeat)
        {
            Phase3();
        }
        else
        {
            rhythm.ScheduleFunction(3, "RecoverBeat", this);
        }
    }

    public void NextPhase()
    {
        phase++;
        if (phase == 3) retaliate = 5;
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
                RandomAttack(2f + random - randomness / 2, new int[2] {0, 3}); ///////////////////////////////// PONER 0
                rhythm.ScheduleFunction(2f, "GoIdle", this);
            }
        }
    }

    public void Phase2()
    {
        if (currentAction == Action.None)
        {
            if (rhythm.normalized < 0.02f)
            {
                currentAction = Action.Attack;
                float random = Random.value * randomness;
                RandomAttack(2f + random - randomness / 2, new int[3] { 0, 1, 2 });
                rhythm.ScheduleFunction(2f, "GoIdle", this);
            }
            else if (rhythm.normalized > 0.98f)
            {
                if (player.currentNode.forward == null) //Melee
                {
                    if(player.currentAction == Action.Attack)
                    {
                        if(Random.value < 0.5f)
                        {
                            currentAction = Action.Wait;
                            rhythm.ScheduleFunction(0.5f, "GoIdle", this);
                        }
                        else
                        {
                            Counter();
                        }
                    }
                    else
                    {
                        currentAction = Action.Tech;
                        float random = Random.value * randomness;
                        FireSweep(2f + random - randomness / 2);
                        rhythm.ScheduleFunction(2f, "GoIdle", this);
                    }
                }
                else
                {
                    if (Random.value < 0.8f && (player.currentAction == Action.Attack || player.currentAction == Action.Block || player.currentAction == Action.Twirl || player.currentAction == Action.Flash || player.currentAction == Action.Reload))
                    {
                        Counter();
                    }
                }
            }
        }
    }

    public void Phase3()
    {
        if (currentAction == Action.None)
        {
            if (Random.value < 0.8f && (player.currentAction == Action.Attack || player.currentAction == Action.Block || player.currentAction == Action.Twirl || player.currentAction == Action.Flash || player.currentAction == Action.Reload))
            {
                if (rhythm.normalized < 0.02f || rhythm.normalized > 0.98f) Counter();
            }
            else if (rhythm.normalized < 0.02f)
            {
                currentAction = Action.Attack;
                float random = Random.value * randomness;
                RandomAttack(2f + random - randomness / 2, new int[3] { 1, 2, 3 });
                rhythm.ScheduleFunction(2f, "GoIdle", this);
            }
            else if (rhythm.normalized > 0.98f)
            {
                if(player.currentAction == Action.Attack)
                {
                    currentAction = Action.Wait;
                    rhythm.ScheduleFunction(2f, "GoIdle", this);
                }
                else
                {
                    float random = Random.value * randomness;
                    if (player.currentNode.forward == null) //Melee
                    {
                        currentAction = Action.Tech;
                        FireSweep(2f + random - randomness / 2);
                        rhythm.ScheduleFunction(2f, "GoIdle", this);
                    }
                    else
                    {
                        if( health < maxHealth * 0.9f && Random.value * 10 <= retaliate)
                        {
                            currentAction = Action.Tech;
                            Prayer(2f + random - randomness / 2);
                            retaliate = 0;
                            rhythm.ScheduleFunction(2f, "GoIdle", this);
                        }
                        else
                        {
                            currentAction = Action.Wait;
                            retaliate = Mathf.Min(retaliate + 1, 10);
                            rhythm.ScheduleFunction(0.5f, "GoIdle", this);
                        }
                    }
                }
            }
        }
    }

    public void Phase4()
    {
        if (currentAction == Action.None)
        {
            if (rhythm.normalized < 0.02f)
            {
                currentAction = Action.Attack;
                float random = Random.value * randomness;
                Random2Combo(2f + random - randomness / 2, new int[1] { 0 });
                rhythm.ScheduleFunction(3f, "GoIdle", this);
            }
        }
    }

    public void Random2Combo(float time, int[] indexes)
    {
        int index = indexes[Mathf.FloorToInt(Random.value * (indexes.Length - 0.01f))];
        UpdateCurrentNode();
        switch (index)
        {
            case 0:
                StormCombo(time);
                break;
            case 1:
                Spear(time);
                break;
            case 2:
                Sweep(time);
                break;
            case 3:
                Storm(time);
                break;
        }
    }

    public void StormCombo(float time)
    {
        SetTrigger("chargeStorm");
        int thunders = maxThunders - Mathf.FloorToInt(Random.value * 3);
        PriorityQueue<Node> targets = new PriorityQueue<Node>(5);
        Node node = currentNode;

        for (int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if(node != player.currentNode)
                {
                    int distance = Mathf.Abs(player.currentNode.GetIndexInAxis() - j) + i < 3 ? i : 6 - i;
                    targets.Add(distance, node);
                }
                if(j!=2) node = node.back;
            }
            node = node.left.GetFirstNodeOnThisAxis();
        }

        player.currentNode.ChargeHere(damage, false, time);
        Node aux;
        for (int i = 0; i < thunders && targets.Count > 0; i++) // First
        {
            GameObject g = new GameObject();
            g.name = "First";
            print("Firsts: "+ targets.PeekPriority());
            aux = targets.RemoveMin();
            g.transform.position = aux.transform.position;
            g.transform.SetParent(halberdPivot);
            aux.ChargeHere(damage, false, time);
        }
        for (int i = 0; i < thunders && targets.Count > 0; i++) // Second
        {
            GameObject g = new GameObject();
            g.name = "Second";
            print("Seconds: " + targets.PeekPriority());
            aux = targets.RemoveMin();
            g.transform.position = aux.transform.position;
            g.transform.SetParent(halberdPivot);
            aux.ChargeHere(damage, true, time, 1);
        }
    }

    public void SweepCombo(float time, int startingRing, bool ext)
    {

    }

    public void GoIdle()
    {
        currentAction = Action.Wait;
        iTween.MoveTo(transform.GetChild(0).gameObject, Vector3.zero, 0.2f);
        StartCoroutine(UpdateCurrentNodeIn(0.2f));
    }

    IEnumerator UpdateCurrentNodeIn(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentAction = Action.None;
        UpdateCurrentNode();
    }

    public void UpdateCurrentNode()
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
        StartCoroutine(RotateByEased(distance, 0.2f + Mathf.Abs(distance/2), iTween.EaseType.easeInOutQuad));
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
        if(currentAction == Action.Tech)
        {
            retaliate = Mathf.Max(retaliate - 1, 0);
        }
        UIController.PopUpText("OFFBEAT!");
        SetTrigger("offBeat");
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
            SetTrigger("chargeBlock");
            effects.Block(rhythm.IsDownBeat(), false);
            float random = Random.value * randomness;
            if (rhythm.IsDownBeat())
            {
                currentAction = Action.Block;
                rhythm.ScheduleFunction(2f + random - randomness / 2, "Block", this);
            }
            else
            {
                currentAction = Action.Block;
                rhythm.ScheduleFunction(1f + random - randomness / 2, "Block", this);
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
                Thunder(time);
                break;
            case 1:
                Spear(time);
                break;
            case 2:
                Sweep(time);
                break;
            case 3:
                Storm(time);
                break;
        }
    }

    #region Attacks

    public void Thunder(float time)
    {
        SetTrigger("chargeThunder");
        Release(time);
        halberdPivot.position = player.currentNode.transform.position;
        player.currentNode.ChargeHere(damage, false, time);
    }

    public void Storm(float time)
    {
        SetTrigger("chargeStorm");
        Release(time);
        int thunders = maxThunders - Mathf.FloorToInt(Random.value * 3);
        List<Node> targets = new List<Node>();
        targets.Add(player.currentNode);
        Node node = currentNode;
        for (int i = 0; i < 3; i++)
        {
            if (node != player.currentNode)
            {
                targets.Add(node);
            }
            targets.Add(node.left);
            targets.Add(node.right);
            if (i != 2) node = node.back;
        }
        for (int i = 0; i < 9-thunders; i++)
        {
            int index = Mathf.FloorToInt(Random.value * (targets.Count-1) * 0.99f) + 1;
            targets.RemoveAt(index);
        }
        foreach(Node n in targets)
        {
            GameObject g = new GameObject();
            g.transform.position = n.transform.position;
            g.transform.SetParent(halberdPivot);
            n.ChargeHere(damage, false, time);
        }
    }

    public void Spear(float time)
    {
        Node node = currentNode;
        SetTrigger("chargeThrust");
        Release(time);
        for (int i = 0; i < 3; i++)
        {
            node.ChargeHere(damage, false, time);
            if(i!=2) node = node.back;
        }
        halberdPivot.position = node.transform.position;
    }

    public void Sweep(float time)
    {
        Node node = player.currentNode;
        SetTrigger("chargeSweep");
        Release(time);
        for (int i = 0; i < 6; i++)
        {
            node.ChargeHere(damage, false, time);
            node = node.left;
        }
        halberdPivot.position = player.currentNode.right.transform.position;
    }

    #endregion

    public void RandomTech(float time, int[] indexes)
    {
        int index = indexes[Mathf.FloorToInt(Random.value * (indexes.Length - 0.01f))];
        UpdateCurrentNode();
        switch (index)
        {
            case 0:
                Prayer(time);
                break;
            case 1:
                FireSweep(time);
                break;
            case 2:

                break;
            case 3:

                break;
        }
    }

    public void Prayer(float time)
    {
        SetTrigger("chargePray");
        Release(time);
        rhythm.ScheduleFunction(time, "Heal", this);
    }

    public void FireSweep(float time)
    {
        SetTrigger("chargeFireSweep");
        Release(time);
        Node node = currentNode;
        for (int i = 0; i < 6; i++)
        {
            node.ChargeHere(damage, true, time);
            node = node.left;
        }
        halberdPivot.position = player.currentNode.left.left.left.transform.position;
    }

    public void RandomAttackPlusTech(float time, int[] indexes)
    {
        int index = indexes[Mathf.FloorToInt(Random.value * (indexes.Length - 0.01f))];
        UpdateCurrentNode();
        switch (index)
        {
            case 0:
                Sweep(time);
                break;
            case 1:
                Spear(time);
                break;
            case 2:
                Sweep(time);
                break;
            case 3:
                Storm(time);
                break;
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
                    SetTrigger("parry");
                    UIController.PopUpNumber(damageToBlock, NumberType.Block, true);
                    player.OffBeat();
                }
                else
                {
                    SetTrigger("release");
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
            else
            {
                retaliate = Mathf.Min(retaliate + 1, 10);
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

    public void Heal()
    {
        health = Mathf.Min(health + Mathf.RoundToInt(healing), maxHealth);
        StartCoroutine(UpdateHealthBar());
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




    private void SetTrigger(string trigger)
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.ResetTrigger(parameter.name);
        }
        anim.SetTrigger(trigger);
    }

    private void Release(float time)
    {
        StartCoroutine(ReleaseIn(time));
    }

    IEnumerator ReleaseIn(float time)
    {
        yield return new WaitForSeconds(time);
        SetTrigger("release");
    }
}
