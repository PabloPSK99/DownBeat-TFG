using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;
    public float delay;
    private bool start;
    // Update is called once per frame

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private void Update()
    {
        if (start)
        {
            transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        start = true;
    }
}
