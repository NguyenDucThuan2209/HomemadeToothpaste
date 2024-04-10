using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("SFX Settings")]
    [SerializeField]
    AudioSource stiringSFX;
    [SerializeField]
    AudioSource stampingSFX;
    [SerializeField]
    AudioSource brushingSFX;
    [SerializeField]
    AudioSource scannedSFX;
    [SerializeField]
    AudioSource blenderSFX;
    [SerializeField]
    AudioSource liquidSFX;
    [SerializeField]
    AudioSource spoonSFX;

    [SerializeField]
    AudioSource maleVomitSFX;
    [SerializeField]
    AudioSource maleScreamSFX;
    [SerializeField]
    AudioSource maleHappySFX;

    [SerializeField]
    AudioSource femaleVomitSFX;
    [SerializeField]
    AudioSource femaleScreamSFX;
    [SerializeField]
    AudioSource femaleHappySFX;
    [SerializeField]
    AudioSource perfectSFX;
    [SerializeField]
    AudioSource failSFX;
    [SerializeField]
    AudioSource unlockSFX;

    [SerializeField]
    AudioSource buttonSFX;

    Coroutine CR_SFXRunner;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void playFailSFX()
    {
        PlaySFX(failSFX, 0.1f);
    }

    public void playStiringSFX()
    {
        stiringSFX.Play();
    }

    public void playStampingSFX()
    {
        PlaySFX(stampingSFX, 0.05f);
    }


    public void playBrushingSFX()
    {
        PlaySFX(brushingSFX, 0.4f);
    }


    public void playScannedSFX()
    {
        PlaySFX(scannedSFX, 0.2f);
    }

    public void playButtonSFX()
    {
        PlaySFX(buttonSFX, 0.1f);
    }

    public void playBlenderSFX()
    {
        blenderSFX.Play();
        blenderSFX.loop = true;
    }
    public void stopBlenderSFX()
    {
        blenderSFX.Stop();
    }
    public void SetBlenderSFXVolume(float value)
    {
        blenderSFX.volume = value;
    }



    public void playLiquidSFX()
    {
        PlaySFX(liquidSFX, 0.2f);
    }



    public void playSpoonSFX()
    {
        PlaySFX(spoonSFX, 0.2f);
    }


    public void playMaleVomitSFX()
    {
        PlaySFX(maleVomitSFX, 0.1f);
    }

    public void playMaleScreamSFX()
    {
        PlaySFX(maleScreamSFX, 0.1f);
    }


    public void playMaleHappySFX()
    {
        PlaySFX(maleHappySFX, 0.1f);
    }

    public void playFemaleVomitSFX()
    {
        PlaySFX(femaleVomitSFX, 0.1f);
    }


    public void playFemaleScreamSFX()
    {
        PlaySFX(femaleScreamSFX, 0.1f);
    }



    public void playFemaleHappySFX()
    {
        PlaySFX(femaleHappySFX, 0.1f);
    }



    public void playPerfectSFX()
    {
        PlaySFX(perfectSFX, 0.1f);
    }



    public void playUnlockSFX()
    {
        PlaySFX(unlockSFX, 0.1f);
    }



    void PlaySFX(AudioSource audioSource, float delay = 0.1f)
    {
        if (CR_SFXRunner == null)
            CR_SFXRunner = StartCoroutine(CR_PlaySFX(audioSource, delay));
    }

    IEnumerator CR_PlaySFX(AudioSource audioSource, float delay)
    {
        audioSource.Play();
        yield return new WaitForSeconds(delay);
        CR_SFXRunner = null;
    }
}