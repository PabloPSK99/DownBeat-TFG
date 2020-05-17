using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public GameObject shockWave;
    public Transform hand;

    public void Punch()
    {
        Destroy(Instantiate(shockWave, hand), 2);
    }

    public void Shot()
    {
        Destroy(Instantiate(shockWave, hand), 2);
    }
}
