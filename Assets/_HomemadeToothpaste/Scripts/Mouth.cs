using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Mouth : MonoBehaviour
{
    [SerializeField]
    bool disableAtStart;
    public List<Transform> dirts;


    public List<Transform> bloodyGums;
    [SerializeField]
    public bool emitParticle;
    [SerializeField]
    ParticleSystem stinkyParticle;

    public Renderer upperJaw;
    public Renderer lowerJaw;

    private void Awake()
    {
        foreach (var c in transform.GetChild(0).transform.GetComponentsInChildren<Transform>(false))
        {
            if (c != transform.GetChild(0).transform)
            {
                dirts.Add(c);
            }
        }
        foreach (var c in transform.GetChild(1).transform.GetComponentsInChildren<Transform>(false))
        {
            if (c != transform.GetChild(1).transform)
            {
                bloodyGums.Add(c);
            }
        }
    }

    private void Start()
    {
        if (disableAtStart)
        {
            gameObject.SetActive(false);
        }
        if (stinkyParticle != null)
        {
            stinkyParticle.Play();
        }
    }

    public void SetParticleColor(Color color)
    {
        if (stinkyParticle != null)
        {


            MainModule main = stinkyParticle.main;
            MainModule cloudMain = stinkyParticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;

            MinMaxGradient stinkyMinMaxGradient = main.startColor;
            MinMaxGradient cloudMinMaxGradient = cloudMain.startColor;

            Color stinkyColor = stinkyMinMaxGradient.color;

            stinkyColor.r = color.r;
            stinkyColor.g = color.g;
            stinkyColor.b = color.b;


            stinkyMinMaxGradient.colorMin = stinkyColor;
            stinkyMinMaxGradient.colorMax = stinkyColor;
            main.startColor = stinkyMinMaxGradient;


            Color cloudMinColor = cloudMinMaxGradient.colorMin;
            Color cloudMaxColor = cloudMinMaxGradient.colorMax;

            cloudMinColor.r = color.r;
            cloudMinColor.g = color.g;
            cloudMinColor.b = color.b;

            cloudMaxColor.r = color.r;
            cloudMaxColor.g = color.g;
            cloudMaxColor.b = color.b;

            cloudMinMaxGradient.colorMin = cloudMinColor;
            cloudMinMaxGradient.colorMax = cloudMaxColor;

            cloudMain.startColor = cloudMinMaxGradient;


        }
    }

    public void EnableStinkyParticles()
    {

        if (stinkyParticle != null)
        {
            stinkyParticle.Play();
        }
    }


    public void DisableAllParticles()
    {
        //if (stinkyParticles.Count > 0)
        //{
        //    foreach (var p in stinkyParticles)
        //    {
        //        p.Stop();
        //    }
        //}
        stinkyParticle.Stop();
    }



}
