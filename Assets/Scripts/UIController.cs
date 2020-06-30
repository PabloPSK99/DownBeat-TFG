using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Transform[] pivots;
    public Transform chancePivot;
    bool[] empty;
    bool emptyChance;


    public int minSize;
    public int maxSize;
    public int minChanceSize;
    public int maxChanceSize;
    public int minValue;
    public int maxValue;


    [Header("Movement")]
    public float distance;
    public float screenTime;
    public float fadeTime;


    [Header("Text")]
    public Color attackColor;
    public Color plusColor;
    public Color damageColor;
    public Color blockColor;
    public Color techColor;
    public Color cutColor;
    public Color chanceColor;


    void Awake()
    {
        empty = new bool[4] { true, true, true, true };
        emptyChance = true;
    }

    public void PopUpNumber(float number, NumberType type, bool crit)
    {
        for (int i = 0; i < pivots.Length; i++)
        {
            if (empty[i])
            {
                empty[i] = false;
                Setup(Instantiate(Resources.Load<GameObject>("PopUpText"), pivots[i]).GetComponent<Text>(), i, number, type, crit);
                break;
            }
        }
    }

    public void PopUpChance(float number, Action action)
    {
        if (emptyChance)
        {
            SetupChance(Instantiate(Resources.Load<GameObject>("PopUpText"), chancePivot).GetComponent<Text>(), number, action);
        }
    }

    public void Setup(Text text, int index, float number, NumberType numberType, bool crit)
    {
        switch (numberType)
        {
            case NumberType.Attack:
                text.color = attackColor;
                break;
            case NumberType.Plus:
                text.color = plusColor;
                break;
            case NumberType.Damage:
                text.color = damageColor;
                break;
            case NumberType.Block:
                text.color = blockColor;
                break;
            case NumberType.Tech:
                text.color = techColor;
                break;
            case NumberType.Cut:
                break;
            case NumberType.Chance:
                break;
            case NumberType.None:
                break;
            default:
                break;
        }
        text.text = Mathf.RoundToInt(number).ToString();
        float percentage = (number - minValue) / (maxValue - minValue);
        int fontSize = minSize + Mathf.RoundToInt((maxSize - minSize) * percentage);
        text.fontSize = fontSize;
        float critRatio = 1;
        if (crit)
        {
            text.text += "!";
            text.fontSize = Mathf.RoundToInt(fontSize * 1.5f);
            critRatio = 1.5f;
        }

        Vector3 force = (text.transform.position - transform.position).normalized * distance * percentage;

        StartCoroutine(Animate(text, index, screenTime * Mathf.Max(percentage, 0.25f) * critRatio, fadeTime * Mathf.Max(percentage, 0.25f) * critRatio, force));
    }

    public void SetupChance(Text text, float number, Action action)
    {
        switch (action)
        {
            case Action.Attack:
                text.color = attackColor;
                break;
            case Action.Block:
                text.color = blockColor;
                break;
            case Action.Tech:
                text.color = techColor;
                break;
            case Action.Cut:
                text.color = cutColor;
                break;
            case Action.Move:
                text.color = chanceColor;
                break;
            default:
                break;
        }

        text.text = Mathf.RoundToInt(number).ToString() + "%";
        int fontSize = minChanceSize + Mathf.RoundToInt((maxChanceSize - minChanceSize) * number / 100);
        text.fontSize = fontSize;
        float critRatio = 1;
        if (number == 100f)
        {
            text.fontSize = Mathf.RoundToInt(fontSize * 1.5f);
            critRatio = 1.5f;
        }

        StartCoroutine(Animate(text, screenTime * Mathf.Max(number / 100, 0.25f) * critRatio, fadeTime * Mathf.Max(number / 100, 0.25f) * critRatio));
    }

    IEnumerator Animate(Text text, int index, float screen, float fade, Vector3 force)
    {
        iTween.MoveBy(text.gameObject, force, screenTime + fadeTime);
        iTween.ScaleTo(text.gameObject, iTween.Hash(
            "scale", new Vector3(1.2f, 1.2f, 1.2f),
            "time", (screenTime + fadeTime) * 0.1f,
            "easetype", iTween.EaseType.easeOutCubic
            )
        );
        yield return new WaitForSeconds((screenTime + fadeTime) * 0.09f);
        iTween.ScaleTo(text.gameObject, iTween.Hash(
            "scale", new Vector3(0.5f, 0.5f, 0.5f),
            "time", (screenTime + fadeTime) * 0.9f,
            "easetype", iTween.EaseType.easeInCubic
            )
        );
        yield return new WaitForSeconds(screenTime - (screenTime + fadeTime) * 0.09f);
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            Color color = text.color;
            color.a = 1 - elapsed/fadeTime;
            text.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(text.gameObject);
        empty[index] = true;
        yield return null;
    }

    IEnumerator Animate(Text text, float screen, float fade)
    {
        emptyChance = false;
        iTween.ScaleTo(text.gameObject, iTween.Hash(
            "scale", new Vector3(1.2f, 1.2f, 1.2f),
            "time", (screenTime + fadeTime) * 0.1f,
            "easetype", iTween.EaseType.easeOutCubic
            )
        );
        yield return new WaitForSeconds((screenTime + fadeTime) * 0.09f);
        iTween.ScaleTo(text.gameObject, iTween.Hash(
            "scale", new Vector3(0.7f, 0.7f, 0.7f),
            "time", (screenTime + fadeTime) * 0.9f,
            "easetype", iTween.EaseType.easeInCubic
            )
        );
        yield return new WaitForSeconds(screenTime - (screenTime + fadeTime) * 0.09f);
        emptyChance = true;
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            Color color = text.color;
            color.a = 1 - elapsed / fadeTime;
            text.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(text.gameObject);
        yield return null;
    }

    public void PopUpText(string message)
    {
        if (emptyChance)
        {
            Text text = Instantiate(Resources.Load<GameObject>("PopUpText"), chancePivot).GetComponent<Text>();
            text.text = message;
            text.color = Color.white;
            text.fontSize = maxChanceSize;
            StartCoroutine(Animate(text, screenTime, fadeTime));
        }
    }
}
