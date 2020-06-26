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
    public Rhythm rhythm;
    public ParticleSystem embers1;
    public ParticleSystem embers2;

    public CanvasGroup menuUI;
    public Text text;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Menu.Confirm.started += context => Confirm();
    }

    void Start()
    {
        AkSoundEngine.SetState("Phase", "Intro");
        AkSoundEngine.PostEvent("Music", gameObject);
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
        iTween.PunchScale(text.gameObject, new Vector3(1, 1, 1), 1f);
        StartCoroutine(SetStateAfter(waitTime - 1));
        float elapsed = waitTime;
        while(elapsed > 0)
        {
            text.text = "Starting in: " + elapsed.ToString("f").Replace(',', '.')+"s";
            elapsed -= Time.deltaTime;
            menuUI.alpha = elapsed / (waitTime-2);
            yield return null;
        }
        rhythm.ActivateGameplay();
        embers2.Play();
        yield return new WaitForSeconds(1);
        Destroy(embers1);
        
    }

    IEnumerator SetStateAfter(float time)
    {
        yield return new WaitForSeconds(time);
        AkSoundEngine.SetState("Phase", "Phase1");
    }

    void Confirm()
    {
        DisableMenuControls();
        StartBattle();
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
