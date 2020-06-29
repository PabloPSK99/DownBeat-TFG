using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    PlayerControls controls;
    float initTime;
    float startTime;
    float[] phaseTime = new float[4];
    int cutsceneIndex;
    public Rhythm rhythm;
    public ParticleSystem embers1;
    public ParticleSystem embers2;
    public GameObject[] cutscenes;
    public CanvasGroup mattes;
    public CutsceneText cutsceneText;

    public CanvasGroup menuUI;
    public Text text;
    public Text countDownText;
    public int[] phaseDurations;

    private void Awake()
    {
        cutsceneIndex = 0;
        controls = new PlayerControls();
        controls.Menu.Confirm.started += context => Confirm();
        mattes.alpha = 0;
    }

    void Start()
    {
        AkSoundEngine.SetState("Phase", "Intro");
        AkSoundEngine.PostEvent("Music", rhythm.gameObject);
        initTime = AkSoundEngine.GetTimeStamp() / 1000f;
    }

    void StartBattle()
    {
        print("START BATTLE");
        StartCoroutine(Count3());
    }

    IEnumerator Count3()
    {
        print("COUNT3");
        float startCountTime = AkSoundEngine.GetTimeStamp() / 1000f - initTime;
        print(startCountTime);
        float waitTime = 0;
        if (Mathf.Round(startCountTime / 2) == Mathf.Floor(startCountTime / 2))
        {
            waitTime = (Mathf.Floor(startCountTime) + 1) - startCountTime + 3;
        }
        else
        {
            waitTime = (Mathf.Floor(startCountTime) + 1) - startCountTime + 4;
        }
        text.text = "Starting in: " + waitTime.ToString("f").Replace(',','.')+"s";
        iTween.PunchScale(text.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 1f);
        StartCoroutine(SetStateAfter(1, waitTime - 1));
        float elapsed = waitTime;
        while(elapsed > 0)
        {
            text.text = "Starting in: " + elapsed.ToString("f").Replace(',', '.')+"s";
            elapsed -= Time.deltaTime;
            menuUI.alpha = elapsed / (waitTime-2);
            yield return null;
        }
        text.color = Color.clear;
        text.transform.SetParent(text.transform.parent.parent);
        StartCoroutine(CutsceneAfter(1, phaseDurations[0]));
        rhythm.player.enemy.StartCombat();
        rhythm.EnableGameplay();
        phaseTime[0] = AkSoundEngine.GetTimeStamp() / 1000f;
        embers2.Play();
        yield return new WaitForSeconds(1);
        Destroy(embers1);
        
    }

    IEnumerator SetStateAfter(int phase, float time)
    {
        yield return new WaitForSeconds(time);
        AkSoundEngine.SetState("Phase", "Phase"+phase);
    }

    IEnumerator CutsceneAfter(int cutscene, float delay)
    {
        print("CUTSCENEAFTER");
        yield return new WaitForSeconds(delay-2);
        mattes.alpha = 1;
        rhythm.DisableGameplay();
        cutsceneIndex = cutscene;
        foreach (Transform matte in mattes.transform)
        {
            matte.gameObject.transform.localScale = new Vector3(1, 0, 1);
            iTween.ScaleTo(matte.gameObject, Vector3.one, 2f);
        }
        yield return new WaitForSeconds(2);
        rhythm.player.ResetAll();
        cutscenes[cutscene - 1].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        cutsceneText.ShowText(cutscene);
        EnableMenuControls();
    }

    void Confirm()
    {
        /*
        switch (cutsceneIndex)
        {
            case 0:
                DisableMenuControls();
                StartBattle();
                break;
            case 1:
                EndCutscene1();
                break;
            default:
                break;
        }
        */
        
        if (cutsceneIndex == 0 || cutsceneText.state == TextState.Ended)
        {
            switch (cutsceneIndex)
            {
                case 0:
                    DisableMenuControls();
                    StartBattle();
                    break;
                case 1:
                    EndCutscene1();
                    break;
                default:
                    EndCutscene2();
                    break;
            }
        }
        else if(cutsceneText.state != TextState.Writing)
        {
            cutsceneText.ShowText(cutsceneIndex);
        }
        
    }


    
    void EndCutscene1()
    {
        DisableMenuControls();
        cutsceneText.EmptyText();
        cutsceneText.state = TextState.Unloaded;
        float currentTime = (AkSoundEngine.GetTimeStamp() / 1000f - (phaseTime[0] + phaseDurations[0])) % 16;
        if(currentTime < 14)
        {
            StartCoroutine(EndCutsceneAfter(1, 16 - currentTime));
        }
        else
        {
            StartCoroutine(EndCutsceneAfter(1, 32 - currentTime));
        }

    }

    void EndCutscene2()
    {
        DisableMenuControls();
        cutsceneText.EmptyText();
        cutsceneText.state = TextState.Unloaded;
        float currentTime = (AkSoundEngine.GetTimeStamp() / 1000f - (phaseTime[1] + phaseDurations[1])) % 4;
        if (currentTime < 2)
        {
            StartCoroutine(EndCutsceneAfter(2, 4 - currentTime));
        }
        else
        {
            StartCoroutine(EndCutsceneAfter(2, 8 - currentTime));
        }
    }

    IEnumerator EndCutsceneAfter(int cutscene, float waitTime)
    {
        StartCoroutine(ShowCountDown(waitTime));
        StartCoroutine(SetStateAfter(cutscene+1, 0));
        yield return new WaitForSeconds(waitTime);
        cutscenes[cutscene - 1].SetActive(false);
        rhythm.player.ResetAll();
        rhythm.player.enemy.NextPhase();
        rhythm.EnableGameplay();
        phaseTime[cutscene] = AkSoundEngine.GetTimeStamp() / 1000f;
        StartCoroutine(CutsceneAfter(cutscene+1, phaseDurations[cutscene]));

        foreach (Transform matte in mattes.transform)
        {
            iTween.ScaleTo(matte.gameObject, new Vector3(1, 0, 1), 1f);
        }
        yield return new WaitForSeconds(1f);
        mattes.alpha = 0;
        mattes.gameObject.transform.localScale = Vector3.one;
    }

    IEnumerator ShowCountDown(float waitTime)
    {
        countDownText.color = Color.white;
        countDownText.text = "Starting in: " + waitTime.ToString("f").Replace(',', '.') + "s";
        iTween.PunchScale(countDownText.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 1f);
        float elapsed = waitTime;
        Color auxColor = Color.white;
        float auxAlpha = 1;
        while (elapsed > 0)
        {
            countDownText.text = "Starting in: " + elapsed.ToString("f").Replace(',', '.') + "s";
            elapsed -= Time.deltaTime;
            auxAlpha = elapsed*3 / waitTime;
            auxColor.a = auxAlpha;
            countDownText.color = auxColor;
            yield return null;
        }
        countDownText.color = Color.clear;
    }

    private void OnEnable()
    {
        EnableMenuControls();
    }

    private void EnableMenuControls()
    {
        controls.Menu.Enable();
    }

    private void DisableMenuControls()
    {
        controls.Menu.Disable();
    }
}
