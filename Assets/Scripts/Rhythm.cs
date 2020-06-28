using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Rhythm : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public PostProcessVolume postVolume;
    public Image flashPanel;
    public CanvasGroup gameplayUI;
    public GameObject AKtarget;

    [Header("Circles")]
    public Image circle;
    public Image dot;
    public Image externalCircle;

    [Header("Colors")]
    public Color downBeatColor;
    public Color offBeatColor;
    public Color upBeatColor;

    [Header("Glow")]
    public Image dotGlow;
    public Image outGlow;
    public Color dotGlowColor;
    public Color outGlowColor;

    [Header("Timing")]
    public float bpm;
    public float timeOffset;
    public float normalized;
    private bool chromVariation;

    [Header("Post Process")]
    public float transitionTime;
    public float aberrationThreshold;
    public float maxAberration;

    public bool beatLocked;

    private PostProcessProfile crimsonProfile;
    private PostProcessProfile offBeatProfile;
    private ChromaticAberration chromaticAberration;


    private bool offBeat;
    private float lastTimeStamp;
    private bool grow;
    private float beatDuration;
    private float syncCorrection;
    private Vector3 minScale = new Vector3(.1f, .1f, 1);

    void Start()
    {
        crimsonProfile = Resources.Load<PostProcessProfile>("PostProcess Profiles/Crimson");
        offBeatProfile = Resources.Load<PostProcessProfile>("PostProcess Profiles/OffBeat");
        postVolume.profile = crimsonProfile;
        postVolume.profile.TryGetSettings(out chromaticAberration);
        chromaticAberration.intensity.value = 1;
        beatDuration = bpm / 60f;
        StartCoroutine(Loop());
        normalized = 1;
    }

    private void Update()
    {
        if (chromVariation)
        {
            float intensity = Mathf.Abs(0.5f - normalized) * 2;
            if (intensity > aberrationThreshold)
            {
                chromaticAberration.intensity.value = (intensity - aberrationThreshold) * (maxAberration / (1 - aberrationThreshold));
            }
            else
            {
                chromaticAberration.intensity.value = 0;
            }
        }
    }

    public void OffBeat(bool offBeat)
    {
        this.offBeat = offBeat;
        if(offBeat)
        {
            AKRESULT result = AkSoundEngine.SetSwitch("OffBeat", "OffBeat", gameObject);
            print(result);
            dot.color = offBeatColor;
            externalCircle.color = offBeatColor;
            circle.color = offBeatColor;
        }
        else
        {
            AkSoundEngine.SetSwitch("OffBeat", "Default", gameObject);
        }
        StartCoroutine(PostProcessTransition());
    }

    public void EnableGameplay()
    {
        chromVariation = true;
        player.enemy.pause = false;
        player.EnableFightControls();
        player.SetIntroTrigger();
        StartCoroutine(RestoreUIAlpha(1));
    }

    public void DisableGameplay()
    {
        chromVariation = false;
        chromaticAberration.intensity.value = 0.4f;
        player.enemy.pause = true;
        player.enemy.GoIdle();
        player.DisableFightControls();
    }

    IEnumerator Loop()
    {
        lastTimeStamp = AkSoundEngine.GetTimeStamp() / 1000f;
        while (true)
        {
            yield return CircleGrow();
            yield return CircleShrink();
        }
    }

    IEnumerator RestoreUIAlpha(float duration)
    {
        float elapsed = 0;
        while(elapsed < duration)
        {
            gameplayUI.alpha = elapsed / duration;
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameplayUI.alpha = 1;
    }

    IEnumerator PostProcessTransition()
    {
        Color panelColor = Color.white;
        flashPanel.color = panelColor;
        if (offBeat)
        {
            postVolume.profile = offBeatProfile;
        }
        else
        {
            postVolume.profile = crimsonProfile;
        }
        float elapsed = 0;
        while (elapsed < transitionTime)
        {
            panelColor.a -= Time.deltaTime / transitionTime;
            flashPanel.color = panelColor;
            elapsed += Time.deltaTime;
            yield return null;
        }
        flashPanel.color = Color.clear;
    }

    IEnumerator CircleGrow()
    {
        grow = true;
        float duration = beatDuration + syncCorrection;
        iTween.ScaleTo(circle.gameObject, iTween.Hash(
            "scale", Vector3.one,
            "time", duration,
            "easetype", iTween.EaseType.easeInQuint
            )
        );
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0,
            "to", 1,
            "time", duration,
            "onupdate", "OnCircleUpdate",
            "easetype", iTween.EaseType.linear
            )
        );
        yield return new WaitForSeconds(duration);
        circle.transform.localScale = Vector3.one;
        normalized = 1;
        if (!offBeat)
        {
            externalCircle.color = upBeatColor;
            outGlow.color = outGlowColor;
        }
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1,
            "to", 0,
            "time", duration,
            "onupdate", "UpdateExternalCircleColor",
            "oncomplete", "FinalExternalCircleColor",
            "easetype", iTween.EaseType.easeOutSine
            )
        );
        FixSync();
        yield return null;
    }

    IEnumerator CircleShrink()
    {
        grow = false;
        float duration = beatDuration + syncCorrection;
        iTween.ScaleTo(circle.gameObject, iTween.Hash(
            "scale", minScale,
            "time", duration,
            "easetype", iTween.EaseType.easeInQuint
            )
        );
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1,
            "to", 0,
            "time", duration,
            "onupdate", "OnCircleUpdate",
            "easetype", iTween.EaseType.linear
            )
        );
        yield return new WaitForSeconds(duration);
        circle.transform.localScale = minScale;
        normalized = 0;
        if (!offBeat)
        {
            dot.color = downBeatColor;
            dotGlow.color = dotGlowColor;
        }
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1,
            "to", 0,
            "time", duration,
            "onupdate", "UpdateDotColor",
            "oncomplete", "FinalDotColor",
            "easetype", iTween.EaseType.easeOutSine
            )
        );
        FixSync();
        yield return null;
    }

    private void FixSync()
    {
        float offset = (lastTimeStamp + beatDuration + syncCorrection) - AkSoundEngine.GetTimeStamp() / 1000f; //tiempo esperado - tiempo real
        //print("Duration: " + (beatDuration + syncCorrection ) + "    Expected time: " + (lastTimeStamp + beatDuration + syncCorrection) + "    Actual time: " + AkSoundEngine.GetTimeStamp() / 1000f);
        lastTimeStamp = AkSoundEngine.GetTimeStamp() / 1000f;
        if(Mathf.Abs(offset) > 5)
        {
            syncCorrection = offset - (Mathf.Floor(offset / beatDuration) * beatDuration);
        }
        else
        {
            syncCorrection = offset;
        }
    }

    public void UpdateColor(float value)
    {
        if (!offBeat)
        {
            if (value >= 0.5)
            {
                value = (value - 0.5f) * 2;
                circle.color = ((1f - value) * offBeatColor) + (value * upBeatColor);
            }
            else
            {
                value = value * 2;
                circle.color = ((1f - value) * downBeatColor) + (value * offBeatColor);
            }
        }
    }

    private void OnCircleUpdate(float newValue)
    {
        normalized = newValue;
        UpdateColor(normalized);
    }

    private void UpdateDotColor(float newValue)
    {
        if (!offBeat)
        {
            dot.color = ((1f - newValue) * offBeatColor) + (newValue * downBeatColor);
            dotGlow.color = new Color(dotGlowColor.r, dotGlowColor.g, dotGlowColor.b, newValue);
        }
    }

    private void FinalDotColor()
    {
        dot.color = offBeatColor;
        dotGlow.color = Color.clear;
    }

    private void UpdateExternalCircleColor(float newValue)
    {
        if (!offBeat)
        {
            externalCircle.color = ((1f - newValue) * offBeatColor) + (newValue * upBeatColor);
            outGlow.color = new Color(outGlowColor.r, outGlowColor.g, outGlowColor.b, newValue);
        }
    }

    private void FinalExternalCircleColor()
    {
        externalCircle.color = offBeatColor;
        outGlow.color = Color.clear;
    }

    public bool IsDownBeat()
    {
        return normalized <= 0.5f;
    }

    public void LockThisBeat()
    {
        beatLocked = true;
        StartCoroutine(WaitForNextBeat());
    }

    public void LockTwoBeats()
    {
        beatLocked = true;
        StartCoroutine(WaitTwoBeats());
    }

    IEnumerator WaitForNextBeat()
    {
        float lastNormalized = normalized;
        bool lastGrowing = grow;
        bool isNextBeat = false;
        while (!isNextBeat)
        {
            if (lastNormalized >= 0.5f)
            {
                isNextBeat = !grow && normalized <= 0.5f;
            }
            else
            {
                isNextBeat = grow && normalized >= 0.5f;
            }
            yield return null;
        }
        beatLocked = false;
    }

    IEnumerator WaitTwoBeats()
    {
        float lastNormalized = normalized;
        bool lastGrowing = grow;
        bool isNextBeat = false;
        while (!isNextBeat)
        {
            if (lastNormalized >= 0.5f)
            {
                isNextBeat = !grow && normalized <= 0.5f;
            }
            else
            {
                isNextBeat = grow && normalized >= 0.5f;
            }
            yield return null;
        }
        lastNormalized = normalized;
        lastGrowing = grow;
        isNextBeat = false;
        while (!isNextBeat)
        {
            if (lastNormalized >= 0.5f)
            {
                isNextBeat = !grow && normalized <= 0.5f;
            }
            else
            {
                isNextBeat = grow && normalized >= 0.5f;
            }
            yield return null;
        }
        beatLocked = false;
    }


    public void ScheduleDischarge(int beats)
    {
        StartCoroutine(DischargeAfter(beats));
    }

    IEnumerator DischargeAfter(int beats)
    {
        Action currentAction = player.currentAction;
        for(int i = 0; i<beats; i++)
        {
            float lastNormalized = normalized;
            bool lastGrowing = grow;
            bool isNextBeat = false;
            while (!isNextBeat)
            {
                if (lastNormalized >= 0.5f)
                {
                    isNextBeat = !grow && normalized <= 0.5f;
                }
                else
                {
                    isNextBeat = grow && normalized >= 0.5f;
                }
                yield return null;
            }
            yield return null;
        }
        if(player.currentAction == currentAction)
        {
            player.charged = false;
            player.currentAction = Action.None;
            player.SetTrigger("fail");
        }
    }

    public void ScheduleFunction(int beats, string function, MonoBehaviour script)
    {
        StartCoroutine(ExecuteFunctionAfter(beats, function, script));
    }

    IEnumerator ExecuteFunctionAfter(int beats, string function, MonoBehaviour script)
    {
        for (int i = 0; i < beats; i++)
        {
            float lastNormalized = normalized;
            bool lastGrowing = grow;
            bool isNextBeat = false;
            while (!isNextBeat)
            {
                if (lastNormalized >= 0.5f)
                {
                    isNextBeat = !grow && normalized <= 0.5f;
                }
                else
                {
                    isNextBeat = grow && normalized >= 0.5f;
                }
                yield return null;
            }
            yield return null;
        }
        script.SendMessage(function);
    }

    public void ScheduleFunction(float time, string function, MonoBehaviour script)
    {
        StartCoroutine(ExecuteFunctionAfter(time, function, script));
    }

    IEnumerator ExecuteFunctionAfter(float time, string function, MonoBehaviour script)
    {
        yield return new WaitForSeconds(time);
        script.SendMessage(function);
    }
}
