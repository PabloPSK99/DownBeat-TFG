using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rhythm : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

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

    public bool beatLocked;

    private float lastTimeStamp;
    private bool grow;
    private float beatDuration;
    private float syncCorrection;
    private Vector3 minScale = new Vector3(.1f, .1f, 1);

    // Start is called before the first frame update
    void Start()
    {
        beatDuration = bpm / 60f;
        normalized = 1;
        StartCoroutine(Loop());
    }

    private void Update()
    {
        
    }


    IEnumerator Loop()
    {
        audioSource.PlayScheduled(timeOffset + beatDuration);
        yield return new WaitForSeconds(timeOffset);
        lastTimeStamp = audioSource.time;
        while (true)
        {
            yield return CircleShrink();
            yield return CircleGrow();
        }
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
        externalCircle.color = upBeatColor;
        outGlow.color = outGlowColor;
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
        dot.color = downBeatColor;
        dotGlow.color = dotGlowColor;
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
        float offset = (lastTimeStamp + beatDuration + syncCorrection) - audioSource.time; //tiempo esperado - tiempo real
        //print("Duration: " + (beatDuration + syncCorrection ) + "    Expected time: " + (lastTimeStamp + beatDuration + syncCorrection) + "    Actual time: " + audioSource.time);
        lastTimeStamp = audioSource.time;
        if(offset > 30) // Si la canción ha vuelto a empezar
        {
            syncCorrection = -audioSource.time;
        }
        else                    // Si no
        {
            syncCorrection = offset;
        }
    }

    public void UpdateColor(float value)
    {
        if(value >= 0.5)
        {
            value = (value - 0.5f) * 2;
            circle.color = ((1f - value) * offBeatColor) + (value * upBeatColor);
        }
        else
        {
            value = value  * 2;
            circle.color = ((1f - value) * downBeatColor) + (value * offBeatColor);
        }
        
    }

    private void OnCircleUpdate(float newValue)
    {
        normalized = newValue;
        UpdateColor(normalized);
    }

    private void UpdateDotColor(float newValue)
    {
        dot.color = ((1f - newValue) * offBeatColor) + (newValue * downBeatColor);
        dotGlow.color = new Color(dotGlowColor.r, dotGlowColor.g, dotGlowColor.b, newValue);
    }

    private void FinalDotColor()
    {
        dot.color = offBeatColor;
        dotGlow.color = Color.clear;
    }

    private void UpdateExternalCircleColor(float newValue)
    {
        externalCircle.color = ((1f - newValue) * offBeatColor) + (newValue * upBeatColor);
        outGlow.color = new Color(outGlowColor.r, outGlowColor.g, outGlowColor.b, newValue);
    }

    private void FinalExternalCircleColor()
    {
        externalCircle.color = offBeatColor;
        outGlow.color = Color.clear;
    }


    public void LockThisBeat()
    {
        beatLocked = true;
        StartCoroutine(WaitForNextBeat());
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

}
