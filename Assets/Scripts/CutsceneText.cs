using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneText : MonoBehaviour
{
    public int maxFiles;
    [TextArea(3, 10)] public string fullText;
    private string currentText;
    private Text text;
    private int index;

    public float letterTime;
    public float pauseTime;
    public float stopTime;

    public TextState state;

    private void Awake()
    {
        state = TextState.Unloaded;
        index = -1;
        text = GetComponent<Text>();
    }

    public void LoadText(int cutscene)
    {
        int file = Mathf.FloorToInt(Random.value * maxFiles * 0.99f) + 1;
        fullText = Resources.Load<TextAsset>("Text/C"+cutscene+"/T"+file).text;
    }
    public void ShowText(int cutscene)
    {
        if (state == TextState.Unloaded)
        {
            LoadText(cutscene);
        }
        print("PERO WENO");
        state = TextState.Writing;
        StartCoroutine(WriteText());
    }

    IEnumerator WriteText()
    {
        text.text = "";
        for (int i = index + 1; i<fullText.Length; i++)
        {
            if(fullText[i] == '$')
            {
                index = i;
                currentText = "";
                if(i == fullText.Length - 1)
                {
                    state = TextState.Ended;
                }
                else
                {
                    state = TextState.Paused;
                }
                break;
            }
            else
            {
                currentText += fullText[i];
                text.text = currentText;
                if (fullText[i] == '.')
                {
                    yield return new WaitForSeconds(stopTime);
                }
                else if (fullText[i] == ',')
                {
                    yield return new WaitForSeconds(pauseTime);
                }
                else
                {
                    yield return new WaitForSeconds(letterTime);
                }
            }
            yield return null;
        }
    }
}
