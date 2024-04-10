using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowderPouringSFX : MonoBehaviour
{
    ParticleSystem particle;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        source.volume = Mathf.Clamp(particle.particleCount / 100f, 0f, 1f);
    }
}
