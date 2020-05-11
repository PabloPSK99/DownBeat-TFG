using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneText : MonoBehaviour
{
    [TextArea(3, 10)] public string fullText;
    private string currentText;
    private Text text;
    private int index;

    public float letterTime;
    public float pauseTime;
    public float stopTime;

    private void Awake()
    {
        index = -1;
        text = GetComponent<Text>();
    }

    public void ShowText()
    {
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

        }
    }
}
