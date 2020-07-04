using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform characterPivot;

    public void Rotate(float rotation, float duration)
    {
        StartCoroutine(RotateBy(rotation, duration));
    }

    IEnumerator RotateBy(float rotation, float duration)
    {
        float lastRotation = transform.rotation.eulerAngles.y;
        iTween.RotateBy(gameObject, Vector3.up * rotation, duration);
        yield return new WaitForSeconds(duration);
        transform.rotation = Quaternion.Euler(0, lastRotation + rotation * 360, 0);
    }

    public void Rotate(float rotation, float duration, iTween.EaseType easetype)
    {
        StartCoroutine(RotateByEased(rotation, duration, easetype));
    }

    IEnumerator RotateByEased(float rotation, float duration, iTween.EaseType easetype)
    {
        float lastRotation = transform.rotation.eulerAngles.y;
        iTween.RotateBy(gameObject, iTween.Hash(
            "amount", Vector3.up * rotation,
            "time", duration,
            "easetype", easetype
            )
        );
        yield return new WaitForSeconds(duration);
        transform.rotation = Quaternion.Euler(0, lastRotation + rotation * 360, 0);
    }
}
