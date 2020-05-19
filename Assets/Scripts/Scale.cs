using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    public Vector3[] scales;
    public float[] times;
    public Light lamp;
    private int index;
    private float intensity;

    // Update is called once per frame
    void Start()
    {
        intensity = lamp.intensity;
        StartCoroutine(Grow());
    }

    void Update()
    {
        lamp.intensity = intensity * transform.localScale.magnitude;
    }

    IEnumerator Grow()
    {
        for(int i = 0; i<scales.Length && i<times.Length; i++)
        {
            if (i==0 || scales[i] != scales[i-1])
            {
                iTween.ScaleTo(gameObject, scales[i], times[i]);
            }
            yield return new WaitForSeconds(times[i]);
        }

        
    }
}
