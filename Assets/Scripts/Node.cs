using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node forward;
    public Node left;
    public Node right;
    public Node back;
    private PlayerController player;
    private Rhythm rhythm;

    MeshRenderer meshRenderer;
    Material basicMaterial;
    Material attackMaterial;
    Material techMaterial;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rhythm = GameObject.FindGameObjectWithTag("Rhythm").GetComponent<Rhythm>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        basicMaterial = meshRenderer.material;
        attackMaterial = Resources.Load<Material>("Materials/AttackTile");
        techMaterial = Resources.Load<Material>("Materials/TechTile");
    }

    public Node GetLastNodeOnThisAxis()
    {
        if (back == null) return this;
        return back.GetLastNodeOnThisAxis();
    }

    public Node GetFirstNodeOnThisAxis()
    {
        if (forward == null) return this;
        return forward.GetFirstNodeOnThisAxis();
    }

    public Node GetFirstNodeOnOppositeAxis()
    {
        return left.left.left.GetFirstNodeOnThisAxis();
    }

    public int GetIndexInAxis()
    {
        Node aux = this;
        for(int i = 2; i >= 0; i--)
        {
            if(aux.back == null)
            {
                return i;
            }
            aux = aux.back;
        }
        return 0;
    }

    public void ChargeHere(float damage, bool tech, float time)
    {
        StartCoroutine(AnimateCharge(damage, tech, time));
    }

    IEnumerator AnimateCharge(float damage, bool tech, float time)
    {
        meshRenderer.material = tech ? techMaterial : attackMaterial;
        if (tech)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0,
                "to", 1,
                "time", time,
                "onupdate", "OnTechUpdate",
                "oncomplete", "OnAttackComplete",
                "easetype", iTween.EaseType.linear
                )
            );
        }
        else
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0,
                "to", 1,
                "time", time,
                "onupdate", "OnAttackUpdate",
                "oncomplete", "OnAttackComplete",
                "easetype", iTween.EaseType.linear
                )
            );
        }
        yield return new WaitForSeconds(time);
        AttackHere(damage, tech);

    }



    public void ChargeHere(float damage, bool tech, float time, float delay)
    {
        StartCoroutine(AnimateCharge(damage, tech, time, delay));
    }

    IEnumerator AnimateCharge(float damage, bool tech, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        meshRenderer.material = tech ? techMaterial : attackMaterial;
        if (tech)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0,
                "to", 1,
                "time", time,
                "onupdate", "OnTechUpdate",
                "oncomplete", "OnAttackComplete",
                "easetype", iTween.EaseType.linear
                )
            );
        }
        else
        {
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0,
                "to", 1,
                "time", time,
                "onupdate", "OnAttackUpdate",
                "oncomplete", "OnAttackComplete",
                "easetype", iTween.EaseType.linear
                )
            );
        }
        yield return new WaitForSeconds(time);
        AttackHere(damage, tech);

    }

    public void Cancel()
    {
        meshRenderer.material = basicMaterial;
        StopAllCoroutines();
    }

    private void OnAttackUpdate(float newValue)
    {
        Color color = attackMaterial.GetColor("_EmissionColor") * (0.5f+newValue * 2);
        meshRenderer.material.SetColor("_EmissionColor", color);
    }

    private void OnAttackComplete()
    {
        meshRenderer.material = basicMaterial;
    }

    private void OnTechUpdate(float newValue)
    {
        Color color = techMaterial.GetColor("_EmissionColor") * (0.5f + newValue * 2);
        meshRenderer.material.SetColor("_EmissionColor", color);
    }


    public void AttackHere(float damage, bool tech)
    {
        float successChance = Mathf.Min(1, (0.5f - rhythm.normalized) * 2);
        //print(successChance * 100f + "%  (attack)");
        if (player.currentNode == this)
        {
            if (tech)
            {
                player.GetDamage(damage);
            }
            else
            {
                
                if (successChance > 0.99f)
                {
                    player.GetAttack(damage * 1.5f);
                }
                else
                {
                    player.GetAttack(damage * successChance);
                }
            }
        }
    }

}
