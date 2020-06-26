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
    int cutsceneIndex;
    public Rhythm rhythm;
    public ParticleSystem embers1;
    public ParticleSystem embers2;
    public GameObject[] cutscenes;
    public CanvasGroup mattes;
    public CutsceneText cutsceneText;

    public CanvasGroup menuUI;
    public Text text;

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
        StartCoroutine(Count3());
    }

    IEnumerator Count3()
    {
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
        StartCoroutine(SetStateAfter(waitTime - 1));
        float elapsed = waitTime;
        while(elapsed > 0)
        {
            text.text = "Starting in: " + elapsed.ToString("f").Replace(',', '.')+"s";
            elapsed -= Time.deltaTime;
            menuUI.alpha = elapsed / (waitTime-2);
            yield return null;
        }
        StartCoroutine(CutsceneIn(1, 10));
        rhythm.player.enemy.StartCombat();
        rhythm.EnableGameplay();
        embers2.Play();
        yield return new WaitForSeconds(1);
        Destroy(embers1);
        
    }

    IEnumerator SetStateAfter(float time)
    {
        yield return new WaitForSeconds(time);
        AkSoundEngine.SetState("Phase", "Phase1");
    }

    IEnumerator CutsceneIn(int cutscene, float delay)
    {
        yield return new WaitForSeconds(delay);
        mattes.alpha = 1;
        rhythm.DisableGameplay();
        foreach(Transform matte in mattes.transform)
        {
            matte.gameObject.transform.localScale = new Vector3(1, 0, 1);
            iTween.ScaleTo(matte.gameObject, Vector3.one, 2f);
        }
        cutscenes[cutscene - 1].SetActive(true);
    }

    void Confirm()
    {
        switch (cutsceneIndex)
        {
            case 0:
                DisableMenuControls();
                StartBattle();
                break;
            default:
                break;
        }

    }

    private void OnEnable()
    {
        controls.Menu.Enable();
    }

    private void DisableMenuControls()
    {
        controls.Menu.Disable();
    }
}
