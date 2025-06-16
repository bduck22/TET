using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFalse : MonoBehaviour
{
    ParticleSystem PS;
    void Start()
    {
        PS = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!PS.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
