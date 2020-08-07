using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject ActivatedEffect;

    public void Activate()
    {
        ActivatedEffect.SetActive(true);
    }

    public void Deactivate()
    {
        ActivatedEffect.SetActive(false);
    }
}
