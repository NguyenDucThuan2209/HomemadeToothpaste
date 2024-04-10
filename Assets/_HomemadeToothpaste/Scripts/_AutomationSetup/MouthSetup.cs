using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthSetup : MonoBehaviour
{
    //[SerializeField]
    //List<Color> teethColor;
    [SerializeField]
    List<int> dirtPosition;
    [SerializeField]
    List<int> bloodyGumPosition;


    [SerializeField]
    Mouth CharacterMouth;
    [SerializeField]
    Mouth RimLightMouth;
    [SerializeField]
    Mouth DissolveMouth;


    List<Transform> CharacterDirtyTeeth;
    List<Transform> CharacterBloodyGums;

    List<Transform> RimLightDirtyTeeth;
    List<Transform> RimLightBloodyGums;

    List<Transform> DissolveDirtyTeeth;
    List<Transform> DissolveBloodyGums;


    [Header("Randomize Position")]
    [SerializeField]
    bool randomizeDirt;
    [SerializeField]
    int numbersOfDirt;


    private void Start()
    {
        ChangeColorInPlayMode();
    }

    private void ChangeColorInPlayMode()
    {
        //Color randColor = teethColor[Random.Range(0, teethColor.Count)];
        Color randColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        if (CharacterMouth.dirts.Count > 0 && DissolveMouth.dirts.Count > 0)
        {

            for (int i = 0; i < CharacterMouth.dirts.Count; i++)
            {
                CharacterMouth.dirts[i].GetComponent<Renderer>().sharedMaterial.color = randColor;
                RimLightMouth.dirts[i].GetComponent<Renderer>().material.color = randColor;
                DissolveMouth.dirts[i].GetComponent<Renderer>().sharedMaterial.color = randColor;
                DissolveMouth.dirts[i].GetComponent<Renderer>().sharedMaterial.SetFloat("_DissolveScale", 1f);
            }
        }
        RimLightMouth.upperJaw.material.color = randColor;
        RimLightMouth.lowerJaw.material.color = randColor;

        DissolveMouth.upperJaw.material.color = randColor;
        DissolveMouth.lowerJaw.material.color = randColor;

        DissolveMouth.upperJaw.sharedMaterial.SetFloat("_DissolveScale", 1f);
        DissolveMouth.lowerJaw.sharedMaterial.SetFloat("_DissolveScale", 1f);

        CharacterMouth.upperJaw.material.color = randColor;
        CharacterMouth.lowerJaw.material.color = randColor;
        CharacterMouth.SetParticleColor(randColor);


    }



    [ContextMenu("Mouth Automation Setup")]
    void Setup()
    {
        if (dirtPosition.Count <= 0)
        {
            SetChildrensEnableState(Array2List(CharacterMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), CharacterMouth.transform.GetChild(0).transform), false);
            SetChildrensEnableState(Array2List(CharacterMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), CharacterMouth.transform.GetChild(1).transform), false);

            SetChildrensEnableState(Array2List(RimLightMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), RimLightMouth.transform.GetChild(0).transform), false);
            SetChildrensEnableState(Array2List(RimLightMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), RimLightMouth.transform.GetChild(1).transform), false);

            SetChildrensEnableState(Array2List(DissolveMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), DissolveMouth.transform.GetChild(0).transform), false);
            SetChildrensEnableState(Array2List(DissolveMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), DissolveMouth.transform.GetChild(1).transform), false);

            return;
        }
        if (CharacterMouth == null || RimLightMouth == null || DissolveMouth == null)
        {
            Debug.LogError("Character mouth, Rim light mouth, Dissolve mouth must not be null");
            return;
        }



        CharacterDirtyTeeth = new List<Transform>();
        CharacterBloodyGums = new List<Transform>();

        RimLightDirtyTeeth = new List<Transform>();
        RimLightBloodyGums = new List<Transform>();

        DissolveDirtyTeeth = new List<Transform>();
        DissolveBloodyGums = new List<Transform>();

        /*Reset if any dirt or bloody gums available */


        SetChildrensEnableState(Array2List(CharacterMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), CharacterMouth.transform.GetChild(0).transform), false);
        SetChildrensEnableState(Array2List(CharacterMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), CharacterMouth.transform.GetChild(1).transform), false);

        SetChildrensEnableState(Array2List(RimLightMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), RimLightMouth.transform.GetChild(0).transform), false);
        SetChildrensEnableState(Array2List(RimLightMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), RimLightMouth.transform.GetChild(1).transform), false);

        SetChildrensEnableState(Array2List(DissolveMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(), DissolveMouth.transform.GetChild(0).transform), false);
        SetChildrensEnableState(Array2List(DissolveMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(), DissolveMouth.transform.GetChild(1).transform), false);

        /*Enable it again*/
        CharacterDirtyTeeth = PositionFilter(Array2List(CharacterMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(true)), dirtPosition);
        CharacterBloodyGums = PositionFilter(Array2List(CharacterMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(true)), bloodyGumPosition);


        SetChildrensEnableState(CharacterDirtyTeeth, true);
        SetChildrensEnableState(CharacterBloodyGums, true);


        RimLightDirtyTeeth = PositionFilter(Array2List(RimLightMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(true)), dirtPosition);
        RimLightBloodyGums = PositionFilter(Array2List(RimLightMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(true)), bloodyGumPosition);


        SetChildrensEnableState(RimLightDirtyTeeth, true);
        SetChildrensEnableState(RimLightBloodyGums, true);

        DissolveDirtyTeeth = PositionFilter(Array2List(DissolveMouth.transform.GetChild(0).transform.GetComponentsInChildren<Transform>(true)), dirtPosition);
        DissolveBloodyGums = PositionFilter(Array2List(DissolveMouth.transform.GetChild(1).transform.GetComponentsInChildren<Transform>(true)), bloodyGumPosition);


        SetChildrensEnableState(DissolveDirtyTeeth, true);
        SetChildrensEnableState(DissolveBloodyGums, true);
    }

    List<Transform> Array2List(Transform[] array, Transform distinct = null)
    {
        List<Transform> result = new List<Transform>();
        foreach (var c in array)
        {
            if (distinct != null)
            {
                if (c != distinct)
                    result.Add(c);
            }
            else
            {
                result.Add(c);
            }
        }
        return result;
    }

    List<Transform> PositionFilter(List<Transform> input, List<int> positionList)
    {
        List<Transform> result = new List<Transform>();
        foreach (var c in input)
        {
            if (positionList.Contains(input.IndexOf(c)))
            {
                result.Add(c);
            }
        }

        return result;
    }

    void SetChildrensEnableState(List<Transform> childs, bool enable)
    {
        foreach (var c in childs)
        {
            c.gameObject.SetActive(enable);
        }
    }

    [ContextMenu("Randomize Position")]

    void RandomizeDirtPosition()
    {
        if (randomizeDirt)
        {
            dirtPosition.Clear();
            for (int i = 0; i < numbersOfDirt; i++)
            {
                int rand = Random.Range(0, 32);
                dirtPosition.Add(rand);
            }
        }
    }
}
